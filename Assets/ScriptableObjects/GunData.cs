using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]
public class GunData : ScriptableObject {

    [Header("Info")]
    public new string name;
    public Sprite artworkImage;

    [Header("Shooting")]
    public float maxDamage;
    public float minDamage;
    
    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public int reservedAmmo;
    public int maxAmmo;
    [Tooltip("In RPM")] public float fireRate;
    public float reloadTime;
    [HideInInspector] public bool reloading;

    [Header("Fire Mode")]
    public bool isAuto;

}