using MCM.Abstractions.Base.Global;
using PartyFatigue.Data;
using PartyFatigue.Helpers;
using PartyFatigue.Settings;
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
    class HourlyTickBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnEveryHour));
            OnEveryHour();
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }

        void OnEveryHour()
        {

            foreach (var party in PartyFatigueTracker.Current.partyFatigueData)
            {
                // Update fatigue rate
                party.Value.fatigueRate = ModCalculations.CalculateFatigueRate(party.Key);

                party.Value.currentFatigue += ModCalculations.CalculateFinalFatigueUpdateValue(party.Key);
                party.Value.currentFatigue = MathF.Clamp(party.Value.currentFatigue, 0f, 1f);

                if (party.Key == Campaign.Current.MainParty && party.Key.CurrentSettlement != null)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Party Fatigue: {party.Value.currentFatigue}"));
                }
            }
        }
        
    }
}
