using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public float deactiveProb = 0.25f;
    // Start is called before the first frame update
    void Awake()
    {
        foreach(Transform child in transform){
            if(Random.Range(0.0f, 1.0f) <= deactiveProb){
                child.gameObject.SetActive(false);
            }else{
                child.gameObject.SetActive(true);
            }
        }
    }
}
