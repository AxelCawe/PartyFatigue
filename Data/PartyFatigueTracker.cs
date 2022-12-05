using PartyFatigue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using TaleWorlds.Engine;
using SandBox.View.Map;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade;

namespace PartyFatigue.Data
{
    public class PartyFatigueTracker
    {
        [SaveableField(1)]
        public Dictionary<MobileParty, PartyFatigueData> partyFatigueData;

        [SaveableField(2)]
        private static PartyFatigueTracker instance;

        public static bool is_sneak_mission = false;
        public static bool is_wish_mission = false;
        public static MobileParty test_party = null;

        public static PartyFatigueTracker Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartyFatigueTracker();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public void AddToDictionary(MobileParty mobileParty)
        {
            float fatigueRatio = ModCalculations.CalculateFatigueRate(mobileParty);
            PartyFatigueData data = new PartyFatigueData(1f, fatigueRatio, mobileParty.MemberRoster.TotalManCount);
            data.aiBehavior = mobileParty.DefaultBehavior;
            data.aiBehaviorObject = mobileParty.TargetSettlement;
            data.aiBehaviorTarget = mobileParty.TargetPosition;
            data.targetParty = mobileParty.TargetParty;
            Current.partyFatigueData.Add(mobileParty, data);
        }

        public PartyFatigueTracker()
        {
            partyFatigueData = new Dictionary<MobileParty, PartyFatigueData>();
        }

        public static void ToggleTent(PartyBase party, bool showTent)
        {
            PartyVisual partyVisuals = (PartyVisual)party.Visuals;
            GameEntity strategicEntity = partyVisuals.StrategicEntity;
            if (strategicEntity == null)
                return;
            if (showTent)
            {
                bool currentlyBusy = party.MobileParty.CurrentSettlement != null || party.MapEvent != null || party.SiegeEvent != null;
                if (!currentlyBusy)
                {
                    bool hasTent = false;
                    foreach (var child in strategicEntity.GetChildren())
                    {
                        if (child.Name == "Tent")
                            hasTent = true;
                    }

                    if (hasTent)
                    {
                        MatrixFrame matrixFrame2 = MatrixFrame.Identity;
                        matrixFrame2.Scale(Vec3.Zero);
                        bool hasVisuals2 = partyVisuals.HumanAgentVisuals != null;
                        if (hasVisuals2)
                        {
                            partyVisuals.HumanAgentVisuals.GetEntity().SetFrame(ref matrixFrame2);
                        }
                        bool flag62 = partyVisuals.MountAgentVisuals != null;
                        if (flag62)
                        {
                            partyVisuals.MountAgentVisuals.GetEntity().SetFrame(ref matrixFrame2);
                        }
                        strategicEntity.CheckResources(true, true);
                        return;


                    }
                    MatrixFrame matrix = MatrixFrame.Identity;
                    matrix.rotation.ApplyScaleLocal(1.2f);
                    if (strategicEntity.Scene == null)
                        return;
                    GameEntity gameEntity = GameEntity.CreateEmpty(strategicEntity.Scene, true);
                    gameEntity.Name = "Tent";
                    gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent", true, false), true);
                    gameEntity.SetFrame(ref matrix);
                    string text = null;
                    Hero leader = party.LeaderHero;
                    bool flag1 = ((leader != null) ? leader.ClanBanner : null) != null;
                    if (flag1)
                    {
                        text = party.LeaderHero.ClanBanner.Serialize();
                    }
                    bool isArmyAndArmyLeader = party.MobileParty.Army != null && party.MobileParty.Army.LeaderParty == party.MobileParty;
                    bool isBannerSerialized = !string.IsNullOrEmpty(text);
                    if (isBannerSerialized)
                    {
                        MatrixFrame identity2 = MatrixFrame.Identity;
                        identity2.origin.z = identity2.origin.z + (isArmyAndArmyLeader ? 0.2f : 0.15f);
                        identity2.rotation.RotateAboutUp(1.57079637f);
                        identity2.rotation.ApplyScaleLocal(0.2f * (isArmyAndArmyLeader ? 1f : 0.6f));
                        MetaMesh bannerOfCharacter = GetBannerOfCharacter(new Banner(text), "campaign_flag");
                        bannerOfCharacter.Frame = identity2;
                        gameEntity.AddMultiMesh(bannerOfCharacter, true);
                    }
                    strategicEntity.AddChild(gameEntity, false);
                    MatrixFrame matrixFrame = MatrixFrame.Identity;
                    matrixFrame.Scale(Vec3.Zero);
                    bool hasVisuals = partyVisuals.HumanAgentVisuals != null;
                    if (hasVisuals)
                    {
                        partyVisuals.HumanAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
                    }
                    bool flag6 = partyVisuals.MountAgentVisuals != null;
                    if (flag6)
                    {
                        partyVisuals.MountAgentVisuals.GetEntity().SetFrame(ref matrixFrame);
                    }



                    strategicEntity.CheckResources(true, true);
                }
            }
            else
            {
                IEnumerable<GameEntity> children = strategicEntity.GetChildren();
                for (int i = children.Count<GameEntity>() - 1; i > -1; i--)
                {
                    GameEntity gameEntity2 = children.ElementAt(i);
                    bool flag7 = gameEntity2.Name != "Tent";
                    if (!flag7)
                    {
                        strategicEntity.RemoveChild(gameEntity2, false, false, false, 0);
                    }
                }
                strategicEntity.CheckResources(true, true);
            }
        }

        private static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName)
        {
            MetaMesh copy = MetaMesh.GetCopy(bannerMeshName, true, false);
            for (int i = 0; i < copy.MeshCount; i++)
            {
                Mesh meshAtIndex = copy.GetMeshAtIndex(i);
                bool flag = !meshAtIndex.HasTag("dont_use_tableau");
                if (flag)
                {
                    Material material = meshAtIndex.GetMaterial();
                    Material tableauMaterial = null;
                    Tuple<Material, BannerCode> key = new Tuple<Material, BannerCode>(material, BannerCode.CreateFrom(banner));
                    bool flag2 = MapScreen.Instance._characterBannerMaterialCache.ContainsKey(key);
                    if (flag2)
                    {
                        tableauMaterial = MapScreen.Instance._characterBannerMaterialCache[key];
                    }
                    else
                    {
                        tableauMaterial = material.CreateCopy();
                        Action<Texture> action = delegate (Texture tex)
                        {
                            tableauMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, tex);
                            uint num = (uint)tableauMaterial.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
                            ulong shaderFlags = tableauMaterial.GetShaderFlags();
                            tableauMaterial.SetShaderFlags(shaderFlags | (ulong)num);
                        };
                        banner.GetTableauTextureLarge(action);
                        MapScreen.Instance._characterBannerMaterialCache[key] = tableauMaterial;
                    }
                    meshAtIndex.SetMaterial(tableauMaterial);
                }
            }
            return copy;
        }
    }
}
