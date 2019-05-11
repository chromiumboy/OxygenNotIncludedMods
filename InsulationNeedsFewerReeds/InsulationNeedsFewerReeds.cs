using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace InsulationNeedsFewerReeds
{
    [HarmonyPatch(typeof(SupermaterialRefineryConfig), "ConfigureBuildingTemplate")]
    internal class SupermaterialRefineryConfig_ConfigureBuildingTemplate
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            bool recipe_found = false;
            bool recipe_output_found = false;

            int idx_start = -1;
            int idx_end = -1;

            // Find the code block to modify
            List<CodeInstruction> code = instr.ToList();
            for (int i = code.Count - 1; i >= 0; i--)
            {
                if (recipe_found && recipe_output_found && code[i].opcode == OpCodes.Newarr)
                { Debug.Log("Insulation recipe start point found!"); idx_start = i; break; }

                if (recipe_found && !recipe_output_found && code[i].opcode == OpCodes.Newarr)
                { recipe_output_found = true; }

                if (code[i].opcode == OpCodes.Ldc_I4 && (int)code[i].operand == SimHashes.SuperInsulator.GetHashCode())
                { Debug.Log("Insulation recipe end point found!"); recipe_found = true; idx_end = i; }
            }
            
            // Modify the recipe
            if (idx_start > -1 && idx_end > -1)
            {
                Debug.Log("Modifying insulation recipe...");
                for (int i = idx_start; i < idx_end; i++)
                {
                    //if (code[i].opcode == OpCodes.Ldc_I4 && (int)code[i].operand == SimHashes.Isoresin.GetHashCode())
                    //{ Debug.Log("  Isoresin: 15 kg --> 15 kg"); code[i + 2].operand = 100f; }

                    if (code[i].opcode == OpCodes.Ldc_I4 && (int)code[i].operand == SimHashes.Katairite.GetHashCode())
                    { Debug.Log("  Abyssalite: 80 kg --> 83 kg"); code[i + 2].operand = 103.75f; }

                    if (code[i].opcode == OpCodes.Ldsfld)
                    { Debug.Log("  Reeds: 5 --> 2"); code[i + 2].operand = 40f; }
                }
                Debug.Log("Insulation recipe successfully modified!");
            }

            else
            { Debug.Log("Insulation recipe not found!"); }
           
            Debug.Log("Transpiler complete");
            return code.AsEnumerable(); 
        }
    }
}