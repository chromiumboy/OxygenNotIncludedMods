using System;
using UnityEngine;
using TUNING;
using Harmony;

class InsulatedPressureDoorConfig : PressureDoorConfig
{
    public new const string ID = "InsulatedPressureDoor";

    public override BuildingDef CreateBuildingDef()
    {
        string id = "InsulatedPressureDoor";
        int width = 1;
        int height = 2;
        string anim = "door_external_kanim";
        int hitpoints = 60;
        float construction_time = 60f;
        float[] construction_tier = { BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0], BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0] };
        string[] construction_materials = { "BuildableRaw", SimHashes.Steel.ToString() };
        float melting_point = 3200f;
        BuildLocationRule build_location_rule = BuildLocationRule.Tile;
        EffectorValues none = NOISE_POLLUTION.NONE;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_tier, construction_materials, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, none, 1f);
        buildingDef.ThermalConductivity = 0.01f;
        buildingDef.Overheatable = false;
        buildingDef.RequiresPowerInput = true;
        buildingDef.EnergyConsumptionWhenActive = 240f;
        buildingDef.Entombable = false;
        buildingDef.IsFoundation = true;
        buildingDef.ViewMode = OverlayModes.Power.ID;
        buildingDef.TileLayer = ObjectLayer.FoundationTile;
        buildingDef.AudioCategory = "Metal";
        buildingDef.PermittedRotations = PermittedRotations.R90;
        buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
        buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
        SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
        SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
        return buildingDef;
    }

    public static Color32 Color()
    {
        return new Color32(255, 65, 65, 255);
    }
}