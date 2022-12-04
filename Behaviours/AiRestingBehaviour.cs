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
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace PartyFatigue.Behaviours
{
    class AiRestingBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(OnPartyHourlyTick));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(OnGameLoad));
            //CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameLoad));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }
        public void OnGameLoad()
        {
            foreach (var party in PartyFatigueTracker.Current.partyFatigueData)
            {
                OnPartyHourlyTick(party.Key);
            }
        }

        public void OnPartyHourlyTick(MobileParty mobileParty)
        {
            if (!PartyFatigueTracker.Current.partyFatigueData.ContainsKey(mobileParty) || mobileParty.LeaderHero == null)
            {
                return;
            }

            // Skip if the party is in the army and not leading it
            if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty)
            {
                return;
            }

            if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty && !mobileParty.IsMainParty && mobileParty.LeaderHero != null) // if army NPC leader
            {
                float armyFatigueRate = PartyFatigueTracker.Current.partyFatigueData[mobileParty].currentFatigue;
                int partyCount = 1;
                foreach (MobileParty party in mobileParty.AttachedParties)
                {
                    if (PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party))
                    {
                        partyCount++;
                        armyFatigueRate += PartyFatigueTracker.Current.partyFatigueData[party].currentFatigue;
                    }
                }
                armyFatigueRate /= (float)partyCount;

                if (PartyFatigueTracker.Current.partyFatigueData[mobileParty].needResetArmy && armyFatigueRate > 1f - MathF.Epsilon)
                {
                    PartyFatigueTracker.Current.partyFatigueData[mobileParty].needResetArmy = false;
                    foreach (MobileParty party in mobileParty.Army.LeaderPartyAndAttachedParties)
                    {
                        //if (party.DefaultBehavior != AiBehavior.JoinParty)
                        {
                            PartyFatigueTracker.ToggleTent(party.Party, false);
                            mobileParty.Ai.SetDoNotMakeNewDecisions(false);
                            LoadPartyAIState(mobileParty);
                        }
                    }
                    
                }
                else if (PartyFatigueTracker.Current.partyFatigueData[mobileParty].needResetArmy)
                {
                    foreach (MobileParty party in mobileParty.Army.LeaderPartyAndAttachedParties)
                    {
                        PartyFatigueTracker.ToggleTent(party.Party, true);
                    }
                    
                }



                if (armyFatigueRate <= MathF.Epsilon) // Army needs to sleep
                {
                    if (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.AssaultingTown)
                    {
                        
                    }
                    else
                    {
                        PartyFatigueTracker.Current.partyFatigueData[mobileParty].needResetArmy = true;
                        foreach (MobileParty party in mobileParty.AttachedParties)
                        {
                            SavePartyState(mobileParty);
                        }
                        //mobileParty.SetMoveGoToPoint(mobileParty.Position2D);
                        mobileParty.SetMoveModeHold();
                        mobileParty.Ai.SetDoNotMakeNewDecisions(true);
                        mobileParty.Army.AIBehavior = Army.AIBehaviorFlags.Unassigned;
                        PartyFatigueTracker.ToggleTent(mobileParty.Party, true);
                        foreach (MobileParty party in mobileParty.AttachedParties)
                        {
                            //if (party.DefaultBehavior != AiBehavior.JoinParty)
                            {
                                PartyFatigueTracker.ToggleTent(party.Party, true);
                                mobileParty.Ai.SetDoNotMakeNewDecisions(true);
                            }
                        }
                        return;
                    }
        
                }
                

 
                return;
            }


            /* party ai */
            if ((mobileParty.Army == null || (mobileParty.Army != null && mobileParty.DefaultBehavior == AiBehavior.JoinParty)) && !mobileParty.IsMainParty)
            {
                if (PartyFatigueTracker.Current.partyFatigueData[mobileParty].needReset && PartyFatigueTracker.Current.partyFatigueData[mobileParty].currentFatigue > 1f - MathF.Epsilon)
                {
                    // Time to wake up
                    PartyFatigueTracker.Current.partyFatigueData[mobileParty].needReset = false;
                    mobileParty.Ai.SetDoNotMakeNewDecisions(false);
                    LoadPartyAIState(mobileParty);
                    PartyFatigueTracker.ToggleTent(mobileParty.Party, false);
                }
                else if (PartyFatigueTracker.Current.partyFatigueData[mobileParty].needReset)
                    PartyFatigueTracker.ToggleTent(mobileParty.Party, true);


                if (PartyFatigueTracker.Current.partyFatigueData[mobileParty].currentFatigue <= MathF.Epsilon)
                {
                    // Extremely tired
                    if (mobileParty.DefaultBehavior == AiBehavior.FleeToGate || mobileParty.ShortTermBehavior == AiBehavior.FleeToGate
                        || mobileParty.DefaultBehavior == AiBehavior.FleeToParty || mobileParty.ShortTermBehavior == AiBehavior.FleeToParty
                        || mobileParty.DefaultBehavior == AiBehavior.FleeToPoint || mobileParty.ShortTermBehavior == AiBehavior.FleeToPoint
                        || mobileParty.DefaultBehavior == AiBehavior.EngageParty || mobileParty.ShortTermBehavior == AiBehavior.EngageParty)
                    {
                        // DO nothing
                    }
                    else
                    {
                        // go to sleep
                        PartyFatigueTracker.Current.partyFatigueData[mobileParty].needReset = true;
                        SavePartyState(mobileParty);
                        mobileParty.SetMoveModeHold();
                        PartyFatigueTracker.ToggleTent(mobileParty.Party, true);
                        mobileParty.Ai.SetDoNotMakeNewDecisions(true);

                    }
                }
            }
        }

        private void SavePartyState(MobileParty mobileParty)
        {
            if (mobileParty.Army != null)
                PartyFatigueTracker.Current.partyFatigueData[mobileParty].armyAIFlags = mobileParty.Army.AIBehavior;
            PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehavior = mobileParty.DefaultBehavior;
            PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorTarget = mobileParty.TargetPosition;
            PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorObject = mobileParty.TargetSettlement;
            PartyFatigueTracker.Current.partyFatigueData[mobileParty].targetParty = mobileParty.TargetParty;
        }

        private void LoadPartyAIState(MobileParty mobileParty)
        {
            switch (PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehavior)
            {
                case AiBehavior.EngageParty: mobileParty.SetMoveEngageParty(PartyFatigueTracker.Current.partyFatigueData[mobileParty].targetParty); break;
                case AiBehavior.EscortParty: mobileParty.SetMoveEscortParty(PartyFatigueTracker.Current.partyFatigueData[mobileParty].targetParty); break;
                case AiBehavior.GoAroundParty: mobileParty.SetMoveGoAroundParty(PartyFatigueTracker.Current.partyFatigueData[mobileParty].targetParty); break;
                case AiBehavior.GoToPoint: mobileParty.SetMoveGoToPoint(PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorTarget); break;
                case AiBehavior.GoToSettlement: mobileParty.SetMoveGoToSettlement(PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorObject); break;
                case AiBehavior.RaidSettlement: mobileParty.SetMoveRaidSettlement(PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorObject); break;
                case AiBehavior.DefendSettlement: mobileParty.SetMoveDefendSettlement(PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorObject); break;
                case AiBehavior.BesiegeSettlement: mobileParty.SetMoveBesiegeSettlement(PartyFatigueTracker.Current.partyFatigueData[mobileParty].aiBehaviorObject); break;
                default: mobileParty.SetMoveModeHold(); break;
            }
            if (mobileParty.Army != null)
                mobileParty.Army.AIBehavior = PartyFatigueTracker.Current.partyFatigueData[mobileParty].armyAIFlags;
        }


    }
}
