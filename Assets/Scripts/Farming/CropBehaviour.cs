using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CropBehaviour : MonoBehaviour
{
    SeedData seedToGrow;
    [Header("Stages of Life")]
    public GameObject seed;
    public GameObject wilted;
    public GameObject seedling;
    public GameObject harvestable; 

    int growth;
    // how many growth points it takes before it becomes harvestable
    int maxGrowth;
    // crop can stay alive for 48h before it dies
    int maxHealth = GameTimeStamp.HoursToMinutes(48);
    int health;

    int landID;

    public enum CropState
    {
        Seed, Seedling, Harvestable, Wilted
    }
    // current crop growth
    public CropState cropState;

    // initialisation for the crop gameobject, called when player plants a seed
    public void Plant(int landID, SeedData seedToGrow)
    {
        LoadCrop(landID, seedToGrow, CropState.Seed, 0, 0);

        LandManager.Instance.RegisterCrop(landID, seedToGrow, cropState, growth, health); 
    }

    public void LoadCrop(int landID, SeedData seedToGrow, CropState cropState, int growth, int health)
    {
        this.landID = landID;
        //Save the seed information
        this.seedToGrow = seedToGrow;

        seedling = Instantiate(seedToGrow.seedling, transform);

        ItemData cropToYield = seedToGrow.cropToYield;

        harvestable = Instantiate(cropToYield.gameModel, transform);

        int hoursToGrow = GameTimeStamp.DaysToHours(seedToGrow.daysToGrow);
        maxGrowth = GameTimeStamp.HoursToMinutes(hoursToGrow);

        this.growth = growth;
        this.health = health; 

        //Check if it is regrowable
        if (seedToGrow.regrowable)
        {
            //Get the RegrowableHarvestBehaviour from the GameObject
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();

            //Initialise the harvestable 
            regrowableHarvest.SetParent(this);
        }

        //Set the initial state to Seed
        SwitchState(cropState);

    }

    public void Grow()
    {
        growth++;

        // restore some health when its watered
        if(health < maxHealth)
        {
            health++;
        }

        // seed will sprout to seedling when growth is at 50% 
        if(growth >= maxGrowth / 2 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Seedling);
        }
        if(growth >= maxGrowth && cropState != CropState.Harvestable)
        {
            SwitchState(CropState.Harvestable);
        }
        //Inform LandManager on the changes
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }

    // crop withers when soil is dry
    public void Wither()
    {
        health--;
        if(health <= 0 && cropState != CropState.Seed)
        {
            SwitchState(CropState.Wilted);
        }

        //Inform LandManager on the changes
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }

    void SwitchState(CropState stateToSwitch)
    {

    seed.SetActive(stateToSwitch == CropState.Seed);
    wilted.SetActive(stateToSwitch == CropState.Wilted);
    if (stateToSwitch == CropState.Seedling || stateToSwitch == CropState.Harvestable)
    {
        seedling.SetActive(true);
    }
    else
    {
        seedling.SetActive(false);
    }

    // Only show harvestable model when in Harvestable state
    harvestable.SetActive(stateToSwitch == CropState.Harvestable);
    
        /* seed.SetActive(false);
        seedling.SetActive(false);
        harvestable.SetActive(false);
        wilted.SetActive(false);
 */

        switch (stateToSwitch)
        {
            case CropState.Seed:
                seed.SetActive(true);
                break;
            case CropState.Seedling:
                seedling.SetActive(true);
                health = maxHealth;
                break;
            case CropState.Harvestable:
                harvestable.SetActive(true);

                // if seed not regrowable, detach the harvestable from this crop gameobject and destroy it
                if (!seedToGrow.regrowable)
                {
                    harvestable.transform.parent = null;
                    RemoveCrop();
                }
                break;
            case CropState.Wilted:
                wilted.SetActive(true);
                break;
        }

        cropState = stateToSwitch;
    }
    //Destroys and Deregisters the Crop
    public void RemoveCrop()
    {
        LandManager.Instance.DeregisterCrop(landID);
        Destroy(gameObject);
    }

    public void Regrow()
    {
        // reset growth, get regrowth time in hours
        int hoursToRegrow = GameTimeStamp.DaysToHours(seedToGrow.daysToRegrow);
        growth = maxGrowth - GameTimeStamp.HoursToMinutes(hoursToRegrow);
        SwitchState(CropState.Seedling);
    }

}
