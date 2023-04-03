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
    public static float AttackCooldown;

    private bool damageCooldown;


    private void OnTriggerEnter(Collider other)
    {
        if(!damageCooldown && other.tag == "Zombie" && Melee.isAttacking){
                damageCooldown = true;
                StartCoroutine(ResetDamageCooldown());
                if(!other.transform.root.GetComponent<AIExample>().isAware){
                    other.transform.root.GetComponent<AIExample>().onHit(damage*stealthMultipler);
                }else{
                    other.transform.root.GetComponent<AIExample>().onHit(damage);
                }

                Instantiate(HitParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z), other.transform.rotation);

                Instantiate(damageEffect, effectPoint.transform.position, Quaternion.identity);
            }
    }

    IEnumerator ResetDamageCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        damageCooldown = false;
    }
}
