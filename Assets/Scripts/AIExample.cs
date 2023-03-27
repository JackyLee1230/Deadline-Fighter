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
    [SerializeField] private bool isAware = false;
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
    [SerializeField] public float DamagedCooldown;
    [SerializeField] public LayerMask playerLayer;

    private int isPlayerStealth;
    
    // audio
    [SerializeField] public AudioClip zombieIdle;
    [SerializeField] public AudioClip zombieChase;
    [SerializeField] public AudioClip zombieDamage;
    [SerializeField] public AudioClip zombieAttack;
    [SerializeField] public AudioClip zombieDie;
    [SerializeField] public AudioClip zombieNotice;

    private AudioSource e_AudioSource;


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
        if (isDead && AttackCooldown > 0f)
        {
            animator.SetBool("Dead1", true);
        }
        else if (isDead)
        {
            animator.SetBool("Dead2", true);
        }
        else if (isDamage)
        {
            if (AttackCooldown > 0.7f && !(DamagedCooldown > 0f))
            {
                animator.SetTrigger("GreatDamage");
                AttackCooldown = 0.977f;
                DamagedCooldown = 0.977f;
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
        else if (isAware)
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
        float modifiedViewDistance = isPlayerStealth == 2 ? viewDistance*2.5f : isPlayerStealth == 1 ? viewDistance/1.5f : viewDistance;
        float modifiedDetectDistance = isPlayerStealth == 2 ? detectDistance*3.5f : isPlayerStealth == 1 ? detectDistance/3f : detectDistance;

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
        else if ((Vector3.Distance(fpsc.transform.position, transform.position) < modifiedDetectDistance)) {
            OnAware();
        }
    }

    public void OnAware()
    {
        e_AudioSource.PlayOneShot(zombieNotice);
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
            if (Physics.Raycast(AttackRaycastArea.transform.position, AttackRaycastArea.transform.forward, out hitInfo, attackRadius)){
                isAttacking = true;
                if (hitInfo.transform.CompareTag("Player")){
                    StartCoroutine(DelayAttackPlayer(hitInfo));
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

    IEnumerator DelayAttackPlayer(RaycastHit hitInfo)
    {
        yield return new WaitForSeconds(0.75f);

        if(!(DamagedCooldown > 0f) && !isDead)
            hitInfo.transform.GetComponent<FirstPersonController>().takeDamage(10, AttackRaycastArea.transform.position, Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f);
            // push the zombie back 20 units
            if (Vector3.Distance(fpsc.transform.position, AttackRaycastArea.transform.position) < 3.4f && !(DamagedCooldown > 0f) && Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(fpsc.transform.position)) < fov / 2f)
            {
                transform.Translate(transform.forward * -1.5f, Space.World);
            }
    }

    public void onHit(float damage) {
        if (!isAware)
        {
            lastSeenPlayerPosition = fpsc.transform.position;
            OnAware();
        }

        health -= damage;
        if(health <= 0) {
            e_AudioSource.clip = zombieDie;
            e_AudioSource.Play();

            agent.speed = 0f;
            isDead = true;

            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<SphereCollider>());
            fpsc.addScore(50);
            Debug.Log(fpsc.score);
            StartCoroutine(RemoveGameObject());
        }
        else
        {
            e_AudioSource.clip = zombieDamage;
            e_AudioSource.Play();
            isDamage = true;
        }
    }
}
