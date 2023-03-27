using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunData gunData;
    
    float timeSinceLastShot;
    public GameObject bulletHole;
    public GameObject damageEffect;
    public GameObject damageHeadEffect;
    public AudioSource m_AudioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyFire;

    private void Start() {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.clip = shootSound;
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
        gunData.currentAmmo = gunData.magSize;
    }

    private void OnDisable() => gunData.reloading = false;

    public void StartReload() {
        if (!gunData.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        gunData.reloading = true;

        m_AudioSource.clip = reloadSound;
        m_AudioSource.Play();
        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void Shoot() {
        if (gunData.currentAmmo > 0) {
            Debug.Log(gunData.currentAmmo);
            m_AudioSource.clip = shootSound;
            if (CanShoot()) {
                m_AudioSource.Play();
                RaycastHit  hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.transform.name == "Zombie(Clone)" || hit.transform.name == "Zombie"){
                        if(hit.collider.GetType() == typeof(SphereCollider)){
                            Instantiate (damageHeadEffect, hit.point, Quaternion.identity);
                            hit.transform.GetComponent<AIExample>().onHit(gunData.damage*5);
                        } else {
                            Instantiate (damageEffect, hit.point, Quaternion.identity);
                            hit.transform.GetComponent<AIExample>().onHit(gunData.damage);
                        }
                    }
                    else{
                        Instantiate(bulletHole, hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                        bulletHole.transform.up = hit.normal;
                    }
                }

                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
        else{
                m_AudioSource.clip = emptyFire;
                m_AudioSource.Play();
        }
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
    }

    private void OnGunShot() {  }
}