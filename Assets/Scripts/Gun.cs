using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] public LayerMask layerMask = 1 << 3;
    
    float timeSinceLastShot;
    public GameObject bulletHole;
    public GameObject damageEffect;
    public GameObject damageHeadEffect;
    public AudioSource m_AudioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyFire;
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

    public void StartReload() {
        if (!gunData.reloading && this.gameObject.activeSelf && gunData.currentAmmo < gunData.magSize)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        if (gunData.reservedAmmo <= 0)
            yield break;

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
                m_AudioSource.PlayOneShot(shootSound);
                RaycastHit  hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
                // add a layer mask to the raycast to only hit the zombies, ignore layer 3
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask)) {
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