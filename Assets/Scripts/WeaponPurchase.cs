using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;


public class WeaponPurchase : MonoBehaviour
{
    public int weaponCost = 10;
    public FirstPersonController fpsc;
    public WeaponSwitching editor;
    [SerializeField] public GameObject textMesh;
    public LayerMask mask; 
    private bool isLookingAtWeapon = false;

    void Start()
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
    }
    void Update()
    {

        OnTriggerEnter();
        if (isLookingAtWeapon && Input.GetKeyDown(KeyCode.E))
        {
            if (fpsc.currency >= weaponCost)
            {
                // Player can afford the weapon, so deduct the cost from their currency and give them the weapon
                fpsc.currency -= weaponCost;
                editor.weapons[editor.selectedWeapon].gameObject.SetActive(false);
                editor.weapons[3].gameObject.SetActive(true);
                (editor.weapons[editor.selectedWeapon] , editor.weapons[3]) = (editor.weapons[3],editor.weapons[editor.selectedWeapon]);
            }
            else
            {
                // Player can't afford the weapon, so display a message
                textMesh.GetComponent<TextMeshProUGUI>().text = "Not enough funds";
            }
        }
    }

    void OnTriggerEnter()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
        if(Physics.Raycast(ray, out var hit, Mathf.Infinity, mask))
        {
            var obj = hit.collider.gameObject;
            if(obj.name == "PurchaseProximity"){
                isLookingAtWeapon = true;
                textMesh.GetComponent<TextMeshProUGUI>().text = "Press <E> to purchase for $"+weaponCost;
            }
                
            else{
                isLookingAtWeapon = false;
                textMesh.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
        if (other.tag == "Player")
        {
            isLookingAtWeapon = false;
        }
    }
}
