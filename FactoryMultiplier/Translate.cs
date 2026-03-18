using System;
using System.Collections.Generic;

namespace FactoryMultiplier
{
	public static class Translate
	{
		private static Dictionary<string, string> TranslateDict = new Dictionary<string, string>();
		public static string getTranslate(this string s)
		{
			bool flag = Localization.isKMG && TranslateDict.ContainsKey(s);
			string result;
			if (flag)
			{
				result = TranslateDict[s];
			}
			else
			{
				result = s;
			}
			return result;
		}

		public static void regAllTranslate()
		{
			TranslateDict.Clear();
			TranslateDict.Add("工厂倍率设置", "Factory Multiplier");
			TranslateDict.Add("生产与科研", "Production & Research");
			TranslateDict.Add("物流、能源与角色", "Logistics, Power & Player");
			TranslateDict.Add("熔炉倍率", "Smelter Multiplier");
			TranslateDict.Add("化工厂倍率", "Chemical Plant Multiplier");
			TranslateDict.Add("精炼厂倍率", "Refinery Multiplier");
			TranslateDict.Add("制造台倍率", "Assembler Multiplier");
			TranslateDict.Add("对撞机倍率", "Collider Multiplier");
			TranslateDict.Add("研究站倍率", "Lab Multiplier");
			TranslateDict.Add("分馏器倍率", "Fractionator Multiplier");
			TranslateDict.Add("弹射器倍率", "Ejector Multiplier");
			TranslateDict.Add("发射井倍率", "Silo Multiplier");
			TranslateDict.Add("射线站倍率", "Ray Receiver Multiplier");
			TranslateDict.Add("行走倍率", "Walk Speed Multiplier");
			TranslateDict.Add("采集速度倍率", "Mining Speed Multiplier");
			TranslateDict.Add("应用", "Apply");
			TranslateDict.Add("快捷键：{0}", "Hotkey: {0}");
			TranslateDict.Add("直接设置行走速度（m/s）", "Set walk speed directly (m/s)");
			TranslateDict.Add("在这里修改倍率并点击应用按钮。", "Adjust values here and click Apply.");
			TranslateDict.Add("设置冶炼倍数", "Smelter Multiple");
			TranslateDict.Add("冶炼倍数更改为", "Smelt Multipe Changed To ");
			TranslateDict.Add("设置化工厂倍数", "Chemical Multiple");
			TranslateDict.Add("化工厂倍数更改为", "Chemical Multipe Changed To ");
			TranslateDict.Add("设置精炼厂倍数", "Refinery Multiple");
			TranslateDict.Add("精炼厂倍数更改为", "Refinery Multipe Changed To ");
			TranslateDict.Add("设置制造台倍数", "Assembler Multiple");
			TranslateDict.Add("制造台倍数更改为", "Assembler Multipe Changed To ");
			TranslateDict.Add("设置对撞机倍数", "Collider Multiple");
			TranslateDict.Add("对撞机倍数更改为", "Collider Multipe Changed To ");
			TranslateDict.Add("设置研究站倍数", "Lab Multiple"); ;
			TranslateDict.Add("研究站倍数更改为", "Lab Multipe Changed To ");
			TranslateDict.Add("设置分馏器倍数", "Fractionator Multiple");
			TranslateDict.Add("分馏器倍数更改为", "Fractionator Multipe Changed To ");
			TranslateDict.Add("设置弹射器倍数", "Ejector Multiple");
			TranslateDict.Add("弹射器倍数更改为", "Ejector Multipe Changed To ");
			TranslateDict.Add("设置发射井倍数", "Silo Multiple");
			TranslateDict.Add("发射井倍数更改为", "Silo Multipe Changed To ");
			TranslateDict.Add("设置射线站倍数", "Gamma Multiple");
			TranslateDict.Add("射线站倍数更改为", "Gamma Multipe Changed To ");
			TranslateDict.Add("设置行走倍数", "WalkSpeed Multiple");
			TranslateDict.Add("行走倍数更改为", "WalkSpeed Multipe Changed To ");
			TranslateDict.Add("设置采集速度倍数", "MiningSpeed Multiple");
			TranslateDict.Add("采集速度倍数更改为", "MiningSpeed Multipe Changed To ");
		}
	}
}


