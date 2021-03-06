using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UIInfoSuite.AdditionalFeatures;
using UIInfoSuite.Infrastructure;
using UIInfoSuite.Options;
using UIInfoSuite2;
using UIInfoSuite2.Options;
using UIInfoSuite.Infrastructure.Extensions;
using System.Reflection;

namespace UIInfoSuite
{
    public class ModEntry : Mod
    {

        #region Properties
        public static IMonitor MonitorObject { get; private set; }

        private SkipIntro _skipIntro; // Needed so GC won't throw away object with subscriptions
        private string _modDataFileName;
        private readonly Dictionary<string, string> _options = new Dictionary<string, string>();

        private ModOptionsPageHandler _modOptionsPageHandler;

        internal static ModEntry Instance;
        internal ModConfig Config;
        #endregion


        #region Entry
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            MonitorObject = Monitor;
            _skipIntro = new SkipIntro(helper.Events);

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.GameLaunched += OnLaunched;

            helper.Events.Display.Rendering += IconHandler.Handler.Reset;
        }
        #endregion


        #region Event subscriptions
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Unload if the main player quits.
            if (Context.ScreenId != 0) return;
            
            _modOptionsPageHandler?.Dispose();
            _modOptionsPageHandler = null;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Only load once for split screen.
            if (Context.ScreenId != 0) return;

            try
            {
                try
                {
                    _modDataFileName = Path.Combine(Helper.DirectoryPath, Game1.player.Name + "_modData.xml");
                }
                catch
                {
                    Monitor.Log("Error: Player name contains character that cannot be used in file name. Using generic file name." + Environment.NewLine +
                        "Options may not be able to be different between characters.", LogLevel.Warn);
                    _modDataFileName = Path.Combine(Helper.DirectoryPath, "default_modData.xml");
                }

                if (File.Exists(_modDataFileName))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(_modDataFileName);

                    foreach (XmlNode node in document.GetElementsByTagName("option"))
                    {
                        string key = node.Attributes["name"]?.Value;
                        string value = node.InnerText;

                        if (key != null)
                            _options[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Error loading mod config. " + ex.Message + Environment.NewLine + ex.StackTrace, LogLevel.Error);
            }

            _modOptionsPageHandler = new ModOptionsPageHandler(Helper, _options);
        }

        private void OnSaved(object sender, EventArgs e)
        {
            // Only save for the main player.
            if (Context.ScreenId != 0) return;

            if (!string.IsNullOrWhiteSpace(_modDataFileName))
            {
                if (File.Exists(_modDataFileName))
                {
                    File.Delete(_modDataFileName);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(File.Open(_modDataFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), settings))
                {
                    writer.WriteStartElement("options");

                    foreach (var option in _options)
                    {
                        writer.WriteStartElement("option");
                        writer.WriteAttributeString("name", option.Key);
                        writer.WriteValue(option.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
            }
        }

        private void OnLaunched(object sender, EventArgs e)
        {
            try
            {
                Config = Helper.ReadConfig<ModConfig>(); //attempt to load (or create) config.json
            }
            catch (Exception ex)
            {
                Monitor.Log($"Encountered an error while loading the config.json file. Default settings will be used instead. Full error message:\n-----\n{ex.ToString()}", LogLevel.Error);
                Config = new ModConfig(); //use the default settings
            }

            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuInterface>("spacechase0.GenericModConfigMenu");
            if (api == null)
                return;
            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            foreach (PropertyInfo property in Config.GetType().GetProperties())
            {
                api.RegisterSimpleOption(ModManifest,
                    optionName: property.Name,
                    optionDesc: Helper.SafeGetString(property.Name.ToLower()),
                    optionGet: () => (bool)property.GetValue(Config),
                    optionSet: (bool value) => property.SetValue(Config, value));
            }
        }
        #endregion

    }
}
