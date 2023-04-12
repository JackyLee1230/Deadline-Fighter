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
    private Animator anim;

    private void Start() {
        PlayerShoot.isGunActive = false;
        PlayerShoot.shootInput = null;
        PlayerShoot.reloadInput = null;
        CollisionDetection.AttackCooldown = AttackCooldown;
        ml_audioSource = GetComponent<AudioSource>();
        anim = Weapon.GetComponent<Animator>();
    }

    private void OnEnable(){
        PlayerShoot.isGunActive = false;
        PlayerShoot.shootInput = null;
        PlayerShoot.reloadInput = null;
    }

    void Update()
    {
        PlayerShoot.isGunActive = false;
        PlayerShoot.shootInput = null;
        PlayerShoot.reloadInput = null;
        if(Input.GetMouseButtonDown(0) && CanAttack)
        {
            MeleeAttack();
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
