using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUNING;
using UnityEngine;
using HarmonyLib;

namespace OverhauledMiniPumps
{
    // Add strings for the new door and add it to the build menu
    [HarmonyPatch(typeof(LiquidMiniPumpConfig), "CreateBuildingDef")]
    internal class OverhauledMiniPumps_LiquidMiniPumpConfig_CreateBuildingDef : KMod.UserMod2
    {
        private static void Postfix(LiquidMiniPumpConfig __instance, ref BuildingDef __result)
        {
            if (__result != null)
            {
                __result.EnergyConsumptionWhenActive = 20f;
                __result.MaterialCategory = new string[] { "RefinedMetal", SimHashes.Polypropylene.ToString() };
                __result.Mass = new float[] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0], BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0] };
            }
        }
    }

    // Add strings for the new door and add it to the build menu
    [HarmonyPatch(typeof(GasMiniPumpConfig), "CreateBuildingDef")]
    internal class OverhauledMiniPumps_GasMiniPumpConfig_CreateBuildingDef
    {
        private static void Postfix(GasMiniPumpConfig __instance, ref BuildingDef __result)
        {
            if (__result != null)
            {
                __result.EnergyConsumptionWhenActive = 20f;
                __result.MaterialCategory = new string[] { "RefinedMetal", SimHashes.Polypropylene.ToString() };
                __result.Mass = new float[] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0], BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0] };
            }
        }
    }
}
