using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class Hazard : MonoBehaviour
{
    [SerializeField] public float oneSecInvincibility = 1.1f;

    // Update is called once per frame
    void Update()
    {
        if (oneSecInvincibility <= 1.0f)
        {
            oneSecInvincibility += Time.deltaTime;
        }
    }

    // ontrigger
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && oneSecInvincibility > 1.0f)
        {
            other.gameObject.GetComponent<FirstPersonController>().currentHealth -= 5;
            oneSecInvincibility = 0.0f;
        }
    }

}
