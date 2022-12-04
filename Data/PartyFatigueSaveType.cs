using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.SaveSystem;

namespace PartyFatigue.Data
{

    public class PartyFatigueSaveType : SaveableTypeDefiner
    {
        public PartyFatigueSaveType() : base(246256563) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(PartyFatigueData), 1);
            AddClassDefinition(typeof(PartyFatigueTracker), 2);
        }
        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<MobileParty, PartyFatigueData>));         
        }
    }
}
