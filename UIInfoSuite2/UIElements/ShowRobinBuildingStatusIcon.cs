using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using Microsoft.Xna.Framework.Graphics;
using UIInfoSuite.Infrastructure;
using UIInfoSuite.Infrastructure.Extensions;
using StardewValley.Buildings;

namespace UIInfoSuite.UIElements
{
    class ShowRobinBuildingStatusIcon : IDisposable
    {
        #region Properties

        private bool _IsBuildingInProgress;
        Rectangle _buildingIconSpriteLocation = new Rectangle(0, 4, 15, 15);
        private string _hoverText;
        private ClickableTextureComponent _buildingIcon;
        private Texture2D _robinIconSheet;

        private readonly IModHelper _helper;
        #endregion

        #region Lifecycle
        public ShowRobinBuildingStatusIcon(IModHelper helper)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            ToggleOption(false);
            _IsBuildingInProgress = false;
        }

        public void ToggleOption(bool showRobinBuildingStatus)
        {
            _helper.Events.GameLoop.DayStarted -= OnDayStarted;
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Display.RenderedHud -= OnRenderedHud;

            if (showRobinBuildingStatus)
            {
                FindRobinSpritesheet();
                UpdateRobinBuindingStatusData();

                _helper.Events.GameLoop.DayStarted += OnDayStarted;
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Display.RenderedHud += OnRenderedHud;
            }
        }
        #endregion

        #region Event subscriptions
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateRobinBuindingStatusData();
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // Draw icon
            if (!Game1.eventUp && _IsBuildingInProgress)
            {
                Point iconPosition = IconHandler.Handler.GetNewIconPosition();
                _buildingIcon =
                    new ClickableTextureComponent(
                        new Rectangle(iconPosition.X, iconPosition.Y, 40, 40),
                        _robinIconSheet,
                        _buildingIconSpriteLocation,
                        8 / 3f);
                _buildingIcon.draw(Game1.spriteBatch);
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Show text on hover
            if (_IsBuildingInProgress && (_buildingIcon?.containsPoint(Game1.getMouseX(), Game1.getMouseY()) ?? false) && !String.IsNullOrEmpty(_hoverText))
            {
                IClickableMenu.drawHoverText(
                    Game1.spriteBatch,
                    _hoverText,
                    Game1.dialogueFont
                );
            }
        }
        #endregion

        #region Logic
        private void UpdateRobinBuindingStatusData()
        {
            _IsBuildingInProgress = Game1.getFarm().isThereABuildingUnderConstruction();

            if (_IsBuildingInProgress)
            {
                Building buildingUnderConstruction = Game1.getFarm().getBuildingUnderConstruction();
                _hoverText = String.Format(_helper.SafeGetString(LanguageKeys.RobinBuildingStatus), buildingUnderConstruction.daysOfConstructionLeft.Value > 0 ? buildingUnderConstruction.daysOfConstructionLeft.Value : buildingUnderConstruction.daysUntilUpgrade.Value);
            }
            else
            {
                _hoverText = String.Empty;
            }
        }

        private void FindRobinSpritesheet()
        {
            NPC robin = Game1.getCharacterFromName<NPC>("Robin");
            _robinIconSheet = robin.Sprite.Texture;
        }
        #endregion
    }
}
