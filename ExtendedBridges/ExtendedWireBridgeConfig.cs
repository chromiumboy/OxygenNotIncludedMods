using System;
using UnityEngine;
using TUNING;

// Done
public class ExtendedWireBridgeConfig : IBuildingConfig
{
    protected virtual string GetID()
    {
        return "ExtendedWireBridge";
    }

    public override BuildingDef CreateBuildingDef()
    {
        string id = this.GetID();
        int width = 4;
        int height = 1;
        string anim = "utilityelectricbridge_kanim";
        int hitpoints = 30;
        float construction_time = 3f;
        float[] tier = { 35f };
        string[] all_METALS = MATERIALS.ALL_METALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.WireBridge;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
        buildingDef.Overheatable = false;
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.ViewMode = OverlayModes.Power.ID;
        buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
        buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
        buildingDef.AudioCategory = "Metal";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.PermittedRotations = PermittedRotations.R360;
        buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
        buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, buildingDef.PrefabID);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    }

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
    {
        base.DoPostConfigurePreview(def, go);
        WireUtilityNetworkLink wireUtilityNetworkLink = this.AddNetworkLink(go);
        wireUtilityNetworkLink.visualizeOnly = true;
        go.AddOrGet<BuildingCellVisualizer>();
    }

    public override void DoPostConfigureUnderConstruction(GameObject go)
    {
        base.DoPostConfigureUnderConstruction(go);
        Constructable component = go.GetComponent<Constructable>();
        component.choreTags = GameTags.ChoreTypes.WiringChores;
        WireUtilityNetworkLink wireUtilityNetworkLink = this.AddNetworkLink(go);
        wireUtilityNetworkLink.visualizeOnly = true;
        go.AddOrGet<BuildingCellVisualizer>();
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        WireUtilityNetworkLink wireUtilityNetworkLink = this.AddNetworkLink(go);
        wireUtilityNetworkLink.visualizeOnly = false;
        go.AddOrGet<BuildingCellVisualizer>();
    }

    protected virtual WireUtilityNetworkLink AddNetworkLink(GameObject go)
    {
        WireUtilityNetworkLink wireUtilityNetworkLink = go.AddOrGet<WireUtilityNetworkLink>();
        wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max1000;
        wireUtilityNetworkLink.link1 = new CellOffset(-1, 0);
        wireUtilityNetworkLink.link2 = new CellOffset(2, 0);
        return wireUtilityNetworkLink;
    }

    public const string ID = "ExtendedWireBridge";
}
