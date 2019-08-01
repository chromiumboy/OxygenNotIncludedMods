using System;
using UnityEngine;
using TUNING;

// Done
public class ExtendedLogicWireBridgeConfig : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "ExtendedLogicWireBridge";
        int width = 4;
        int height = 1;
        string anim = "logic_bridge_kanim";
        int hitpoints = 30;
        float construction_time = 3f;
        float[] tier_TINY = { 10f };
        string[] refined_METALS = MATERIALS.REFINED_METALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.LogicBridge;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier_TINY, refined_METALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
        buildingDef.ViewMode = OverlayModes.Logic.ID;
        buildingDef.ObjectLayer = ObjectLayer.LogicGates;
        buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
        buildingDef.Overheatable = false;
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.AudioCategory = "Metal";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.PermittedRotations = PermittedRotations.R360;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        buildingDef.UtilityOutputOffset = new CellOffset(3, 0);
        GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, buildingDef.PrefabID);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
        GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    }

    public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
    {
        base.DoPostConfigurePreview(def, go);
        LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
        logicUtilityNetworkLink.visualizeOnly = true;
        go.AddOrGet<BuildingCellVisualizer>();
        GeneratedBuildings.RegisterLogicPorts(go, ExtendedLogicWireBridgeConfig.INPUT_PORTS);
    }

    public override void DoPostConfigureUnderConstruction(GameObject go)
    {
        base.DoPostConfigureUnderConstruction(go);
        LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
        logicUtilityNetworkLink.visualizeOnly = true;
        go.AddOrGet<BuildingCellVisualizer>();
        GeneratedBuildings.RegisterLogicPorts(go, ExtendedLogicWireBridgeConfig.INPUT_PORTS);
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
        logicUtilityNetworkLink.visualizeOnly = false;
        go.AddOrGet<BuildingCellVisualizer>();
        GeneratedBuildings.RegisterLogicPorts(go, ExtendedLogicWireBridgeConfig.INPUT_PORTS);
    }

    private LogicUtilityNetworkLink AddNetworkLink(GameObject go)
    {
        LogicUtilityNetworkLink logicUtilityNetworkLink = go.AddOrGet<LogicUtilityNetworkLink>();
        logicUtilityNetworkLink.link1 = new CellOffset(-1, 0);
        logicUtilityNetworkLink.link2 = new CellOffset(2, 0);
        return logicUtilityNetworkLink;
    }

    public const string ID = "ExtendedLogicWireBridge";

    public static readonly HashedString BRIDGE_LOGIC_EXTENDED_IO_ID = new HashedString("BRIDGE_LOGIC_EXTENDED_IO");

    public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
    {
        LogicPorts.Port.InputPort(ExtendedLogicWireBridgeConfig.BRIDGE_LOGIC_EXTENDED_IO_ID, new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_INACTIVE, false),
        LogicPorts.Port.InputPort(ExtendedLogicWireBridgeConfig.BRIDGE_LOGIC_EXTENDED_IO_ID, new CellOffset(2, 0), STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_INACTIVE, false)
    };
}
