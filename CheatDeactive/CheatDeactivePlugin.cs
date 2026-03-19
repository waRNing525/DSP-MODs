using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CheatDeactive
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class CheatDeactivePlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "waRNing.dsp.plugins.CheatDeactive";
        public const string PluginName = "CheatDeactive";
        public const string PluginVersion = "1.1.0";

        internal static ManualLogSource Log;

        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;
            harmony = new Harmony(PluginGuid);

            int appliedPatchCount = PatchRegistrar.Apply(harmony);
            Log.LogInfo($"{PluginName} loaded. Applied {appliedPatchCount} patches to abnormal checks and achievement ban flags.");
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        internal static void ClearAchievementBanFlag(GameHistoryData history)
        {
            // In the decompiled game code, this flag is set by
            // GameHistoryData.AddPropertyItemConsumption(..., banAchievement: true)
            // and checked again in AchievementSystem.UnlockAchievement(...).
            if (history != null)
            {
                history.hasUsedPropertyBanAchievement = false;
            }
        }
    }
}
