using System.Collections;
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
    public float health = 100;
    public float wanderSpeed = 4f;
    public float chaseSpeed = 7f;
    public float fov = 120f;
    public float viewDistance = 10f;
    public float wanderRadius = 7f;
    public float attackRadius = 0.5f;
    public Transform[] waypoints; //Array of waypoints is only used when waypoint wandering is selected

    [SerializeField] private bool isAware = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDamage = false;
    [SerializeField] private bool isDead = false;
    private Vector3 wanderPoint;
    private NavMeshAgent agent;
    private Renderer renderer;
    private int waypointIndex = 0;
    private Animator animator;

    [SerializeField] public float AttackCooldown;
    [SerializeField] public float DamagedCooldown;
    [SerializeField] public LayerMask playerLayer;
    

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        wanderPoint = RandomWanderPoint();
        AttackCooldown = 0f;
    }
    public void Update()
    {

        if (isDead)
        {
            animator.SetBool("Dead", true);
        }
        else if (isDamage)
        {
            if(AttackCooldown > 0f && DamagedCooldown > 0f)
            {
                animator.SetTrigger("GreatDamage");
                AttackCooldown = 1.667f;
                DamagedCooldown = 1.667f;
                agent.speed = 0f;
            }
            else
            {
                animator.SetTrigger("Damage");
                AttackCooldown = 0.8f;
            }
            

            isDamage = false;
        }
        else if (isAttacking)
        {
            animator.SetTrigger("Attack");
            agent.speed = wanderSpeed;

            isAttacking = false;
        }
        else if (isAware)
        {
            if (!isAttacking) { 
                agent.SetDestination(fpsc.transform.position);
                animator.SetBool("Aware", true);
                if(!(AttackCooldown > 0f))
                    agent.speed = chaseSpeed;
                AttackPlayer();
                //renderer.material.color = Color.red;
            }

            if(Vector3.Distance(fpsc.transform.position, AttackRaycastArea.transform.position) > 17){
                if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f){
                    RaycastHit hit;
                    if (Physics.Linecast(transform.position, fpsc.transform.position, out hit, -1)){
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
            SearchForPlayer();
            Wander();
            animator.SetBool("Aware", false);
            agent.speed = wanderSpeed;
            //renderer.material.color = Color.blue;
        }


        // if attack cooldown is greater than 0, reduce it by 1 every second
        if (AttackCooldown > 0f && !(DamagedCooldown > 0f) && !isDead){
            agent.speed = wanderSpeed;
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
        if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f)
        {
            if (Vector3.Distance(fpsc.transform.position, transform.position) < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, fpsc.transform.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        OnAware();
                        Debug.Log("Tracked Player");
                        AttackPlayer();
                    }
                }
            }
        }
    }

    public void OnAware()
    {
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
                    if (waypointIndex == waypoints.Length - 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        waypointIndex++;
                    }
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
            if (Physics.Raycast(AttackRaycastArea.transform.position, AttackRaycastArea.transform.forward, out hitInfo, 2.4f)){
                isAttacking = true;
                if (hitInfo.transform.CompareTag("Player")){
                    StartCoroutine(AttackPlyaer(hitInfo));
                    Debug.Log("Zombie Hitting Player"); 
                    AttackCooldown = 1.4f;
                }
            }

            // scale the model to 2x
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
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

    IEnumerator AttackPlyaer(RaycastHit hitInfo)
    {
        yield return new WaitForSeconds(0.8f);

        if(!(DamagedCooldown > 0f) && !isDead)
            hitInfo.transform.GetComponent<FirstPersonController>().takeDamage(10, AttackRaycastArea.transform.position);
            // push the zombie back 20 units
            if (Vector3.Distance(fpsc.transform.position, AttackRaycastArea.transform.position) < 3.4f)
            {
                transform.Translate(transform.forward * -1.5f, Space.World);
            }
    }
    


    public void onHit(float damage) {
        health -= damage;
        if(health <= 0) {
            agent.speed = 0f;
            isDead = true;

            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
            fpsc.addScore(50);
            Debug.Log(fpsc.score);
            StartCoroutine(RemoveGameObject());
        }
        else
        {
            isDamage = true;
        }
    }
}
