using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class DynamicCrosshair : MonoBehaviour
{
    private RectTransform crosshair;

    private float currentSize;
    private float targetSize;
    public float speed;

    private void Start()
    {
        crosshair = GetComponent<RectTransform>();
    }

    private void Update()
    {
        targetSize =  CalcSize(Gun.shotSpread);
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * speed);
        crosshair.sizeDelta = new Vector2(currentSize, currentSize);
    }

    private float CalcSize(float bulletSpread){
        return 50f + (bulletSpread - 1f) * 5f;
    }
}
