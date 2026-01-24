using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour, ITimeTracker
{
    public static GameStateManager Instance { get; private set; }

    //Check if the screen has finished fading out
    bool screenFadedOut;


    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Add this to TimeManager's Listener list
        TimeManager.Instance.RegisterTracker(this); 
    }

    public void ClockUpdate(GameTimeStamp timestamp)
        {
            //Update the Land and Crop Save states as long as the player is outside of the Farm scene
            if(SceneTransitionManager.Instance.currentLocation != SceneTransitionManager.Location.farmCity)
            {
                //Retrieve the Land and Farm data from the static variable
                List<LandSaveState> landData = LandManager.farmData.Item1;
                List<CropSaveState> cropData = LandManager.farmData.Item2;

                //If there are no crops planted, we don't need to worry about updating anything
                if (cropData.Count == 0) return;

                for (int i = 0; i < cropData.Count; i++)
                {
                    //Get the crop and corresponding land data
                    CropSaveState crop = cropData[i];
                    LandSaveState land = landData[crop.landID];

                    //Check if the crop is already wilted
                    if (crop.cropState == CropBehaviour.CropState.Wilted) continue;

                    //Update the Land's state
                    land.ClockUpdate(timestamp); 
                    //Update the crop's state based on the land state
                    if(land.landStatus == Land.LandStatus.Watered)
                    {
                        crop.Grow(); 
                    } else if(crop.cropState != CropBehaviour.CropState.Seed)
                    {
                        crop.Wither(); 
                    }

                    //Update the element in the array
                    cropData[i] = crop;
                    landData[crop.landID] = land; 

                }

                LandManager.farmData.Item2.ForEach((CropSaveState crop) => {
                    Debug.Log(crop.seedToGrow + "\n Health: " + crop.health + "\n Growth: " + crop.growth + "\n State: " + crop.cropState.ToString());
                });
            }
        }

    public void Sleep()
    {
        UIManager.Instance.FadeOutScreen();
        screenFadedOut = false;
        StartCoroutine(TransitionTime());
    }

    IEnumerator TransitionTime()
    {
        //Calculate how many ticks we need to advance the time to 6am

        //Get the time stamp of 6am the next day
        GameTimeStamp timestampOfNextDay = TimeManager.Instance.GetGameTimeStamp();
        timestampOfNextDay.day += 1;
        timestampOfNextDay.hour = 6;
        timestampOfNextDay.minute = 0;
        Debug.Log(timestampOfNextDay.day + " " + timestampOfNextDay.hour + ":" + timestampOfNextDay.minute);
        
        while (!screenFadedOut)
        {
            yield return new WaitForSeconds(1f);
        }
        TimeManager.Instance.SkipTime(timestampOfNextDay);
        //Reset the boolean
        screenFadedOut = false;
        UIManager.Instance.ResetFadeDefaults();

    }

    public void OnFadeOutComplete()
    {
        screenFadedOut = true;

    }
}