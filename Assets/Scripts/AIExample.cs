﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;

public class AIExample : MonoBehaviour {
    public enum WanderType { Random, Waypoint};

    [Header("zombie camera and hit scan")]
    public Camera AttackRaycastArea;

    public FirstPersonController fpsc;
    public WanderType wanderType = WanderType.Random;
    [SerializeField] public float health;
    [SerializeField] public float wanderSpeed;
    [SerializeField] public float chaseSpeed;
    [SerializeField] public float fov;
    [SerializeField] public float viewDistance;
    [SerializeField] public float chaseDistance;
    [SerializeField] public float wanderRadius;
    [SerializeField] public float attackRadius;
    public Transform[] waypoints; //Array of waypoints is only used when waypoint wandering is selected

    [SerializeField] private float detectDistance;
    [SerializeField] public bool isAware = false;

    [SerializeField] public bool forcedAware = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDamage = false;
    [SerializeField] public bool isDead = false;
    private Vector3 wanderPoint;
    private NavMeshAgent agent;
    private Renderer renderer;
    private int waypointIndex = 0;
    private Animator animator;

    private Vector3 lastSeenPlayerPosition;

    [SerializeField] public float AttackCooldown;
    [SerializeField] public float AttackCooldownMultiplier;
    [SerializeField] public float DamagedCooldown;
    [SerializeField] public float DamagedCooldownMultiplier;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask layerMask = 1 << 8;

    private int isPlayerStealth;

    [Header("audio")]
    [SerializeField] public AudioClip zombieIdle;
    [SerializeField] public AudioClip zombieChase;
    [SerializeField] public AudioClip zombieDamage;
    [SerializeField] public AudioClip zombieAttack;
    [SerializeField] public AudioClip zombieDie;
    [SerializeField] public AudioClip zombieNotice;

    private AudioSource e_AudioSource;

    [Header("drops")]
    [SerializeField] public GameObject healthBox;
//    [SerializeField] public Animator healthBoxAnimator;
    [SerializeField] public GameObject ammoBox;
//    [SerializeField] public Animator ammoBoxAnimator;


    public void Start()
    {
        e_AudioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        wanderPoint = RandomWanderPoint();
        AttackCooldown = 0f;
    }
    public void Update()
    {
        isPlayerStealth = fpsc.GetPlayerStealthProfile();
        if (isDead)
        {
            if (AttackCooldown > 0f)
            {
                animator.SetBool("Dead1", true);
            }
            else
            {
                animator.SetBool("Dead2", true);
            }
        }
        else if (isDamage)
        {
            if (AttackCooldown > 0.7f && !(DamagedCooldown > 0f))
            {
                animator.SetTrigger("GreatDamage");
                AttackCooldown = 0.977f * AttackCooldownMultiplier;
                DamagedCooldown = 0.977f * DamagedCooldownMultiplier;
                transform.Translate(transform.forward * -0.5f, Space.World);
            }
            else
            {
                animator.SetTrigger("Damage");
            }
            

            isDamage = false;
        }
        else if (isAttacking)
        {
            e_AudioSource.clip = zombieAttack;
            e_AudioSource.Play();
            animator.SetTrigger("Attack");
            isAttacking = false;
        }
        else if (isAware || forcedAware)
        {
            if (!e_AudioSource.isPlaying) {
                e_AudioSource.clip = zombieChase;
                e_AudioSource.Play();
            }

            if (!isAttacking) { 
                lastSeenPlayerPosition = fpsc.transform.position;
                agent.SetDestination(fpsc.transform.position);
                animator.SetBool("Aware", true);
                if(!(AttackCooldown > 0f))
                    agent.speed = chaseSpeed;
                AttackPlayer();
                //renderer.material.color = Color.red;
            }

            if(Vector3.Distance(fpsc.transform.position, AttackRaycastArea.transform.position) > chaseDistance){
                if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f){
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, fpsc.transform.position, out hit, ~layerMask)){
                        if (!hit.transform.CompareTag("Player")){
                            isAware = false;
                        }
                    }
                    else
                    {
                        isAware = false;
                    }
                }
            }
        } else
        {
            if (!e_AudioSource.isPlaying) {
                e_AudioSource.clip = zombieIdle;
                e_AudioSource.Play();
            }

            SearchForPlayer();
            if(lastSeenPlayerPosition != Vector3.zero){
                if(agent.transform.position.x == lastSeenPlayerPosition.x && agent.transform.position.z == lastSeenPlayerPosition.z){
                    lastSeenPlayerPosition = Vector3.zero;
                }
                else{
                    agent.speed = wanderSpeed*2f;
                    agent.SetDestination(lastSeenPlayerPosition);
                }
            }
            else{
                agent.speed = wanderSpeed;
                Wander();
            }
            animator.SetBool("Aware", false);
            //renderer.material.color = Color.blue;
        }

        // if attack cooldown is greater than 0, reduce it by 1 every second
        if (AttackCooldown > 0f && !(DamagedCooldown > 0f) && !isDead){
            agent.speed = chaseSpeed*0.55f;
            AttackCooldown -= Time.deltaTime;
        }

        // if attack cooldown is greater than 0, reduce it by 1 every second
        else if (DamagedCooldown > 0f && !isDead)
        {
            agent.speed = 0f;
            DamagedCooldown -= Time.deltaTime;
        }
    }

    public void SearchForPlayer()
    {
        float modifiedViewDistance = isPlayerStealth == 2 ? viewDistance*2.5f : isPlayerStealth == 1 ? viewDistance : viewDistance/1.5f;
        float modifiedDetectDistance = isPlayerStealth == 2 ? detectDistance*3.5f : isPlayerStealth == 1 ? detectDistance : 0;

        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f)
        {
            if (Vector3.Distance(fpsc.transform.position, transform.position) < modifiedViewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, fpsc.transform.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        lastSeenPlayerPosition = hit.point;

                        OnAware();
                        Debug.Log("Tracked Player");
                        AttackPlayer();
                    }
                }
            }
        }
        else if ((Vector3.Distance(fpsc.transform.position, transform.position) < modifiedDetectDistance && modifiedDetectDistance != 0)) {
            OnAware();
        }
    }

    public void OnAware()
    {
        e_AudioSource.clip = zombieNotice;
        e_AudioSource.Play();
        isAware = true;
    }


    public void Wander()
    {
        if (wanderType == WanderType.Random)
        {
            if (Vector3.Distance(transform.position, wanderPoint) < 2f)
            {
                wanderPoint = RandomWanderPoint();
            }
            else
            {
                agent.SetDestination(wanderPoint);
            }
        }
        else
        {
            //Waypoint wandering
            if (waypoints.Length >= 2)
            {
                if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) < 2f)
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Length;
                }
                else
                {
                    agent.SetDestination(waypoints[waypointIndex].position);
                }
            } else
            {
                Debug.LogWarning("Please assign more than 1 waypoint to the AI: " + gameObject.name);
            }
        }
    }

    public void AttackPlayer(){
        // if (AttackCooldown <= 0){
        if(AttackCooldown <= 0f){
            RaycastHit hitInfo;
            if (Physics.Raycast(AttackRaycastArea.transform.position, AttackRaycastArea.transform.forward, out hitInfo, attackRadius, ~layerMask)){
                if (hitInfo.transform.CompareTag("Player")){
                    isAttacking = true;
                    FirstPersonController fpsc = hitInfo.transform.GetComponent<FirstPersonController>();
                    StartCoroutine(DelayAttackPlayer(hitInfo, fpsc));
                    Debug.Log("Zombie Hitting Player"); 
                    AttackCooldown = 1.4f * AttackCooldownMultiplier;
                }
            }

            // transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }

    public Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }

    IEnumerator RemoveGameObject()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        Destroy(GetComponent<NavMeshAgent>());
    }

    IEnumerator DelayAttackPlayer(RaycastHit hitInfo, FirstPersonController fpsc)
    {
        yield return new WaitForSeconds(0.75f);

        if(!(DamagedCooldown > 0f) && !isDead)
            fpsc.takeDamage(10, AttackRaycastArea.transform.position, Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f);
            // push the zombie back 20 units
            if (Vector3.Distance(fpsc.transform.position, AttackRaycastArea.transform.position) < 3.4f && !(DamagedCooldown > 0f) && Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f)
            {
                transform.Translate(transform.forward * -1.5f, Space.World);
            }
    }

    public void onHit(float damage) {
        if(isDead){
            return;
        }

        if (!isAware)
        {
            lastSeenPlayerPosition = fpsc.transform.position;
            OnAware();
        }

        fpsc.damageDealt += Mathf.Min(damage, health);
        health -= damage;
        if(health <= 0) {
            e_AudioSource.clip = zombieDie;
            e_AudioSource.Play();

            agent.speed = 0f;
            isDead = true;

            var allColliders = GetComponentsInChildren<Collider>();
            foreach(var childCollider in allColliders) Destroy(childCollider);

            fpsc.addScore(50);
            fpsc.kills +=1;
            fpsc.currency += 10;
            DropBox();
            StartCoroutine(RemoveGameObject());
        }
        else
        {
            e_AudioSource.clip = zombieDamage;
            e_AudioSource.Play();
            isDamage = true;
        }
    }

    private void DropBox()
    {
        int randomVar = Random.Range(0, 2);
        Debug.Log(randomVar);
        if (randomVar == 0)
        {
            if (Random.Range(0, 10) < 2) // 20%?
            {
                Debug.Log("Zombie dropped health box");
                GameObject box = Instantiate(healthBox, transform.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
                HealthBox hb = box.GetComponent<HealthBox>();
                hb.cam = Camera.main;
                hb.fpsc = fpsc;
                box.SetActive(true);
                Destroy(box, 10f);
            }
        }
        else
        {
            if (Random.Range(0, 10) < 4) // 40%?
            {
                Debug.Log("Zombie dropped ammo box");
                GameObject box = Instantiate(ammoBox, transform.position + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);
                AmmoBox ab = box.GetComponent<AmmoBox>();
                ab.fpsc = fpsc;
                ab.cam = Camera.main;
                box.SetActive(true);
                Destroy(box, 10f);
            }
        }
        
    }
}
