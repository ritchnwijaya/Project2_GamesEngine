using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShippingBin : InteractableObject
{
    public static int hourToShip = 18;
    public static List<ItemSlotData> itemsToShip = new();

    public override void Pickup()
    {
        //Get the item data of the item the player is trying to throw in
        ItemData handSlotItem = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Item);
        if (handSlotItem == null) return; 
        UIManager.Instance.TriggerPrompt($"Do you want to sell {handSlotItem.name} ? ", PlaceItemsInShippingBin);
    }


    void PlaceItemsInShippingBin()
    {
        ItemSlotData handSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);
        itemsToShip.Add(new ItemSlotData(handSlot));

        //Empty out the hand slot since it's moved to the shipping bin
        handSlot.Empty();
        InventoryManager.Instance.RenderHand();
        UIManager.Instance.RenderInventory(); 
    }

    public static void ShipItems()
    {
        int moneyToReceive = CalculateItems(itemsToShip);
        //Convert the items to money
        PlayerStats.Earn(moneyToReceive);
        itemsToShip.Clear(); 
    }

    static int CalculateItems(List<ItemSlotData> items)
    {
        int total = 0;
        foreach(ItemSlotData item in items)
        {
            total += item.quantity * item.itemData.cost; 
        }
        return total; 
    }
}
