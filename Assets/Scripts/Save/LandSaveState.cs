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
}