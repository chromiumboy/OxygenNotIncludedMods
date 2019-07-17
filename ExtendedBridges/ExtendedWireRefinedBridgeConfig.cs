using System;
using UnityEngine;
using TUNING;

// Done
public class ExtendedWireRefinedBridgeConfig : ExtendedWireBridgeConfig
{
    protected override string GetID()
    {
        return "ExtendedWireRefinedBridge";
    }

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = base.CreateBuildingDef();
        buildingDef.AnimFiles = new KAnimFile[]
        {
            Assets.GetAnim("utilityelectricbridgeconductive_kanim")
        };
        buildingDef.Mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
        buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, buildingDef.PrefabID);
        return buildingDef;
    }

    protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
    {
        WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
        wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max2000;
        return wireUtilityNetworkLink;
    }

    public new const string ID = "ExtendedWireRefinedBridge";
}
