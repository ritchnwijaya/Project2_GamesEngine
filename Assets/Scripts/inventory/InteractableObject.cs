using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    // the item information the gameobject is supposed to represent
    public ItemData item;

    public virtual void Pickup()
    {
        InventoryManager.Instance.EquipHandSlot(item);
        InventoryManager.Instance.RenderHand();

        Destroy(gameObject);
        
    }
}
