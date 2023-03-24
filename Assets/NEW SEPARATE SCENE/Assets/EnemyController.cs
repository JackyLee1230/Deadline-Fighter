using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class EnemyController : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent nav;
    Transform Player;
    Animator controller;
    float health;
    GameManagement game;
    CapsuleCollider capsuleCollider;
    // Use this for initialization
    void Awake()
    {
        nav = GetComponent <UnityEngine.AI.NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponentInParent<Animator>();
        game = FindAnyObjectByType<GameManagement>();
        health = 20 + (1.25F * game.round);
    }
    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(Player.position);
        controller.SetFloat("speed", Mathf.Abs(nav.velocity.x) + Mathf.Abs(nav.velocity.z));
    }

    void ApplyDamage(float damage)

    {
        //print (damage); 
        health -= damage;
        if (health <= 0)
            Death();
        
     }

    void Death()
    {
        // The enemy is dead.
        //isDead = true;
        nav.Stop();
        // Turn the collider into a trigger so shots can pass through it. 
        capsuleCollider.isTrigger = true;
        //anim.SetTrigger ("Dead"); 
        //GameManagement.score += 10;
                             
        //GameManagement.money+= 10;
        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing). 
        //enemyAudio.clip deathClip;
        //enemyAudio.Play ();
        Destroy(gameObject, 4f);
    }
}
