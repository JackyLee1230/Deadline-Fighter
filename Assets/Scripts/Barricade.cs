using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class Barricade : MonoBehaviour
{
    public int barricadeCost = 200;
    public FirstPersonController fpsc;
    public LayerMask mask; 
    public float radius;
    private float cooldown;
    [SerializeField] public GameObject textMesh;
    private bool isLookingAtBarricade = false;

void Start()
    {
        textMesh.GetComponent<TextMeshProUGUI>().text = "";
        cooldown = 0f;
    }
    void Update()
    {

        OnTriggerEnter();
        if(!(cooldown > 0)){
            if (isLookingAtBarricade && Input.GetKeyDown(KeyCode.E))
            {
                if(fpsc.currency <= barricadeCost){
                textMesh.GetComponent<TextMeshProUGUI>().text = "Not enough funds";
            }
                cooldown = 1f;
                if (fpsc.currency >= barricadeCost)
                {
                    // Player can afford the Barricade, so deduct the cost from their currency and give them the Barricade
                    fpsc.currency -= barricadeCost;
                    Destroy(gameObject);
                }
                else
                {
                    // Player can't afford the Barricade, so display a message
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
                if(obj.name == "B_PurchaseProximity" || obj.name == "Barricades"){
                    if(Vector3.Distance(transform.position, fpsc.transform.position) < radius){
                        isLookingAtBarricade = true;
                        if(!(cooldown > 0))
                            textMesh.GetComponent<TextMeshProUGUI>().text = "Press <E> to purchase for $"+barricadeCost;
                    }
                    else{
                        isLookingAtBarricade = false;
                        if(!(cooldown > 0))
                            textMesh.GetComponent<TextMeshProUGUI>().text = "";
                    }
                }
                    
                else{
                    isLookingAtBarricade = false;
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
            isLookingAtBarricade = false;
        }
    }

}
