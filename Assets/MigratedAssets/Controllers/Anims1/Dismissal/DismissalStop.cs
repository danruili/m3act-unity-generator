using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DismissalStop : MonoBehaviour
{
    public Vector3 target;
    Animator m_Animator;
    void Start()
    {
        m_Animator = transform.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x - target.x;
        float z = transform.position.z - target.z;
        double distance = Math.Sqrt(x * x + z * z);
        if (distance < 3f)
        {
            m_Animator.SetTrigger("stop");
        }
        transform.LookAt(target);
    }
}
