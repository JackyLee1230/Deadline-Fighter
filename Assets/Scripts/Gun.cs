using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("References")]
    [SerializeField] public GunData gunData;
    [SerializeField] public LayerMask layerMask = 1 << 3;
    
    public FirstPersonController fpsc;

    float timeSinceLastShot;
    public GameObject[] bulletHole;
    public GameObject bulletImpactEffect;
    public GameObject bulletImpactFreshEffect;
    public GameObject damageEffect;
    public GameObject damageHeadEffect;
    public AudioSource m_AudioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyFire;

    public GameObject muzzleFlash;
    private GameObject holdFlash;
    public GameObject muzzleSpawnPoint;

    public TrailRenderer bulletTrail;
    public GameObject bulletSpawnPoint;

    public GameObject bulletCasing;
    public GameObject bulletShellSpawnPoint;

    public bool isAutoReload;
    bool AutoReloading = false;

    [SerializeField] private float noClipRadius;
	[SerializeField] private float noClipDistance;

    [SerializeField] private AnimationCurve offsetCurve;

	[SerializeField] private LayerMask clippingLayerMask;

    private Vector3 _originalLocalPosition;


    private void Start() {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.clip = shootSound;
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
        gunData.currentAmmo = gunData.magSize;
        gunData.reservedAmmo = gunData.magSize * 3;
        _originalLocalPosition = transform.localPosition;
    }

    private void OnDisable() => gunData.reloading = false;


    
    float calcDropOffDamage(float bulletDist, float minDamage, float maxDamage, float dropOffStart, float dropOffEnd) {
            if (bulletDist <= dropOffStart) return maxDamage;
            if (bulletDist >= dropOffEnd) return minDamage;

            float dropOffRange = dropOffEnd - dropOffStart;
            return Mathf.Lerp(maxDamage, minDamage, (bulletDist - dropOffStart) / dropOffRange);
        }

    public void StartReload() {
        if (!gunData.reloading && this.gameObject.activeSelf && gunData.currentAmmo < gunData.magSize)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        if (gunData.reservedAmmo <= 0){
            m_AudioSource.PlayOneShot(emptyFire);
            yield break;
        }

        gunData.reloading = true;

        m_AudioSource.clip = reloadSound;
        m_AudioSource.Play();
        yield return new WaitForSeconds(gunData.reloadTime);

        // if there are still ammo left in the mag, add it to the currentAmmo
        gunData.reservedAmmo += gunData.currentAmmo;
        int min = Mathf.Min(gunData.reservedAmmo, gunData.magSize);
        gunData.reservedAmmo -= min ;
        gunData.currentAmmo = min;

        gunData.reloading = false;

        AutoReloading = false;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void Shoot() {
        if (gunData.currentAmmo > 0) {
            Debug.Log("In Mag Ammo:" + gunData.currentAmmo + " Remaining Ammo" + gunData.reservedAmmo);
            if (CanShoot()) {
                GameObject bulletShell = Instantiate(bulletCasing, bulletShellSpawnPoint.transform.position, bulletShellSpawnPoint.transform.rotation);

                holdFlash = Instantiate(muzzleFlash, muzzleSpawnPoint.transform.position, muzzleSpawnPoint.transform.rotation * Quaternion.Euler(0,0,90) ) as GameObject;
                holdFlash.transform.parent = muzzleSpawnPoint.transform;

                Destroy(bulletShell, 0.3f);
                Destroy(holdFlash, 0.15f);

                m_AudioSource.PlayOneShot(shootSound);
                RaycastHit  hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
                // add a layer mask to the raycast to only hit the zombies, ignore layer 3
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask)) {    
                    TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                    trail.transform.parent = bulletSpawnPoint.transform;
                    StartCoroutine(SpawnTrail(trail, hit.point, true));
                    if (hit.transform.name == "Zombie(Clone)" || hit.transform.name == "Zombie"){
                        Instantiate (bulletImpactFreshEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        float damage = calcDropOffDamage(hit.distance, gunData.minDamage, gunData.maxDamage, 30, 100);
                        if(hit.collider.GetType() == typeof(SphereCollider)){
                            Instantiate (damageHeadEffect, hit.point, Quaternion.identity);
                            hit.transform.GetComponent<AIExample>().onHit(damage*5);
                            Debug.Log("Hit for " + damage*5 + " damage; Distance" + hit.distance );
                        } else {
                            Instantiate (damageEffect, hit.point, Quaternion.identity);
                            hit.transform.GetComponent<AIExample>().onHit(damage);
                            Debug.Log("Hit for " + damage + " damage; Distance" + hit.distance );
                        }
                    }
                    else{
                        int randomNumberForBulletHole = UnityEngine.Random.Range(0,3);

                        Instantiate(bulletImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Instantiate(bulletHole[randomNumberForBulletHole], hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                        bulletHole[randomNumberForBulletHole].transform.up = hit.normal;
                    }
                }
                else{
                    Vector3 hitProjectPoint = fpsc.transform.position + transform.forward *100f;

                    TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                    trail.transform.parent = bulletSpawnPoint.transform;
                    StartCoroutine(SpawnTrail(trail, hitProjectPoint, false));
                }

                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
        else{
            if(CanShoot()){
                m_AudioSource.PlayOneShot(emptyFire);
                timeSinceLastShot = 0;
            }
            if(isAutoReload){
                StartCoroutine(DelayReload());
            }
        }
    }

    IEnumerator DelayReload()
    {
        if(!AutoReloading && gunData.reservedAmmo > 0){
            AutoReloading = true;
            yield return new WaitForSeconds(0.5f);
            StartReload();
        }

    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, bool hasHit)
    {
        float time = 0;
        Vector3 startPosition = bulletSpawnPoint.transform.position;

        while(time < (hasHit? 1 : 3)){
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        Destroy(trail.gameObject, trail.time);
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
        if (Physics.SphereCast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), noClipRadius, out var hit, noClipDistance, clippingLayerMask))
		{
            Debug.Log(hit.distance);
			transform.localPosition = _originalLocalPosition - new Vector3(0.0f, 0.0f, offsetCurve.Evaluate(hit.distance / noClipDistance));
		}
		else
		{
			transform.localPosition = _originalLocalPosition;
		}
    }

    private void OnGunShot() {  }
}