using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace PartyFatigue.Data
{
    public class PartyFatigueData
    {
        [SaveableField(1)]
        public float fatigueRate;
        [SaveableField(2)]
        public float currentFatigue;
        [SaveableField(3)]
        public int manCount;
        [SaveableField(4)]
        public AiBehavior aiBehavior;
        [SaveableField(5)]
        public Settlement aiBehaviorObject;
        [SaveableField(6)]
        public Vec2 aiBehaviorTarget;
        [SaveableField(7)]
        public bool needRecovery;
        [SaveableField(8)]
        public MobileParty targetParty;
        [SaveableField(9)]
        public IMapPoint armyAiBehaviorObject;
        [SaveableField(10)]
        public Army.AIBehaviorFlags armyAiBehaviorFlags;
        [SaveableField(11)]
        public bool needResetArmy = false;
        [SaveableField(12)]
        public bool needReset;
        [SaveableField(13)]
        public bool isBusy;
        [SaveableField(14)]
        public bool isFleeing;
        [SaveableField(15)]
        public int discoveredState = 0; //0 - default， 1 - discovered， 2-undsicovered
        [SaveableField(16)]
        public Army.AIBehaviorFlags armyAIFlags;


        public PartyFatigueData(float currentFatigue, float _reduce_rate, int manCount)
        {
            this.fatigueRate = _reduce_rate;
            this.currentFatigue = currentFatigue;
            this.manCount = manCount;
            needRecovery = false;
            isBusy = false;
            isFleeing = false;
        }

        public PartyFatigueData()
        {
            fatigueRate = 0;
            currentFatigue = 0;
            manCount = 0;
            needRecovery = false;        
            isBusy = false;
            isFleeing = false;
        }
    }
}
