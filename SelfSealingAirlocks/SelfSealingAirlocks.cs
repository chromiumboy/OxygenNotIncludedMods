using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

namespace SelfSealingAirlocks
{
    // Add anim override (necesary to prevent game crash)
    [HarmonyPatch(typeof(Door), "OnPrefabInit")]
    internal class SelfSealingAirlocks_Door_OnPrefabInit
    {
        private static void Postfix(ref Door __instance)
        {
            Debug.Log("Adding door anim override");
            __instance.overrideAnims = new KAnimFile[]
            {
                Assets.GetAnim("anim_use_remote_kanim")
            };
        }
    }

    // Load the config file on save game load
    [HarmonyPatch(typeof(Game), "OnPrefabInit")]
    internal class SelfSealingAirlocks_Game_OnPrefabInit
    {
        private static void Postfix()
        {
            SelfSealingAirlocksConfig.LoadConfig();
        }
    }

    // Ensure cell properties are cleared on clean up
    [HarmonyPatch(typeof(Door), "OnCleanUp")]
    internal class SelfSealingAirlocks_Door_OnCleanUp
    {
        private static void Postfix(Door __instance)
        {
            foreach (int cell in __instance.building.PlacementCells)
            {
                SimMessages.ClearCellProperties(cell, 3);
            }
        }
    }

    // Update sim state setter to make airlock doors gas impermable
    [HarmonyPatch(typeof(Door), "SetSimState")]
    internal class SelfSealingAirlocks_Door_SetSimState
    {
        private static bool Prefix(Door __instance, bool is_door_open, IList<int> cells)
        {
            // If the attached gameobject doesn't exist, exit here
            if (__instance.gameObject == null)
            { return true; }

            // Get the door control state
            Door.ControlState controlState = Traverse.Create(__instance).Field("controlState").GetValue<Door.ControlState>();

            // Get the door type
            Door.DoorType doorType = __instance.doorType;

            // Exit here if the door type is 'Internal', or the door state is set to 'Opened'
            if (doorType == Door.DoorType.Internal || controlState == Door.ControlState.Opened)
            { return true; }

            // Get the mass of the door (per cell)
            PrimaryElement element = __instance.GetComponent<PrimaryElement>();
            float mass_per_cell = element.Mass / cells.Count;

            // Get the config message value
            int message = 1;
            if (SelfSealingAirlocksConfig.Config.AirlocksBlockLiquids)
            { message = 3; }

            // Loop over each cell making up the door
            for (int i = 0; i < cells.Count; i++)
            {
                int cell = cells[i];

                // On opening
                if (is_door_open)
                {
                    // Get the sim call back for when a door is opened
                    MethodInfo method_opened = AccessTools.Method(typeof(Door), "OnSimDoorOpened", null, null);
                    System.Action cb_opened = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_opened);
                    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_opened, false));
                    
                    // Remove the tile making up the door
                    SimMessages.Dig(cell, handle.index);

                    // If the cell above is vacuum, remove any door tile that gets displaced into it
                    int cellAbove = Grid.CellAbove(cell);
                    //if (ElementLoader.elements[Grid.ElementIdx[cellAbove]] == ElementLoader.FindElementByHash(SimHashes.Vacuum))
                    if (!Grid.IsSolidCell(cellAbove))
                    { SimMessages.Dig(cellAbove); }

                    // If the cell below is vacuum, remove any door tile that gets displaced into it
                    int cellBelow = Grid.CellBelow(cell);
                    //if (ElementLoader.elements[Grid.ElementIdx[cellBelow]] == ElementLoader.FindElementByHash(SimHashes.Vacuum))
                    if (!Grid.IsSolidCell(cellBelow))
                    { SimMessages.Dig(cellBelow); }

                    // If the cell to the left is vacuum, any the door tile that gets displaced into it
                    int cellLeft = Grid.CellLeft(cell);
                    //if (ElementLoader.elements[Grid.ElementIdx[cellLeft]] == ElementLoader.FindElementByHash(SimHashes.Vacuum))
                    if (!Grid.IsSolidCell(cellLeft))
                    { SimMessages.Dig(cellLeft); }

                    // If the cell to the right is vacuum, any the door tile that gets displaced into it
                    int cellRight = Grid.CellRight(cell);
                    //if (ElementLoader.elements[Grid.ElementIdx[cellRight]] == ElementLoader.FindElementByHash(SimHashes.Vacuum))
                    if (!Grid.IsSolidCell(cellRight))
                    { SimMessages.Dig(cellRight); }

                    // For horizontal doors
                    if (__instance.ShouldBlockFallingSand)
                    {
                        // Update the cell properties (block gases but not solids)
                        SimMessages.SetCellProperties(cell, (byte)message); SimMessages.ClearCellProperties(cell, 4);
                    }

                    // For vertical doors
                    else
                    {
                        // Update the cell properties (block solids and gases)
                        SimMessages.SetCellProperties(cell, (byte)(4 + message));
                    }
                }

                // On closing
                else
                {
                    // Get the sim call back for when a door is opened
                    MethodInfo method_closed = AccessTools.Method(typeof(Door), "OnSimDoorClosed", null, null);
                    System.Action cb_closed = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_closed);
                    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_closed, false));

                    // Replace the missing door tile
                    SimMessages.ReplaceAndDisplaceElement(cell, element.ElementID, CellEventLogger.Instance.DoorClose, mass_per_cell, element.Temperature, byte.MaxValue, 0, handle.index);

                    // Update the cell properties (block solids and not gases)
                    SimMessages.ClearCellProperties(cell, (byte)message);
                    SimMessages.SetCellProperties(cell, 4);
                }
            }

            // Exit, do not run the orginal method
            return false;
        }
    }
}

