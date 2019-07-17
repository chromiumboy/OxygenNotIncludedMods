using System;
using UnityEngine;
using TUNING;

// Done
public class ExtendedSolidConduitBridgeConfig : SolidConduitBridgeConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "ExtendedSolidConduitBridge";
        int width = 4;
        int height = 1;
        string anim = "utilities_conveyorbridge_kanim";
        int hitpoints = 10;
        float construction_time = 30f;
        float[] tier = { 600f };
        string[] all_METALS = MATERIALS.ALL_METALS;
        float melting_point = 1600f;
        BuildLocationRule build_location_rule = BuildLocationRule.Conduit;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, none, 0.2f);
        buildingDef.ObjectLayer = ObjectLayer.SolidConduitConnection;
        buildingDef.SceneLayer = Grid.SceneLayer.SolidConduitBridges;
        buildingDef.InputConduitType = ConduitType.Solid;
        buildingDef.OutputConduitType = ConduitType.Solid;
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.Overheatable = false;
        buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
        buildingDef.AudioCategory = "Metal";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.PermittedRotations = PermittedRotations.R360;
        buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
        buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, buildingDef.PrefabID);
        return buildingDef;
    }

    public new const string ID = "ExtendedSolidConduitBridge";
}
