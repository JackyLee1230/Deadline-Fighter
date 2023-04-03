using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using TMPro;

public class WeaponSwitching : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform[] weapons;
    [SerializeField] public GameObject Ammo;
    [SerializeField] public GameObject WeaponIcon;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    public int selectedWeapon;

    public FirstPersonController fpsc;
    private float timeSinceLastSwitch;

    private void Start() {
        SetWeapons();
        Select(selectedWeapon);
        Ammo.GetComponent<TextMeshProUGUI>().text = "0/0";
        timeSinceLastSwitch = 0f;
    }

    private void SetWeapons() {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            weapons[i] = transform.GetChild(i);

        if (keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Update() {
        if(!fpsc.m_Aiming){
            int previousSelectedWeapon = selectedWeapon;

            /*
            * Select with keys
            */
            for (int i = 0; i < keys.Length; i++)
                if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
                    selectedWeapon = i;
                    int reserved = weapons[selectedWeapon].gameObject.GetComponent<Gun>().gunData.reservedAmmo;
                    int current = weapons[selectedWeapon].gameObject.GetComponent<Gun>().gunData.currentAmmo;
                    Sprite icon = weapons[selectedWeapon].gameObject.GetComponent<Gun>().gunData.artworkImage;
                    Ammo.GetComponent<TextMeshProUGUI>().text = current + " / " + reserved;
                    WeaponIcon.GetComponent<Image>().sprite = icon;
                    if (current == 0){
                        Ammo.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    }
                    else{
                        Ammo.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }

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