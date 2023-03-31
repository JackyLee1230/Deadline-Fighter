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
    private AudioSource ml_audioSource;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame

    private void Start() {
        PlayerShoot.isGunActive = false;
        PlayerShoot.shootInput = null;
        PlayerShoot.reloadInput = null;
        ml_audioSource = GetComponent<AudioSource>();
    }


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

    private void OnDisable(){
        isAttacking = false;
        CanAttack = true;
    }

    public void MeleeAttack()
    {
        CanAttack = false;
        isAttacking = true;
        ml_audioSource.PlayOneShot(AttackSound);
        Animator anim = Weapon.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown*2);
        isAttacking = false;
        CanAttack = true;
    }
}
