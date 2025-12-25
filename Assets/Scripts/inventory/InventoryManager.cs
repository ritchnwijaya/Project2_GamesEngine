using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance{ get; private set;}

    private void Awake()
    {
        //if there is more than one instance, destroy the extra
        if(Instance!= null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //setting the static instance to this instance
            Instance = this;
        }
    }

    //tool slots
    [Header("Tools")]
    public ItemData[] tools = new ItemData[8];
    public ItemData equippedTool = null;

    //Item slots
    [Header("Items")]
    public ItemData[] items = new ItemData[8];
    public ItemData equippedItem = null;

    void Start()
    {
        
    }

}
