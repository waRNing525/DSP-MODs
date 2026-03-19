namespace CheatDeactive
{
    internal static class AbnormalityPatchCallbacks
    {
        // GameAbnormalityData_0925.NothingAbnormal() is queried by save/UI/achievement code
        // to decide whether the current save should be treated as clean.
        public static bool AlwaysReportNormal(ref bool __result)
        {
            __result = true;
            return false;
        }

        // TriggerAbnormality(...) writes runtime abnormality records and emits warning logs.
        // Skipping the original prevents the record from ever being created.
        public static bool SkipAbnormalityRegistration()
        {
            return false;
        }

        // UIAchievementPanel iterates abnormal ids and asks IsAbnormalTriggerred(i).
        // Returning false keeps the details panel empty even if some code path queries it directly.
        public static bool AlwaysReportNotTriggered(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    internal static class AchievementBanFlagPatchCallbacks
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
