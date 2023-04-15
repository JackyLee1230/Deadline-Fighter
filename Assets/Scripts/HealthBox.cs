using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Madhur.InfoPopup;

public class HealthBox : MonoBehaviour
{
    [Header("Health Boost")]
    public FirstPersonController fpsc;
    private float healthBoost = 25.0f; // amount of health to give
    private float radius = 1.38f; // radius player can reach the box, leave some room since camera isnt in floor level
    public bool used = false;
    public bool seen = false;

    public Camera cam;

    [Header("Sound")]
    public AudioClip HealthBoxSound;
    private AudioSource audioSource;

    [Header("Health Box Animator")]
    public Animator animator;
    //    public GameObject cross;

    public GameObject popup;

    void Start()
    {
        audioSource = fpsc.GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        //       cross = gameObject.transform.Find("Cross").Find("box_med").gameObject;
    }

    private bool IsVisible(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        var targetRender = gameObject.GetComponent<Renderer>();
        if (IsVisible(cam, gameObject))
        {
            seen = true;
        }
        else
        {
            seen = false;
        }
        if (Vector3.Distance(transform.position, fpsc.transform.position) < radius && seen)
        {
            //            cross.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && !used)
            {
                used = true;
                Debug.Log("Player got the Health Box from " + Vector3.Distance(transform.position, fpsc.transform.position) + " units away");
                animator.SetBool("Open", true);
                fpsc.currentHealth = Mathf.Min(fpsc.currentHealth + healthBoost, fpsc.maxHealth);
                fpsc.addScore(50);
                InfoPopupUtil.ShowInformation("+" + healthBoost + " Health");
                audioSource.PlayOneShot(HealthBoxSound);
                Destroy(gameObject, 1.5f);
            }
        }
        //        else{
        //            cross.SetActive(false);
        //        }
    }
}
