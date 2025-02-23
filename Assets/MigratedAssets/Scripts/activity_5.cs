using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;
using Random = UnityEngine.Random;
using System.Collections.Specialized;
using UnityEngine.Perception.Randomization.Scenarios;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif
public class activity_5 : MonoBehaviour
{
    public bool applySeed;
    public int seed;
    public bool addSyntheticHumans = false;
    private bool isFirstUpdate = true;
    public int maxNumOfRows;
    private int numOfRows;
    public float interval;
    public float distance;
    public int minNumOfAgents;
    public int maxNumOfAgents;
    private int numOfAgents;
    public int numOfOtherAgents;
    public GameObject[] models;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> controllers_others = new List<RuntimeAnimatorController>();
    public GameObject camera, active_env, env_collections;
    private UnityEngine.Perception.GroundTruth.LabelManagement.Labeling change_label;
    public int index;
    public List<GameObject> Children;
    Vector3 origin;

    private AnimControllerManager AnimationManager;

    List<GameObject> model_list = new List<GameObject>();

    private void Awake()
    {
        if (addSyntheticHumans)
        {
            GameObject SyntheticHumans = GameObject.Find("HumanPool__HumanGenerationRandomizer");
            if (!SyntheticHumans)
            {
                GameObject ScenarioGameObject = GameObject.Find("Simulation Scenario");
                ScenarioGameObject.GetComponent<FixedLengthScenario>().GetRandomizer(0).enabled = true;
            }
        }
    }

    void Init()
    {
        if (applySeed)
        {
            Random.seed = seed;
        }
        numOfAgents = Random.Range(minNumOfAgents, maxNumOfAgents + 1);
        maxNumOfRows = Mathf.Max(maxNumOfRows, 1);
        numOfRows = Random.Range(1, maxNumOfRows + 1);

        if (!camera)
        {
            foreach (Transform child in env_collections.transform)
            {
                Children.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
            index = Random.Range(0, Children.Count);

            active_env = Children[index];
            active_env.SetActive(true);
            camera = active_env.transform.GetChild(0).gameObject;
        }

        origin = new Vector3(transform.position.x, 0f, transform.position.z);
        int[] agents = new int[numOfAgents * numOfRows + numOfOtherAgents];
        Vector3[] positions = new Vector3[numOfAgents * numOfRows + numOfOtherAgents];

        float xpos = 0f;
        float zpos = 0f;
        float xpos0 = xpos;
        float xpos_lookat = 50 + Random.Range(-20f, 20f);
        float zpos_lookat = -50 + Random.Range(-20f, 20f);


        if (addSyntheticHumans)
        {
            GameObject SyntheticHumans = GameObject.Find("HumanPool__HumanGenerationRandomizer");
            models = new GameObject[transform.childCount + SyntheticHumans.transform.childCount];
            int c = 0;
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<Animator>().applyRootMotion = true;
                child.gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
                child.gameObject.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;

                models[c] = child.gameObject;
                c++;
            }
            foreach (Transform child in SyntheticHumans.transform)
            {
                child.gameObject.GetComponent<Animator>().applyRootMotion = true;
                child.gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
                child.gameObject.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
                //var syntheticHuman = Instantiate(child.gameObject, transform);
                models[c] = child.gameObject;
                c++;
            }
            GameObject ScenarioGameObject = GameObject.Find("Simulation Scenario");
            ScenarioGameObject.GetComponent<FixedLengthScenario>().GetRandomizer(0).enabled = false;


        }
        else
        {
            models = new GameObject[transform.childCount];
            int c = 0;
            foreach (Transform child in transform)
            {
                models[c] = child.gameObject;
                c++;
            }
        }

        for (int r = 0; r < numOfRows; r++)
        {
            xpos = xpos0;
            for (int i = 0; i < numOfAgents; i++)
            {
                int index = Random.Range(0, models.Length);
                agents[numOfAgents * r + i] = index;
                xpos = xpos + distance;
                positions[numOfAgents * r + i] = new Vector3(xpos + Random.Range(-0.3f, 0.3f), -0.9077931f, zpos - r * interval + Random.Range(-0.3f, 0.3f));
                // model_list.Add(models[agents[numOfAgents * r + i]]);
            }
        }


        for (int i = numOfAgents * numOfRows; i < numOfAgents * numOfRows + numOfOtherAgents; i++)
        {
            float num = Random.Range(1, 10);
            float num2 = Random.Range(1, 10);

            int index = Random.Range(0, models.Length);
            agents[i] = index;
            xpos = xpos + distance;
            positions[i] = new Vector3(num2, -0.9077931f, num);
            // model_list.Add(models[agents[i]]);

        }

        for (int r = 0; r < numOfRows; r++)
        {
            for (int i = 0; i < numOfAgents; i++)
            {
                GameObject character = Instantiate(models[agents[numOfAgents * r + i]], this.transform);
                character.SetActive(true);
                character.transform.position = positions[numOfAgents * r + i] + origin;
                character.transform.eulerAngles = new Vector3(0, 0, 0);
                character.GetComponent<Animator>().runtimeAnimatorController = defaultController;
                character.transform.LookAt(new Vector3(xpos_lookat + Random.Range(-10f, 10f), 0f, zpos_lookat + Random.Range(-10f, 10f)));
                model_list.Add(character);
                /*
                models[agents[numOfAgents * r + i]].SetActive(true);
                models[agents[numOfAgents * r + i]].transform.position = positions[numOfAgents * r + i];
                models[agents[numOfAgents * r + i]].transform.eulerAngles = new Vector3(0, 180, 0);
                models[agents[numOfAgents * r + i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
                models[agents[numOfAgents * r + i]].transform.LookAt(new Vector3(xpos_lookat + Random.Range(-10f, 10f), 0f, zpos_lookat + Random.Range(-10f, 10f)));
                */
            }
        }



        for (int i = numOfAgents * numOfRows; i < numOfAgents * numOfRows + numOfOtherAgents; i++)
        {
            GameObject character = Instantiate(models[agents[i]], this.transform);
            character.SetActive(true);
            character.transform.position = positions[i] + origin;
            character.transform.eulerAngles = new Vector3(0, 180, 0);
            character.GetComponent<Animator>().runtimeAnimatorController = defaultController;
            character.transform.LookAt(new Vector3(0, 0, 0));
            model_list.Add(character);
            /*
            models[agents[i]].SetActive(true);
            models[agents[i]].transform.position = positions[i];
            models[agents[i]].transform.eulerAngles = new Vector3(0, 180, 0);
            models[agents[i]].GetComponent<Animator>().runtimeAnimatorController = defaultController;
            models[agents[i]].transform.LookAt(new Vector3(0, 0, 0));
            */

        }

        Vector3 center = Vector3.zero;
        for (int i = 0; i < numOfAgents; i++)
        {
            center += positions[i];
        }
        center /= numOfAgents;
        center += origin;
        camera.GetComponent<CameraController>().center = center;
        //camera.GetComponent<CameraController>().center = Vector3.zero;

        camera.GetComponent<CameraController>().updateCameraViews();

        AnimationManager = GetComponent<AnimControllerManager>();
#if UNITY_EDITOR
        AnimationManager.controller1 = AnimatorController.CreateAnimatorControllerAtPath(AnimationManager.AnimatorControllerPath);
#endif
        AnimationManager.Characters = model_list;

        AnimationManager.CharacterAnimators = new List<Animator>(new Animator[model_list.Count]);
        AnimationManager.CharacterRunningAnimationSequences = new List<IEnumerator>(new IEnumerator[model_list.Count]);

#if UNITY_EDITOR
        AnimationManager.LoadClipsToController();
#endif
        AnimationManager.AttachControllerToAllCharacters();


        //controllers[Random.Range(0, controllers.Count)]
        int anim_id = Random.Range(0, controllers.Count);
        float start_timing = Random.Range(0f, 1f);
        MixamoBlendTree t = AnimationManager.GetGroupByName("Dancing").RandomBlendTree();
        string[] paramNames = t.GetClipParameterNames();

        for (int r = 0; r < numOfRows; r++)
        {
            for (int i = 0; i < numOfAgents; i++)
            {
                model_list[numOfAgents * r + i].GetComponent<Animator>().speed = Random.Range(0.4f, 0.8f);
                foreach (string p in paramNames)
                {
                    AnimationManager.SetParameterRnd(numOfAgents * r + i, p);
                }
                AnimationManager.PlayClip(numOfAgents * r + i, t.name, 0.0f, Mathf.Clamp(start_timing + Random.Range(-0.0025f, 0.0025f), 0f, 1f));// Random.Range(0f, 1f));

            }
        }


        for (int i = numOfAgents * numOfRows; i < numOfAgents * numOfRows + numOfOtherAgents; i++)
        {
            model_list[i].GetComponent<Animator>().runtimeAnimatorController = controllers_others[Random.Range(0, controllers_others.Count)];
            Animator anim = model_list[i].GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            change_label = model_list[i].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            change_label.labels[0] = model_list[i].GetComponent<Animator>().runtimeAnimatorController.name;
        }

        //GetComponent<RandomMaterialColor>().GenerateMaterialColor();
        /*
        string path = "Assets/group_label/activity_2.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        for (int i = 0; i < numOfAgents; i++)
        {
            writer.WriteLine(i.ToString() + ",");
            writer.Write("\n");

        }
        writer.Close();
        */

    }

    // Update is called once per frame
    void Update()
    {
        if (addSyntheticHumans)
        {
            GameObject SyntheticHumans = GameObject.Find("HumanPool__HumanGenerationRandomizer");
            if (!SyntheticHumans)
            { return; }
            DontDestroyOnLoad(SyntheticHumans);

        }

        if (isFirstUpdate)
        {
            Init();
            isFirstUpdate = false;
        }

        for (int i = 0; i < model_list.Count; i++)
        {
            change_label = model_list[i].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            if (model_list[i].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length == 0)
            {
                change_label.RefreshLabeling();
                continue;
            }
            string clipName = model_list[i].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (clipName.Contains("Walk") || clipName.Contains("walk"))
            {
                change_label.labels[0] = "Walking";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Text") || clipName.Contains("text"))
            {
                change_label.labels[0] = "Texting";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Talk") || clipName.Contains("talk") || clipName.Contains("Argu") || clipName.Contains("argu"))
            {
                change_label.labels[0] = "Talking";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Point") || clipName.Contains("point"))
            {
                change_label.labels[0] = "Pointing";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Jog") || clipName.Contains("jog") || clipName.Contains("Run") || clipName.Contains("run"))
            {
                change_label.labels[0] = "Running";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Danc") || clipName.Contains("danc"))
            {
                change_label.labels[0] = "Dancing";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Wave") || clipName.Contains("wave") || clipName.Contains("Waving") || clipName.Contains("waving"))
            {
                change_label.labels[0] = "Waving";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Bow") || clipName.Contains("bow"))
            {
                change_label.labels[0] = "Bowing";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Fight") || clipName.Contains("fight") || clipName.Contains("Punch") || clipName.Contains("punch"))
            {
                change_label.labels[0] = "Fighting";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Salut") || clipName.Contains("salut"))
            {
                change_label.labels[0] = "Saluting";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("hand") || clipName.Contains("Hand") || clipName.Contains("Shak") || clipName.Contains("shak"))
            {
                change_label.labels[0] = "Handshaking";
                change_label.RefreshLabeling();
            }
            else if (clipName.Contains("Idle") || clipName.Contains("idle") || clipName.Contains("Stand") || clipName.Contains("stand") || clipName.Contains("Breath") || clipName.Contains("breath"))
            {
                change_label.labels[0] = "Standing-idle";
                change_label.RefreshLabeling();
            }
        }

    }


}
