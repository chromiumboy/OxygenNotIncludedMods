using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace SelfSealingAirlocks
{
    // Add anim override (necesary to prevent game crash)
    [HarmonyPatch(typeof(Door), "OnPrefabInit")]
    internal class SelfSealingAirlocks_Door_OnPrefabInit : KMod.UserMod2
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

            // Loop over each cell making up the door
            for (int i = 0; i < cells.Count; i++)
            {
                int cell = cells[i];
                SimMessages.SetCellProperties(cell, 4);
                

                // On opening
                if (is_door_open)
                {
                    MethodInfo method_opened = AccessTools.Method(typeof(Door), "OnSimDoorOpened", null, null);
                    System.Action cb_opened = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_opened);
                    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_opened, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, element.ElementID, CellEventLogger.Instance.DoorOpen, mass_per_cell, element.Temperature, byte.MaxValue, 0, handle.index);
                }

                // On closing
                else
                {
                    MethodInfo method_closed = AccessTools.Method(typeof(Door), "OnSimDoorClosed", null, null);
                    System.Action cb_closed = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_closed);
                    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_closed, false));
                    SimMessages.ReplaceAndDisplaceElement(cell, element.ElementID, CellEventLogger.Instance.DoorClose, mass_per_cell, element.Temperature, byte.MaxValue, 0, handle.index);
                }
            }

            // Exit, do not run the orginal method
            return false;
        }
    }
}

