using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fressen : MonoBehaviour
{
    int score = 0;
    public Text scoreText;

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
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "boid");// || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )

        {
            score += 1;
            Destroy(col.gameObject);

        }
    }
}
