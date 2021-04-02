using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private float MAX_HEALTH_POINTS = 100f;
    private float healthPoints = 100f;
    public GameObject healthBar;

    void Start()
    {

    }

    public void inflictDamage(int damageAmount)
    {
        this.healthPoints -= damageAmount;
        Debug.Log("DAMAGE DAMAGE DAMAGE " + this.healthPoints);


        float healthBarPercentage = this.healthPoints / this.MAX_HEALTH_POINTS;
        Vector3 tempLocalScale = this.healthBar.transform.localScale;
        this.healthBar.transform.localScale = new Vector3(tempLocalScale.x * healthBarPercentage, tempLocalScale.y, tempLocalScale.z);

        if (this.healthPoints < 0)
        {
            Debug.Log("DEAD DEAD DEAD DEAD DEAD ");
            // TODO CHANGE GLOBAL GAME STATE HERE
            // TODO CHANGE GLOBAL GAME STATE HERE
            // TODO CHANGE GLOBAL GAME STATE HERE
        }
    }
}
