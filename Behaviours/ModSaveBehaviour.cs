using PartyFatigue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace PartyFatigue.Behaviours
{
    class ModSaveBehaviour : CampaignBehaviorBase
    {
       
        public override void RegisterEvents()
        {
        }        

        public override void SyncData(IDataStore dataStore)
        {
            PartyFatigueTracker instance = PartyFatigueTracker.Current;
            dataStore.SyncData("PartyFatigueTracker", ref instance);
            PartyFatigueTracker.Current = instance;
        }


    }
}
