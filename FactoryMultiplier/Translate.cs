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
			TranslateDict.Add("设置弹射器倍数", "Ejector Multiple ");
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


