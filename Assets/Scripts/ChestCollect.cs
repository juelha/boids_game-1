using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChestCollect : MonoBehaviour
{
    public TextMeshProUGUI chest;
    public Transform chests;
    private int _chestsCollected = 0;

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


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "chest")
        {
            _chestsCollected += 1;
            DisplayChestsUI();

            Destroy(col.gameObject);

            
        }
    }
}
