using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        int NumGroups = transform.childCount;
        for (int i=0; i<NumGroups; i++)
        {
            transform.GetChild(i).GetComponent<GroupID>().groupID = (uint)(i+1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
