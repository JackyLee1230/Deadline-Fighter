using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public GameObject hurtEffect;
    public MenuScript menuHandler;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


public void CallTakeDamage(float damage)
    {
        StartCoroutine(TakeDamage(damage));
    }

IEnumerator TakeDamage(float damage)
    {
        health -= damage;
        hurtEffect.SetActive(true);
        if (health <= 0)
        {
            //menuHandler.GameOver();


         }
        yield return new WaitForSeconds(1);
        hurtEffect.SetActive(false);
    }
}