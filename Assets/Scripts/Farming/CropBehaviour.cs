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

    public enum CropState
    {
        Seed, Seedling, Harvestable, Wilted
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

        // check if regrowable
        if (seedToGrow.regrowable)
        {
            // get RegrowableHarvestBehaviour from the game object
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();
            regrowableHarvest.SetParent(this);
        }

        SwitchState(CropState.Seed);

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
    }

    // crop withers when soil is dry
    public void Wither()
    {
        health--;
        if(health <= 0 && cropState != CropState.Seed)
        {
            SwitchState(CropState.Wilted);
        }
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
                    Destroy(gameObject);
                }
                break;
            case CropState.Wilted:
                wilted.SetActive(true);
                break;
        }

        cropState = stateToSwitch;
    }

    public void Regrow()
    {
        // reset growth, get regrowth time in hours
        int hoursToRegrow = GameTimeStamp.DaysToHours(seedToGrow.daysToRegrow);
        growth = maxGrowth - GameTimeStamp.HoursToMinutes(hoursToRegrow);
        SwitchState(CropState.Seedling);
    }
}
