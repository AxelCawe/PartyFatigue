using MCM.Abstractions.Base.Global;
using PartyFatigue.Data;
using PartyFatigue.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PartyFatigue.Helpers
{
    static class ModCalculations
    {
        public static float CalculateFatigueRate(MobileParty mobileParty)
        {
            
            if (mobileParty.Army == null)
            {
                TroopRoster roster = mobileParty.MemberRoster;
                TroopRoster prison_roster = mobileParty.PrisonRoster;


                int totalTiers = 0;
                int totalTroops = roster.TotalManCount + prison_roster.TotalManCount;

                foreach (TroopRosterElement regular in roster.GetTroopRoster())
                {
                    totalTiers += regular.Character.Tier * regular.Number;
                }
                // Add hero tiers
                totalTiers += roster.TotalHeroes * 6; // we give each hero a tier 6
                // Add prisoner tiers. All prisoners will give a tier of 1
                totalTiers += prison_roster.TotalManCount;

                float averageTier = (float)totalTiers / (float)totalTroops;
                return MathF.Clamp(1f / averageTier, 0f, 1f);
            }
            else
            {
                int totalTiers = 0;
                int totalTroops = 0;
                foreach (MobileParty lordParty in mobileParty.Army.LeaderPartyAndAttachedParties)
                {
                    TroopRoster roster = lordParty.MemberRoster;
                    TroopRoster prison_roster = lordParty.PrisonRoster;

                    totalTroops += roster.TotalManCount + prison_roster.TotalManCount;
                    foreach (TroopRosterElement regular in roster.GetTroopRoster())
                    {
                        totalTiers += regular.Character.Tier * regular.Number;
                    }
                    // Add hero tiers
                    totalTiers += roster.TotalHeroes * 6; // we give each hero a tier 6
                    // Add prisoner tiers. All prisoners will give a tier of 1
                    totalTiers += prison_roster.TotalManCount;
                }
               
                float averageTier = (float)totalTiers / (float)totalTroops;
                return MathF.Clamp(1f / averageTier, 0f, 1f);
            }
        }

        public static float CalculateFinalFatigueUpdateValue(MobileParty mobileParty)
        {
            PartyFatigueData party = PartyFatigueTracker.Current.partyFatigueData[mobileParty];
            
            float settlementBonus = mobileParty.CurrentSettlement != null ? GlobalSettings<MCMSettings>.Instance.SettlementRecoveryBonus : 1f;
            //if (party.Key.IsMainParty)
            {
                if (mobileParty.LeaderHero == null)
                {
                    if (mobileParty.DefaultBehavior == AiBehavior.FleeToPoint || mobileParty.DefaultBehavior == AiBehavior.FleeToParty || mobileParty.DefaultBehavior == AiBehavior.FleeToGate
                        || mobileParty.ShortTermBehavior == AiBehavior.FleeToPoint || mobileParty.ShortTermBehavior == AiBehavior.FleeToParty || mobileParty.ShortTermBehavior == AiBehavior.FleeToGate)
                    {
                        return -party.fatigueRate * GlobalSettings<MCMSettings>.Instance.FatigueRate * GlobalSettings<MCMSettings>.Instance.MiscPartyFatigueMultiplier;
                    }
                    else
                    {
                        return settlementBonus * GlobalSettings<MCMSettings>.Instance.RestingRecoveryBonus * (CampaignTime.Now.IsDayTime ? GlobalSettings<MCMSettings>.Instance.DayRecoveryRate : GlobalSettings<MCMSettings>.Instance.NightRecoveryRate) * GlobalSettings<MCMSettings>.Instance.MiscPartyFatigueMultiplier;
                    }
                }
                else if (mobileParty.Army == null)
                {
                    // Not in army

                    // Check whether party is moving
                    bool isMoving = ModCalculations.IsPartyMoving(mobileParty);
                    if (isMoving)
                    {
                        return -party.fatigueRate * GlobalSettings<MCMSettings>.Instance.FatigueRate;
                    }
                    else
                    {
                        return settlementBonus * GlobalSettings<MCMSettings>.Instance.RestingRecoveryBonus * (CampaignTime.Now.IsDayTime ? GlobalSettings<MCMSettings>.Instance.DayRecoveryRate : GlobalSettings<MCMSettings>.Instance.NightRecoveryRate);
                    }
                }
                else
                {
                    bool isMoving = ModCalculations.IsPartyMoving(mobileParty.Army.LeaderParty);
                    if (isMoving)
                    {
                        return -party.fatigueRate * GlobalSettings<MCMSettings>.Instance.FatigueRate;
                    }
                    else
                    {
                        return settlementBonus * GlobalSettings<MCMSettings>.Instance.RestingRecoveryBonus * (CampaignTime.Now.IsDayTime ? GlobalSettings<MCMSettings>.Instance.DayRecoveryRate : GlobalSettings<MCMSettings>.Instance.NightRecoveryRate);
                    }
                }
            }
        }

        public static int CalculateRemainingHours(PartyFatigueData data)
        {
            int remaining_hour = 0;
            float temp = data.currentFatigue;
            while (temp > 0)
            {
                temp -= data.fatigueRate;
                remaining_hour++;
            }
            return remaining_hour;
        }

        public static float CalculateSpeedRatio(PartyFatigueData data)
        {
            float currentFatigue = data.currentFatigue;
            if (currentFatigue >= GlobalModSettings.excitedFatigue)
            {
                return 1.1f;
            }
            else if (currentFatigue >= GlobalModSettings.normalFatigue)
            {
                return 1f;
            }
            else if (currentFatigue >= GlobalModSettings.tiredFatigue)
            {
                return MathF.Clamp((float)(1f - (GlobalModSettings.excitedFatigue - currentFatigue)), 0f, 1f);
            }
            else if (currentFatigue > MathF.Epsilon)
            {
                return 0f;
            }
            else
            {
                return 0f;
            }
        }

        public static bool IsPartyMoving(MobileParty party)
        {
            bool flag_busy = PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party) ? PartyFatigueTracker.Current.partyFatigueData[party].isBusy : false;
            return !(party.DefaultBehavior == AiBehavior.Hold || party.DefaultBehavior == AiBehavior.BesiegeSettlement || party.DefaultBehavior == AiBehavior.RaidSettlement
                          || (party.Position2D - party.TargetPosition).Length < 1E-8f || !party.IsMoving) && (!flag_busy);
        }

        public static int CalculateSleepHours(MobileParty party, float recoverToPercent = 0.8f)
        {
            float temp = PartyFatigueTracker.Current.partyFatigueData[party].currentFatigue;
            if (party.Army != null && party.Army.LeaderParty == party)
            {
        
                foreach (MobileParty partyInArmy in party.AttachedParties)
                {
                    if (PartyFatigueTracker.Current.partyFatigueData.ContainsKey(partyInArmy))
                    {
                        temp += PartyFatigueTracker.Current.partyFatigueData[partyInArmy].currentFatigue;
                    }
                }
                temp /= party.Army.Parties.Count;
            }


            int ans = 0;
            
            float recoveryValue = CalculateFinalFatigueUpdateValue(party);

            if (recoveryValue > 0)
            {
                while (temp < recoverToPercent)
                {
                    temp += recoveryValue;
                    ans++;
                }
                return ans;
            }
            else return -1;

        }

        public static string GetFatigueStatus(float rate)
        {
            if (rate > GlobalModSettings.excitedFatigue)
            {
                return "Excited";
            }
            else if (rate > GlobalModSettings.normalFatigue)
            {
                return "Normal";
            }
            else if (rate > GlobalModSettings.tiredFatigue)
            {
                return "Tired";
            }
            else
            {
                return "Near collapse";
            }
        }
    }
}
