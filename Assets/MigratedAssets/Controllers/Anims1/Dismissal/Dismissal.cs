using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

public class Dismissal : MonoBehaviour
{
    public int numOfAgents;
    public int radius;
    public GameObject[] models;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    public GameObject camera;
    //    public Script c;
    // Start is called before the first frame update
    int time = 0;
    Dictionary<int, GameObject> startTime = new Dictionary<int, GameObject>();
    Vector3 origin;

    void Start()
    {
        origin = new Vector3(transform.position.x, 0f, transform.position.z);
        models = new GameObject[transform.childCount];
        int c = 0;
        foreach (Transform child in transform)
        {
            models[c] = child.gameObject;
            c++;
        }

        int[] agents = new int[numOfAgents];

        HashSet<int> set = new HashSet<int>();
        Vector3[] positions = new Vector3[numOfAgents];
        Vector3[] targetPositions = new Vector3[numOfAgents];
        int distance = 6 / (numOfAgents + 1);
        int xpos = -3;
        for (int i = 0; i < numOfAgents; i++)
        {
            float theta = i * 2 * Mathf.PI / numOfAgents;
            float x = Mathf.Sin(theta) * radius;
            float z = Mathf.Cos(theta) * radius;
            float tx = Mathf.Sin(theta) * (radius + 10);
            float tz = Mathf.Cos(theta) * (radius + 10);
            int index = Random.Range(0, models.Length);
            while (set.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            set.Add(agents[i]);
            xpos = xpos + distance;
            positions[i] = new Vector3(x, 0, z);
            targetPositions[i] = new Vector3(tx, 0, tz);
            int tempTime = Random.Range(0, 100);
            while (startTime.ContainsKey(tempTime))
            {
                tempTime = Random.Range(0, 100);
            }
            startTime.Add(tempTime, models[agents[i]]);

        }
        camera.GetComponent<CameraController>().center = positions[positions.Length / 2] + origin;
        camera.GetComponent<CameraController>().updateCameraViews();

        for (int i = 0; i < controllers.Count; i++)
        {
            //if(!set.Contains(i))
            models[i].SetActive(false);
        }

        //print(models[1].IsActive);
        for (int i = 0; i < numOfAgents; i++)
        {
            models[agents[i]].SetActive(true);
            models[agents[i]].transform.position = positions[i] + origin;
            models[agents[i]].transform.LookAt(targetPositions[i]);
            //= new Vector3(0, 180, 0);
            models[agents[i]].AddComponent<DismissalStop>();
            models[agents[i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
            models[agents[i]].GetComponent<DismissalStop>().target = targetPositions[i];

        }
        //controllers[Random.Range(0, controllers.Count)]

    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if (startTime.ContainsKey(time))
        {
            startTime[time].GetComponent<Animator>().runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        }
    }
}
