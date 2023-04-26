using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;


public class WeaponPurchase : MonoBehaviour
{
    public int weaponCost = 650;
    public FirstPersonController fpsc;
    public WeaponSwitching editor;
    public float radius;
    private float cooldown;
    [SerializeField] public GameObject textMesh;
    public LayerMask mask; 
    private bool isLookingAtWeapon = false;
    public bool hasBoughtWeapon = false;

    void Start()
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
        cooldown = 0f;
    }
    void Update()
    {
        if(!isLookingAtWeapon){
            textMesh.GetComponent<TextMeshProUGUI>().text = "";
        }

        OnTriggerEnter();
        if(!(cooldown > 0)){
            if (isLookingAtWeapon && Input.GetKeyDown(KeyCode.E))
            {
                cooldown = 1f;
                if (fpsc.currency >= weaponCost)
                {
                    // Player can afford the weapon, so deduct the cost from their currency and give them the weapon
                    fpsc.currency -= weaponCost;
                    editor.weapons[editor.selectedWeapon].gameObject.SetActive(false);
                    editor.weapons[3].gameObject.SetActive(true);
                    editor.selectedWeapon = 3;
                    (editor.weapons[editor.selectedWeapon] , editor.weapons[3]) = (editor.weapons[3],editor.weapons[3]);
                    editor.weapons[editor.selectedWeapon].gameObject.GetComponent<Gun>().gunData.reservedAmmo = editor.weapons[editor.selectedWeapon].gameObject.GetComponent<Gun>().gunData.maxAmmo;
                    editor.weapons[editor.selectedWeapon].gameObject.GetComponent<Gun>().gunData.currentAmmo = editor.weapons[editor.selectedWeapon].gameObject.GetComponent<Gun>().gunData.magSize;
                    hasBoughtWeapon = true;
                }
                else
                {
                    // Player can't afford the weapon, so display a message
                    textMesh.GetComponent<TextMeshProUGUI>().text = "Not enough funds";
                }
            }
        }

        if(cooldown > 0){
            cooldown -= Time.deltaTime;
        }
    }

    void OnTriggerEnter()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
        if(Physics.Raycast(ray, out var hit, Mathf.Infinity, mask))
        {
            var obj = hit.collider.gameObject;
                if(obj.name == "PurchaseProximity"){
                    if(Vector3.Distance(transform.position, fpsc.transform.position) < radius){
                        isLookingAtWeapon = true;
                        if(!(cooldown > 0))
                            textMesh.GetComponent<TextMeshProUGUI>().text = "Press <E> to purchase for $"+weaponCost;
                    }
                    else{
                        isLookingAtWeapon = false;
                        if(!(cooldown > 0))
                            textMesh.GetComponent<TextMeshProUGUI>().text = "";
                    }
                }
                    
                else{
                    isLookingAtWeapon = false;
                    if(!(cooldown > 0))
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
