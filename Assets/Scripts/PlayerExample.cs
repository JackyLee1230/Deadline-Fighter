﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerExample : MonoBehaviour {
    public AudioClip shootSound;
    public float soundIntensity = 5f;
    public float walkEnemyPerceptionRadius = 1f;
    public float sprintEnemyPerceptionRadius = 1.5f;
    public LayerMask zombieLayer;

    public GameObject pauseMenu;

    [SerializeField] private AudioSource audioSource;
    private FirstPersonController fpsc;
    private SphereCollider sphereCollider;
    private WeaponSwitching w;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpsc = GetComponent<FirstPersonController>();
        sphereCollider = GetComponent<SphereCollider>();
        w = GetComponent<WeaponSwitching>();
    }

    public void Update()
    {
        if(pauseMenu.activeSelf == true)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (w.isGun())
            {
                Fire();
            }
            // don't play shootSound with a melee weapon
        }
        if (fpsc.GetPlayerStealthProfile() == 0)
        {
            sphereCollider.radius = walkEnemyPerceptionRadius;
        } else
        {
            sphereCollider.radius = sprintEnemyPerceptionRadius;
        }
    }

    public void Fire()
    {
        audioSource.PlayOneShot(shootSound);
        Collider[] zombies = Physics.OverlapSphere(transform.position, soundIntensity, zombieLayer);
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<AIExample>().OnAware();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            other.GetComponent<AIExample>().OnAware();
        }
    }
}