using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToToggle : MonoBehaviour
{
    public GameObject Panel;

    public void OpenPanel ()
    {
        if(Panel != null)
        {
            //change visibility
            Panel.SetActive(!Panel.activeSelf);
        }
    }

}
