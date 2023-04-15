using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Madhur.InfoPopup;

public class AmmoBox : MonoBehaviour
{
    [Header("Ammo Box")]
    public FirstPersonController fpsc;
    private int ammoGiven; // amount of ammo to give
    private float radius = 1.38f; // radius player can reach the box, leave some room since camera isnt in floor level
    public bool used = false;
    public bool seen = false;

    public Camera cam;
    // public BoxData boxData;

    [Header("Sound")]
    public AudioClip HealthBoxSound;
    private AudioSource audioSource;

    [Header("Health Box Animator")]
    public Animator animator;

    public GameObject popup;

    void Start()
    {
        audioSource = fpsc.GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ammoGiven = UnityEngine.Random.Range(5, 24);
        cam = Camera.main;
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
        cam = Camera.main;
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
            if (Input.GetKeyDown(KeyCode.E) && !used)
            {
                Debug.Log("Pressed E");
                used = true;
                Debug.Log("Player got the Ammo Box from " + Vector3.Distance(transform.position, fpsc.transform.position) + " units away");
                animator.SetBool("Open", true);
                fpsc.addScore(50);
                InfoPopupUtil.ShowInformation("+" + ammoGiven + " Ammo");
                audioSource.PlayOneShot(HealthBoxSound);

                for (int j = 0; j < fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).childCount; j++)
                {
                    fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(j).GetComponent<Gun>().gunData.reservedAmmo += ammoGiven;
                    //            fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(j).GetComponent<Gun>().gunData.reservedAmmo =
                    //            fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(j).GetComponent<Gun>().gunData.maxAmmo +
                    //            (fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(j).GetComponent<Gun>().gunData.magSize - fpsc.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(j).GetComponent<Gun>().gunData.currentAmmo);
                }

                Destroy(gameObject, 1.5f);
            }
        }
    }
}
