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
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.NAME", "Thermal Isolation Door");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.DESC", "While closed, this door will slow temperature changes between two rooms.");
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
    public class InsulatedPressureDoor_ApplyProperties
    {
        public static void Postfix(ref BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "InsulatedPressureDoorComplete") == 0)
            {
                Debug.Log("InsulatedPressureDoor spawned");

                __instance.GetComponent<KAnimControllerBase>().TintColour = InsulatedPressureDoorConfig.Color();
                Debug.Log("Applying colour to InsulatedPressureDoor");

                IList<int> cells = __instance.GetComponent<Building>().PlacementCells;
                for (int i = 0; i < cells.Count; i++)
                {
                    SimMessages.SetInsulation(cells[i], 0.01f);
                    Debug.Log("Applying insulation to cell " + (i + 1) + " of " + cells.Count);
                }

                return;
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnCleanUp")]
    public class InsulatedPressureDoor_RemoveProperties
    {
        public static void Postfix(ref BuildingComplete __instance)
        {
            if (string.Compare(__instance.name, "InsulatedPressureDoorComplete") == 0)
            {
                Debug.Log("InsulatedPressureDoor removed");

                IList<int> cells = __instance.GetComponent<Building>().PlacementCells;
                for (int i = 0; i < cells.Count; i++)
                {
                    SimMessages.SetInsulation(cells[i], 1f);
                    Debug.Log("Removing insulation from cell " + (i + 1) + " of " + cells.Count);
                }

                return;
            }
        }
    }
}
