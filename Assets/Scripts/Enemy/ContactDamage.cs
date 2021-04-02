using UnityEngine;

public class ContactDamage : MonoBehaviour
{

    public int damage = 34;

    //public GameObject healthSytem;
    public HealthSystem healthSystem;


    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("trigger");

        if (col.gameObject.tag == "shark")
        {
            Debug.Log("trigger PLAYER");
            //this.healthSytem.  GetComponent<Scri . inflictDamage(34);
            this.healthSystem.inflictDamage(this.damage);
        }
    }
}
