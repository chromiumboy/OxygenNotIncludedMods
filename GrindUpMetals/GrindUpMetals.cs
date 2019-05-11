using System.Collections.Generic;
using Harmony;

namespace GrindUpMetals
{
    [HarmonyPatch(typeof(RockCrusherConfig), "ConfigureBuildingTemplate")]
    internal class RockCrusherConfig_ConfigureBuildingTemplate
    {
        private static void Postfix(RockCrusherConfig __instance)
        {
            Debug.Log("RockCrusherConfig.ConfigureBuildingTemplate postfix loaded");
            ComplexRecipe complexRecipe;

            Tag regolith_tag = SimHashes.Regolith.CreateTag();
            Tag sand_tag = SimHashes.Sand.CreateTag();

            List<Element> list = new List<Element>() {
                ElementLoader.GetElement(SimHashes.Cuprite.CreateTag()),
                ElementLoader.GetElement(SimHashes.GoldAmalgam.CreateTag()),
                ElementLoader.GetElement(SimHashes.IronOre.CreateTag()),
                ElementLoader.GetElement(SimHashes.Wolframite.CreateTag()),
            };

            foreach (Element element in list)
            {
                Element highTempTransition = element.highTempTransition;
                Element lowTempTransition = highTempTransition.lowTempTransition;
                if (lowTempTransition != element)
                {
                    ComplexRecipe.RecipeElement[] inputs = new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(lowTempTransition.tag, 100f),
                        new ComplexRecipe.RecipeElement(regolith_tag, 50f),
                    };
                    ComplexRecipe.RecipeElement[] outputs = new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(element.tag, 100f),
                        new ComplexRecipe.RecipeElement(sand_tag, 50f),
                    };
                    string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element.tag);
                    string text = ComplexRecipeManager.MakeRecipeID("RockCrusher", inputs, outputs);
                    complexRecipe = new ComplexRecipe(text, inputs, outputs);
                    complexRecipe.time = 40f;
                    complexRecipe.description = string.Format("Grind up {0} to create {1}.", lowTempTransition.name, element.name);
                    complexRecipe.useResultAsDescription = true;
                    complexRecipe.displayInputAndOutput = true;
                    complexRecipe.fabricators = new List<Tag>
                    {
                        TagManager.Create("RockCrusher")
                    };
                    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, text);
                }
            }
        }
    }
}