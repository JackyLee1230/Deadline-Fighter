using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Melee Melee;
    public GameObject HitParticle;
    public GameObject damageEffect;
    public int damage = 100;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Zombie" && Melee.isAttacking)
        {
            other.transform.GetComponent<AIExample>().onHit(damage);
            Debug.Log(other.name);
            Instantiate(HitParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z), other.transform.rotation);
        }
    }
}
