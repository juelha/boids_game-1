using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        GameState.Transition(GameState.Event.StartGame);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
