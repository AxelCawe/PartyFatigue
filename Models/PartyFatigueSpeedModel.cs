using PartyFatigue.Data;
using PartyFatigue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace PartyFatigue.Models
{
    class PartyFatigueSpeedModel : DefaultPartySpeedCalculatingModel
    {
        public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
        {
            ExplainedNumber baseSpeed = base.CalculateFinalSpeed(mobileParty, finalSpeed);
            if (PartyFatigueTracker.Current == null)
                return baseSpeed;

            PartyFatigueData data = null;
            PartyFatigueTracker.Current.partyFatigueData.TryGetValue(mobileParty, out data);    
            if (data != null) 
            {
                baseSpeed.AddFactor(ModCalculations.CalculateSpeedRatio(data) - 1f, new TaleWorlds.Localization.TextObject("Party Fatigue Boost"));
                
                return baseSpeed;
            }
            return baseSpeed;
        }
    }
}
