using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CropBehaviour : MonoBehaviour
{
    SeedData seedToGrow;
    [Header("Stages of Life")]
    public GameObject seed;
    public GameObject seedling;
    public GameObject harvestable;

    int growth;
    // how many growth points it takes before it becomes harvestable
    int maxGrowth;

    public enum CropState
    {
        Seed, Seedling, Harvestable
    }
    // current crop growth
    public CropState cropState;

    // initialisation for the crop gameobject, called when player plants a seed
    public void Plant(SeedData seedToGrow)
    {
        this.seedToGrow = seedToGrow;
        seedling = Instantiate(seedToGrow.seedling, transform);
        
        ItemData cropToYield = seedToGrow.cropToYield;

        harvestable = Instantiate(cropToYield.gameModel, transform);

        // convert days to grow into hours
        int hoursToGrow  = GameTimeStamp.DaysToHours(seedToGrow.daysToGrow);
        maxGrowth = GameTimeStamp.HoursToMinutes(hoursToGrow);

        SwitchState(CropState.Seed);

    }

    public void Grow()
    {
        growth++;

        // seed will sprout to seedling when growth is at 50% 
        
        if(growth >= maxGrowth / 2 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Seedling);
        }
        if(growth >= maxGrowth && cropState == CropState.Seedling)
        {
            SwitchState(CropState.Harvestable);
        }
    }

    void SwitchState(CropState stateToSwitch)
    {
        seed.SetActive(false);
        seedling.SetActive(false);
        harvestable.SetActive(false);

        switch (stateToSwitch)
        {
            case CropState.Seed:
                seed.SetActive(true);
                break;
            case CropState.Seedling:
                seedling.SetActive(true);
                break;
            case CropState.Harvestable:
                harvestable.SetActive(true);
                harvestable.transform.parent = null;
                Destroy(gameObject);
                break;
        }

        cropState = stateToSwitch;
    }
}
