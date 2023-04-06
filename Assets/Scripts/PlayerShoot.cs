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
    public MouseLook ml;
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


    
	[HideInInspector]	public float recoilAmount_z = 0.5f;
	[HideInInspector]	public float recoilAmount_x = 0.5f;
	[HideInInspector]	public float recoilAmount_y = 0.5f;
	[Header("Recoil Not Aiming")]
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_z_non = 0.5f;
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_x_non = 0.5f;
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_y_non = 0.5f;
	[Header("Recoil Aiming")]
	[Tooltip("Recoil amount on that AXIS while aiming")]
	public float recoilAmount_z_ = 0.5f;
	[Tooltip("Recoil amount on that AXIS while aiming")]
	public float recoilAmount_x_ = 0.5f;
	[Tooltip("Recoil amount on that AXIS while aiming")]
	public float recoilAmount_y_ = 0.5f;
	[HideInInspector]public float velocity_z_recoil,velocity_x_recoil,velocity_y_recoil;
	[Header("")]
	[Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
	public float recoilOverTime_z = 0.5f;
	[Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
	public float recoilOverTime_x = 0.5f;
	[Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
	public float recoilOverTime_y = 0.5f;

	[Header("Gun Precision")]
	[Tooltip("Gun rate precision when player is not aiming. THis is calculated with recoil.")]
	public float gunPrecision_notAiming = 200.0f;
	[Tooltip("Gun rate precision when player is aiming. THis is calculated with recoil.")]
	public float gunPrecision_aiming = 100.0f;
	[Tooltip("FOV of first camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
	[HideInInspector]
	public float gunPrecision;


    private Vector3 gunPosVelocity;
	private float cameraZoomVelocity;
	private float secondCameraZoomVelocity;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    public void Start(){
        fpsc.m_Aiming = false;
        gunAnimator = GetComponentInChildren<Animator>();
    }

    public void FixedUpdate(){
        gunAnimator = GetComponentInChildren<Animator>();
        if(isGunActive && shootInput != null){
            // ADS
            if(Input.GetMouseButton(1) && !reloading){
                fpsc.m_Aiming = true;
                gunAnimator.SetBool("isAiming", true);

                gunPrecision = gunPrecision_aiming;
                recoilAmount_x = recoilAmount_x_;
                recoilAmount_y = recoilAmount_y_;
                recoilAmount_z = recoilAmount_z_;


                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
                fpsc.m_Camera.fieldOfView = Mathf.SmoothDamp(fpsc.m_Camera.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
                fpsc.m_WeaponCamera.fieldOfView = Mathf.SmoothDamp(fpsc.m_WeaponCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
            }
            // Not ADS
            else{
                fpsc.m_Aiming = false;
                gunAnimator.SetBool("isAiming", false);

                gunPrecision = gunPrecision_notAiming;
                recoilAmount_x = recoilAmount_x_non;
                recoilAmount_y = recoilAmount_y_non;
                recoilAmount_z = recoilAmount_z_non;

                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
                fpsc.m_Camera.fieldOfView = Mathf.SmoothDamp(fpsc.m_Camera.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
                fpsc.m_WeaponCamera.fieldOfView = Mathf.SmoothDamp(fpsc.m_WeaponCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
            }   
        }
    }

    private Vector3 velV;
	[HideInInspector]
	public Transform mainCamera;
	private Camera secondCamera;
    private float currentRecoilZPos;
	private float currentRecoilXPos;
	private float currentRecoilYPos;
    void PositionGun(){
		transform.position = Vector3.SmoothDamp(transform.position,
			mainCamera.transform.position  - 
			(mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) + 
			(mainCamera.transform.up * (currentGunPosition.y+ currentRecoilYPos)) + 
			(mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)),ref velV, 0);

		Camera.main.transform.position = new Vector3(currentRecoilXPos,currentRecoilYPos, 0);

		currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
		currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
		currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);
	}

    public void RecoilMath(){
		currentRecoilZPos -= recoilAmount_z;
		currentRecoilXPos -= (UnityEngine.Random.value - 0.5f) * recoilAmount_x;
		currentRecoilYPos -= (UnityEngine.Random.value - 0.5f) * recoilAmount_y;
		fpsc.xRecoil = Mathf.Abs(currentRecoilYPos * gunPrecision) * (isAuto == true ? 1f : 2.5f);
		fpsc.yRecoil = (currentRecoilXPos * gunPrecision) * (isAuto == true ? 1f: 2.5f);	 
	}

    private void Update()
    {
        if(isGunActive && shootInput != null){
           if(isAuto){
                if (Input.GetMouseButton(0)){
                    RecoilMath();
                    fpsc.m_Shooting = true;
                    shootInput?.Invoke();
                    StartCoroutine(fpsc.RemoveShootingStatus());
                }
            }
            else{
                if (Input.GetMouseButtonDown(0)){
                    RecoilMath();
                    fpsc.m_Shooting = true;
                    shootInput?.Invoke();
                    StartCoroutine(fpsc.RemoveShootingStatus());
                }
            }

            if (Input.GetKeyDown(reloadKey))
                reloadInput?.Invoke();
        }
    }
}