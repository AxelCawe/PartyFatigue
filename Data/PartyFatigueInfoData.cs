using PartyFatigue.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace PartyFatigue.Data
{
    public class PartyFatigueInfoData
    {
        [SaveableField(1)]
        public Queue<PartyFatigueInfoNode> data;

        [SaveableField(2)]
        public TextObject name;

        public PartyFatigueInfoData(TextObject name)
        {
            data = new Queue<PartyFatigueInfoNode>();
            //data = new Dictionary<float, string>();
            this.name = name;
        }

        public void AddInfo(string text, float time)
        {
            PartyFatigueInfoNode newNode = new PartyFatigueInfoNode(text, time);
            data.Enqueue(newNode);
        }

        public void RemoveOldInfo()
        {
            float expiryHour = (float)(CampaignTime.Now.ToHours - GlobalModSettings.hoursForInfoToExpire);
            while (!data.IsEmpty()) // Loop through the queue
            {
                PartyFatigueInfoNode node = data.Peek(); // ref the value at the first of the queue
                if (node.time < expiryHour)
                    data.Dequeue();
                else break; // end the loop as we are certain that later nodes will not expire. Queue was already sorted based on time
            }
        }
    }
}
