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

    int score = 0;
    public float gametime = 60f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownTimeText;
    public TextMeshProUGUI speed;

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
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateGameTime();

        this.DisplayScoreUI();
        this.DisplayCountdownTimeUI();
        this.DisplaySpeedUI();

        // this.ShrinkShark();

        if (Input.GetKeyDown(KeyCode.P) || Input.GetButton("Cancel")) // "Cancel" means Escape key
        {
            GameState.Transition(GameState.Event.TogglePause);
        }

        if (this.IsGameOver())
        {
            GameState.Transition(GameState.Event.FinishGame);
        }
    }

    private void UpdateGameTime()
    {
        this.gametime -= Time.deltaTime;
        this.gametime = Mathf.Max(0, gametime); // Min should be 0
    }

    private bool IsGameOver()
    {
        return this.gametime <= 0;
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

    private void DisplayScoreUI()
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

    private void OnCollisionEnter(Collision col)
    //private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Boid") // || col.gameObject.tag == "wall2" || col.gameObject.tag == "wall3" || col.gameObject.tag == "wall4" )
        {
            //Increase shark, when he eats fish, but with maximum size
            transform.localScale += new Vector3(1f, 1f, 0f) * increaseSize;
            if (transform.localScale.x > maxSize)
            {
                transform.localScale = new Vector3(maxSize, maxSize, transform.localScale.z);
            }

            this.score += 1;

            //Destroy
            DestroyBoidAfter(3f, col);

            //slow down when eat fish
            // player.slowDown();
        }
    }
    private void DestroyBoidAfter(float time, Collision coll)
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
