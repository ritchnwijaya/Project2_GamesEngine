using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BedSleep : InteractableObject
{
    public override void Pickup()
    {
        UIManager.Instance.TriggerPrompt("Do you want to sleep?", GameStateManager.Instance.Sleep); 
    }
}
