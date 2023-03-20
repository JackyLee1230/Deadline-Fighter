using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public GameObject playerCam;
    public float range = 100f;
    public float damage = 25f;
    public Animator playerAnimator;
    public ParticleSystem muzzleFlash;
    public GameObject hitParticles;

    public AudioClip gunShot;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (playerAnimator.GetBool("isShooting")) {
            playerAnimator.SetBool("isShooting", false);
        }

        if (Input.GetButtonDown("Fire1")) {
            Shoot();

        }
    }

    void Shoot() {
        muzzleFlash.Play();
        audioSource.PlayOneShot(gunShot);
        playerAnimator.SetBool("isShooting", true);

        RaycastHit hit;

        if(Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range)){
            //Debug.Log("hit");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if(enemyManager != null) {
                enemyManager.Hit(damage);
                GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                instParticles.transform.parent = hit.transform;
                Destroy(instParticles, 2f);
            }
        }

    }
}
