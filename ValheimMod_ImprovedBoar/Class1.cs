using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimMod
{
    [BepInPlugin("SkYMaN.ValheimMod_ImprovedBoar", "ImprovedBoar", "1.0.2")]
    [BepInProcess("valheim.exe")]
    public class Valheim_TeleportActivationDistanceClass : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("SkYMaN.ValheimMod_ImprovedBoar");
        private static ConfigEntry<bool> configEntry_CommandableBoar;
        private static ConfigEntry<bool> configEntry_AfraidFireBoar;
        private static ConfigEntry<bool> configEntry_MonsterFaction;
        void Awake()
        {
            Debug.Log("Starting loading mod - ImprovedBoar");
            harmony.PatchAll();
            configEntry_CommandableBoar = Config.Bind<bool>("General", "Commandable Boar", false, "Commandable Boar");
            configEntry_AfraidFireBoar = Config.Bind<bool>("General", "Boar Afraid Fire", true, "Boar Afraid Fire");
            configEntry_MonsterFaction = Config.Bind<bool>("General", "Monster Faction Boar", false, "Monster Faction Boar");
            Debug.Log("Finish loading mod - ImprovedBoar");
        }
        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
        [HarmonyPatch(typeof(Tameable), "Interact")]
        class Patch_Boar
        {
            static void Prefix()
            {
                foreach (BaseAI obj in BaseAI.GetAllInstances())
                {
                    if (obj.name == "Boar(Clone)")
                    {
                        obj.m_avoidFire = false;
                        obj.m_afraidOfFire = configEntry_AfraidFireBoar.Value;
                        Tameable tamedcomponent = obj.GetComponent<Tameable>();
                        if ((bool)tamedcomponent)
                        {
                            tamedcomponent.m_commandable = configEntry_CommandableBoar.Value;
                            Character character = obj.GetComponent<Character>();
                            if (configEntry_MonsterFaction.Value == true)
                            {
                                character.m_faction = Character.Faction.ForestMonsters;
                            }
                            else
                            {
                                character.m_faction = Character.Faction.AnimalsVeg;
                            }
                        }
                    }
                    else if(obj.name == "Boar_piggy(Clone)")
                    {
                        obj.m_avoidFire = false;
                        obj.m_afraidOfFire = configEntry_AfraidFireBoar.Value;
                    }
                }
            }
        }
    }
}