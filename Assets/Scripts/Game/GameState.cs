using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameState : MonoBehaviour
{

    public enum State
    {
        MainMenu,
        Playing,
        GameOver,
        Pause
    }

    private static State state = State.MainMenu;

    public static State GetState()
    {
        return state;
    }

    public enum Event
    {
        StartGame,
        FinishGame,
        EnterMainMenu,
        TogglePause,
    }

    public static void Transition(Event event_)
    {

        switch (state)
        {
            case State.MainMenu:
                if (event_ == Event.StartGame)
                {
                    state = State.Playing;
                    LoadFirstLevel();
                }
                break;
            case State.Playing:
                if (event_ == Event.FinishGame)
                {
                    state = State.GameOver;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    // GamoOverPanelManager.cs shows/hides panel
                }
                if (event_ == Event.TogglePause)
                {
                    state = State.Pause;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                break;
            case State.Pause:
                if (event_ == Event.TogglePause)
                {
                    state = State.Playing;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                if (event_ == Event.EnterMainMenu)
                {
                    state = State.MainMenu;
                    LoadMainMenu();
                }
                break;
            case State.GameOver:
                if (event_ == Event.StartGame)
                {
                    state = State.Playing;
                    // TODO reset time, score and starting position
                    // LoadFirstLevel();
                    throw new NotImplementedException("TODO reset time, score and starting position");
                }
                if (event_ == Event.EnterMainMenu)
                {
                    state = State.MainMenu;
                    LoadMainMenu();
                }
                break;
        }
    }


    private static void LoadFirstLevel()
    {
        SceneManager.LoadScene("Felix Boids Scene");
    }

    private static void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnEnable()
    {
        // registeres delegate function OnSceneLoaded below
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // function is called when a scene is loaded
    // manually sets the state to `Playing` for directly loading the playing scene
    // during development. This function and OnEnable are not invoked when the main
    // menu is loaded, because the main menu
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log("LoadSceneMode: " + mode + " isEditor? " + Application.isEditor);

        string SCENE_LOADED_VIA_EDITOR = "4";

        if (scene.name == "FelixBoidsScene"
            && Application.isEditor
            && mode.ToString() == SCENE_LOADED_VIA_EDITOR)
        {
            state = State.Playing;
        }
    }
}
