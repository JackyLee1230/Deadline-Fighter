using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public GameObject Weapon;
    public bool CanAttack = true;
    public float AttackCooldown = 0.5f;
    public AudioClip AttackSound;
    public bool isAttacking = false;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (CanAttack)
            {
                MeleeAttack();
            }
        }
    }

    public void MeleeAttack()
    {
        CanAttack = false;
        isAttacking = true;
        Animator anim = Weapon.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        AudioSource ml_audioSource = GetComponent<AudioSource>();
        ml_audioSource.PlayOneShot(AttackSound);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(0.167f);
        isAttacking = false;
    }
}
