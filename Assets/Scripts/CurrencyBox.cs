using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Madhur.InfoPopup;

public class CurrencyBox : MonoBehaviour
{
    [Header("Currnecy Gain")]
    public FirstPersonController fpsc;
    private int currencyBoost = 50; // amount of health to give
    private float radius = 20.0f; // radius player can reach the box, leave some room since camera isnt in floor level
    public bool used = false;
    public bool seen = false;

    public Camera cam;

    [Header("Sound")]
    public AudioClip HealthBoxSound;
    private AudioSource audioSource;

    [Header("Currency Box Animator")]
    public Animator animator;

    public GameObject popup;

    void Start(){
        audioSource = fpsc.GetComponent<AudioSource>();
    }

    private bool IsVisible(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point)< 0)
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
        if (IsVisible(cam,gameObject))
        {
           seen=true;
        }
        else
        {
           seen = false;
        }

     if(Vector3.Distance(transform.position, fpsc.transform.position) < radius && seen)
    //  make it so it only works when player is facing the box

        {
            if (Input.GetKeyDown(KeyCode.E) && !used)
            {
                used = true;
                Debug.Log("Player got the Currency Box from " + Vector3.Distance(transform.position, fpsc.transform.position) +  " units away");
                animator.SetBool("Open", true);
                fpsc.currency += currencyBoost;
                fpsc.addScore(50);
                InfoPopupUtil.ShowInformation ( "+" + currencyBoost + "$" );
                audioSource.PlayOneShot(HealthBoxSound);
                Destroy(gameObject, 1.5f);
            }
        }
       
    }
}
