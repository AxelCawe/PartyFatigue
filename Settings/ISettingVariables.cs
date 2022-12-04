using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyFatigue.Settings
{
    internal interface ISettingVariables
    {
        float DayRecoveryRate { get; set; }
        float NightRecoveryRate { get; set; }
        float RestingRecoveryBonus { get; set; }
        float SettlementRecoveryBonus { get; set; }
        float FatigueRate { get; set; }

        float MiscPartyFatigueMultiplier { get;}
    }
}
