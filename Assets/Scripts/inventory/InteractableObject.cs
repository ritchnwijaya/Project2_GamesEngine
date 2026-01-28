using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    // the item information the gameobject is supposed to represent
    public ItemData item;
    public UnityEvent onInteract = new(); 

    public virtual void Pickup()
    {
        onInteract?.Invoke();
        InventoryManager.Instance.EquipHandSlot(item);
        InventoryManager.Instance.RenderHand();

        Destroy(gameObject);
        
    }
}
