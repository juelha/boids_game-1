using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public int damage = 34;

    public HealthSystem healthSystem;

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("collision");
        Debug.Log(col.gameObject.tag);
        Debug.Log(col.gameObject);
        if (col.gameObject.tag == "shark")
        {
            Debug.Log("hit");
            this.healthSystem.InflictDamage(this.damage);
        }
    }
}
