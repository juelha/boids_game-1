using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EatingBehavior : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement player;
    //public Transform parent;

    [HideInInspector]
    public int score = 0;

    public float gametime = 60f;
    public TextMeshProUGUI addScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownTimeText;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI highScore;

    public bool scaling = false;

    public float maxSize = 50f;
    public float minSize = 3f;
    public float increaseSize = 0.1f;
    public float decreaseSize = 0.0001f;

    //Immer nur einen auf einmal
    //private bool amFressen = false;
    // Start is called before the first frame update
    void Start()
    {
        this.score = 0;
        this.highScore.text = "HighScore: " + PlayerPrefs.GetInt("HighScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateGameTime();

        this.DisplayScoreUI();
        this.DisplayCountdownTimeUI();
        this.DisplaySpeedUI();

        // this.ShrinkShark();

        if (this.IsGameOver())
        {
            // TODO CHANGE GLOBAL GAME STATE HERE
            throw new NotImplementedException("TIME IS OVER! TODO CHANGE GAME STATE");
        }
    }

    private void UpdateGameTime()
    {
        this.gametime -= Time.deltaTime;
        this.gametime = Mathf.Max(0, gametime); // Min should be 0
    }

    private bool IsGameOver()
    {
        return this.gametime <= 0 || Input.GetButton("Cancel"); // "Cancel" means Escape key
    }


    private void ShrinkShark()
    {
        // Make shark smaller with time
        transform.localScale -= new Vector3(1f, 1f, 0f) * decreaseSize;
        if (transform.localScale.x < minSize)
        {
            transform.localScale = new Vector3(minSize, minSize, transform.localScale.z);
            //ende!
        }
    }

    //shows sth for x secs
    IEnumerator DisplayAddScoreUI(int PlusScore,  float time)
    {
        this.addScore.text = "+" + PlusScore.ToString();
        yield return new WaitForSeconds(time);
        this.addScore.text = "";
    }

    private void HighScore()
    {
        if(this.score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", this.score);
            highScore.text = "HighScore: " + this.score.ToString();
        }
    }
    //Adds some points to your score
    public void AddScore(int plusScore)
    {
        this.score += plusScore;

        HighScore();
        //shows your benefit for 3 secs
        StartCoroutine(DisplayAddScoreUI(plusScore, 3f));
        DisplayScoreUI();
    }
    public void DisplayScoreUI()
    {
        this.scoreText.text = "Score: " + score.ToString();
    }



    private void DisplayCountdownTimeUI()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds((int)gametime);
        this.countdownTimeText.text = timeSpan.ToString("m':'ss");
    }

    private void DisplaySpeedUI()
    {
        this.speed.text = player.speed.ToString("F2");
    }

    //private void OnCollisionEnter(Collision col)
    ////private void OnTriggerEnter(Collider col)
    //{
    //    Debug.Log("collision");
    //    Debug.Log(col.gameObject.tag);
    //    Debug.Log(col.gameObject);
    //    Debug.Log("Tag Collider" + col.collider.gameObject.tag);
    //    if (col.gameObject.tag == "Boid") // || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )
    //    {
    //        //Increase shark, when he eats fish, but with maximum size
    //        if (scaling)
    //        {
    //            transform.localScale += new Vector3(1f, 1f, 0f) * increaseSize;
    //            if (transform.localScale.x > maxSize)
    //            {
    //                transform.localScale = new Vector3(maxSize, maxSize, transform.localScale.z);
    //            }
    //        }
    //        this.score += 1;
    //
    //        //Destroy
    //        DestroyBoidAfter(3f, col);
    //
    //slow down when eat fish
    // player.slowDown();
        //}
    //}
    private void OnTriggerEnter(Collider col)
    //private void OnTriggerEnter(Collider col)
    {
        Debug.Log("collision");
        Debug.Log(col.gameObject.tag);
        Debug.Log(col.gameObject);
        //Debug.Log("Tag Collider" + gameObject.tag);
        if (col.gameObject.tag == "Boid") // || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )
        {
            //Increase shark, when he eats fish, but with maximum size
            if (scaling)
            {
                transform.localScale += new Vector3(1f, 1f, 0f) * increaseSize;
                if (transform.localScale.x > maxSize)
                {
                    transform.localScale = new Vector3(maxSize, maxSize, transform.localScale.z);
                }
            }

            
            AddScore(1);
            //Destroy
            DestroyBoidAfter(3f, col);
    
            //slow down when eat fish
            // player.slowDown();
        }
    }

    private void DestroyBoidAfter(float time, Collider coll)
    {
        //Highlight boid
        Material boid = coll.gameObject.GetComponentInChildren<Renderer>().material;
        boid.SetColor("_Color", Color.red);

        //Destroy all movement skripts
        Destroy(coll.gameObject.GetComponent<BoidAlignment>());
        Destroy(coll.gameObject.GetComponent<BoidCohesion>());
        Destroy(coll.gameObject.GetComponent<BoidContainerBehavior>());
        Destroy(coll.gameObject.GetComponent<BoidSeparation>());
        Destroy(coll.gameObject.GetComponent<Boid>());

        //Set new Parent = Player
        coll.gameObject.transform.SetParent(player.gameObject.transform);

        Destroy(coll.gameObject, time);

    }
}
