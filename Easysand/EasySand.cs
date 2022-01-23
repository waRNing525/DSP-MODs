using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace EasySand
{
	[BepInPlugin("waRNing.dsp.plugins.EasySand", "EasySand", "1.1.0")]

	public class EasySand : BaseUnityPlugin
	{
		private ConfigEntry<KeyCode> AddSandKeyConfig;
		private ConfigEntry<KeyCode> AddFoundationKeyConfig;
		private ConfigEntry<int> AddSandCountConfig;
		private ConfigEntry<int> AddFoundationCountConfig;
		private void Start()
		{
			AddSandKeyConfig = Config.Bind<KeyCode>("config", "AddSandHotKey", KeyCode.F10, "增加沙土的热键");
			AddFoundationKeyConfig = Config.Bind<KeyCode>("config", "AddFoundationKey", KeyCode.F9, "增加地基的热键");
			AddSandCountConfig = Config.Bind<int>("config", "AddSandCount", 1000000, "每次增加的沙土");
			AddFoundationCountConfig = Config.Bind<int>("config", "AddFoundationCount", 1000, "每次增加的地基");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020F4 File Offset: 0x000002F4
		private void Update()
		{
			if (GameMain.mainPlayer != null && Input.GetKeyDown(AddSandKeyConfig.Value))
			{
				GameMain.mainPlayer.SetSandCount(GameMain.mainPlayer.sandCount + AddSandCountConfig.Value);
			}
			if (GameMain.mainPlayer != null && Input.GetKeyDown(AddFoundationKeyConfig.Value))
			{
				GameMain.mainPlayer.TryAddItemToPackage(1131, AddFoundationCountConfig.Value, 0, false);
			}
		}
	}
}

