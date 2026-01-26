using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;



public class MainMenu : MonoBehaviour
{
    public Button loadGameButton; 

    public void NewGame()
    {
        StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.Home, null));
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadGameAsync(SceneTransitionManager.Location.Home, LoadGame));
    }

    void LoadGame()
    {
        if(GameStateManager.Instance == null)
        {
            Debug.LogError("Cannot find Game State Manager!");
            return;
        }
        GameStateManager.Instance.LoadSave();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadGameAsync(SceneTransitionManager.Location scene, Action onFirstFrameLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.ToString());
        //Make this GameObject persistent so it can continue to run after the scene is loaded
        DontDestroyOnLoad(gameObject);
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Loading"); 
        }

        Debug.Log("Loaded!");

        yield return new WaitForEndOfFrame();
        Debug.Log("First frame is loaded");
        onFirstFrameLoad?.Invoke(); 

        Destroy(gameObject);
    }


    void Start()
    {
        //Disable or enable the Load Game button based on whether there is a save file
        loadGameButton.interactable = SaveManager.HasSave(); 
    }
}