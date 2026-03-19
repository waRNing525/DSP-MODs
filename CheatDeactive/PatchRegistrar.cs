using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace CheatDeactive
{
    internal static class PatchRegistrar
    {
        public static int Apply(Harmony harmony)
        {
            List<PatchDefinition> patchDefinitions = new List<PatchDefinition>();

            Type abnormalityType = AbnormalityTypeResolver.Resolve();
            if (abnormalityType == null)
            {
                CheatDeactivePlugin.Log.LogWarning("Unable to find the abnormality data type. Abnormality checks will not be bypassed.");
            }
            else
            {
                patchDefinitions.AddRange(CreateAbnormalityPatches(abnormalityType));
            }

            patchDefinitions.AddRange(CreateAchievementBanFlagPatches());

            int appliedPatchCount = 0;
            foreach (PatchDefinition patchDefinition in patchDefinitions)
            {
                appliedPatchCount += patchDefinition.Apply(harmony);
            }

            return appliedPatchCount;
        }

        private static IEnumerable<PatchDefinition> CreateAbnormalityPatches(Type abnormalityType)
        {
            // Decompiled source:
            // - GameAbnormalityData_0925.TriggerAbnormality(...) records evidence into runtimeDatas.
            // - NothingAbnormal() and IsAbnormalTriggerred(...) read those records for save/UI status.
            yield return PatchDefinition.CreatePrefix(
                "Force abnormality summary checks to stay clean",
                AccessTools.Method(abnormalityType, "NothingAbnormal"),
                typeof(AbnormalityPatchCallbacks),
                nameof(AbnormalityPatchCallbacks.AlwaysReportNormal));

            yield return PatchDefinition.CreatePrefix(
                "Prevent abnormality records from being written",
                AccessTools.Method(abnormalityType, "TriggerAbnormality"),
                typeof(AbnormalityPatchCallbacks),
                nameof(AbnormalityPatchCallbacks.SkipAbnormalityRegistration));

            yield return PatchDefinition.CreatePrefix(
                "Force per-id abnormality lookups to stay clear",
                AccessTools.Method(abnormalityType, "IsAbnormalTriggerred"),
                typeof(AbnormalityPatchCallbacks),
                nameof(AbnormalityPatchCallbacks.AlwaysReportNotTriggered));
        }

        private static IEnumerable<PatchDefinition> CreateAchievementBanFlagPatches()
        {
            // Decompiled source:
            // - GameHistoryData.Import(...) restores hasUsedPropertyBanAchievement from save data.
            // - GameHistoryData.AddPropertyItemConsumption(...) sets the same flag with |= banAchievement.
            // - AchievementSystem.UnlockAchievement(...) checks the flag right before unlocking.
            yield return PatchDefinition.CreatePostfix(
                "Clear the imported achievement-ban flag",
                AccessTools.Method(typeof(GameHistoryData), nameof(GameHistoryData.Import), new[] { typeof(BinaryReader), typeof(bool) }),
                typeof(AchievementBanFlagPatchCallbacks),
                nameof(AchievementBanFlagPatchCallbacks.ClearFlagOnHistoryInstance));

            yield return PatchDefinition.CreatePostfix(
                "Clear the achievement-ban flag after property consumption is recorded",
                AccessTools.Method(typeof(GameHistoryData), nameof(GameHistoryData.AddPropertyItemConsumption), new[] { typeof(int), typeof(int), typeof(bool) }),
                typeof(AchievementBanFlagPatchCallbacks),
                nameof(AchievementBanFlagPatchCallbacks.ClearFlagOnHistoryInstance));

            yield return PatchDefinition.CreatePrefix(
                "Clear the global history flag before achievement unlock checks",
                AccessTools.Method(typeof(AchievementSystem), nameof(AchievementSystem.UnlockAchievement), new[] { typeof(int), typeof(long) }),
                typeof(AchievementBanFlagPatchCallbacks),
                nameof(AchievementBanFlagPatchCallbacks.ClearFlagOnGameMainHistory));
        }
    }
}
