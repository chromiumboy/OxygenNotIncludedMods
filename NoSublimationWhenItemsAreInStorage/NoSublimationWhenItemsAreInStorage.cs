using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace NoSublimationWhenItemsAreInStorage
{
    [HarmonyPatch(typeof(Storage), "OnPrefabInit")]
    internal class NoSublimationWhenItemsAreInStorage_Storage_OnPrefabInit : KMod.UserMod2
    {
        private static void Postfix(ref Storage __instance)
        {
            __instance.allowSublimation = false;

            List<Storage.StoredItemModifier> defaultStoredItemModifers = Traverse.Create(__instance).Field("defaultStoredItemModifers").GetValue<List<Storage.StoredItemModifier>>();
            bool sealFound = false;

            for (int i = 0; i < defaultStoredItemModifers.Count; i++)
            {
                if (defaultStoredItemModifers[i] == Storage.StoredItemModifier.Seal)
                { sealFound = true; break; }
            }

            if (!sealFound)
            {
                defaultStoredItemModifers.Add(Storage.StoredItemModifier.Seal);
                __instance.SetDefaultStoredItemModifiers(defaultStoredItemModifers);
            }
        }
    }
}
