using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;
using System;

namespace ValheimMod
{
    [BepInPlugin("SkYMaN.ValheimMod_ImprovedBoar", "ImprovedBoar", "2.0.0")]
    [BepInProcess("valheim.exe")]
    public class Valheim_ImprovedBoar : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony("SkYMaN.ValheimMod_ImprovedBoar");

        private static ConfigEntry<bool> _configEntryIsCommandableBoar;

        private static ConfigEntry<bool> _configEntryIsMonsterFaction;

        private const string BoarEntityName = "Boar(Clone)";

        void Awake()
        {
            Debug.Log("Start loading mod - ImprovedBoar");

            _harmony.PatchAll();

            _configEntryIsCommandableBoar = Config.Bind<bool>("General", "Commandable Boar", true, "Commandable Boar");
            _configEntryIsMonsterFaction = Config.Bind<bool>("General", "Monster Faction Boar", false, "Monster Faction Boar");

            Debug.Log("Finished loading mod - ImprovedBoar");
        }

        void OnDestroy() => _harmony.UnpatchSelf();

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
        class BoarHoverTextPatch
        {
            const string FollowText = " / Follow";
            const string StopText = " / Stay";
            static void Postfix(MonsterAI ___m_monsterAI, ref string __result)
            {
                var followTarget = ___m_monsterAI.GetFollowTarget();

                string actionHoverText = followTarget != null ? StopText : FollowText;

                if (_configEntryIsCommandableBoar.Value)
                {
                    int idx = __result.IndexOf("Pet");
                    __result = __result.Insert(idx + 3, actionHoverText);
                }
                else
                {
                    __result = __result.Replace(actionHoverText, String.Empty);
                }
            }
        }

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
        class BoarCommandsPatch
        {
            static void Prefix(Tameable __instance)
            {
                if (!__instance.name.Equals(BoarEntityName))
                {
                    return;
                }

                __instance.m_commandable = _configEntryIsCommandableBoar.Value;
            }
        }

        [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.IsEnemy), new Type[] { typeof(Character), typeof(Character) })]
        class BoarRelationsPatch
        {
            /// <param name="a">Source entity</param>
            /// <param name="b">Target entity</param>
            static bool Prefix(ref bool __result, ref Character a, ref Character b)
            {
                bool shouldBoarIgnoreEnemy = a.name.Equals(BoarEntityName) && a.IsTamed() && _configEntryIsMonsterFaction.Value;
                bool shouldEnemyIgnoreBoar = b.name.Equals(BoarEntityName) && b.IsTamed() && _configEntryIsMonsterFaction.Value;

                if (shouldBoarIgnoreEnemy || shouldEnemyIgnoreBoar)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }
    }
}