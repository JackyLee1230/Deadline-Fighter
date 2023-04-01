using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform[] weapons;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    private int selectedWeapon;
    private float timeSinceLastSwitch;

    private void Start() {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    private void SetWeapons() {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            weapons[i] = transform.GetChild(i);

        if (keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Update() {
        int previousSelectedWeapon = selectedWeapon;

        /*
         * Select with keys
        */
        for (int i = 0; i < keys.Length; i++)
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
                selectedWeapon = i;

        /*
         *  Select with scroll wheel
        */ 
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && timeSinceLastSwitch >= switchTime){
            selectedWeapon++;
			if(selectedWeapon > 2){
				selectedWeapon = 0;
			}
		}
		if(Input.GetAxis("Mouse ScrollWheel") < 0 && timeSinceLastSwitch >= switchTime){
            selectedWeapon--;
			if(selectedWeapon < 0){
				selectedWeapon = 2;
			}
		}

        if (previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex) {
        for (int i = 0; i < weapons.Length; i++){
            weapons[i].gameObject.SetActive(i == weaponIndex);
            if( i == weaponIndex){
                isGun();
            }
        }

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    public bool isGun()
    { 
        if (weapons[selectedWeapon].gameObject.GetComponent<Gun>() != null){
            Debug.Log("Gun Selected");
            return true;
        }
        else{
            Debug.Log("Melee Selected");
            return false;
        }
    }

    private void OnWeaponSelected() {  }
}