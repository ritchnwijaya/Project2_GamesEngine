using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public enum Location { farmCity, Home}
    public Location currentLocation;
    Transform playerPoint;
    bool screenFadedOut;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; 
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLocationLoad;

        //Find the player's transform
        playerPoint = FindFirstObjectByType<PlayerMovement>().transform;
    }

    public void SwitchLocation(Location locationToSwitch)
    {
        UIManager.Instance.FadeOutScreen();
        screenFadedOut = false;
        StartCoroutine(ChangeScene(locationToSwitch)); 
    } 

    IEnumerator ChangeScene(Location locationToSwitch)
    {
        CharacterController playerCharacter = playerPoint.GetComponent<CharacterController>();
        playerCharacter.enabled = false;
        while (!screenFadedOut)
        {
            yield return new WaitForSeconds(0.1f); 
        }
        screenFadedOut = false;
        UIManager.Instance.ResetFadeDefaults();
        SceneManager.LoadScene(locationToSwitch.ToString());
    }

    public void OnFadeOutComplete()
    {
        screenFadedOut = true;
        
    }

    public void OnLocationLoad(Scene scene, LoadSceneMode mode)
    {
        //The location the player is coming from when the scene loads
        Location oldLocation = currentLocation;

        //Get the new location by converting the string of our current scene into a Location enum value
        Location newLocation = (Location) Enum.Parse(typeof(Location), scene.name);

        if (currentLocation == newLocation) return; 

        //Find and change the player position to the start point
        Transform startPoint = LocationManager.Instance.GetPlayerStartingPosition(oldLocation);

        CharacterController playerCharacter = playerPoint.GetComponent<CharacterController>();
        playerCharacter.enabled = false; 

        playerPoint.position = startPoint.position;
        playerPoint.rotation = startPoint.rotation;

        playerCharacter.enabled = true; 

        //Save the current location that we just switched to
        currentLocation = newLocation; 
    }

}