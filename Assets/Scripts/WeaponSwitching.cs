using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponSwitching : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform[] weapons;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;
    [SerializeField] public GameObject Ammo;

    public int selectedWeapon;
    private float timeSinceLastSwitch;
    public GameObject currentWeapon;

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
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
                selectedWeapon = i;
                int reserved = weapons[selectedWeapon].gameObject.GetComponent<Gun>().gunData.reservedAmmo;
                int current = weapons[selectedWeapon].gameObject.GetComponent<Gun>().gunData.currentAmmo;
                Ammo.GetComponent<TextMeshProUGUI>().text = current + " / " + reserved;
                if (current == 0){
                    Ammo.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                }
                else{
                    Ammo.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }

        if (previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex) {
        for (int i = 0; i < weapons.Length; i++){
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }
        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected() {  }
}