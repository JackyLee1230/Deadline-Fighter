using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Barricade : MonoBehaviour
{
    public int barricadeCost = 10;
    public FirstPersonController fpsc;
    public LayerMask mask; 
    private bool isLookingAtBarricade = false;

void Update()
    {
        Debug.Log(isLookingAtBarricade);
        if (isLookingAtBarricade && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("123WTF");
            if (fpsc.currency >= barricadeCost)
            {
                fpsc.currency -= barricadeCost;
                Destroy(gameObject);
            }
            else
            {
                // Player can't afford the weapon, so display a message
                Debug.Log("You can't afford this barricade!");
            }
        }
    }

    void OnTriggerEnter()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
        if(Physics.Raycast(ray, out var hit, Mathf.Infinity, mask))
        {
            var obj = hit.collider.gameObject;
            Debug.Log(obj.name);
            if(obj.name == "B_PurchaseProximity" || obj.name == "Barricades"){
                isLookingAtBarricade = true;
            }
            else
                isLookingAtBarricade = false;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isLookingAtBarricade = false;
        }
    }

}
