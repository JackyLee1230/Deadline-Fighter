using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Melee Melee;
    public GameObject HitParticle;
    public GameObject damageEffect;
    public GameObject effectPoint;
    public int damage;
    public float stealthMultipler;

    private GameObject onHitEffectHold;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Zombie" && Melee.isAttacking)
        {
            if(!other.transform.GetComponent<AIExample>().isAware){
                other.transform.GetComponent<AIExample>().onHit(damage*stealthMultipler);
            }else{
                other.transform.GetComponent<AIExample>().onHit(damage);
            }

            Instantiate(HitParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z), other.transform.rotation);

            onHitEffectHold = Instantiate(damageEffect, effectPoint.transform.position, Quaternion.identity ) as GameObject;
//            onHitEffectHold.transform.parent = effectPoint.transform;
        }
    }
}
