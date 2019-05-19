using System;
using System.Collections.Generic;
using UnityEngine;
using Harmony;

namespace InsulatedPressureDoor
{
    // Add strings for the new door and add it to the build menu
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    internal class InsulatedPressureDoor_GeneratedBuildings_LoadGeneratedBuildings
    {
        private static void Prefix()
        {
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.NAME", "Thermal Isolation Door");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.DESC", "While closed, this door will slow temperature changes between two rooms.");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.INSULATEDPRESSUREDOOR.EFFECT", "Helps retain ambient heat in an area.");

            ModUtil.AddBuildingToPlanScreen("Base", InsulatedPressureDoorConfig.ID);
        }
    }

    // Add the new door to the tech tree
    [HarmonyPatch(typeof(Db), "Initialize")]
    internal class InsulatedPressureDoor_Db_Initialize
    {
        private static void Prefix(Db __instance)
        {
            List<string> ls = new List<string>(Database.Techs.TECH_GROUPING["TemperatureModulation"]);
            ls.Add(InsulatedPressureDoorConfig.ID);
            Database.Techs.TECH_GROUPING["TemperatureModulation"] = ls.ToArray();
        }
    }

    // Triggers when a new building is spawned
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class InsulatedPressureDoor_BuildingComplete_OnSpawn
    {
        public static void Postfix(ref BuildingComplete __instance)
        {
            // Trigger if the building is an InsulatedPressureDoor
            if (string.Compare(__instance.name, "InsulatedPressureDoorComplete") == 0)
            {
                // Apply the custom colour tint
                __instance.GetComponent<KAnimControllerBase>().TintColour = InsulatedPressureDoorConfig.Color();

                // Add the InsulatingDoor component
                __instance.gameObject.AddOrGet<InsulatingDoor>();
            }

            // If the building has the InsulatingDoor component, apply its insulation
            InsulatingDoor insulatingDoor = __instance.gameObject.GetComponent<InsulatingDoor>();

            if (insulatingDoor != null)
            {
                insulatingDoor.SetInsulation(__instance.gameObject, insulatingDoor.door.building.Def.ThermalConductivity);
            }
        }
    }

    // Triggers when a building is removed
    [HarmonyPatch(typeof(BuildingComplete), "OnCleanUp")]
    public class InsulatedPressureDoor_BuildingComplete_OnCleanUp
    {
        public static void Postfix(ref BuildingComplete __instance)
        {
            // If the building has the InsulatingDoor component, remove its insulation
            InsulatingDoor insulatingDoor = __instance.gameObject.GetComponent<InsulatingDoor>();

            if (insulatingDoor != null)
            {
                insulatingDoor.SetInsulation(__instance.gameObject, 1f);
            }
        }
    }

    // Add anim override. Modifications of the Door component without this patch will cause the game to crash when the door control state is changed maunually
    [HarmonyPatch(typeof(Door), "OnPrefabInit")]
    internal class InsulatedPressureDoor_Door_OnPrefabInit
    {
        private static void Postfix(ref Door __instance)
        {
            Debug.Log("  InsulatedPressureDoor - Applying animation override");
            __instance.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_use_remote_kanim") };
        }
    }

    // Triggers when a door is opened
    [HarmonyPatch(typeof(Door), "OnSimDoorOpened")]
    public class InsulatedPressureDoor_Door_OnSimDoorOpened
    {
        public static void Postfix(ref Door __instance)
        {
            // If the door has the InsulatingDoor component, remove its insulation
            InsulatingDoor insulatingDoor = __instance.gameObject.GetComponent<InsulatingDoor>();

            if (insulatingDoor != null)
            {
                insulatingDoor.SetInsulation(__instance.gameObject, 1f);
            }
        }
    }

    // Triggers when a door is closed
    [HarmonyPatch(typeof(Door), "OnSimDoorClosed")]
    public class InsulatedPressureDoor_Door_Closed
    {
        public static void Postfix(ref BuildingComplete __instance)
        {
            // If the door has the InsulatingDoor component, apply its insulation
            InsulatingDoor insulatingDoor = __instance.gameObject.GetComponent<InsulatingDoor>();

            if (insulatingDoor != null)
            {
                insulatingDoor.SetInsulation(__instance.gameObject, insulatingDoor.door.building.Def.ThermalConductivity);
            }
        }
    }

    // InsulatingDoor component
    class InsulatingDoor : KMonoBehaviour
    {
        // References the Door component
        [MyCmpGet]
        public Door door;

        // Method to set cell insulation levels
        public void SetInsulation(GameObject go, float insulation)
        {
            IList<int> cells = go.GetComponent<Building>().PlacementCells;
            for (int i = 0; i < cells.Count; i++)
            {
                SimMessages.SetInsulation(cells[i], insulation);
            }
        }
    }
}
