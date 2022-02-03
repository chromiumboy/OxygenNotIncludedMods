using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;

public class ChemicalBurnMonitor : KMonoBehaviour
{
    // Create a class to describe corrosive chemicals
    public class CorrosiveChemical
    {
        public float criticalMass = 0f;
        public float damageDealt = 10f;
    }

    // Corrosive chemicals list
    public Dictionary<SimHashes, CorrosiveChemical> chemicalList = new Dictionary<SimHashes, CorrosiveChemical>
    {
        { SimHashes.ChlorineGas, new CorrosiveChemical { criticalMass = 0.1f, damageDealt = 10f } },
        { SimHashes.Chlorine, new CorrosiveChemical { criticalMass = 0.1f, damageDealt = 10f } },
        { SimHashes.SourGas, new CorrosiveChemical { criticalMass = 0.1f, damageDealt = 15f } },
    };

    // Last time the duplicant was burned
    public float lastBurnTime;

    // Make the chemical burn notification
    public static StatusItem status_item = MakeStatusItem();
    public static StatusItem MakeStatusItem()
    {
        StatusItem statusItem = new StatusItem("ChemicalBurns", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 63486);
        statusItem.AddNotification(null, null, null);
        return statusItem;
    }

    // Check for hazardous areas
    public void CheckForCorrosiveChemicals(float dt)
    {
        Health health = gameObject.GetComponent<Health>();
        CorrosiveChemical chemical = CorrosiveChemicalSearch();
        if (chemical != null && health && Time.time - lastBurnTime > 5f)
        {
            health.Damage(chemical.damageDealt);
            lastBurnTime = Time.time;
            gameObject.GetComponent<KSelectable>().AddStatusItem(status_item, this);
        }

        else if (Time.time - lastBurnTime > 5f)
        {
            gameObject.GetComponent<KSelectable>().RemoveStatusItem(status_item, this);
        }
    }

    // Is a chemical hazardous at its current mass
    public bool IsHazardousChemical(SimHashes chemical, float mass)
    {
        if (chemicalList.ContainsKey(chemical) && mass > chemicalList[chemical].criticalMass)
        { return true; }
        
        return false;
    }

    // Is wearing a protective suit
    public Equippable IsWearingProtectiveSuit()
    {
        Equippable result = null;
        MinionIdentity minionIdentity = GetComponent<MinionIdentity>();

        if (minionIdentity != null) { 
            Equipment equipment = minionIdentity.GetEquipment();

            foreach (AssignableSlotInstance assignableSlotInstance in equipment.Slots)
            {
                EquipmentSlotInstance equipmentSlotInstance = (EquipmentSlotInstance)assignableSlotInstance;
                Equippable equippable = equipmentSlotInstance.assignable as Equippable;
                if (equippable && equippable.GetComponent<SuitTank>())
                {
                    result = equippable;
                    break;
                }
            }
        }

        return result;
    }

    // Is standing in a hazardous area
    public CorrosiveChemical CorrosiveChemicalSearch()
    {
        if (!gameObject.HasTag(GameTags.Dead))
        {
            Equippable isWearingProtectiveSuit = IsWearingProtectiveSuit();
            if (!isWearingProtectiveSuit)
            {
                int cell_1 = Grid.PosToCell(gameObject);
                Element element_1 = Grid.Element[cell_1];
                float mass_1 = Grid.Mass[cell_1];

                if (IsHazardousChemical(element_1.id, mass_1))
                { return chemicalList[element_1.id]; }

                int cell_2 = Grid.CellAbove(Grid.PosToCell(gameObject));
                Element element_2 = Grid.Element[cell_2];
                float mass_2 = Grid.Mass[cell_2];

                if (IsHazardousChemical(element_2.id, mass_2))
                { return chemicalList[element_2.id]; }
            }
        }

        return null;
    }
}
