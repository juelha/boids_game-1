using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public int damage = 34;

    private float timeSinceLastHit = 0f;
    public HealthSystem healthSystem;

    private void Update()
    {
        if(timeSinceLastHit < 5f)
        {
            timeSinceLastHit += Time.deltaTime;
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("collision");
        Debug.Log(col.gameObject.tag);
        Debug.Log(col.gameObject);
        //Just if collision with body and prevent 2 hits at same time
        if (col.gameObject.tag == "shark" && col.collider.gameObject.tag == "body" && timeSinceLastHit > 2f) 
        {
            Debug.Log(col.collider.gameObject.tag);
            
            timeSinceLastHit = 0f;

            Debug.Log("hit");
            this.healthSystem.InflictDamage(this.damage);
        }
    }
}
