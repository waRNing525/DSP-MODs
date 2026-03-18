using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CheatDeactive
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class CheatDeactivePlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "waRNing.dsp.plugins.CheatDeactive";
        public const string PluginName = "CheatDeactive";
        public const string PluginVersion = "1.1.0";

        internal static ManualLogSource Log;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;
            _harmony = new Harmony(PluginGuid);

            int appliedPatchCount = PatchRegistrar.Apply(_harmony);
            Log.LogInfo($"{PluginName} loaded. Applied {appliedPatchCount} patches to abnormal checks and achievement ban flags.");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }

        internal static void ClearAchievementBanFlag(GameHistoryData history)
        {
            if (history != null)
            {
                history.hasUsedPropertyBanAchievement = false;
            }
        }
    }

    internal static class PatchRegistrar
    {
        private const string AbnormalityTypeFullName = "ABN.GameAbnormalityData_0925";
        private const string AbnormalityTypePrefix = "GameAbnormalityData_";

        public static int Apply(Harmony harmony)
        {
            int appliedPatchCount = 0;

            Type abnormalityType = FindAbnormalityType();
            if (abnormalityType == null)
            {
                CheatDeactivePlugin.Log.LogWarning("Unable to find the abnormality data type. Abnormality checks will not be bypassed.");
            }
            else
            {
                appliedPatchCount += PatchPrefix(harmony, AccessTools.Method(abnormalityType, "NothingAbnormal"), typeof(AbnormalityPatchCallbacks), nameof(AbnormalityPatchCallbacks.ForceTrueResult));
                appliedPatchCount += PatchPrefix(harmony, AccessTools.Method(abnormalityType, "TriggerAbnormality"), typeof(AbnormalityPatchCallbacks), nameof(AbnormalityPatchCallbacks.SkipOriginal));
                appliedPatchCount += PatchPrefix(harmony, AccessTools.Method(abnormalityType, "IsAbnormalTriggerred"), typeof(AbnormalityPatchCallbacks), nameof(AbnormalityPatchCallbacks.ForceFalseResult));
            }

            appliedPatchCount += PatchPostfix(
                harmony,
                AccessTools.Method(typeof(GameHistoryData), nameof(GameHistoryData.Import), new[] { typeof(BinaryReader), typeof(bool) }),
                typeof(HistoryPatchCallbacks),
                nameof(HistoryPatchCallbacks.ClearFlagOnHistoryInstance));

            appliedPatchCount += PatchPostfix(
                harmony,
                AccessTools.Method(typeof(GameHistoryData), nameof(GameHistoryData.AddPropertyItemConsumption), new[] { typeof(int), typeof(int), typeof(bool) }),
                typeof(HistoryPatchCallbacks),
                nameof(HistoryPatchCallbacks.ClearFlagOnHistoryInstance));

            appliedPatchCount += PatchPrefix(
                harmony,
                AccessTools.Method(typeof(AchievementSystem), nameof(AchievementSystem.UnlockAchievement), new[] { typeof(int), typeof(long) }),
                typeof(HistoryPatchCallbacks),
                nameof(HistoryPatchCallbacks.ClearFlagOnGameMainHistory));

            return appliedPatchCount;
        }

        private static Type FindAbnormalityType()
        {
            Type type = AccessTools.TypeByName(AbnormalityTypeFullName);
            if (type != null)
            {
                return type;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = Array.FindAll(ex.Types, candidate => candidate != null);
                }

                foreach (Type candidate in types)
                {
                    if (candidate.Namespace == "ABN" &&
                        candidate.Name.StartsWith(AbnormalityTypePrefix, StringComparison.Ordinal))
                    {
                        CheatDeactivePlugin.Log.LogInfo($"Resolved abnormality data type dynamically: {candidate.FullName}");
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static int PatchPrefix(Harmony harmony, MethodBase original, Type patchType, string prefixMethodName)
        {
            return Patch(harmony, original, prefix: AccessTools.Method(patchType, prefixMethodName), postfix: null);
        }

        private static int PatchPostfix(Harmony harmony, MethodBase original, Type patchType, string postfixMethodName)
        {
            return Patch(harmony, original, prefix: null, postfix: AccessTools.Method(patchType, postfixMethodName));
        }

        private static int Patch(Harmony harmony, MethodBase original, MethodInfo prefix, MethodInfo postfix)
        {
            if (original == null)
            {
                CheatDeactivePlugin.Log.LogWarning("Skipped a patch because the target method was not found.");
                return 0;
            }

            harmony.Patch(
                original,
                prefix: prefix == null ? null : new HarmonyMethod(prefix),
                postfix: postfix == null ? null : new HarmonyMethod(postfix));

            return 1;
        }
    }

    internal static class AbnormalityPatchCallbacks
    {
        public static bool ForceTrueResult(ref bool __result)
        {
            __result = true;
            return false;
        }

        public static bool ForceFalseResult(ref bool __result)
        {
            __result = false;
            return false;
        }

        public static bool SkipOriginal()
        {
            return false;
        }
    }

    internal static class HistoryPatchCallbacks
    {
        public static void ClearFlagOnHistoryInstance(GameHistoryData __instance)
        {
            CheatDeactivePlugin.ClearAchievementBanFlag(__instance);
        }

        public static void ClearFlagOnGameMainHistory()
        {
            CheatDeactivePlugin.ClearAchievementBanFlag(GameMain.history);
        }
    }
}
