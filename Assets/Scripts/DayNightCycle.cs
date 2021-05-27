using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    public float cycleDuration;
    private float ellapsedDuration;

    private bool becomingDay = false;

    public Transform streetLightContainer;
    
    private List<StreetLight> streetLights = new List<StreetLight>();
    private bool streetLightsOn = false;

    public List<Material> treeMat = new List<Material>();

    public bool cycle = false;

    private void Start()
    {
        for(int i = 0; i < streetLightContainer.childCount - 1; i++)
        {
            StreetLight light = streetLightContainer.GetChild(i).GetComponent<StreetLight>();
            light.OffImmediate();
            streetLights.Add(light);
        }
        
        // foreach(StreetLight light in streetLights)
        //     light.OffImmediate();
        
        foreach(Material mat in treeMat)
            mat.SetColor("_Color", Color.white);
    }

    private void Update()
    {
        if(!cycle)
            return;

        ellapsedDuration += Time.deltaTime;
        float startVal = becomingDay ? 0 : 1;
        float endVal = becomingDay ? 1 : 0;

        float t = ellapsedDuration/cycleDuration;

        globalLight.intensity = Mathf.Clamp01(Mathf.Lerp(startVal, endVal, t));
        Color startColor = becomingDay ? Color.black : Color.white;
        Color endColor = becomingDay ? Color.white : Color.black;

        foreach(Material mat in treeMat)
            mat.SetColor("_Color", Color.Lerp(startColor, endColor, t));

        if(globalLight.intensity == 0 && !becomingDay)
        {
            ellapsedDuration = 0;
            becomingDay = true;
        }
        else if(globalLight.intensity == 1 && becomingDay)
        {
            ellapsedDuration = 0;
            becomingDay = false;
        }
        else if(globalLight.intensity >= .4f && becomingDay && streetLightsOn)
        {
            streetLightsOn = false;
            
            foreach(StreetLight light in streetLights)
                light.Off();
        }
        else if(globalLight.intensity <= .4f && !becomingDay && !streetLightsOn)
        {
            streetLightsOn = true;
            foreach(StreetLight light in streetLights)
                light.On();
        }
        
    }
}