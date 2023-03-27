using UnityEngine;
using UnityEngine.AI;

public class ZombieHandler : MonoBehaviour
{
    public float health;
    public GameObject deadEffect;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    Transform target;

    float attackCoolDownTimer;
    // Start is called before the first frame update 


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent.SetDestination(target.position);
     

    }
    // Update is called once per frame 


    void Update()
    {
        float _distance = Vector3.Distance(transform.position, target.position);
        if (_distance > 2)
        {
            Vector3.Distance(transform.position, target.position);
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            navMeshAgent.isStopped = true;
            //Attack
            if (attackCoolDownTimer <= 0)
            {
                target.GetComponent<PlayerHealth>().CallTakeDamage(10);
                attackCoolDownTimer = 2;
                animator.SetTrigger("Attack");
            }

            Vector3 _dir = (target.position - transform.position).normalized;
            Quaternion _lookAngle = Quaternion.LookRotation(new Vector3(_dir.x, 0, _dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookAngle, Time.deltaTime * 10);

          
            if (attackCoolDownTimer > 0)
                attackCoolDownTimer -= Time.deltaTime;



        }
    }
    
    public void TakeDamage()
    {
        health -= 10;
        if (health <= 0)
        {
            GameObject _effect = Instantiate(deadEffect, transform.position + new Vector3(0,2,0) , Quaternion.identity);
            Destroy(_effect, 3f);

            MenuScript _menu = FindObjectOfType<MenuScript>();
            if (_menu)
                _menu.score += 10;

            Destroy(gameObject);

      

            
 
}

    }
}


