using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryMultiplier
{
    [BepInPlugin("waRNing.dsp.plugins.FactoryMultiplier", "FactoryMultiplier", "2.2.0")]
    public class FactoryMultiplier : BaseUnityPlugin
    {
        internal enum MultiplierField
        {
            Smelt,
            Chemical,
            Refine,
            Assemble,
            Particle,
            Lab,
            Fractionator,
            Ejector,
            Silo,
            Gamma,
            WalkSpeed,
            Mining
        }

        internal static ManualLogSource Log;
        private static int walkspeed_tech;
        private static ConfigEntry<int> walkspeedMultiply;
        private static ConfigEntry<int> miningMultiply;
        private static ConfigEntry<int> smeltMultiply;
        private static ConfigEntry<int> chemicalMultiply;
        private static ConfigEntry<int> refineMultiply;
        private static ConfigEntry<int> assembleMultiply;
        private static ConfigEntry<int> particleMultiply;
        private static ConfigEntry<int> labMultiply;
        private static ConfigEntry<int> fractionatorMultiply;
        private static ConfigEntry<int> ejectorMultiply;
        private static ConfigEntry<int> siloMultiply;
        private static ConfigEntry<int> gammaMultiply;
        private static ConfigEntry<KeyboardShortcut> mainWindowHotkey;
        private static ConfigEntry<Boolean> Speed_set;

        private string walkspeedmulti_str = "";
        private string smeltmulti_str = "";
        private string chemicalmulti_str = "";
        private string refinemulti_str = "";
        private string assemblemulti_str = "";
        private string particlemulti_str = "";
        private string labmulti_str = "";
        private string fractionatormulti_str = "";
        private string ejectormulti_str = "";
        private string silomulti_str = "";
        private string gammamulti_str = "";
        private string miningmulti_str = "";
        private string tempText = "";
        private bool Showwindow = false;
        private FactoryMultiplierWindow window;

        private void Awake() { Log = base.Logger; }
        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(harmony_Patch), null);
            Translate.regAllTranslate();

            mainWindowHotkey = Config.Bind("Keyboard Shortcuts", "mainWindowHotkey", KeyboardShortcut.Deserialize("R + LeftControl"),
                "开关窗口的热键");
            walkspeedMultiply = Config.Bind("Config", "walkspeedMultiply", 1, "行走速度倍率");
            miningMultiply = Config.Bind("Config", "miningMultiply", 1, "采集速度倍率");
            smeltMultiply = Config.Bind("Config", "smeltMultiply", 1, "熔炉速度倍率");
            chemicalMultiply = Config.Bind("Config", "chemicalMultiply", 1, "化工厂速度倍率");
            refineMultiply = Config.Bind("Config", "refineMultiply", 1, "精炼速度倍率");
            assembleMultiply = Config.Bind("Config", "assembleMultiply", 1, "制造台速度倍率");
            particleMultiply = Config.Bind("Config", "particleMultiply", 1, "对撞机速度倍率");
            labMultiply = Config.Bind("Config", "labMultiply", 1, "研究站速度倍率");
            fractionatorMultiply = Config.Bind("Config", "fractionatorMultiply", 1, "分馏塔生产概率倍率");
            ejectorMultiply = Config.Bind("Config", "ejectorMultiply", 1, "弹射器速度倍率");
            siloMultiply = Config.Bind("Config", "siloMultiply", 1, "发射井速度倍率");
            gammaMultiply = Config.Bind("Config", "gammaMultiply", 1, "射线接受倍率");
            Speed_set = Config.Bind("Bool Config", "Speedsetting", false, "设置行走速度");

            //初始化窗口文本设置
            walkspeedmulti_str = walkspeedMultiply.Value.ToString();
            smeltmulti_str = smeltMultiply.Value.ToString();
            chemicalmulti_str = chemicalMultiply.Value.ToString();
            refinemulti_str = refineMultiply.Value.ToString();
            assemblemulti_str = assembleMultiply.Value.ToString();
            particlemulti_str = particleMultiply.Value.ToString();
            labmulti_str = labMultiply.Value.ToString();
            fractionatormulti_str = fractionatorMultiply.Value.ToString();
            ejectormulti_str = ejectorMultiply.Value.ToString();
            silomulti_str = siloMultiply.Value.ToString();
            gammamulti_str = gammaMultiply.Value.ToString();
            miningmulti_str = miningMultiply.Value.ToString();
        }

        private void Update()
        {
            ToggleWindow();
            if (window == null)
            {
                window = new FactoryMultiplierWindow(this);
            }
            window.Tick(Showwindow);
            if (Showwindow && window != null && !window.IsVisible)
            {
                Showwindow = false;
            }

            if (GameMain.isRunning) 
            {
                UpdateDirectWalkSpeed();
                FactorySystemPatcher();
                MiningSpeedScale_patch();
            }
                
        }

        private void ToggleWindow()
        {
            if (!GameMain.isRunning || GameMain.isPaused || GameMain.instance.isMenuDemo)
            {
                Showwindow = false;
                return;  //只有在游戏运行时才可以用快捷键唤出窗口
            }
            if (mainWindowHotkey.Value.IsDown())
            {
                Showwindow = !Showwindow;
                tempText = "";
            }
            if (Input.GetKeyDown("escape"))
            {
                Showwindow = false;
            }
        }
        private static int ParsePositiveIntOrDefault(string input, int defaultValue)
        {
            string digits = Regex.Replace(input ?? string.Empty, @"[^0-9]", "");
            if (string.IsNullOrEmpty(digits))
            {
                return defaultValue;
            }
            if (int.TryParse(digits, out int value))
            {
                return value;
            }
            return defaultValue;
        }

        private void UpdateDirectWalkSpeed()
        {
            if (Speed_set.Value)
            {
                int walkSpeed = ParsePositiveIntOrDefault(walkspeedmulti_str, 1);
                if (GameMain.mainPlayer?.mecha != null)
                {
                    GameMain.mainPlayer.mecha.walkSpeed = walkSpeed;
                }
                walkspeedMultiply.Value = walkSpeed;
            }
        }

        private void ApplyConfigValue(ref string textValue, ConfigEntry<int> config, string changeText, int defaultValue = 1)
        {
            int value = ParsePositiveIntOrDefault(textValue, defaultValue);
            config.Value = value;
            textValue = value.ToString();
            tempText = changeText.getTranslate() + textValue + "X";
        }

        private void ApplyWalkSpeedValue()
        {
            Speed_set.Value = false;
            int value = ParsePositiveIntOrDefault(walkspeedmulti_str, 1);
            walkspeedMultiply.Value = value;
            walkspeedmulti_str = value.ToString();
            tempText = "行走倍数更改为".getTranslate() + walkspeedmulti_str + "X";
        }

        internal bool DirectWalkSpeedMode
        {
            get => Speed_set.Value;
            set => Speed_set.Value = value;
        }

        internal bool IsWindowVisible => Showwindow;

        internal void SetWindowVisible(bool visible)
        {
            Showwindow = visible;
            if (!visible)
            {
                tempText = "";
            }
        }

        internal string GetWindowTitle()
        {
            return "工厂倍率设置".getTranslate();
        }

        internal string GetSectionTitle(bool isLeftSection)
        {
            return (isLeftSection ? "生产与科研" : "物流、能源与角色").getTranslate();
        }

        internal string GetStatusText()
        {
            if (!string.IsNullOrEmpty(tempText))
            {
                return tempText;
            }
            return "在这里修改倍率并点击应用按钮。".getTranslate();
        }

        internal string GetHotkeyHint()
        {
            return string.Format("快捷键：{0}".getTranslate(), mainWindowHotkey?.Value.ToString() ?? "Ctrl + R");
        }

        internal string GetWalkSpeedModeText()
        {
            return "直接设置行走速度（m/s）".getTranslate();
        }

        internal string GetApplyButtonText()
        {
            return "应用".getTranslate();
        }

        internal int GetCharacterLimit(MultiplierField field)
        {
            switch (field)
            {
                case MultiplierField.Fractionator:
                case MultiplierField.WalkSpeed:
                    return 2;
                default:
                    return 1;
            }
        }

        internal string GetFieldLabel(MultiplierField field)
        {
            switch (field)
            {
                case MultiplierField.Smelt:
                    return "熔炉倍率".getTranslate();
                case MultiplierField.Chemical:
                    return "化工厂倍率".getTranslate();
                case MultiplierField.Refine:
                    return "精炼厂倍率".getTranslate();
                case MultiplierField.Assemble:
                    return "制造台倍率".getTranslate();
                case MultiplierField.Particle:
                    return "对撞机倍率".getTranslate();
                case MultiplierField.Lab:
                    return "研究站倍率".getTranslate();
                case MultiplierField.Fractionator:
                    return "分馏器倍率".getTranslate();
                case MultiplierField.Ejector:
                    return "弹射器倍率".getTranslate();
                case MultiplierField.Silo:
                    return "发射井倍率".getTranslate();
                case MultiplierField.Gamma:
                    return "射线站倍率".getTranslate();
                case MultiplierField.WalkSpeed:
                    return "行走倍率".getTranslate();
                case MultiplierField.Mining:
                    return "采集速度倍率".getTranslate();
                default:
                    return string.Empty;
            }
        }

        internal string GetFieldValue(MultiplierField field)
        {
            switch (field)
            {
                case MultiplierField.Smelt:
                    return smeltmulti_str;
                case MultiplierField.Chemical:
                    return chemicalmulti_str;
                case MultiplierField.Refine:
                    return refinemulti_str;
                case MultiplierField.Assemble:
                    return assemblemulti_str;
                case MultiplierField.Particle:
                    return particlemulti_str;
                case MultiplierField.Lab:
                    return labmulti_str;
                case MultiplierField.Fractionator:
                    return fractionatormulti_str;
                case MultiplierField.Ejector:
                    return ejectormulti_str;
                case MultiplierField.Silo:
                    return silomulti_str;
                case MultiplierField.Gamma:
                    return gammamulti_str;
                case MultiplierField.WalkSpeed:
                    return walkspeedmulti_str;
                case MultiplierField.Mining:
                    return miningmulti_str;
                default:
                    return string.Empty;
            }
        }

        internal void SetFieldValue(MultiplierField field, string value)
        {
            string safeValue = value ?? string.Empty;
            switch (field)
            {
                case MultiplierField.Smelt:
                    smeltmulti_str = safeValue;
                    break;
                case MultiplierField.Chemical:
                    chemicalmulti_str = safeValue;
                    break;
                case MultiplierField.Refine:
                    refinemulti_str = safeValue;
                    break;
                case MultiplierField.Assemble:
                    assemblemulti_str = safeValue;
                    break;
                case MultiplierField.Particle:
                    particlemulti_str = safeValue;
                    break;
                case MultiplierField.Lab:
                    labmulti_str = safeValue;
                    break;
                case MultiplierField.Fractionator:
                    fractionatormulti_str = safeValue;
                    break;
                case MultiplierField.Ejector:
                    ejectormulti_str = safeValue;
                    break;
                case MultiplierField.Silo:
                    silomulti_str = safeValue;
                    break;
                case MultiplierField.Gamma:
                    gammamulti_str = safeValue;
                    break;
                case MultiplierField.WalkSpeed:
                    walkspeedmulti_str = safeValue;
                    break;
                case MultiplierField.Mining:
                    miningmulti_str = safeValue;
                    break;
            }
        }

        internal void ApplyField(MultiplierField field)
        {
            switch (field)
            {
                case MultiplierField.Smelt:
                    ApplyConfigValue(ref smeltmulti_str, smeltMultiply, "冶炼倍数更改为");
                    break;
                case MultiplierField.Chemical:
                    ApplyConfigValue(ref chemicalmulti_str, chemicalMultiply, "化工厂倍数更改为");
                    break;
                case MultiplierField.Refine:
                    ApplyConfigValue(ref refinemulti_str, refineMultiply, "精炼厂倍数更改为");
                    break;
                case MultiplierField.Assemble:
                    ApplyConfigValue(ref assemblemulti_str, assembleMultiply, "制造台倍数更改为");
                    break;
                case MultiplierField.Particle:
                    ApplyConfigValue(ref particlemulti_str, particleMultiply, "对撞机倍数更改为");
                    break;
                case MultiplierField.Lab:
                    ApplyConfigValue(ref labmulti_str, labMultiply, "研究站倍数更改为");
                    break;
                case MultiplierField.Fractionator:
                    ApplyConfigValue(ref fractionatormulti_str, fractionatorMultiply, "分馏器倍数更改为");
                    break;
                case MultiplierField.Ejector:
                    ApplyConfigValue(ref ejectormulti_str, ejectorMultiply, "弹射器倍数更改为");
                    break;
                case MultiplierField.Silo:
                    ApplyConfigValue(ref silomulti_str, siloMultiply, "发射井倍数更改为");
                    break;
                case MultiplierField.Gamma:
                    ApplyConfigValue(ref gammamulti_str, gammaMultiply, "射线站倍数更改为");
                    break;
                case MultiplierField.WalkSpeed:
                    ApplyWalkSpeedValue();
                    break;
                case MultiplierField.Mining:
                    ApplyConfigValue(ref miningmulti_str, miningMultiply, "采集速度倍数更改为");
                    break;
            }
        }

        private void OnDestroy()
        {
            window?.Dispose();
        }

        //工厂
        private static void FactorySystemPatcher()
        {
            foreach (var factory in GameMain.data.factories)
            {
                if (factory == null) continue;
                if (factory.factorySystem != null)
                {
                    Assemblerpatch(factory.factorySystem,factory);
                }
            }
        }   
        private static void Assemblerpatch(FactorySystem factorySystem, PlanetFactory factory)
        {
            
                int multiple = 0;
                for (int i = 1; i < factorySystem.assemblerCursor; i++)
                {
                    if (factorySystem.assemblerPool[i].id == i)
                    {
                        int entityId = factorySystem.assemblerPool[i].entityId;
                        if (entityId > 0)
                        {
                                ItemProto assemblerProto = LDB.items.Select((int)factory.entityPool[entityId].protoId);
                                ERecipeType assemblerRecipeType = factorySystem.assemblerPool[i].recipeType;
                                switch (assemblerRecipeType)   //判断生产类型
                                {
                                    //熔炉
                                    case ERecipeType.Smelt:
                                        multiple = smeltMultiply.Value;
                                        break;
                                    //化工厂
                                    case ERecipeType.Chemical:
                                        multiple = chemicalMultiply.Value;
                                        break;
                                    //精炼厂
                                    case ERecipeType.Refine:
                                        multiple = refineMultiply.Value;
                                        break;
                                    //制造台
                                    case ERecipeType.Assemble:
                                        multiple = assembleMultiply.Value;
                                        break;
                                    //对撞机
                                    case ERecipeType.Particle:
                                        multiple = particleMultiply.Value;
                                        break;

                                    default:
                                        continue;
                                }
                                factorySystem.assemblerPool[i].speed = multiple * assemblerProto.prefabDesc.assemblerSpeed;
                        }
                    }
                        
                }
        }
        //矿物速度
        private static void MiningSpeedScale_patch()
            {
                GameHistoryData history = GameMain.history;
                for (int i = 4; i > 0; i--)
                {
                    if (history.techStates[3605].unlocked)
                    {
                        history.miningSpeedScale = ((float)(history.techStates[3606].curLevel - 1) / 10 + Configs.freeMode.miningSpeedScale) * miningMultiply.Value;
                        break;
                    }
                    else if (!history.techStates[3601].unlocked)
                    {
                        history.miningSpeedScale = Configs.freeMode.miningSpeedScale * miningMultiply.Value;
                        break;
                    }
                    else if (history.techStates[3600 + i].unlocked)
                    {
                        history.miningSpeedScale = ((float)i / 10 + Configs.freeMode.miningSpeedScale) * miningMultiply.Value;
                        break;
                    }
                }
            }

        static class harmony_Patch
        {
            //机甲速度
            [HarmonyPrefix, HarmonyPatch(typeof(Player), "GameTick")]
            private static void WalkSpeed_Tech()
            {
                GameHistoryData history = GameMain.history;
                for (int i = 8; i > 0; i--)
                {
                    if (!history.techStates[2201].unlocked)
                    {
                        walkspeed_tech = 0;
                        break;
                    }
                    else if (history.techStates[2200 + i].unlocked)
                    {
                        walkspeed_tech = i;
                        break;
                    }
                }
            }

            [HarmonyPrefix, HarmonyPatch(typeof(Mecha), "GameTick")]
            private static void WalkSpeed_patch(ref Mecha __instance)
            {
                if (!Speed_set.Value)
                {
                    if (walkspeed_tech == 0)
                    {
                        __instance.walkSpeed = Configs.freeMode.mechaWalkSpeed * walkspeedMultiply.Value;
                    }
                    else if (walkspeed_tech >= 7)
                    {
                        __instance.walkSpeed = (Configs.freeMode.mechaWalkSpeed + (walkspeed_tech - 6) * 2 + 6) * walkspeedMultiply.Value;
                    }
                    else if (walkspeed_tech < 7)
                    {
                        __instance.walkSpeed = (Configs.freeMode.mechaWalkSpeed + walkspeed_tech) * walkspeedMultiply.Value;
                    }
                }
            }


            //研究站生产
            [HarmonyPrefix, HarmonyPatch(typeof(LabComponent), "InternalUpdateAssemble")]
            private static void Lab_patch(ref LabComponent __instance)
            {
                ItemProto LabProto = LDB.items.Select(2901);
                if (!__instance.researchMode)
                {
                    if (__instance.recipeId > 0)
                    {
                        RecipeProto labRecipe = LDB.recipes.Select(__instance.recipeId);
                        if (labRecipe != null && labRecipe.Type == ERecipeType.Research)
                        {
                            __instance.speed = LabProto.prefabDesc.labAssembleSpeed * labMultiply.Value;
                        }
                    }
                }
            }

            //科技研究速率
            [HarmonyPrefix, HarmonyPatch(typeof(MechaLab), "GameTick")]
            private static void Techspeed_patch(MechaLab __instance)
            {
                GameHistoryData history = GameMain.history;
                for (int i = 2; i > 0; i--)
                {
                    if (history.techStates[3903].unlocked)
                    {
                        history.techSpeed = history.techStates[3904].curLevel * labMultiply.Value;
                        break;
                    }
                    else if (!history.techStates[3901].unlocked)
                    {
                        history.techSpeed = Configs.freeMode.techSpeed * labMultiply.Value;
                        break;
                    }
                    else if (history.techStates[3900 + i].unlocked)
                    {
                        history.techSpeed = (i + Configs.freeMode.techSpeed) * labMultiply.Value;
                        break;
                    }
                }
            }

            //分馏塔
            [HarmonyPrefix, HarmonyPatch(typeof(FractionatorComponent), "InternalUpdate")]
            private static void Fractionate_patch(ref FractionatorComponent __instance)
            {
                __instance.produceProb = fractionatorMultiply.Value * 0.01f;
            }

            //弹射器
            [HarmonyPrefix, HarmonyPatch(typeof(EjectorComponent), "InternalUpdate")]
            private static void Ejector_patch(ref EjectorComponent __instance)
            {
                ItemProto Ejector = LDB.items.Select(2311);
                __instance.chargeSpend = Ejector.prefabDesc.ejectorChargeFrame * 10000 / ejectorMultiply.Value;
                __instance.coldSpend = Ejector.prefabDesc.ejectorColdFrame * 10000 / ejectorMultiply.Value;
            }

            //发射井
            [HarmonyPrefix, HarmonyPatch(typeof(SiloComponent), "InternalUpdate")]
            private static void Silo_patch(ref SiloComponent __instance)
            {
                ItemProto Silo = LDB.items.Select(2312);
                __instance.chargeSpend = Silo.prefabDesc.siloChargeFrame * 10000 / siloMultiply.Value;
                __instance.coldSpend = Silo.prefabDesc.siloColdFrame * 10000 / siloMultiply.Value;
            }

            //射线接受站
            [HarmonyPrefix, HarmonyPatch(typeof(PowerGeneratorComponent), "GameTick_Gamma")]
            private static void Gamma_patch(ref PowerGeneratorComponent __instance)
            {
                ItemProto Gamma = LDB.items.Select(2208);
                if (__instance.gamma)
                {
                    __instance.genEnergyPerTick = gammaMultiply.Value * Gamma.prefabDesc.genEnergyPerTick;
                }
            }

            //工作电量
            [HarmonyPrefix, HarmonyPatch(typeof(PowerSystem), "GameTick")]
            private static void Power_patch(PowerSystem __instance)
            {
                float powermultiple = 0f;
                for (int j = 1; j < __instance.consumerCursor; j++)
                {
                    int entityId = __instance.consumerPool[j].entityId;
                    if (entityId > 0)
                    {
                        ItemProto consumer = LDB.items.Select((int)__instance.factory.entityPool[entityId].protoId);
                        if (consumer.prefabDesc.isAssembler)    //判断是不是制造类
                        {
                            ERecipeType consumerRecipeType = consumer.prefabDesc.assemblerRecipeType;
                            switch (consumerRecipeType)
                            {
                                case ERecipeType.Smelt:
                                    powermultiple = smeltMultiply.Value;
                                    break;

                                case ERecipeType.Chemical:
                                    powermultiple = chemicalMultiply.Value;
                                    break;

                                case ERecipeType.Refine:
                                    powermultiple = refineMultiply.Value;
                                    break;

                                case ERecipeType.Assemble:
                                    powermultiple = assembleMultiply.Value;
                                    break;

                                case ERecipeType.Particle:
                                    powermultiple = particleMultiply.Value;
                                    break;

                                default:
                                    continue;
                            }
                        }
                        else if (consumer.prefabDesc.isLab)
                        {
                            powermultiple = labMultiply.Value;
                        }
                        else if (consumer.prefabDesc.isEjector)
                        {
                            powermultiple = ejectorMultiply.Value;
                        }
                        else if (consumer.prefabDesc.isSilo)
                        {
                            powermultiple = siloMultiply.Value;
                        }
                        else if (consumer.prefabDesc.isFractionator)
                        {
                            bool fracMultiplyDefault = fractionatorMultiply.Value == 1;
                            if (fracMultiplyDefault)
                            {
                                powermultiple = 1f;
                            }
                            else
                            {
                                powermultiple = (float)(Math.Pow(1.055, fractionatorMultiply.Value) * fractionatorMultiply.Value);
                            }
                        }
                        else if (consumer.prefabDesc.minerType != EMinerType.None && consumer.prefabDesc.minerPeriod > 0)
                        {
                            powermultiple = miningMultiply.Value;
                        }
                        else continue;  //封闭未改动类型

                        __instance.consumerPool[j].workEnergyPerTick = (long)(powermultiple * consumer.prefabDesc.workEnergyPerTick);
                    }
                }
            }
        }
    }
}
