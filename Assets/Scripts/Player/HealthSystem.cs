using System;
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

    public void InflictDamage(int damageAmount)
    {
        this.healthPoints -= damageAmount;
        Debug.Log("health: " + this.healthPoints);

        this.UpdateHealthBarUI();

        if (!this.IsAlive())
        {
            this.ChangeGameState();
        }
    }

    private bool IsAlive()
    {
        return this.healthPoints > 0;
    }

    private void UpdateHealthBarUI()
    {
        float healthBarUIPoints = 0;

        if (this.IsAlive())
        {
            healthBarUIPoints = this.healthPoints / this.MAX_HEALTH_POINTS;
        }
        this.healthBar.transform.localScale = new Vector3(healthBarUIPoints, 1, 1);
    }

    private void ChangeGameState()
    {
        // TODO CHANGE GLOBAL GAME STATE HERE
        throw new NotImplementedException("YOU ARE DEAD! TODO CHANGE GLOBAL GAME STATE HERE");
    }
}
