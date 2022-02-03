using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SteelNeedsLessLime
{
    [HarmonyPatch(typeof(MetalRefineryConfig), "ConfigureBuildingTemplate")]
    internal class MetalRefineryConfig_ConfigureBuildingTemplate : KMod.UserMod2
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        { 
            var steel_hash = SimHashes.Steel;
            bool recipe_found = false;
            bool recipe_modified = false;

            List<CodeInstruction> code = instr.ToList();
            foreach (CodeInstruction codeInstruction in code)
            {
                // Only run if the recipe hasn't been modified yet
                if (!recipe_modified)
                {
                    // Triggers when we find a reference to steel
                    if (codeInstruction.opcode == OpCodes.Ldc_I4 && (int)codeInstruction.operand == steel_hash.GetHashCode())
                    {
                        // The first reference triggers recipe modification, the second ends the modification
                        if (!recipe_found)
                        { recipe_found = true; Debug.Log("Steel recipe found! Modifying..."); }
                        else
                        { recipe_modified = true; Debug.Log("Steel recipe successfully modified"); }
                    }

                    // If the recipe has been found, allow the modification of ingredient values
                    if (recipe_found)
                    {
                        if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 70f)
                        { Debug.Log("  Iron: 70 kg --> 75 kg"); codeInstruction.operand = 75f; }

                        //if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 20f)
                        //{ Debug.Log("  Carbon: 20 kg --> 20 kg"); codeInstruction.operand = 20f; }

                        if (codeInstruction.opcode == OpCodes.Ldc_R4 && (float)codeInstruction.operand == 10f)
                        { Debug.Log("  Lime: 10 kg --> 5 kg"); codeInstruction.operand = 5f; }
                    }
                }
                yield return codeInstruction;
            }

            Debug.Log("Transpiler complete");
        }
    }
}
