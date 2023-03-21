using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using Madhur.InfoPopup;

public class HealthBox : MonoBehaviour
{
    [Header("Health Boost")]
    public FirstPersonController fpsc;
    private float healthBoost = 50.0f; // amount of health to give
    private float radius = 20.0f; // radius player can reach the box, leave some room since camera isnt in floor level

    [Header("Sound")]
    public AudioClip HealthBoxSound;
    private AudioSource audioSource;

    [Header("Health Box Animator")]
    public Animator animator;
    public GameObject cross;

    public GameObject popup;

    void Start(){
        audioSource = fpsc.GetComponent<AudioSource>();
//        cross = gameObject.transform.Find("Cross").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    //    Debug.Log(Vector3.Distance(transform.position, fpsc.transform.position));
     if(Vector3.Distance(transform.position, fpsc.transform.position) < radius)
        {
            cross.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Pressed E");
                animator.SetBool("Open", true);
                fpsc.currentHealth += healthBoost;
                InfoPopupUtil.ShowInformation ( "+" + healthBoost + " Health" );
                audioSource.PlayOneShot(HealthBoxSound);
                Destroy(gameObject);
            }
        }
        else{
            cross.SetActive(false);
        }
    }
}
