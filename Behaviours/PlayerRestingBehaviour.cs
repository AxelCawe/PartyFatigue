using PartyFatigue.Data;
using PartyFatigue.Helpers;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace PartyFatigue.Behaviours
{
    class PlayerRestingBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(OnTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        void OnTick(float tick)
        {
            MobileParty playerParty = Campaign.Current.MainParty;
            if (playerParty.Army != null) // if player is in army
            {
                if (playerParty.Army.LeaderParty == playerParty) // if player is leading the army
                {
                    foreach (MobileParty mobileParty in playerParty.Army.LeaderPartyAndAttachedParties)
                    {
                        if (mobileParty.DefaultBehavior == AiBehavior.JoinParty)
                            PartyFatigueTracker.ToggleTent(mobileParty.Party, false);
                        else
                            PartyFatigueTracker.ToggleTent(mobileParty.Party, !ModCalculations.IsPartyMoving(playerParty));
                    }
                }
            }
            else
                PartyFatigueTracker.ToggleTent(playerParty.Party, !ModCalculations.IsPartyMoving(playerParty));
        }

    }
}
