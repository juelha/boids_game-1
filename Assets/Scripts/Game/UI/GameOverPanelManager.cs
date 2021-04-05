using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanelManager : MonoBehaviour
{

    public GameObject gameOverPanel;

    void Start()
    {
        this.gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.GetState() == GameState.State.GameOver
            && this.gameOverPanel.activeSelf == false)
        {
            this.gameOverPanel.SetActive(true);
            Debug.Log("GameOver Panel actived");
        }
        if (GameState.GetState() != GameState.State.GameOver
            && this.gameOverPanel.activeSelf == true)
        {
            this.gameOverPanel.SetActive(false);
            Debug.Log("GameOver Panel deactivated");
        }
    }

    public void MainMenu()
    {
        GameState.TransitionTo(GameState.State.MainMenu);
    }

    public void Play()
    {
        GameState.TransitionTo(GameState.State.Playing);
    }
}
