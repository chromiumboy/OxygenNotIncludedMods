using System;
using UnityEngine;
using TUNING;

// Done
public class ExtendedGasConduitBridgeConfig : GasConduitBridgeConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "ExtendedGasConduitBridge";
        int width = 4;
        int height = 1;
        string anim = "utilitygasbridge_kanim";
        int hitpoints = 10;
        float construction_time = 3f;
        float[] tier = { 75f };
        string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.Conduit;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, raw_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none, 0.2f);
        buildingDef.ObjectLayer = ObjectLayer.GasConduitConnection;
        buildingDef.SceneLayer = Grid.SceneLayer.GasConduitBridges;
        buildingDef.InputConduitType = ConduitType.Gas;
        buildingDef.OutputConduitType = ConduitType.Gas;
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.Overheatable = false;
        buildingDef.ViewMode = OverlayModes.GasConduits.ID;
        buildingDef.AudioCategory = "Metal";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.PermittedRotations = PermittedRotations.R360;
        buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
        buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
        return buildingDef;
    }

    public new const string ID = "ExtendedGasConduitBridge";
}
