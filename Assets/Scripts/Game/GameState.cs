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

    public static void TransitionTo(State nextState)
    {

        switch (state)
        {
            case State.MainMenu:
                if (nextState == State.Playing)
                {
                    state = nextState;
                    LoadFirstLevel();
                }
                break;
            case State.Playing:
                if (nextState == State.GameOver)
                {
                    state = nextState;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    // GamoOverPanelManager.cs shows/hides panel
                }
                break;
            case State.GameOver:
                if (nextState == State.Playing)
                {
                    state = nextState;
                    // TODO reset time, score and starting position
                    // LoadFirstLevel();
                    throw new NotImplementedException("TODO reset time, score and starting position");
                }
                if (nextState == State.MainMenu)
                {
                    state = nextState;
                    LoadMainMenu();
                    break;
                }
                break;
        }
    }

    private static void LoadFirstLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private static void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // 1 or "SomeScene"
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

        if (scene.name == "SampleScene"
            && Application.isEditor
            && mode.ToString() == SCENE_LOADED_VIA_EDITOR)
        {
            state = State.Playing;
        }
    }
}
