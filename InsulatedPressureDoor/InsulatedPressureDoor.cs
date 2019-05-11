using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace InsulatedPressureDoor
{
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class InsulatedPressureDoor_GeneratedBuildings_LoadGeneratedBuildings
    {
        private static void Prefix()
        {
            Debug.Log("Prefix applied");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.NAME", "Fire Safety Door");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.DESC", "The insulation in the door slows temperature changes between two rooms.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.EFFECT", "Helps retain ambient heat in an area.");

            ModUtil.AddBuildingToPlanScreen("Base", InsulatedPressureDoorConfig.ID);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class InsulatedPressureDoor_Db_Initialize
    {
        private static void Prefix(Db __instance)
        {
            Debug.Log("Db_Initialize");
            List<string> ls = new List<string>(Database.Techs.TECH_GROUPING["TemperatureModulation"]);
            ls.Add(InsulatedPressureDoorConfig.ID);
            Database.Techs.TECH_GROUPING["TemperatureModulation"] = ls.ToArray();
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ApplyColor
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "InsulatedPressureDoorComplete") == 0)
            {
                __instance.GetComponent<KAnimControllerBase>().TintColour = InsulatedPressureDoorConfig.Color();
                return;
            }
        }
    }
}
