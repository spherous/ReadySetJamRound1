using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashyBox : MonoBehaviour
{
    public Vector3 smallSize;
    public Vector3 bigSize;
    bool shrinking = true;
    public float blinkSpeed;
    private float ellapsedBlinkTime;
    public bool flashing = false;

    private void Update()
    {
        if(flashing)
        {
            Vector3 startSize = shrinking ? bigSize : smallSize;
            Vector3 endSize = shrinking ? smallSize : bigSize;
            ellapsedBlinkTime += Time.deltaTime;

            transform.localScale = Vector3.Lerp(startSize, endSize, Mathf.Clamp01(ellapsedBlinkTime/blinkSpeed));
            if(transform.localScale == endSize)
            {
                ellapsedBlinkTime = 0;
                shrinking = !shrinking;
            }
        }
    }
}