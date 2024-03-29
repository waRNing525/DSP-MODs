﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FactoryMultiplier
{
    [BepInPlugin("waRNing.dsp.plugins.FactoryMultiplier", "FactoryMultiplier", "2.2.0")]
    public class FactoryMultiplier : BaseUnityPlugin
    {
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
        private MyUI myUI;

        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Patch), null);
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
            myUI = new MyUI(new GUI.WindowFunction(MainUI), 750f, 550f, "FactoryMultiplier", 0);
        }

        private void Update()
        {
            ToggleWindow();
        }

        private void ToggleWindow()
        {
            if (!GameMain.isRunning || GameMain.isPaused || GameMain.instance.isMenuDemo)
            {
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

        private void OnGUI()
        {
            myUI.Render(Showwindow);
        }

        private void MainUI(int id)
        {
            GUILayout.BeginArea(new Rect(0, 20, 750, 550));
            smeltmulti_str = GUI.TextField(new Rect(60, 20, 150, 30), smeltmulti_str, 1);
            chemicalmulti_str = GUI.TextField(new Rect(60, 105, 150, 30), chemicalmulti_str, 1);
            refinemulti_str = GUI.TextField(new Rect(60, 190, 150, 30), refinemulti_str, 1);
            assemblemulti_str = GUI.TextField(new Rect(60, 275, 150, 30), assemblemulti_str, 1);
            particlemulti_str = GUI.TextField(new Rect(60, 360, 150, 30), particlemulti_str, 1);
            labmulti_str = GUI.TextField(new Rect(300, 20, 150, 30), labmulti_str, 1);
            fractionatormulti_str = GUI.TextField(new Rect(300, 105, 150, 30), fractionatormulti_str, 2);
            ejectormulti_str = GUI.TextField(new Rect(300, 190, 150, 30), ejectormulti_str, 1);
            silomulti_str = GUI.TextField(new Rect(300, 275, 150, 30), silomulti_str, 1);
            gammamulti_str = GUI.TextField(new Rect(300, 360, 150, 30), gammamulti_str, 1);
            walkspeedmulti_str = GUI.TextField(new Rect(540, 20, 75, 30), walkspeedmulti_str, 2);
            miningmulti_str = GUI.TextField(new Rect(540, 105, 150, 30), miningmulti_str, 1);

            GUI.Label(new Rect(300, 460, 150, 50), tempText);    //输出文字

            if (GUI.Button(new Rect(60, 55, 150, 30), "设置冶炼倍数".getTranslate()))
            {
                smeltMultiply.Value = int.Parse(Regex.Replace(smeltmulti_str, @"[^0-9]", ""));
                tempText = "冶炼倍数更改为".getTranslate() + smeltmulti_str + "X";
            }
            if (GUI.Button(new Rect(60, 140, 150, 30), "设置化工厂倍数".getTranslate()))
            {
                chemicalMultiply.Value = int.Parse(Regex.Replace(chemicalmulti_str, @"[^0-9]", ""));
                tempText = "化工厂倍数更改为".getTranslate() + chemicalmulti_str + "X";
            }
            if (GUI.Button(new Rect(60, 225, 150, 30), "设置精炼厂倍数".getTranslate()))
            {
                refineMultiply.Value = int.Parse(Regex.Replace(refinemulti_str, @"[^0-9]", ""));
                tempText = "精炼厂倍数更改为".getTranslate() + refinemulti_str + "X";
            }
            if (GUI.Button(new Rect(60, 310, 150, 30), "设置制造台倍数".getTranslate()))
            {
                assembleMultiply.Value = int.Parse(Regex.Replace(assemblemulti_str, @"[^0-9]", ""));
                tempText = "制造台倍数更改为".getTranslate() + assemblemulti_str + "X";
            }
            if (GUI.Button(new Rect(60, 395, 150, 30), "设置对撞机倍数".getTranslate()))
            {
                particleMultiply.Value = int.Parse(Regex.Replace(particlemulti_str, @"[^0-9]", ""));
                tempText = "对撞机倍数更改为".getTranslate() + particlemulti_str + "X";
            }
            if (GUI.Button(new Rect(300, 55, 150, 30), "设置研究站倍数".getTranslate()))
            {
                labMultiply.Value = int.Parse(Regex.Replace(labmulti_str, @"[^0-9]", ""));
                tempText = "研究站倍数更改为".getTranslate() + labmulti_str + "X";
            }
            if (GUI.Button(new Rect(300, 140, 150, 30), "设置分馏器倍数".getTranslate()))
            {
                fractionatorMultiply.Value = int.Parse(Regex.Replace(fractionatormulti_str, @"[^0-9]", ""));
                tempText = "分馏器倍数更改为".getTranslate() + fractionatormulti_str + "X";
            }
            if (GUI.Button(new Rect(300, 225, 150, 30), "设置弹射器倍数".getTranslate()))
            {
                ejectorMultiply.Value = int.Parse(Regex.Replace(ejectormulti_str, @"[^0-9]", ""));
                tempText = "弹射器倍数更改为".getTranslate() + ejectormulti_str + "X";
            }
            if (GUI.Button(new Rect(300, 310, 150, 30), "设置发射井倍数".getTranslate()))
            {
                siloMultiply.Value = int.Parse(Regex.Replace(silomulti_str, @"[^0-9]", ""));
                tempText = "发射井倍数更改为".getTranslate() + silomulti_str + "X";
            }
            if (GUI.Button(new Rect(300, 395, 150, 30), "设置射线站倍数".getTranslate()))
            {
                gammaMultiply.Value = int.Parse(Regex.Replace(gammamulti_str, @"[^0-9]", ""));
                tempText = "射线站倍数更改为".getTranslate() + gammamulti_str + "X";
            }
            if (GUI.Button(new Rect(540, 55, 150, 30), "设置行走倍数".getTranslate()))
            {
                Speed_set.Value = false;
                walkspeedMultiply.Value = int.Parse(Regex.Replace(walkspeedmulti_str, @"[^0-9]", ""));
                tempText = "行走倍数更改为".getTranslate() + walkspeedmulti_str + "X";
            }
            if (Speed_set.Value = GUI.Toggle(new Rect(620, 20, 70, 30), Speed_set.Value, "m/s"))
            {
                GameMain.mainPlayer.mecha.walkSpeed = float.Parse(Regex.Replace(walkspeedmulti_str, @"[^0-9]", ""));
                walkspeedMultiply.Value = int.Parse(Regex.Replace(walkspeedmulti_str, @"[^0-9]", ""));
            }
            if (GUI.Button(new Rect(540, 140, 150, 30), "设置采集速度倍数".getTranslate()))
            {
                miningMultiply.Value = int.Parse(Regex.Replace(miningmulti_str, @"[^0-9]", ""));
                tempText = "采集速度倍数更改为".getTranslate() + miningmulti_str + "X";
            }
            GUILayout.EndArea();
            GUI.DragWindow();
        }

        static class Patch
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
            private static void WalkSpeed_patch(Mecha __instance)
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

            //采集速率
            [HarmonyPrefix, HarmonyPatch(typeof(FactorySystem), "GameTick", new Type[] { typeof(long), typeof(bool), typeof(int), typeof(int), typeof(int) })]
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

            //工厂
            [HarmonyPrefix, HarmonyPatch(typeof(FactorySystem), "GameTick", new Type[] { typeof(long), typeof(bool), typeof(int), typeof(int), typeof(int) })]
            private static void Assembler_patch(FactorySystem __instance)
            {
                int multiple = 0;
                for (int j = 1; j < __instance.assemblerCursor; j++)
                {
                    int entityId = __instance.assemblerPool[j].entityId;
                    if (entityId > 0)
                    {
                        ItemProto assemblerProto = LDB.items.Select((int)__instance.factory.entityPool[entityId].protoId);
                        ERecipeType assemblerRecipeType = __instance.assemblerPool[j].recipeType;
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
                        __instance.assemblerPool[j].speed = multiple * assemblerProto.prefabDesc.assemblerSpeed;
                    }
                }
            }

            //研究站生产
            [HarmonyPrefix, HarmonyPatch(typeof(LabComponent), "InternalUpdateAssemble")]
            private static void Lab_patch(ref LabComponent __instance)
            {
                if (!__instance.researchMode)
                {
                    if (__instance.recipeId > 0)
                    {
                        RecipeProto labRecipe = LDB.recipes.Select(__instance.recipeId);
                        if (labRecipe != null && labRecipe.Type == ERecipeType.Research)
                        {
                            __instance.timeSpend = labRecipe.TimeSpend * 10000 / labMultiply.Value;
                            __instance.extraTimeSpend = labRecipe.TimeSpend * 100000 / labMultiply.Value;
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