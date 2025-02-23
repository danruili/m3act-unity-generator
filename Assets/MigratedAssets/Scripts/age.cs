using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class age : MonoBehaviour
{
    // Start is called before the first frame update

    NavMeshAgent nav;
    Vector3 test = new Vector3(0,0,0);

    void Start()
    {

        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(test);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
