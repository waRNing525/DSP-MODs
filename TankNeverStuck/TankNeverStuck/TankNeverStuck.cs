using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TankNeverStuck
{
    [BepInPlugin("waRNing.dsp.plugins.TankNeverStuck", "TankNeverStuck", "1.0.1")]
    public class TankNeverStuck : BaseUnityPlugin
    {
        private void Start()
        {
            Harmony.CreateAndPatchAll(typeof(TankNeverStuck), null);

        }
        private void Update()
        {
        }
        [HarmonyPostfix]
		[HarmonyPatch(typeof(FactoryStorage), "GameTick")]
		public static void Tank_patch(FactoryStorage __instance)
		{
			TankComponent[] tankPool = __instance.tankPool;
			for (int i = 1; i < __instance.tankCursor; i++)
			{
				if (tankPool[i].id == i && tankPool[i].fluidCapacity > 0 && tankPool[i].nextTankId <= 0 && tankPool[i].fluidCount >= tankPool[i].fluidCapacity)
				{
					tankPool[i].fluidCount = tankPool[i].fluidCapacity - 1;
				}
			}
		}
	}
}
