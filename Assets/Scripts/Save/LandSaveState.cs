using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct LandSaveState
{
    public Land.LandStatus landStatus;
    public GameTimeStamp lastWatered;

    public LandSaveState(Land.LandStatus landStatus, GameTimeStamp lastWatered)
    {
        this.landStatus = landStatus;
        this.lastWatered = lastWatered;
    }

    public void ClockUpdate(GameTimeStamp timestamp)
    {
        //Checked if 24 hours has passed since last watered
        if (landStatus == Land.LandStatus.Watered)
        {
            //Hours since the land was watered
            int hoursElapsed = GameTimeStamp.CompareTimeStamps(lastWatered, timestamp);
            Debug.Log(hoursElapsed + " hours since this was watered");

            if (hoursElapsed > 24)
            {
                //Dry up (Switch back to farmland)
                landStatus = Land.LandStatus.Farmland; 
            }
        }

        
    }
}