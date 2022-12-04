using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyFatigue.Settings
{
    internal static class GlobalModSettings
    {
        public const string moduleName = "PartyFatigue";

        public const float hoursForInfoToExpire = 24 * 14; // 14 days

        public const float excitedFatigue = 0.8f;
        public const float normalFatigue = 0.3f;
        public const float tiredFatigue = 0.1f; // anything < 0.1f will means the party is dead tired

        public const float maxDayRecoverRate = 1f;
        public const float minDayRecoverRate = 0.01f;
        
        public const float maxNightRecoverRate = 1f;
        public const float minNightRecoverRate = 0.01f;

        public const float minRestingRecoverRate = 1f;
        public const float maxRestingRecoverRate = 100f;

        public const float minSettlementRecoverRate = 1f;
        public const float maxSettlementRecoverRate = 100f;

        public const float minFatigueRate = 0.01f;
        public const float maxFatigueRate = 1f;

        public const float minMinTroopMultiplierSpeed = 0.01f;
        public const float maxMinTroopMultiplierSpeed = 0.99f;

        public const float minMiscPartyFatigueMultiplier = 0.01f;
        public const float maxMiscPartyFatigueMultiplier = 10f;
    }
}
