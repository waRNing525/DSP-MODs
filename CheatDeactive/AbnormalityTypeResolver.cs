using System;
using System.Reflection;
using HarmonyLib;

namespace CheatDeactive
{
    internal static class AbnormalityTypeResolver
    {
        private const string PreferredTypeFullName = "ABN.GameAbnormalityData_0925";
        private const string AbnormalityNamespace = "ABN";
        private const string AbnormalityTypePrefix = "GameAbnormalityData_";

        public static Type Resolve()
        {
            Type preferredType = AccessTools.TypeByName(PreferredTypeFullName);
            if (preferredType != null)
            {
                return preferredType;
            }

            // DSP appends a version suffix to this type name, so fall back to a prefix scan
            // if the exact type name changes in a later game update.
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type resolvedType = FindAbnormalityType(assembly);
                if (resolvedType != null)
                {
                    CheatDeactivePlugin.Log.LogInfo($"Resolved abnormality data type dynamically: {resolvedType.FullName}");
                    return resolvedType;
                }
            }

            return null;
        }

        private static Type FindAbnormalityType(Assembly assembly)
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
                if (candidate.Namespace == AbnormalityNamespace &&
                    candidate.Name.StartsWith(AbnormalityTypePrefix, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
