using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace PartyFatigue.Data
{
    public class PartyFatigueInfoNode
    {
        [SaveableField(1)]
        public string info;
        [SaveableField(2)]
        public float time;
        public PartyFatigueInfoNode(string info, float time)
        {
            this.info = info;
            this.time = time;
        }
    }
}
