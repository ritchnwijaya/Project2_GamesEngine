using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegrowableHarvestBehaviour : InteractableObject
{
    CropBehaviour parentCrop;

    public void SetParent(CropBehaviour parentCrop)
    {
        this.parentCrop = parentCrop;

    }
    public override void Pickup()
    {
        if (!isActiveAndEnabled) return;
        InventoryManager.Instance.EquipHandSlot(item);
        InventoryManager.Instance.RenderHand();
        UIManager.Instance.RenderInventory();

        parentCrop.Regrow();

    }
}
