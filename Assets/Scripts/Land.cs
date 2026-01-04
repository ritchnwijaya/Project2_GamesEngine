using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour, ITimeTracker
{
    public enum LandStatus
    {
        Soil, Farmland, Watered
    }

    public LandStatus landStatus;

    public Material soilMat, farmlandMat, wateredMat;

    new Renderer renderer;

// cache the time the land was watered
    public GameObject select;

    GameTimeStamp timeWatered; 

    [Header("Crops")]

    public GameObject cropPrefab;

    CropBehaviour cropPlanted = null; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        renderer = GetComponent<Renderer>();
        SwitchLandStatus(LandStatus.Soil);
        Select(false);

        // add this to TimeManager listener list
        TimeManager.Instance.RegisterTracker(this);
    }


    public void SwitchLandStatus(LandStatus statusToSwitch)
    {
        landStatus = statusToSwitch;
        Material materialToSwitch = soilMat;
        switch (statusToSwitch)
        {
            case LandStatus.Soil:
                materialToSwitch = soilMat;
                break;
            case LandStatus.Farmland:
                materialToSwitch = farmlandMat;
                break;
            case LandStatus.Watered:
                materialToSwitch = wateredMat;

                //cache the time it was watered
                timeWatered = TimeManager.Instance.GetGameTimeStamp();
                break;
        }
        renderer.material = materialToSwitch;
    }

    public void Select(bool toggle)
    {
        select.SetActive(toggle);
    }

    public void Interact()
    {
    ItemData toolSlot = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);
        if(!InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Tool))
        {
            return;
        }
        EquipmentData equipmentTool = toolSlot as EquipmentData;
        if (equipmentTool != null)
        {
            EquipmentData.ToolType toolType = equipmentTool.toolType;
            switch (toolType)
            {
                case EquipmentData.ToolType.Hoe:
                    SwitchLandStatus(LandStatus.Farmland);
                    break;
                case EquipmentData.ToolType.WateringCan:
                    SwitchLandStatus(LandStatus.Watered);
                    break;

                case EquipmentData.ToolType.Shovel:
                    if(cropPlanted != null)
                    {
                        Destroy(cropPlanted.gameObject);
                    }
                    break;
            }
            // we don't have to check for seeds if we have already confirmed the tool to be an equipment
            return;
        }
        SeedData seedTool = toolSlot as SeedData;

        // conditions for player to be able to plant a seed
        //1: player is holding a tool of type seedData
        //2: The Land state must be either watered or farmland
        // 3: There isn't already a crop that has been planted
        if(seedTool != null && landStatus != LandStatus.Soil && cropPlanted == null)
        {
            GameObject cropObject = Instantiate(cropPrefab, transform);
            // move crop object to top of land object
            cropObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            cropPlanted = cropObject.GetComponent<CropBehaviour>();
            cropPlanted.Plant(seedTool);
            InventoryManager.Instance.ConsumeItem(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));

        }
    }

    public void ClockUpdate(GameTimeStamp timestamp)
    {
        // check if 24h passed since last watered
        if(landStatus == LandStatus.Watered)
        {
            //hours since last watered
            int hoursElapsed = GameTimeStamp.CompareTimeStamps(timeWatered, timestamp);
            Debug.Log(hoursElapsed  + "since this was watered");

            // grow the planted crop
            if(cropPlanted != null)
            {
                cropPlanted.Grow();
            }

            if(hoursElapsed > 24)
            {
                // dry up/switch back to farmland
                SwitchLandStatus(LandStatus.Farmland);
            }
        }

        // handle wilting
        if(landStatus != LandStatus.Watered && cropPlanted != null)
        {
            if(cropPlanted.cropState != CropBehaviour.CropState.Seed)
            {
                cropPlanted.Wither();
            }
        }

    }
}
