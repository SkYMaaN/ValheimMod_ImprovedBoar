using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimMod
{
    [BepInPlugin("SkYMaN.ValheimMod_ImprovedBoar", "ImprovedBoar", "1.0.1")]
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
            configEntry_CommandableBoar = Config.Bind<bool>("General", "Commandable Boar", true, "Commandable Boar");
            configEntry_AfraidFireBoar = Config.Bind<bool>("General", "Boar Afraid Fire", true, "Boar Afraid Fire");
            configEntry_MonsterFaction = Config.Bind<bool>("General", "Monster Faction Boar", false, "Monster Faction Boar");
            Debug.Log("Finish loading mod - ImprovedBoar");
        }
        [HarmonyPatch(typeof(Tameable), "Interact")]
        class Patch_Boar
        {
            static void Prefix(ref Character ___m_character)
            {
                Tameable tameable = ___m_character.GetComponent<Tameable>();
                if (tameable.name == "Boar(Clone)")
                {
                    tameable.m_commandable = configEntry_CommandableBoar.Value;
                    MonsterAI monsterAI = ___m_character.GetComponent<MonsterAI>();
                    monsterAI.m_afraidOfFire = configEntry_AfraidFireBoar.Value;
                    if (configEntry_MonsterFaction.Value == true)
                    {
                        ___m_character.m_faction = Character.Faction.ForestMonsters;
                    }
                    else
                    {
                        ___m_character.m_faction = Character.Faction.AnimalsVeg;
                    }
                }
            }
        }
    }
}