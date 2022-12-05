using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyFatigue.Settings
{
    internal class MCMSettings : AttributeGlobalSettings<MCMSettings>, ISettingVariables
    {
        public override string Id => "PartyFatigueSettings";

        public override string DisplayName => "Party Fatigue";

        public override string FolderName => "Party Fatigue";

        public override string FormatType => "xml";


        [SettingPropertyFloatingInteger("{=SettingsValue_DayRecoveryRate}Day Fatigue Recovery Rate", minValue: GlobalModSettings.minDayRecoverRate, maxValue: GlobalModSettings.maxDayRecoverRate, Order = 0, HintText = "{=SettingsDesc_DayRecoveryRate}Percentage of fatigue to recover per hour during the day.", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_RecoveryRates}Recovery Rates", GroupOrder = 0)]
        public float DayRecoveryRate { get; set; } = 0.05f;
        [SettingPropertyFloatingInteger("{=SettingsValue_NightRecoveryRate}Night Fatigue Recovery Rate", minValue: GlobalModSettings.minNightRecoverRate, maxValue: GlobalModSettings.maxNightRecoverRate, Order = 1, HintText = "{=SettingsDesc_NightRecoveryRate}Percentage of fatigue to recover per hour during the night.", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_RecoveryRates}Recovery Rates", GroupOrder = 0)]
        public float NightRecoveryRate { get; set; } = 0.1f;
        [SettingPropertyFloatingInteger("{=SettingsValue_RestingRecoveryRate}Resting Recovery Multiplier", minValue: GlobalModSettings.minRestingRecoverRate, maxValue: GlobalModSettings.maxRestingRecoverRate, Order = 2, HintText = "{=SettingsDesc_RestingRecoveryRate}Recovery Multiplier when party is not moving.", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_RecoveryRates}Recovery Rates", GroupOrder = 0)]
        public float RestingRecoveryBonus { get; set; } = 1.1f;
        [SettingPropertyFloatingInteger("{=SettingsValue_SettlementRecoveryRate}Settlement Recovery Multiplier", minValue: GlobalModSettings.minSettlementRecoverRate, maxValue: GlobalModSettings.maxSettlementRecoverRate, Order = 3, HintText = "{=SettingsDesc_SettlementRecoveryRate}Recovery Multiplier when party is in a settlement.", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_RecoveryRates}Recovery Rates", GroupOrder = 0)]
        public float SettlementRecoveryBonus { get; set; } = 1.5f;

        [SettingPropertyFloatingInteger("{=SettingsValue_FatigueRate}Fatigue Rate", minValue: GlobalModSettings.minFatigueRate, maxValue: GlobalModSettings.maxFatigueRate, Order = 0, HintText = "{=SettingsDesc_FatigueRate}How fast fatigue grows per hour", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_FatigueSettings}Fatigue Settings", GroupOrder = 1)]
        public float FatigueRate { get; set; } = 0.2f;

        [SettingPropertyFloatingInteger("{=SettingsValue_MiscPartyFatigueRate}Misc Party Fatigue Multiplier", minValue: GlobalModSettings.minMiscPartyFatigueMultiplier, maxValue: GlobalModSettings.maxMiscPartyFatigueMultiplier, Order = 1, HintText = "{=SettingsDesc_MiscPartyFatigueRate}How much faster/slower do non-hero parties such as caravans, bandits, villagers fatigue grow when fleeing.", RequireRestart = false)]
        [SettingPropertyGroup("{=SettingsGroup_FatigueSettings}Fatigue Settings", GroupOrder = 1)]
        public float MiscPartyFatigueMultiplier { get; set; } = 0.5f;
    }
}
