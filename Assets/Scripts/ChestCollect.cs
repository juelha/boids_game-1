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

    // Start is called before the first frame update
    void Start()
    {
        DisplayChestsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayChestsUI()
    {
        chest.text = "Chests: " + _chestsCollected.ToString() + " / " + chests.childCount.ToString();
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "shark")
        {
            _chestsCollected += 1;
            DisplayChestsUI();

            Destroy(this.gameObject);

            
        }
    }
}
