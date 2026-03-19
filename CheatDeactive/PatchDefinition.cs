using System;
using System.Reflection;
using HarmonyLib;

namespace CheatDeactive
{
    internal sealed class PatchDefinition
    {
        public string Description { get; }

        private readonly MethodBase original;
        private readonly HarmonyMethod prefix;
        private readonly HarmonyMethod postfix;

        private PatchDefinition(string description, MethodBase original, MethodInfo prefix, MethodInfo postfix)
        {
            Description = description;
            this.original = original;
            this.prefix = prefix == null ? null : new HarmonyMethod(prefix);
            this.postfix = postfix == null ? null : new HarmonyMethod(postfix);
        }

        public static PatchDefinition CreatePrefix(string description, MethodBase original, Type patchType, string prefixMethodName)
        {
            return new PatchDefinition(description, original, AccessTools.Method(patchType, prefixMethodName), null);
        }

        public static PatchDefinition CreatePostfix(string description, MethodBase original, Type patchType, string postfixMethodName)
        {
            return new PatchDefinition(description, original, null, AccessTools.Method(patchType, postfixMethodName));
        }

        public int Apply(Harmony harmony)
        {
            if (original == null)
            {
                CheatDeactivePlugin.Log.LogWarning($"Skipped patch '{Description}' because the target method was not found.");
                return 0;
            }

            harmony.Patch(original, prefix: prefix, postfix: postfix);

            string declaringTypeName = original.DeclaringType == null ? "<unknown>" : original.DeclaringType.FullName;
            CheatDeactivePlugin.Log.LogInfo($"Applied patch: {Description} -> {declaringTypeName}.{original.Name}");
            return 1;
        }
    }
}
