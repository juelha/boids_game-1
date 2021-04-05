using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : MonoBehaviour
{

    public void Play()
    {
        GameState.TransitionTo(GameState.State.Playing);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
