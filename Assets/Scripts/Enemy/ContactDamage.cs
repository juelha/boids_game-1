using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public int damage = 34;

    public HealthSystem healthSystem;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "shark")
        {
            this.healthSystem.InflictDamage(this.damage);
        }
    }
}
