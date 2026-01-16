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
        playerPoint = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>().transform;
    }

    public void SwitchLocation(Location locationToSwitch)
    {
        SceneManager.LoadScene(locationToSwitch.ToString());
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