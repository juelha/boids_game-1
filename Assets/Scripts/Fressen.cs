using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Fressen : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement player;
    int score = 0;
    public float gametime = 60f; 
    public Text scoreText;
    public Text timeText;
    public Text speed;

    public float maxSize = 50f;
    public float minSize = 3f;
    public float increaseSize = 0.1f;
    public float decreaseSize = 0.001f;

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

        //display speed
        speed.text = player.speed.ToString("F2");
        //Make shark smaller with time
        transform.localScale -= new Vector3(1f, 1f, 0f) * decreaseSize;
        if (transform.localScale.x < minSize)
        {
            transform.localScale = new Vector3(minSize, minSize, transform.localScale.z);
            //ende!
        }




        //Stop Game if gametime == 0 or if esc
        if (gametime <= 0 || Input.GetButton("Cancel"))
        {
            //Debug.Log("Ende");
            //Not in the Editor, just in build version
            Application.Quit();
        }


    }

    private void OnCollisionEnter(Collision col)
    //private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Boid") // || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )
        {
            //Increase shark, when he eats fish, but with maximum size 
            transform.localScale += new Vector3(1f, 1f, 0f) * increaseSize;
            if(transform.localScale.x > maxSize)
            {
                transform.localScale = new Vector3(maxSize, maxSize, transform.localScale.z);
            }

                //Debug.Log(score);
            score += 1;
            Destroy(col.gameObject);

            //slow down when eat fish
            player.slowDown();

        }
    }
}
