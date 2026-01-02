using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Seed")]
public class SeedData : ItemData
{
    public int daysToGrow;
    public ItemData cropToYield;

    // the seedling gameobject
    public GameObject seedling;

    [Header("Regrowable")]
    public bool regrowable;
    public int daysToRegrow;

}
