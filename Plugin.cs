using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Parmesan.Patches;

namespace Parmesan
{
    public enum PartizanMode
    {
        Vanilla,
        IncludeAiPmcs,
        AiPmcsOnly,
    }

    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "com.20fpsguy.parmesan";
        public const string NAME = "Parmesan - Partizan Rework";
        public const string VERSION = "1.0.0";

        public static ManualLogSource Log;
        public static ConfigEntry<PartizanMode> Mode;
        public static ConfigEntry<bool> Debug;

        public static ConfigEntry<bool> FasterRoaming;
        public static ConfigEntry<float> ChangeRouteTime;
        public static ConfigEntry<float> LoiterTime;
        public static ConfigEntry<bool> PlantTraps;

        private void Awake()
        {
            Log = Logger;

            Mode = Config.Bind(
                "1. Targeting",
                "Aggro mode",
                PartizanMode.IncludeAiPmcs,
                "Who Partizan is allowed to hunt. IncludeAiPmcs: you + AI PMCs (random pick). " +
                "AiPmcsOnly: only AI PMCs, never you. Vanilla: stock (hunts you only).");

            Debug = Config.Bind(
                "2. Misc",
                "Debug logging",
                false,
                "Log every target pick / trigger redirect to the BepInEx console. For troubleshooting only.");

            FasterRoaming = Config.Bind(
                "3. Roaming",
                "Faster roaming",
                true,
                "Make Partizan prowl the map instead of loitering. Lowers his change-route / stay timers and lets him sprint to patrol points.");

            ChangeRouteTime = Config.Bind(
                "3. Roaming",
                "Change route every (sec)",
                30f,
                new ConfigDescription("How often he re-routes while roaming (stock is ~9325, i.e. basically never).",
                    new AcceptableValueRange<float>(5f, 600f)));

            LoiterTime = Config.Bind(
                "3. Roaming",
                "Max loiter at a point (sec)",
                12f,
                new ConfigDescription("How long he lingers at a patrol point before moving on (stock 72).",
                    new AcceptableValueRange<float>(0f, 120f)));

            PlantTraps = Config.Bind(
                "4. Traps",
                "Plant tripwire traps",
                true,
                "Enable his grenade-trap (tripwire) planting. He spawns with grenades by default, so this gives him something to plant.");

            new PartizanTargetPatch().Enable();
            new PartizanTriggerPatch().Enable();
            new PartizanSettingsPatch().Enable();

            Log.LogInfo(NAME + " " + VERSION + " loaded. Mode=" + Mode.Value +
                        ", roam=" + FasterRoaming.Value + ", traps=" + PlantTraps.Value);
        }

        public static void Dbg(string msg)
        {
            if (Debug != null && Debug.Value)
                Log.LogInfo("[Parmesan] " + msg);
        }
    }
}
