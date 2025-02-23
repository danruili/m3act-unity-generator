using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

//using UnityEditor;
using System.IO;

public class activity_2_old : MonoBehaviour
{
    public int numOfAgents;
    public int numOfOtherAgents;
    private GameObject[] models;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> controllers_others = new List<RuntimeAnimatorController>();

    private UnityEngine.Perception.GroundTruth.LabelManagement.Labeling change_label;
    public float ambient_intensity = 2.0f;


    List<GameObject> model_list = new List<GameObject>();
    void Start()
    {
        RenderSettings.ambientIntensity = ambient_intensity;

        int[] agents = new int[numOfAgents + numOfOtherAgents];
        Vector3[] positions = new Vector3[numOfAgents + numOfOtherAgents];

        int distance = 1;
        int xpos = -3;
        float zpos = 5.02030561f;
        HashSet<int> existedModels = new HashSet<int>();
        //        while (model_list.Count == numOfAgents)

        models = new GameObject[transform.childCount];
        int c = 0;
        foreach (Transform child in transform)
        {
            models[c] = child.gameObject;
            c++;
        }


        for (int i = 0; i < numOfAgents; i++)
        {
            int index = Random.Range(0, models.Length);
            while(existedModels.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            existedModels.Add(index);
            agents[i] = index;
            xpos = xpos + distance;
            float noise_z = Random.Range(-1, 1);
            positions[i] = new Vector3(xpos, 0, zpos + noise_z);
            model_list.Add(models[agents[i]]);
        }
        //HashSet<int> existedModels2 = new HashSet<int>();
        for (int i= numOfAgents; i < numOfAgents + numOfOtherAgents; i++)
        {
            float num = Random.Range(1, 10);
            float num2 = Random.Range(1, 10);

            int index = Random.Range(0, models.Length);
            while (existedModels.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            existedModels.Add(index);
            agents[i] = index;
            xpos = xpos + distance;
            positions[i] = new Vector3(num2, 0, num);
            model_list.Add(models[agents[i]]);

        }


        //print(models[1].IsActive);
        for (int i = 0; i < numOfAgents; i++)
        {
            models[agents[i]].SetActive(true);
            models[agents[i]].transform.position = positions[i];
            models[agents[i]].transform.eulerAngles = new Vector3(0, 180, 0);
            models[agents[i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
            models[agents[i]].transform.LookAt(new Vector3(xpos - 50, 0, zpos));


        }

        for (int i = numOfAgents; i < numOfAgents+  numOfOtherAgents; i++)
        {
            models[agents[i]].SetActive(true);
            models[agents[i]].transform.position = positions[i];
            models[agents[i]].transform.eulerAngles = new Vector3(0, 180, 0);
            models[agents[i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
            models[agents[i]].transform.LookAt(new Vector3(0, 0, 0));

        }


        //controllers[Random.Range(0, controllers.Count)]

        for (int i = 0; i < numOfAgents; i++)
        {
            model_list[i].GetComponent<Animator>().runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
            Animator anim = model_list[i].GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            change_label = model_list[i].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            change_label.labels[0] = model_list[i].GetComponent<Animator>().runtimeAnimatorController.name;
        }


        for (int i = numOfAgents; i < numOfAgents+ numOfOtherAgents; i++)
        {
            model_list[i].GetComponent<Animator>().runtimeAnimatorController = controllers_others[Random.Range(0, controllers_others.Count)];
            Animator anim = model_list[i].GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            float walk_speed = Random.Range(1, 2);
            anim.speed = walk_speed;
            change_label = model_list[i].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            change_label.labels[0] = model_list[i].GetComponent<Animator>().runtimeAnimatorController.name;
        }


        string path = "Assets/group_label/activity_2.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        for (int i = 0; i < numOfAgents; i++)
        {
            writer.WriteLine(i.ToString() + ",");
            writer.Write("\n");

        }
        writer.Close();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
