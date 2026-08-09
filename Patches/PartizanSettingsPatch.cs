using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace Parmesan.Patches
{
    internal class PartizanSettingsPatch : ModulePatch
    {
        private static bool _loggedOnce;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotInternalSettingsController), nameof(BotInternalSettingsController.GetSettings));
        }

        [PatchPostfix]
        private static void Postfix(WildSpawnType role, BotSettingsComponents __result)
        {
            if (role != WildSpawnType.bossPartisan || __result == null)
                return;

            if (Plugin.FasterRoaming.Value && __result.Patrol != null && __result.Move != null)
            {
                __result.Patrol.CHANGE_WAY_TIME = Plugin.ChangeRouteTime.Value;
                __result.Patrol.RESERVE_TIME_STAY = Plugin.LoiterTime.Value;
                __result.Patrol.RESERVE_OUT_TIME = Plugin.LoiterTime.Value;
                __result.Patrol.CHANCE_TO_CHANGE_WAY_0_100 = 80f;
                __result.Patrol.SPRINT_BETWEEN_CACHED_POINTS = 1f;
                __result.Move.CAN_SPRINT_GO_TO_SOME_POINT = true;
            }

            if (Plugin.PlantTraps.Value && __result.Grenade != null)
            {
                __result.Grenade.CHEAT_START_GRENADE_PLACE = true;
            }

            if (!_loggedOnce)
            {
                _loggedOnce = true;
                Plugin.Dbg("applied roam/trap settings to bossPartisan (roam=" + Plugin.FasterRoaming.Value +
                           ", traps=" + Plugin.PlantTraps.Value + ")");
            }
        }
    }
}
