using System.Collections.Generic;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace Parmesan.Patches
{
    internal class PartizanTargetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(AIPlaceLogicPartisan), nameof(AIPlaceLogicPartisan.PlayerWithWorstKarma));
        }

        [PatchPrefix]
        private static bool Prefix(ref IPlayer __result)
        {
            if (Plugin.Mode.Value == PartizanMode.Vanilla)
                return true;

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.AllAlivePlayersList == null)
                return true;

            IPlayer pick = PartizanTargeting.PickTarget(gameWorld.AllAlivePlayersList, Plugin.Mode.Value);
            __result = pick;
            Plugin.Dbg("method_0 picked: " + (pick == null ? "<none>" : pick.Profile?.Nickname + " (AI=" + pick.IsAI + ", side=" + pick.Side + ")"));
            return false;
        }
    }

    internal static class PartizanTargeting
    {
        public static bool IsAiPmc(Player p)
        {
            return p != null && p.IsAI && (p.Side == EPlayerSide.Bear || p.Side == EPlayerSide.Usec);
        }

        public static IPlayer PickTarget(List<Player> players, PartizanMode mode)
        {
            var candidates = new List<Player>();
            foreach (var p in players)
            {
                if (p == null || p.HealthController == null || !p.HealthController.IsAlive)
                    continue;

                bool isHuman = !p.IsAI;
                bool eligible = mode == PartizanMode.AiPmcsOnly
                    ? IsAiPmc(p)
                    : (isHuman || IsAiPmc(p));

                if (eligible)
                    candidates.Add(p);
            }

            if (candidates.Count == 0)
                return null;

            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }
    }
}
