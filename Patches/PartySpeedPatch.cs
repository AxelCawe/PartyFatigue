using HarmonyLib;
using PartyFatigue.Data;
using PartyFatigue.Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace PartyFatigue.Patches
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    public static class PartySpeedPatch
    {
        [HarmonyPostfix]
        public static void CalculateFinalSpeed(
            ref MobileParty mobileParty,
            ref ExplainedNumber finalSpeed,
            ref ExplainedNumber __result)
        {
            if (PartyFatigueTracker.Current == null)
                return;

            PartyFatigueData data = null;
            PartyFatigueTracker.Current.partyFatigueData.TryGetValue(mobileParty, out data);
            if (data != null)
            {
                __result.AddFactor(ModCalculations.CalculateSpeedRatio(data) - 1f, new TaleWorlds.Localization.TextObject("{=SpeedTooltip_SpeedBoost}Party Fatigue Boost"));
            }
        }
    }
}
