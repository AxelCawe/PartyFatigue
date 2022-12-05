using MCM.Abstractions.Base.Global;
using PartyFatigue.Settings;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;
using PartyFatigue.Data;
using PartyFatigue.Models;
using PartyFatigue.Behaviours;
using HarmonyLib;
using System.Reflection;

namespace PartyFatigue
{
    public class SubModule : MBSubModuleBase
    {

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.OnGameStart(game, starterObject);
            Initialize(game, starterObject);
        }
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("PartyFatigue").PatchAll(Assembly.GetExecutingAssembly());

            PartyFatigueTracker.Current = PartyFatigueTracker.Current;
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if (starter == null)
            {
                return;
            }

            starter.AddBehavior(new ModSaveBehaviour());
            starter.AddBehavior(new HourlyTickBehaviour());
            starter.AddBehavior(new FatigueUpdateBehaviour());
            starter.AddBehavior(new AiRestingBehaviour());
            starter.AddBehavior(new PlayerRestingBehaviour());
        }

        private void Initialize(Game game, IGameStarter gameStarter)
        {
            PartyFatigueTracker.Current = PartyFatigueTracker.Current;
            ReplaceModels(gameStarter as CampaignGameStarter);
            AddBehaviours(gameStarter as CampaignGameStarter);
        }

        private void ReplaceModels(CampaignGameStarter gameStarter)
        {
            IList<GameModel> models = gameStarter.Models as IList<GameModel>;
            if (models != null)
            {
                gameStarter.AddModel(new PartyFatigueSpeedModel());
            }
        }

       
    }
}
