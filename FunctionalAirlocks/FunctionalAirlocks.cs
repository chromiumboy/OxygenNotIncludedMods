using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FunctionalAirlocks
{
    [HarmonyPatch(typeof(Door), "OnPrefabInit")]
    internal class FunctionalAirlocks_Door_OnPrefabInit : KMod.UserMod2
    {
        private static void Postfix(ref Door __instance)
        {
            Debug.Log("Adding anim override");
            __instance.overrideAnims = new KAnimFile[]
            {
                Assets.GetAnim("anim_use_remote_kanim")
            };
        }
    }

    /*[HarmonyPatch(typeof(Door), "OnSpawn")]
    internal class FunctionalAirlocks_Door_OnSpawn
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_I4_8)
                { codeInstruction.opcode = OpCodes.Ldc_I4; codeInstruction.operand = 9; Debug.Log("Made 9"); }
                yield return codeInstruction;
            }
        }
    }*/

    [HarmonyPatch(typeof(Door), "OnCleanUp")]
    internal class FunctionalAirlocks_Door_OnCleanUp
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            List<CodeInstruction> code = instr.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                if (codeInstruction.opcode == OpCodes.Ldc_I4_S && (sbyte)codeInstruction.operand == 12)
                { codeInstruction.operand = 13; Debug.Log("Made 13"); }
                yield return codeInstruction;
            }
        }
    }

    [HarmonyPatch(typeof(Door), "SetSimState")]
    internal class FunctionalAirlocks_Door_SetSimState
    {
        private static bool Prefix(Door __instance, bool is_door_open, IList<int> cells)
        {
            Door.ControlState controlState = Traverse.Create(__instance).Field("controlState").GetValue<Door.ControlState>();
            PrimaryElement component = __instance.GetComponent<PrimaryElement>();
            float num = component.Mass / (float)cells.Count;

            MethodInfo method_closed = AccessTools.Method(typeof(Door), "OnSimDoorClosed", null, null);
            System.Action cb_closed = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_closed);

            MethodInfo method_opened = AccessTools.Method(typeof(Door), "OnSimDoorOpened", null, null);
            System.Action cb_opened = (System.Action)Delegate.CreateDelegate(typeof(System.Action), __instance, method_opened);

            for (int i = 0; i < cells.Count; i++)
            {
                int num2 = cells[i];
                Door.DoorType doorType = __instance.doorType;
                if (doorType == Door.DoorType.Pressure || doorType == Door.DoorType.Sealed || doorType == Door.DoorType.ManualPressure)
                {
                    World.Instance.groundRenderer.MarkDirty(num2);

                    if (is_door_open)
                    {
                        SimMessages.Dig(num2, Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_opened, false)).index);

                        if (controlState == Door.ControlState.Auto)
                        {
                            HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_opened, false));
                            float temperature = component.Temperature;
                            if (temperature <= 0f)
                            {
                                temperature = component.Temperature;
                            }
                            int gameCell = num2;
                            SimHashes elementID = component.ElementID;
                            CellElementEvent doorOpen = CellEventLogger.Instance.DoorOpen;
                            float mass = num;
                            float temperature2 = temperature;
                            int index = handle.index;
                            SimMessages.ReplaceAndDisplaceElement(gameCell, elementID, doorOpen, mass, temperature2, byte.MaxValue, 0, index);
                            SimMessages.SetCellProperties(num2, 4);
                        }

                        else if (__instance.ShouldBlockFallingSand)
                        {
                            SimMessages.ClearCellProperties(num2, 4);
                        }

                        else
                        {
                            SimMessages.SetCellProperties(num2, 4);
                        }
                    }
                    else
                    {
                        HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(cb_closed, false));
                        float temperature = component.Temperature;
                        if (temperature <= 0f)
                        {
                            temperature = component.Temperature;
                        }
                        int gameCell = num2;
                        SimHashes elementID = component.ElementID;
                        CellElementEvent doorClose = CellEventLogger.Instance.DoorClose;
                        float mass = num;
                        float temperature2 = temperature;
                        int index = handle.index;
                        SimMessages.ReplaceAndDisplaceElement(gameCell, elementID, doorClose, mass, temperature2, byte.MaxValue, 0, index);
                        SimMessages.SetCellProperties(num2, 4);
                    }
                }
            }

            return false;
        }
    }
}
