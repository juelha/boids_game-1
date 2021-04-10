

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class UI_Testing : MonoBehaviour {

    [SerializeField] private HighscoreTable highscoreTable;

    private void Start()
    {

        //Unterscheiden, ob zum Ersten Mal gespielt wird oder ob ein Level beendet wurde, nur dann muss der highscore angegeben werden
        if (PlayerPrefs.GetString("event", "start") != "start")
        {
            //erneut auf start setzten, falls während dem Spiel unterbrochen wird, am Ende dann auf "finished"
            PlayerPrefs.SetString("event", "start");
            UI_Blocker.Show_Static();

            UI_InputWindow.Show_Static("Player Name", "", "ABCDEFGIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvwxyz", 3, () =>
            {
                // Cancel
                UI_Blocker.Hide_Static();
            }, (string nameText) =>
            {
                // Ok
                UI_Blocker.Hide_Static();

                //Get last score
                int score = PlayerPrefs.GetInt("ActScore", 0);
                //Set last score to 0
                PlayerPrefs.SetInt("ActScore", 0);
                highscoreTable.AddHighscoreEntry(score, nameText);
            });

        }
    }
}

