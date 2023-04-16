using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectShoot : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectToActivate;
    private Gun gun;

    void Start(){
        gun = GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButton(0) && gun.gunData.currentAmmo > 0)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            objectToActivate.SetActive(false);
        }
    }
}
