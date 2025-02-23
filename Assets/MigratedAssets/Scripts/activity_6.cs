using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Perception.Randomization.Scenarios;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif
using System.IO;


using UnityEngine.AI;

public class activity_6 : MonoBehaviour
{
    public bool applySeed;
    public int seed;
    public bool addSyntheticHumans = false;
    private bool isFirstUpdate = true;
    public int minNumOfAgents;
    public int maxNumOfAgents;
    private int numOfAgents;
    public int numOfOtherAgents;
    public float distanceBetweenAgents;
    private GameObject[] models;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    public GameObject camera, active_env, env_collections;
    private UnityEngine.Perception.GroundTruth.LabelManagement.Labeling change_label;

    public int index;
    List<GameObject> model_list = new List<GameObject>();
    List<GameObject> other_models = new List<GameObject>();
    Vector3[] destinations;

    int[] agents;
    int[] otheragents;
    HashSet<int> existedIndex = new HashSet<int>();
    HashSet<float> existedPos = new HashSet<float>();
    Vector3 origin;
    private AnimControllerManager AnimationManager;
    private List<float> AgentSpeeds = new List<float>();
    public List<GameObject> Children;
    // Start is called before the first frame update

    //    int time = 0;
    //    Dictionary<int, GameObject> startTime = new Dictionary<int, GameObject>();

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

    private void Init()
    {
        if (applySeed)
        {
            Random.seed = seed;
        }
        numOfAgents = Random.Range(minNumOfAgents, maxNumOfAgents + 1);

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

        origin = new Vector3(transform.position.x, 0f, transform.position.z);
        agentsActive();
        otherAgentsActive();
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

        agentsSetting();
        otherAgentsSetting();

        //GetComponent<RandomMaterialColor>().GenerateMaterialColor();

        /*
        string path = "Assets/group_label/test.txt";
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

            ///// Collision avoidance
            GameObject model = model_list[i];
            Vector3 to = model.transform.forward;
            foreach (GameObject model_ in model_list)
            {
                float dist = Vector3.Distance(model.transform.position, model_.transform.position);
                Vector3 from = model_.transform.position - model.transform.position;

                if (dist != 0f && dist <= 1.0f && Vector3.Angle(from, to) < 60)
                {
                    model.GetComponent<Animator>().speed = Mathf.Min(model.GetComponent<Animator>().speed * 0.96f, 0.20f);
                }
                else
                {
                    model.GetComponent<Animator>().speed = Mathf.Min(model.GetComponent<Animator>().speed * 1.03f, AgentSpeeds[i]);
                }
            }

        }
    }

    private Vector3[] placeInLine()
    {
        Vector3[] positions = new Vector3[numOfAgents];
        float firstx = Random.Range(-3, 0) + origin.x;
        float firstz = Random.Range(0, 3) + origin.z;
        float dist = distanceBetweenAgents;
        for (int i = 0; i < numOfAgents; i++)
        {
            int index = Random.Range(0, models.Length);
            while (existedIndex.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            existedIndex.Add(index);
            GameObject character = Instantiate(models[agents[i]], this.transform);
            character.SetActive(true);
            model_list.Add(character);
            positions[i] = new Vector3(firstx + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents, -0.9277931f, firstz + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents);
            existedPos.Add(firstx + firstz);
            firstx = firstx + dist;
        }
        return positions;
    }
    private Vector3[] placeInCircle()
    {
        Vector3[] positions = new Vector3[numOfAgents];
        float radius = distanceBetweenAgents;
        float rx = Random.Range(-3, 3) + origin.x;
        float rz = Random.Range(-3, 3) + origin.z;
        for (int i = 0; i < numOfAgents; i++)
        {
            int index = Random.Range(0, models.Length);
            while (existedIndex.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            existedIndex.Add(index);
            GameObject character = Instantiate(models[agents[i]], this.transform);
            character.SetActive(true);
            model_list.Add(character);
            float theta = i * 2 * Mathf.PI / numOfAgents;
            float xpos = Mathf.Sin(theta) * radius;
            float zpos = Mathf.Cos(theta) * radius;
            existedPos.Add(xpos + zpos);
            positions[i] = new Vector3(xpos + rx + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents, -0.9277931f, zpos + rz + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents);
        }
        return positions;
    }
    private Vector3[] placeInRect()
    {
        Vector3[] positions = new Vector3[numOfAgents];

        float xpos = Random.Range(-3, 3) + origin.x;
        float zpos = Random.Range(-3, 3) + origin.z;
        float xorigin = xpos;
        //float perimeter = length * 2 + width * 2;
        float dist = distanceBetweenAgents;
        float perimeter = dist * numOfAgents;
        float length = perimeter / 2;
        int numEachSide = numOfAgents / 2;
        int i = 0;
        for(; i < numEachSide; i++)
        {
            int index = Random.Range(0, models.Length);
            while (existedIndex.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            existedIndex.Add(index);
            GameObject character = Instantiate(models[agents[i]], this.transform);
            character.SetActive(true);
            model_list.Add(character);
            positions[i] = new Vector3(xpos + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents, -0.9277931f, zpos + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents);
            xpos = xpos + dist;
        }
        xpos = xorigin;
        zpos = zpos - distanceBetweenAgents;
        for(; i < positions.Length; i++)
        {
            int index = Random.Range(0, models.Length);
            while (existedIndex.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            agents[i] = index;
            existedIndex.Add(index);
            GameObject character = Instantiate(models[agents[i]], this.transform);
            character.SetActive(true);
            model_list.Add(character);
            positions[i] = new Vector3(xpos + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents, -0.9277931f, zpos + Random.Range(-0.25f, 0.25f) * distanceBetweenAgents);
            xpos = xpos + dist;
        }
        
        return positions;
    }
    private void agentsActive()
    {
        agents = new int[numOfAgents];
        int type = Random.Range(0, 4);
        Vector3[] positions = new Vector3[numOfAgents];
        if (type == 0)
        {
            positions = placeInLine();
        }
        else if (type == 1)
        {
            positions = placeInCircle();
        }
        else
        {
            positions = placeInRect();
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
        for (int i = 0; i < numOfAgents; i++)
        {
            model_list[i].SetActive(true);
            //models[agents[i]].GetComponent<NavMeshAgent>().enabled = false;
            model_list[i].transform.position = positions[i] + origin;
            /*     print(models[agents[i]].activeSelf);*/
            model_list[i].transform.eulerAngles = new Vector3(0, 180, 0);
        }



        //NavMeshAgent navme = models[agents[i]].GetComponent<NavMeshAgent>();
        //anim.SetFloat("walking", navme.velocity.magnitude);
    }
    private void agentsSetting()
    {
        float rotate = Random.Range(-180, 180);
        //AnimatorController agentsController = controllers[Random.Range(0, controllers.Count)];

        for (int i = 0; i < numOfAgents; i++)
        {
            model_list[i].transform.Rotate(0.0f, rotate, 0.0f, Space.Self);

            //model_list[i].GetComponent<Animator>().runtimeAnimatorController = agentsController;
            //model_list[i].GetComponent<Animator>().speed = Random.Range(1.5f, 2.0f);
            //model_list[i].AddComponent<activity_1_agent>();
            ///*model_list[i].GetComponent<activity_1_agent>().m_Animator = model_list[i].GetComponent<Animator>();*/
            ///*model_list[i].GetComponent<activity_1_agent>().model_list = model_list;*/
            //model_list[i].GetComponent<activity_1_agent>().isNotOther = true;
            //model_list[i].GetComponent<activity_1_agent>().model_list = model_list;
            //model_list[i].GetComponent<activity_1_agent>().other_models = other_models;

            ///*model_list[i].AddComponent<Rigidbody>();
            //model_list[i].GetComponent<Rigidbody>().useGravity = false;
            //BoxCollider bc = model_list[i].AddComponent<BoxCollider>();
            //*//*bc.center = new Vector3(0, 0, 1);*//*
            //bc.size = new Vector3(1, 2, 1);
            //bc.isTrigger = true;*/
            //model_list[i].transform.Rotate(0.0f, rotate, 0.0f, Space.Self);
            //Animator anim = model_list[i].GetComponent<Animator>();
            //AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index#
            //anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));

            //change_label = models[agents[i]].GetComponent<Labeling>();
            //change_label.labels[0] = "Running";

            //navme.SetDestination(destinations[i]);

            float speed = Random.Range(0.3f, 0.6f);
            AgentSpeeds.Add(speed);
            model_list[i].GetComponent<Animator>().speed = speed;
            MixamoBlendTree t = AnimationManager.GetGroupByName("Running").RandomBlendTree();
            string[] paramNames = t.GetClipParameterNames();
            foreach (string p in paramNames)
            {
                AnimationManager.SetParameterRnd(i, p);
            }
            AnimationManager.PlayClip(i, t.name, 0.0f, Random.Range(0f, 1f));
        }
    }
    private void otherAgentsActive()
    {
        otheragents = new int[numOfOtherAgents];

        Vector3[] otherPositions = new Vector3[numOfOtherAgents];


        for (int i = 0; i < numOfOtherAgents; i++)
        {
            int index = Random.Range(0, models.Length);
            while (existedIndex.Contains(index))
            {
                index = Random.Range(0, models.Length);
            }
            otheragents[i] = index;
            existedIndex.Add(index);
            float xPos = Random.Range(-10, 10);
            float zPos = Random.Range(-10, 10);

            while (existedPos.Contains(xPos + zPos))
            {
                xPos = Random.Range(-10, 10);
                zPos = Random.Range(-10, 10);
            }

            otherPositions[i] = new Vector3(xPos, -0.9277931f, zPos);
            existedPos.Add(xPos + zPos);
            other_models.Add(models[index]);
        }

        for (int i = 0; i < numOfOtherAgents; i++)
        {
            models[otheragents[i]].SetActive(true);
            models[otheragents[i]].transform.position = otherPositions[i];
        }


    }
    private void otherAgentsSetting()
    {
        for (int i = 0; i < numOfOtherAgents; i++)
        {
            other_models[i].GetComponent<Animator>().runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
            other_models[i].GetComponent<Animator>().speed = Random.Range(1.5f, 3.2f);
            other_models[i].AddComponent<activity_1_agent>();
            /*model_list[i].GetComponent<activity_1_agent>().m_Animator = model_list[i].GetComponent<Animator>();*/
            other_models[i].GetComponent<activity_1_agent>().model_list = model_list;
            other_models[i].GetComponent<activity_1_agent>().other_models = other_models;
            /*model_list[i].AddComponent<Rigidbody>();
            model_list[i].GetComponent<Rigidbody>().useGravity = false;
            BoxCollider bc = model_list[i].AddComponent<BoxCollider>();
            *//*bc.center = new Vector3(0, 0, 1);*//*
            bc.size = new Vector3(1, 2, 1);
            bc.isTrigger = true;*/
            other_models[i].transform.LookAt(new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15)));
            Animator anim = other_models[i].GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index#
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));

            change_label = models[agents[i]].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            change_label.labels[0] = "Running";

            //navme.SetDestination(destinations[i]);
        }
    }
    public void k_clustering()
    {
        Vector3 center1 = new Vector3(-5, 0, -5);
        Vector3 center2 = new Vector3(5, 0, 5);
        HashSet<Vector3> set1 = new HashSet<Vector3>();
        HashSet<Vector3> set2 = new HashSet<Vector3>();
        for (int i = 0; i < 5; i++)
        {

            set1 = new HashSet<Vector3>();
            set2 = new HashSet<Vector3>();
            foreach (GameObject model in model_list)
            {
                Vector3 temp = model.transform.position;
                if (distance(center1, temp) < distance(center2, temp))
                {
                    set1.Add(temp);
                }
                else
                {
                    set2.Add(temp);
                }
            }
            float posx = 0;
            float posz = 0;
            foreach (Vector3 pos in set1)
            {
                posx = posx + pos.x;
                posz = posz + pos.z;
            }
            center1 = new Vector3(posx / set1.Count, 0, posz / set1.Count);
            posx = 0;
            posz = 0;
            foreach (Vector3 pos in set2)
            {
                posx = posx + pos.x;
                posz = posz + pos.z;
            }
            center2 = new Vector3(posx / set2.Count, 0, posz / set2.Count);
        }
        camera.GetComponent<CameraController>().center = set1.Count > set2.Count ? center1 : center2;
        camera.GetComponent<CameraController>().updateCameraViews();
    }


    public float distance(Vector3 a, Vector3 b)
    {
        float x = a.x - b.x;
        float z = a.z - b.z;
        return Mathf.Sqrt(x * x + z * z);
    }
}

