using System.Collections.Generic;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace Parmesan.Patches
{
    internal class PartizanTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(AIPlaceLogicPartisan), nameof(AIPlaceLogicPartisan.OnPlayerEnter));
        }

        [PatchPrefix]
        private static bool Prefix(Player player, AIPlaceLogicPartisan __instance)
        {
            if (Plugin.Mode.Value == PartizanMode.Vanilla)
                return true;

            IPlayer target = __instance.PlayerWithWorstKarma();
            if (target == null)
            {
                Plugin.Dbg("trigger fired but no eligible target — ignoring.");
                return false;
            }

            var trav = Traverse.Create(__instance);
            var spawns = trav.Field("allPartisans").GetValue<List<BossLocationSpawn>>();
            if (spawns != null)
            {
                foreach (var spawn in spawns)
                {
                    var offset = new Vector3(__instance.RndCoord(), 0f, __instance.RndCoord());
                    spawn.PerfectPos = target.Position + offset;
                }
            }

            trav.Field("_isPartisansWavesinited").SetValue(false);

            Singleton<GlobalEventDispatcher>.Instance.AnyEvent("PARTISAN_TRIGGER");

            Plugin.Dbg("trigger redirected from " + (player?.Profile?.Nickname ?? "?") +
                       " to " + (target.Profile?.Nickname ?? "?") + " (AI=" + target.IsAI + ")");
            return false;
        }
    }
}
