using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChestCollect : MonoBehaviour
{
    [SerializeField]
    private EatingBehavior shark;
    public TextMeshProUGUI chest;
    public Transform chests;
    private int _chestsCollected = 0;

    public int pointsForOneChest = 10;

    private int _chestCountAll = 0;
    // Start is called before the first frame update
    void Start()
    {
        _chestCountAll = chests.childCount;
        DisplayChestsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayChestsUI()
    {
        chest.text = "Chests: " + _chestsCollected.ToString() + " / " + _chestCountAll.ToString() ;
    }


    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Hit");
        if (col.gameObject.tag == "chest")
        {
            _chestsCollected += 1;
            DisplayChestsUI();

            //change Color
            Material thisChest = col.gameObject.GetComponentInChildren<Renderer>().material;
            thisChest.SetColor("_Color", Color.red);
            Destroy(col.gameObject, 1f);

            //add score for chest,
            //1. chest +10, 5. chest +50
            shark.AddScore(pointsForOneChest * _chestsCollected);


            //shark.score += pointsForOneChest * _chestsCollected;
            //shark.DisplayScoreUI();



        }
    }
}
