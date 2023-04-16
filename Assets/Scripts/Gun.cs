using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class Gun : MonoBehaviour
{

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

    public static float shotSpread;
    public static float shootingSpread;
    public float maxShootingSpread;
    public float eachShotSpread;

    public GameObject muzzleFlash;
    private GameObject holdFlash;
    public GameObject muzzleSpawnPoint;

    public TrailRenderer bulletTrail;
    public GameObject bulletSpawnPoint;

    public GameObject bulletCasing;
    public GameObject bulletShellSpawnPoint;

    public GameObject onHitX;

    public float headShotMultiplier;
    public float limbShotMultiplier;

    public bool isAutoReload;
    bool AutoReloading = false;

    [SerializeField] private float noClipRadius;
    [SerializeField] private float noClipDistance;

    [SerializeField] private AnimationCurve offsetCurve;

    [SerializeField] private LayerMask clippingLayerMask;

    private Vector3 _originalLocalPosition;

    private Animator gunAnimator;

    private void Start()
    {
        gunAnimator = GetComponentInChildren<Animator>();
        PlayerShoot.isGunActive = false;
        StartCoroutine(SwitchDelay());
        PlayerShoot.reloading = gunData.reloading;
        m_AudioSource = GetComponent<AudioSource>();
        PlayerShoot.shootInput = Shoot;
        PlayerShoot.reloadInput = StartReload;
        PlayerShoot.isAuto = gunData.isAuto;
        PlayerShoot.haveAmmo = gunData.currentAmmo > 0;
        gunData.currentAmmo = gunData.magSize;
        gunData.reservedAmmo = gunData.magSize*2;
        _originalLocalPosition = transform.localPosition;

        RecoilInit();
    }

    // private void OnDisable() => gunData.reloading = false;

    private void OnDisable()
    {
        if(onHitX != null){
            onHitX.SetActive(false);
        }
        fpsc.setReloadIcon(false);
        fpsc.m_Shooting = false;
        PlayerShoot.isGunActive = false;
        gunData.reloading = false;
        if (bulletSpawnPoint != null)
        {
            foreach (Transform child in bulletSpawnPoint.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        shootingSpread = 0;
        RecoilInit();
        if (m_AudioSource != null)
        {
            m_AudioSource.Stop();
        }
        PlayerShoot.isGunActive = false;
        StartCoroutine(SwitchDelay());
        PlayerShoot.shootInput = Shoot;
        PlayerShoot.reloadInput = StartReload;
        PlayerShoot.isAuto = gunData.isAuto;
    }

    public bool getIsReloading()
    {
        return gunData.reloading;
    }

    // private void OnEnable() {
    //     for (var child in transform){
    //         if child name contain bsp
    //     }
    // }

    private void RecoilInit()
    {
        fpsc.recoilX = gunData.recoilX;
        fpsc.recoilY = gunData.recoilY;
        fpsc.recoilZ = gunData.recoilZ;
        fpsc.ADSrecoilX = gunData.ADSrecoilX;
        fpsc.ADSrecoilY = gunData.ADSrecoilY;
        fpsc.ADSrecoilZ = gunData.ADSrecoilZ;
        fpsc.snappiness = gunData.snappiness;
        fpsc.returnSpeed = gunData.returnSpeed;
    }



    float calcDropOffDamage(float bulletDist, float minDamage, float maxDamage, float dropOffStart, float dropOffEnd)
    {
        if (bulletDist <= dropOffStart) return maxDamage;
        if (bulletDist >= dropOffEnd) return minDamage;

        float dropOffRange = dropOffEnd - dropOffStart;
        return Mathf.Lerp(maxDamage, minDamage, (bulletDist - dropOffStart) / dropOffRange);
    }

    public void StartReload()
    {
        if (!gunData.reloading && this.gameObject.activeSelf && gunData.currentAmmo < gunData.magSize)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        if (gunData.reservedAmmo <= 0)
        {
            m_AudioSource.PlayOneShot(emptyFire);
            yield break;
        }

        gunAnimator.SetTrigger("Reloading");

        gunData.reloading = true;
        fpsc.setReloadIcon(true);
        m_AudioSource.clip = reloadSound;
        m_AudioSource.Play();
        yield return new WaitForSeconds(gunData.reloadTime);

        // if there are still ammo left in the mag, add it to the currentAmmo
        gunData.reservedAmmo += gunData.currentAmmo;
        int min = Mathf.Min(gunData.reservedAmmo, gunData.magSize);
        gunData.reservedAmmo -= min;
        gunData.currentAmmo = min;

        gunData.reloading = false;
        fpsc.setReloadIcon(false);

        AutoReloading = false;
    }

    public bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void Shoot()
    {
        if (gameObject.activeSelf)
        {
            if (gunData.currentAmmo > 0)
            {
                // Debug.Log("In Mag Ammo:" + gunData.currentAmmo + " Remaining Ammo" + gunData.reservedAmmo);
                if (CanShoot())
                {
                    if(shootingSpread < maxShootingSpread){
                        shootingSpread += eachShotSpread;
                    }
                    else{
                        shootingSpread = maxShootingSpread;
                    }

                    fpsc.shotsFired++;
                    gunAnimator.SetTrigger("Shooting");

                    GameObject bulletShell = Instantiate(bulletCasing, bulletShellSpawnPoint.transform.position, bulletShellSpawnPoint.transform.rotation);

                    holdFlash = Instantiate(muzzleFlash, muzzleSpawnPoint.transform.position, muzzleSpawnPoint.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
                    holdFlash.transform.parent = muzzleSpawnPoint.transform;

                    Destroy(bulletShell, 0.3f);
                    Destroy(holdFlash, 0.15f);

                    m_AudioSource.PlayOneShot(shootSound);
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3((1 - 2 * UnityEngine.Random.value) * shotSpread, (1 - 2 * UnityEngine.Random.value) * shotSpread, 0));

                    fpsc.RecoilFire();
                    // add a layer mask to the raycast to only hit the zombies, ignore layer 3
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask))
                    {
                        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                        StartCoroutine(SpawnTrail(trail, hit.point));

                        if (hit.transform.CompareTag("Zombie"))
                        {
                            if(!onHitX.activeSelf){
                                StartCoroutine(DisableX());
                            }

                            fpsc.shotsHit++;
                            Instantiate(bulletImpactFreshEffect, hit.point, Quaternion.LookRotation(hit.normal));
                            float damage = calcDropOffDamage(hit.distance, gunData.minDamage, gunData.maxDamage, 30, 100);

                            // Debug.Log("Type: "+ hit.collider.GetType());

                            if (hit.collider.GetType() == typeof(SphereCollider))
                            {
                                Instantiate(damageHeadEffect, hit.point, Quaternion.identity);
                                hit.transform.root.GetComponent<AIExample>().onHit(damage * headShotMultiplier);
                                fpsc.headshots++;
                                Debug.Log("Hit for " + damage * headShotMultiplier + " damage; Distance" + hit.distance);
                            }
                            else if (hit.collider.GetType() == typeof(BoxCollider))
                            {
                                Instantiate(damageEffect, hit.point, Quaternion.identity);
                                hit.transform.root.GetComponent<AIExample>().onHit(damage);
                                Debug.Log("Hit for " + damage + " damage; Distance" + hit.distance);
                            }
                            else
                            {
                                Instantiate(damageEffect, hit.point, Quaternion.identity);
                                hit.transform.root.GetComponent<AIExample>().onHit(damage * limbShotMultiplier);
                                Debug.Log("Hit for " + damage * limbShotMultiplier + " damage; Distance" + hit.distance);
                            }
                        }
                        else if (hit.transform.tag == "Barrier")
                        {
                            Debug.Log("Barrier Hit");
                        }
                        else
                        {
                            int randomNumberForBulletHole = UnityEngine.Random.Range(0, 3);

                            Instantiate(bulletImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                            Instantiate(bulletHole[randomNumberForBulletHole], hit.point + hit.normal * 0.0001f, Quaternion.LookRotation(hit.normal));
                            bulletHole[randomNumberForBulletHole].transform.up = hit.normal;
                        }
                    }
                    else
                    {
                        Vector3 hitProjectPoint = fpsc.transform.position + transform.forward * 100f;

                        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.transform.position, Quaternion.identity);
                        StartCoroutine(SpawnTrail(trail, hitProjectPoint));
                    }

                    gunData.currentAmmo--;
                    timeSinceLastShot = 0;
                    OnGunShot();
                }
            }
            else
            {
                shootingSpread = 0f;
                if (CanShoot())
                {
                    m_AudioSource.PlayOneShot(emptyFire);
                    timeSinceLastShot = 0;
                }
                if (isAutoReload && gameObject.activeSelf)
                {
                    StartCoroutine(DelayReload());
                }
            }
        }
    }

    IEnumerator DelayReload()
    {
        if (!AutoReloading && gunData.reservedAmmo > 0)
        {
            AutoReloading = true;
            yield return new WaitForSeconds(0.5f);
            StartReload();
        }

    }

    IEnumerator SwitchDelay()
    {
        yield return new WaitForSeconds(0.3f);
        PlayerShoot.isGunActive = true;
    }


    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        float time = 0;
        Vector3 startPosition = bulletSpawnPoint.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hitPoint;
        trail.Clear();
        Destroy(trail.gameObject, trail.time);
    }

    IEnumerator DisableX()
    {
        if(!onHitX.activeSelf){
            onHitX.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            onHitX.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (fpsc.m_Jumping)
            {
                shotSpread = 50.0f + shootingSpread;
            }
            else if (fpsc.m_Aiming){
                shotSpread = 1.0f + shootingSpread/2f;
            }
            else if (fpsc.m_IsWalking == false)
            {
                shotSpread = 30.0f + shootingSpread;
            }else{
                shotSpread = 12.0f + shootingSpread;
            }

            gunAnimator = GetComponentInChildren<Animator>();

            PlayerShoot.haveAmmo = gunData.currentAmmo > 0;

            PlayerShoot.reloading = gunData.reloading;

            PlayerShoot.isAuto = gunData.isAuto;

            if (gunData.reservedAmmo > gunData.maxAmmo){
                gunData.reservedAmmo = gunData.maxAmmo;
            }

            timeSinceLastShot += Time.deltaTime;
            if (Physics.SphereCast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), noClipRadius, out var hit, noClipDistance, clippingLayerMask))
            {
                transform.localPosition = _originalLocalPosition - new Vector3(0.0f, 0.0f, offsetCurve.Evaluate(hit.distance / noClipDistance));
            }
            else
            {
                transform.localPosition = _originalLocalPosition;
            }
        }
        else
        {
            PlayerShoot.isGunActive = false;
        }
    }

    private void OnGunShot() { }
}