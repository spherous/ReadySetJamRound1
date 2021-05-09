using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class StreetLight : MonoBehaviour
{
    public List<Light2D> lights = new List<Light2D>();
    public float fadeDuration;
    private float ellapsedDuration;
    private bool turningOn = false;
    private bool transitioning = false;

    private void Awake() {
        fadeDuration *= Random.Range(0.75f, 1.5f);
    }

    private void Update()
    {
        if(transitioning)
        {
            ellapsedDuration += Time.deltaTime;
            SetLightVal(Mathf.Clamp01(ellapsedDuration/fadeDuration));
        }
    }

    public void SetLightVal(float val)
    {
        float startVal = turningOn ? 0 : 1;
        float endVal = turningOn ? 1 : 0;
        foreach(Light2D light in lights)
            light.intensity = Mathf.Clamp01(Mathf.Lerp(startVal, endVal, val));

        if(val == endVal)
            transitioning = false;
    }

    public void On()
    {
        foreach(Light2D light in lights)
            light.intensity = 0;

        ellapsedDuration = 0;
        transitioning = true;
        turningOn = true;
    }
    public void Off()
    {
        foreach(Light2D light in lights)
            light.intensity = 1;
        
        ellapsedDuration = 0;
        transitioning = true;
        turningOn = false;
    }

    public void OffImmediate()
    {
        foreach(Light2D light in lights)
            light.intensity = 0;
    }
}
