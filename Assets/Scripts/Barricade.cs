using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class Barricade : MonoBehaviour
{
    public int barricadeCost = 10;
    public FirstPersonController fpsc;
    public LayerMask mask; 
    [SerializeField] public GameObject textMesh;
    private bool isLookingAtBarricade = false;


private void Start()
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
    }
void Update()
    {
        Debug.Log(isLookingAtBarricade);
        if (isLookingAtBarricade && Input.GetKeyDown(KeyCode.E))
        {
            if (fpsc.currency >= barricadeCost)
            {
                fpsc.currency -= barricadeCost;
                Destroy(gameObject);
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
            Debug.Log(obj.name);
            if(obj.name == "B_PurchaseProximity" || obj.name == "Barricades"){
                isLookingAtBarricade = true;
                textMesh.GetComponent<TextMeshProUGUI>().text = "Press <E> to purchase for $"+barricadeCost;
            }
            else{
                Debug.Log("wtf?");
                isLookingAtBarricade = false;
                textMesh.GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        else
        {
            textMesh.GetComponent<TextMeshProUGUI>().text = "";
        }
    }
    void OnTriggerExit(Collider other)
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
        if (other.tag == "Player")
        {
            isLookingAtBarricade = false;
        }
    }

}
