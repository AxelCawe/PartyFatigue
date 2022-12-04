using PartyFatigue.Data;
using PartyFatigue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace PartyFatigue.Behaviours
{
    class FatigueUpdateBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            //CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameLoad));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(OnNewMobileParty));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(OnPartyRemove));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGame));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(OnDailyInterval));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(OnMobilePartyDestroyed));
            CampaignEvents.OnPartySizeChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(OnPartySizeChanged));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        void OnGameLoad(CampaignGameStarter gameStarter)
        {
            foreach (var party in PartyFatigueTracker.Current.partyFatigueData) 
            {
                party.Value.currentFatigue = ModCalculations.CalculateFatigueRate(party.Key);
            }
        }


        void OnNewMobileParty(MobileParty party)
        {
            if (party== null) return;
            if (!PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party))
                PartyFatigueTracker.Current.AddToDictionary(party);
        }

        void OnPartyRemove(PartyBase partyBase)
        {
            if (partyBase.IsMobile && partyBase.MobileParty != null && PartyFatigueTracker.Current.partyFatigueData.ContainsKey(partyBase.MobileParty))
                PartyFatigueTracker.Current.partyFatigueData.Remove(partyBase.MobileParty);
        }

        void OnNewGame(CampaignGameStarter gameStarter)
        {
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            for (int i = 0; i < parties.Count; ++i) 
            {
                if (parties[i] == null) continue;
                if (!PartyFatigueTracker.Current.partyFatigueData.ContainsKey(parties[i]))
                    PartyFatigueTracker.Current.AddToDictionary(parties[i]);
            }
        }

        // Not neccessary, but just in case
        void OnDailyInterval()
        {
            MBReadOnlyList<MobileParty> parties = Campaign.Current.MobileParties;
            for (int i = 0; i < parties.Count; i++)
            {
                if (parties[i] == null)
                {
                    continue;
                }
                if (!PartyFatigueTracker.Current.partyFatigueData.ContainsKey(parties[i]))
                {
                    PartyFatigueTracker.Current.AddToDictionary(parties[i]);
                }

            }
            var keys = new List<MobileParty>(PartyFatigueTracker.Current.partyFatigueData.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if (!keys[i].IsActive)
                {
                    PartyFatigueTracker.Current.partyFatigueData.Remove(keys[i]);
                }
            }
        }


        void OnMobilePartyDestroyed(MobileParty party, PartyBase attacker)
        {
            if (party != null && PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party))
                PartyFatigueTracker.Current.partyFatigueData.Remove(party);
        }

        void OnPartySizeChanged(PartyBase party)
        {
            if (party == null || PartyFatigueTracker.Current == null || !party.IsMobile)
            {
                return;
            }
            if (party.MobileParty != null && PartyFatigueTracker.Current.partyFatigueData.ContainsKey(party.MobileParty))
            {
                PartyFatigueTracker.Current.partyFatigueData[party.MobileParty].fatigueRate = ModCalculations.CalculateFatigueRate(party.MobileParty);
                PartyFatigueTracker.Current.partyFatigueData[party.MobileParty].manCount = party.MobileParty.MemberRoster.TotalManCount;
            }
        }
    }
}
