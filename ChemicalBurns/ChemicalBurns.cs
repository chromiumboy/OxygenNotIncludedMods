using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;

namespace ChemicalBurns
{
    // Add mod strings
    [HarmonyPatch(typeof(EntityConfigManager), "LoadGeneratedEntities")]
    internal class ChemicalBurns_EntityConfigManager_LoadGeneratedEntities
    {
        private static void Prefix()
        {

            Strings.Add(new string[]
            {
                "STRINGS.DUPLICANTS.STATUSITEMS.CHEMICALBURNS.NOTIFICATION_NAME",
                "Chemical burns"
            });
            Strings.Add(new string[]
            {
                "STRINGS.DUPLICANTS.STATUSITEMS.CHEMICALBURNS.NOTIFICATION_TOOLTIP",
                "Duplicants have been exposed to hazardous chemicals."
            });
            Strings.Add(new string[]
            {
                "STRINGS.DUPLICANTS.STATUSITEMS.CHEMICALBURNS.NAME",
                "Chemical burns"
            });
            Strings.Add(new string[]
            {
                "STRINGS.DUPLICANTS.STATUSITEMS.CHEMICALBURNS.TOOLTIP",
                "This duplicant has been injured from their recent exposure to dangerous chemicals."
            });
        }
    }

    // Update oxygen breather to check for corrosive chemicals
    [HarmonyPatch(typeof(OxygenBreather), "Sim200ms")]
    internal class ChemicalBurns_OxygenBreather_Sim200ms
    {
        private static void Postfix(OxygenBreather __instance, float dt)
        {
            ChemicalBurnMonitor chemicalBurnMonitor = __instance.gameObject.AddOrGet<ChemicalBurnMonitor>();
            chemicalBurnMonitor.CheckForCorrosiveChemicals(dt);
        }
    }
}
