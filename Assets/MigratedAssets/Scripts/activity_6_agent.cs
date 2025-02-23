using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activity_6_agent : MonoBehaviour
{
    public Animator m_Animator;
    public List<GameObject> model_list = new List<GameObject>();
    public List<GameObject> other_models = new List<GameObject>();
    public bool isNotOther = false;
    int count = 0;
    void Start()
    {
        m_Animator = transform.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        /*var ray = new Ray(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), new Vector3(this.transform.forward.x, this.transform.position.y + 0.5f, this.transform.forward.z));
        RaycastHit hit;
        var dir1 = new Vector3(Mathf.Sin(20), 0, Mathf.Cos(20));
        var dir2 = new Vector3(Mathf.Sin(-20), 0, Mathf.Cos(-20));
        if (Physics.Raycast(ray, out hit, 5))
        {
            print("There is something in front of the object!");
            m_Animator.SetTrigger("stop");
            count = 0;
        }
        else if (count > 30)
        {
            m_Animator.SetTrigger("start");
        }
        count++;
        */

        transform.position = new Vector3(transform.position.x, -0.9277931f, transform.position.z);
        Vector3 to = this.transform.forward;
        foreach (GameObject model in other_models)
        {
            if (distance(model.transform.position, this.transform.position) != 0f && distance(model.transform.position, this.transform.position) <= 3f)
            {

                Vector3 from = model.transform.position - this.transform.position;
                if (Vector3.Angle(from, to) < 40)
                {

                    /*this.transform.LookAt(new Vector3(-transform.forward.x, 0, -transform.forward.z));*/
                    m_Animator.SetTrigger("stop");
                    count = 0;
                }
                else if (count > 10)
                {
                    m_Animator.SetTrigger("start");
                }

            }
            else if (count > 10)
            {
                m_Animator.SetTrigger("start");
            }

        }

        foreach (GameObject model in model_list)
        {
            if (distance(model.transform.position, this.transform.position) != 0f && distance(model.transform.position, this.transform.position) <= 2f)
            {
                Vector3 from = model.transform.position - this.transform.position;
                if (Vector3.Angle(from, to) < 40)
                {
                    /*this.transform.LookAt(new Vector3(-transform.forward.x, 0, -transform.forward.z));*/
                    m_Animator.SetTrigger("stop");
                    count = 0;
                }
                else if (count > 10)
                {
                    m_Animator.SetTrigger("start");
                }

            }
            else if (count > 10)
            {
                m_Animator.SetTrigger("start");
            }

        }
        count++;
        /*if (isCollide)
        {

            m_Animator.SetTrigger("stop");
            count = 0;
        } else if(count > 50)
        {
            m_Animator.SetTrigger("start");
        }
        
        isCollide = false;*/
    }
    public float distance(Vector3 a, Vector3 b)
    {
        float x = a.x - b.x;
        float z = a.z - b.z;
        return Mathf.Sqrt(x * x + z * z);
    }
}

