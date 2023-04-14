using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerShoot : MonoBehaviour {

    public static Action shootInput;
    public static Action reloadInput;
    public static bool isAuto;
    public static bool haveAmmo;
    public static bool reloading;
    public static bool isGunActive;
    public FirstPersonController fpsc;
    private Animator gunAnimator;


    [HideInInspector]
	public Vector3 currentGunPosition;
	[Header("Gun Positioning")]
	[Tooltip("Vector 3 position from player SETUP for NON AIMING values")]
	public Vector3 restPlacePosition;
	[Tooltip("Vector 3 position from player SETUP for AIMING values")]
	public Vector3 aimPlacePosition;
	[Tooltip("Time that takes for gun to get into aiming stance.")]
	public float gunAimTime = 0.1f;


    public float cameraZoomRatio_notAiming = 60;
	[Tooltip("FOV of first camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
	public float cameraZoomRatio_aiming = 40;
	[Tooltip("FOV of second camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
	public float secondCameraZoomRatio_notAiming = 60;
	[Tooltip("FOV of second camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
	public float secondCameraZoomRatio_aiming = 40;


    private Vector3 gunPosVelocity;
	private float cameraZoomVelocity;
	private float secondCameraZoomVelocity;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    public void Start(){
        fpsc.m_Aiming = false;
        if(isGunActive && shootInput != null){
            gunAnimator = GetComponentInChildren<Animator>();
        }
    }

    public void FixedUpdate(){
        if(isGunActive && shootInput != null){
            gunAnimator = GetComponentInChildren<Animator>();
            // ADS
            if(Input.GetMouseButton(1) && !reloading){
                fpsc.m_Aiming = true;
                gunAnimator.SetBool("isAiming", true);
                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
                fpsc.m_Camera.fieldOfView = Mathf.SmoothDamp(fpsc.m_Camera.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
                fpsc.m_WeaponCamera.fieldOfView = Mathf.SmoothDamp(fpsc.m_WeaponCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
            }
            // Not ADS
            else{
                fpsc.m_Aiming = false;
                gunAnimator.SetBool("isAiming", false);
                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
                fpsc.m_Camera.fieldOfView = Mathf.SmoothDamp(fpsc.m_Camera.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
                fpsc.m_WeaponCamera.fieldOfView = Mathf.SmoothDamp(fpsc.m_WeaponCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
            }   
        }
    }

    private void Update()
    {
        if(isGunActive && shootInput != null){
           if(isAuto){
                if (Input.GetMouseButton(0)){
                    fpsc.m_Shooting = true;
                    shootInput?.Invoke();
                }
                else{
                    if(fpsc.m_Shooting){
                        StartCoroutine(fpsc.RemoveShootingStatus());
                    }

                    if(Gun.shootingSpread > 0f){
                        Gun.shootingSpread -= Time.deltaTime*40;
                    } 
                }   
            }
            else{
                if (Input.GetMouseButtonDown(0)){
                    fpsc.m_Shooting = true;
                    shootInput?.Invoke();
                }
                else{
                    if(fpsc.m_Shooting){
                        StartCoroutine(fpsc.RemoveShootingStatus());
                    } 

                    if (Gun.shootingSpread > 0f){
                        Gun.shootingSpread -= Time.deltaTime*23;
                    }
                }
            }

            if (Input.GetKeyDown(reloadKey))
                reloadInput?.Invoke();
        }
    }


    public IEnumerator RemoveShootingSpread()
    {
        yield return new WaitForSeconds(0.1f);


        Gun.shootingSpread = 0f;
    }
}