using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fressen : MonoBehaviour
{
    int score = 0;
    public float gametime = 60f; 
    public Text scoreText;
    public Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        

    }

    // Update is called once per frame
    void Update()
    {
        //Update Score
        scoreText.text = "Score: " + score.ToString();

        //Countdown
        timeText.text = (int)gametime + "s";
        gametime -= Time.deltaTime;
        //Min should be 0
        gametime = Mathf.Max(0, gametime);

        //Stop Game if gametime == 0 or if esc
        if (gametime <= 0 || Input.GetButton("Cancel"))
        {
            //Debug.Log("Ende");
            //Not in the Editor, just in build version
            Application.Quit();
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boid") // || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )
        {
            score += 1;
            Destroy(col.gameObject);

        }
    }
}
