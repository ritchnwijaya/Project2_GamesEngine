using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats 
{
    public static int Money { get; private set; }
    public static int Stamina;

    public const string CURRENCY = "€"; 

    public static void Spend(int cost)
    {
        if(cost > Money)
        {
            Debug.LogError("Player does not have enough money");
            return; 
        }
        Money -= cost;
        UIManager.Instance.RenderPlayerStats(); 
    }

    public static void Earn(int income)
    {
        Money += income;
        UIManager.Instance.RenderPlayerStats(); 
    }
    public static void LoadStats(int money, int stamina)
    {
        Money = money;
        Stamina = stamina;
        UIManager.Instance.RenderPlayerStats();
    }

    public static void UseStamina(int staminaLost)
    {
        Stamina -= staminaLost;
        UIManager.Instance.RenderPlayerStats();
    }

    public static void RestoreStamina()
    {
        Stamina = 150;
        UIManager.Instance.RenderPlayerStats();
    }

}
