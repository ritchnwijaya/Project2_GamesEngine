using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandInventorySlot : InventorySlot
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        InventoryManager.Instance.HandToInventory(inventoryType);
    }

}
