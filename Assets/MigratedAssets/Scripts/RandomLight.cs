using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLight : MonoBehaviour
{
    public float maxIntensityRange = 100.0f;
    public float minIntensityRange = 1.0f;
    public float maxLightRange = 30.0f;
    public float minLightRange = 1.0f;
    public float maxPos = 6.0f;
    // Start is called before the first frame update
    void Awake()
    {
        foreach(Transform child in transform){
            if(child.gameObject.GetComponent<Light>()){
                Light light = child.gameObject.GetComponent<Light>();
                light.intensity = Random.Range(minIntensityRange, maxIntensityRange);

                Color newColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
                //light.color = newColor;

                light.range = Random.Range(minLightRange, maxLightRange);

                light.transform.position = new Vector3(Random.Range(-maxPos, maxPos), 5f, Random.Range(-maxPos, maxPos));
                light.transform.LookAt(Vector3.zero);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
