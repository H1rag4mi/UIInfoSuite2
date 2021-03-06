using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIInfoSuite;
using UIInfoSuite.Infrastructure.Extensions;

namespace UIInfoSuite2.Options
{
    public class ModConfig
    {
        public bool ShowLuckIcon { get; set; } = true;
        public bool ShowExactValue { get; set; } = false;
        /// <summary>If true, show notification when leveling up.</summary>
        public bool ShowLevelUpAnimation { get; set; } = true;
        public bool ShowExperienceBar { get; set; } = true;
        public bool AllowExperienceBarToFadeOut { get; set; } = true;
        /// <summary>If true, show notification when gaining xp.</summary>
        public bool ShowExperienceGain { get; set; } = true;
        public bool ShowLocationOfTownsPeople { get; set; } = true;
        public bool ShowBirthdayIcon { get; set; } = true;
        /// <summary>If true, show accurate hearts.</summary>
        public bool ShowHeartFills { get; set; } = true;
        public bool ShowAnimalsNeedPets { get; set; } = true;
        public bool HideAnimalPetOnMaxFriendship { get; set; } = true;
        public bool DisplayCalendarAndBillboard { get; set; } = true;
        public bool ShowCropAndBarrelTooltip { get; set; } = true;
        public bool ShowItemEffectRanges { get; set; } = true;
        public bool ShowExtraItemInformation { get; set; } = true;
        public bool ShowTravelingMerchant { get; set; } = true;
        public bool HideMerchantWhenVisited { get; set; } = false;
        public bool ShowRainyDay { get; set; } = true;
        public bool ShowHarvestPricesInShop { get; set; } = true;
        public bool ShowWhenNewRecipesAreAvailable { get; set; } = true;
        public bool ShowToolUpgradeStatus { get; set; } = false;
        public bool ShowRobinBuildingStatusIcon { get; set; } = true;
    }
}