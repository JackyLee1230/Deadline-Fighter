using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]
public class GunData : ScriptableObject {

    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    
    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public int reservedAmmo;
    public int maxAmmo;
    [Tooltip("In RPM")] public float fireRate;
    public float reloadTime;
    [HideInInspector] public bool reloading;
    
}