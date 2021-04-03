using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public int damage = 34;
    public float impulse;
    public HealthSystem healthSystem;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "shark")
        {
            col.gameObject.GetComponent<Rigidbody>().velocity = -col.contacts[0].normal * 100000;
            Debug.Log(col.gameObject);
            //col.gameObject.transform.GetComponent<CharacterController>().Move(col.contacts[0].normal * impulse * Time.deltaTime);
            this.healthSystem.InflictDamage(this.damage);
        }
    }
}
