using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

public class ModelController : MonoBehaviour
{
    public int numOfAgents;
    public GameObject[] models;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    List<GameObject> test = new List<GameObject>();
    public GameObject camera;
    Vector3 origin;
    // Start is called before the first frame update

    int time = 0;
    Dictionary<int, GameObject> startTime = new Dictionary<int, GameObject>();
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

        //models = GameObject.FindGameObjectsWithTag("Model");
        /*if (models.Length > 2)
        {
            models[0].SetActive(false);
            models[1].SetActive(false);
        }*/
        int[] agents = new int[numOfAgents];
        
        HashSet<int> set = new HashSet<int>();
        Vector3[] positions = new Vector3[numOfAgents];
        int distance = 6 / (numOfAgents+1);
        int xpos = -3;
        for(int i = 0; i < numOfAgents; i++)
        {
            int index = Random.Range(0, models.Length);
            while(set.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            set.Add(agents[i]);
            xpos = xpos + distance;
            positions[i] = new Vector3(xpos, -0.9277931f, 0.02030561f);
            int tempTime = Random.Range(0, 100);
            while (startTime.ContainsKey(tempTime))
            {
                tempTime = Random.Range(0, 100);
            }
            startTime.Add(tempTime, models[agents[i]]);
            test.Add(models[agents[i]]);
        }  
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
            models[agents[i]].transform.eulerAngles = new Vector3(0, 180, 0);            
            models[agents[i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
           
        }
        //controllers[Random.Range(0, controllers.Count)]
        camera.GetComponent<CameraController>().center = positions[positions.Length / 2] + origin;
        camera.GetComponent<CameraController>().updateCameraViews();
    }

    // Update is called once per frame
    void Update()
    {
        if (time == 0) { 
            for (int i = 0; i < numOfAgents; i++)
            {
                test[i].GetComponent<Animator>().runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
                Animator anim = test[i].GetComponent<Animator>();
                AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
                anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            }
        }
        time++;

    }


}
