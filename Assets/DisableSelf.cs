using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable(){
        StartCoroutine(DisableSelfDelay());
    }

    public IEnumerator DisableSelfDelay(){
        yield return new WaitForSeconds(0.8f);
        gameObject.active = false;
    }

}
