using HarmonyLib;
using PartyFatigue.Data;
using PartyFatigue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Localization;
using static TaleWorlds.MountAndBlade.CompressionInfo;

namespace PartyFatigue.Patches
{
    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "UpdateTooltip", typeof(PropertyBasedTooltipVM), typeof(MobileParty), typeof(bool), typeof(bool))]
    public class FatigueTooltipPartyPatch
    {
        [HarmonyPostfix]
        static void PostFix(PropertyBasedTooltipVM propertyBasedTooltipVM, MobileParty mobileParty, bool openedFromMenuLayout, bool checkForMapVisibility)
        {
            if (mobileParty != null)
            {
                if (PartyFatigueTracker.Current.partyFatigueData.ContainsKey(mobileParty)) 
                {
                    PartyFatigueData data = PartyFatigueTracker.Current.partyFatigueData[mobileParty];
                    TextObject textFatigue = new TextObject("{=TooltipFatigueStat}Fatigue");
                    
                    if (!propertyBasedTooltipVM.IsExtended)
                        propertyBasedTooltipVM.AddProperty(textFatigue.ToString(), ModCalculations.GetFatigueStatus(data.currentFatigue) + " (" + data.currentFatigue.ToString() + ")", 0);
                    else
                        propertyBasedTooltipVM.AddProperty(textFatigue.ToString(), ModCalculations.GetFatigueStatus(data.currentFatigue), 0);
               
                    if (data.needReset && checkForMapVisibility)
                    {
                        TextObject textSleeping = new TextObject("{=TooltipSleepingStat}Sleeping");
                        TextObject textSleepingHours = new TextObject("{=TooltipSleepingStatValue}{SLEEPING_HOURS} hours left");
                        textSleepingHours.SetTextVariable("SLEEPING_HOURS", ModCalculations.CalculateSleepHours(mobileParty, 1f).ToString());
                        propertyBasedTooltipVM.AddProperty(textSleeping.ToString(), textSleepingHours.ToString(), 0);
                    }
                }        
            }
        }
    }

    [HarmonyPatch(typeof(PropertyBasedTooltipVMExtensions), "UpdateTooltip", typeof(PropertyBasedTooltipVM), typeof(Army), typeof(bool), typeof(bool))]
    public class FatigueTooltipArmyPatch
    {
        [HarmonyPostfix]
        static void PostFix(PropertyBasedTooltipVM propertyBasedTooltipVM, Army army, bool openedFromMenuLayout, bool checkForMapVisibility)
        {
            MobileParty leaderParty = army.LeaderParty;
            if (leaderParty != null)
            {
                if (PartyFatigueTracker.Current.partyFatigueData.ContainsKey(leaderParty))
                {
                    float armyFatigueRate = PartyFatigueTracker.Current.partyFatigueData[leaderParty].currentFatigue;
                    int subPartyCount = 0;
                    foreach (MobileParty party in leaderParty.AttachedParties)
                    {
                        if (PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party) && party.DefaultBehavior != AiBehavior.JoinParty)
                        {
                            ++subPartyCount;
                            armyFatigueRate += PartyFatigueTracker.Current.partyFatigueData[party].currentFatigue;
                        }
                    }
                    armyFatigueRate /= subPartyCount + 1; //adding leader

                    PartyFatigueData data = PartyFatigueTracker.Current.partyFatigueData[leaderParty];
                    TextObject textFatigue = new TextObject("{=TooltipFatigueStat}Fatigue");
                    if (!propertyBasedTooltipVM.IsExtended)
                        propertyBasedTooltipVM.AddProperty(textFatigue.ToString(), ModCalculations.GetFatigueStatus(armyFatigueRate) + " (" + armyFatigueRate.ToString() + ")", 0);
                    else
                        propertyBasedTooltipVM.AddProperty(textFatigue.ToString(), ModCalculations.GetFatigueStatus(armyFatigueRate), 0);
                    
                    if (data.needResetArmy && checkForMapVisibility)
                    {
                        TextObject textSleeping = new TextObject("{=TooltipSleepingStat}Sleeping");
                        TextObject textSleepingHours = new TextObject("{=TooltipSleepingStatValue}{SLEEPING_HOURS} hours left");
                        textSleepingHours.SetTextVariable("SLEEPING_HOURS", ModCalculations.CalculateSleepHours(leaderParty, 1f).ToString());
                        propertyBasedTooltipVM.AddProperty(textSleeping.ToString(), textSleepingHours.ToString(), 0);
                    }
                }
            }
                
        }
    }
}
