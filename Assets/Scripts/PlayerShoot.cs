using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerShoot : MonoBehaviour {

    public static Action shootInput;
    public static Action reloadInput;
    public static bool isAuto;
    public FirstPersonController fpsc;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    private void Update()
    {
        if(isAuto){
            if (Input.GetMouseButton(0)){
                fpsc.m_Shooting = true;
                shootInput?.Invoke();
                StartCoroutine(fpsc.RemoveShootingStatus());
            }
        }
        else{
            if (Input.GetMouseButtonDown(0)){
                fpsc.m_Shooting = true;
                shootInput?.Invoke();
                StartCoroutine(fpsc.RemoveShootingStatus());
            }
        }

        if (Input.GetKeyDown(reloadKey))
            reloadInput?.Invoke();
    }
}