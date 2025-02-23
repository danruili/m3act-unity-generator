using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStop : MonoBehaviour
{
    Animator m_Animator;
    void Start()
    {
        m_Animator = transform.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        float z = transform.position.z;
        double distance = Math.Sqrt(x * x + z * z);
        if(distance < 3f)
        {
            m_Animator.SetTrigger("stop");
        }
        transform.LookAt(new Vector3(0, 0, 0));
    }
}
