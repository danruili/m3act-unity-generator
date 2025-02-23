using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

//using UnityEditor;
using System.IO;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Perception.Randomization.Scenarios;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif
public class activity_2 : MonoBehaviour
{
    public bool applySeed;
    public int seed;
    public bool addSyntheticHumans = false;
    private bool isFirstUpdate = true;

    public int maxNumOfRows;
    private int numOfRows;
    public float RowInterval;
    public float distance;
    public int minNumOfAgents;
    public int maxNumOfAgents;
    private int numOfAgents;
    public int numOfOtherAgents;
    private GameObject[] models;
    public int maxNumOfTwoPersonGroups;
    public int maxNumOfThreePersonGroups;

    private int numOfTwoPersonGroups;
    private int numOfThreePersonGroups;
    public RuntimeAnimatorController defaultController;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> controllers_group = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> controllers_others = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> controllers_walkandstop = new List<RuntimeAnimatorController>();
    public List<string> actions = new List<string>();
    public List<string> actions_group = new List<string>();
    public float ambient_intensity = 2.0f;
    public GameObject camera, active_env, env_collections;
    public int index;
    Vector3 origin;

    public enum QueueLineType
    {
        StraightLine,
        // OneCornerStraightLine,
        // TwoCornerStraightLine,
        // Parabola,
        // Curve
    }

    public QueueLineType QueueLineMethod = QueueLineType.StraightLine;

    private List<int> GroupOrder;
    private List<List<int>> GroupOrderForEachRow = new List<List<int>>();
    private UnityEngine.Perception.GroundTruth.LabelManagement.Labeling change_label;
    private float xpos = 0;
    private float xpos_lookat = -50;
    private float zpos_lookat = 50;
    private float zpos = 0;
    private AnimControllerManager AnimationManager;
    public List<GameObject> Children;
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

    private void Init()
    {
        if (applySeed)
        {
            Random.seed = seed;
        }
        numOfAgents = Random.Range(minNumOfAgents, maxNumOfAgents + 1);
        maxNumOfTwoPersonGroups = Mathf.Max(maxNumOfTwoPersonGroups, 0);
        numOfTwoPersonGroups = Random.Range(0, maxNumOfTwoPersonGroups + 1);
        maxNumOfThreePersonGroups = Mathf.Max(maxNumOfThreePersonGroups, 1);
        numOfThreePersonGroups = Random.Range(0, maxNumOfThreePersonGroups + 1);
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
        RenderSettings.ambientIntensity = ambient_intensity;
        int numAgentsPerRow = numOfAgents + numOfTwoPersonGroups * 2 + numOfThreePersonGroups * 3;
        int[] agents = new int[numOfRows * numAgentsPerRow + numOfOtherAgents];
        Vector3[] positions = new Vector3[numOfRows * numAgentsPerRow + numOfOtherAgents];
        float a0 = Random.Range(-1.2f, 1.2f);
        float a1 = Random.Range(-1.2f, 1.2f);
        float a2 = Random.Range(-1.2f, 1.2f);
        float thresh1 = Random.Range(5f, 8f);
        float thresh2 = Random.Range(13f, 18f);
        float pb0 = Random.Range(-0.2f, 0.2f);
        float pb1 = Random.Range(-1.2f, 1.2f);
        float radius = Random.Range(5f, 20f);
        float curvesign = Mathf.Sign(Random.Range(-1f, 1f));

        xpos_lookat = - a0 * zpos_lookat;
        Vector3 backwardsDir = new Vector3(-xpos_lookat, 0f, -zpos_lookat).normalized;

        HashSet<int> existedModels = new HashSet<int>();
        //        while (model_list.Count == numOfAgents)

        /*
        for (int i = 0; i<GroupOrder.Count; i++)
        {
            Debug.Log(GroupOrder[i]);
        }
        */
        float xpos0 = xpos;

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
            ShuffleGroupOrder();
            GroupOrderForEachRow.Add(GroupOrder);
            for (int i = 0; i < numOfAgents + numOfTwoPersonGroups * 2 + numOfThreePersonGroups * 3; i++)
            {
                float noise_x, noise_z;
                int index = Random.Range(0, models.Length);
                while (existedModels.Contains(index))
                {
                    index = Random.Range(0, models.Length);
                }
                existedModels.Add(index);
                agents[numAgentsPerRow * r + i] = index;

                if (GroupOrder[i] == 1)
                {
                    xpos = xpos + distance / Random.Range(0.9f, 1.5f);
                    noise_z = Random.Range(-.5f, .5f);
                    noise_x = Random.Range(-.25f, .25f);
                }
                else if (GroupOrder[i] == 2)
                {
                    xpos = xpos + distance / 1.4f;
                    noise_z = Random.Range(-.5f, .5f);
                    noise_x = Random.Range(-.2f, .2f);
                }
                else
                {
                    xpos = xpos + distance / 1.5f;
                    noise_z = Random.Range(-1f, 1f);
                    noise_x = Random.Range(-.25f, .25f);
                }


                positions[numAgentsPerRow * r + i] = new Vector3(xpos + noise_x, -0.9277931f, StraightLine(xpos - xpos0, a0) + noise_z) + backwardsDir * r * RowInterval * Random.Range(0.8f, 1.2f);

                GameObject character = Instantiate(models[agents[numAgentsPerRow * r + i]], this.transform);
                character.SetActive(true);
                character.transform.position = positions[numAgentsPerRow * r + i] + origin;
                character.transform.eulerAngles = new Vector3(0, 180, 0);
                character.GetComponent<Animator>().runtimeAnimatorController = defaultController;
                model_list.Add(character);

            }

        }
        Vector3 center = Vector3.zero;
        for (int i = 0; i < numOfRows * numAgentsPerRow; i++)
        {
            center += positions[i];
        }
        center /= (numOfRows * numAgentsPerRow);
        center += origin;
        camera.GetComponent<CameraController>().center = center;
        //camera.GetComponent<CameraController>().center = Vector3.zero;

        camera.GetComponent<CameraController>().updateCameraViews();

        for (int r = 0; r < numOfRows; r++)
        {
            int j = 0;
            for (int i = 0; i < numOfAgents + numOfTwoPersonGroups * 2 + numOfThreePersonGroups * 3; i++)
            {
                if (GroupOrderForEachRow[r][i] == 1)
                {
                    model_list[numAgentsPerRow * r + i].transform.LookAt(new Vector3(xpos_lookat + Random.Range(-20f, 20f), -0.9277931f, zpos_lookat + Random.Range(-20f, 20f)));
                }
                else if (GroupOrderForEachRow[r][i] == 2)
                {
                    if (j % 2 == 0)
                    {
                        model_list[numAgentsPerRow * r + i].transform.LookAt(model_list[numAgentsPerRow * r + i + 1].transform.position + new Vector3(Random.Range(-.5f, .5f), 0f, Random.Range(-.5f, .5f)));
                    }
                    else
                    {
                        model_list[numAgentsPerRow * r + i].transform.LookAt(model_list[numAgentsPerRow * r + i - 1].transform.position + new Vector3(Random.Range(-.5f, .5f), 0f, Random.Range(-.5f, .5f)));
                    }
                    j = (j + 1) % 2;
                }
                else
                {
                    if (j % 3 == 0)
                    {
                        model_list[numAgentsPerRow * r + i].transform.LookAt(.5f * model_list[numAgentsPerRow * r + i + 1].transform.position + .5f * model_list[numAgentsPerRow * r + i + 2].transform.position + new Vector3(Random.Range(-.25f, .25f), 0f, Random.Range(-.25f, .25f)));
                    }
                    else if (j % 3 == 1)
                    {
                        model_list[numAgentsPerRow * r + i].transform.LookAt(.5f * model_list[numAgentsPerRow * r + i - 1].transform.position + .5f * model_list[numAgentsPerRow * r + i + 1].transform.position + new Vector3(Random.Range(-.25f, .25f), 0f, Random.Range(-.25f, .25f)));
                    }
                    else
                    {
                        model_list[numAgentsPerRow * r + i].transform.LookAt(.5f * model_list[numAgentsPerRow * r + i - 1].transform.position + .5f * model_list[numAgentsPerRow * r + i - 2].transform.position + new Vector3(Random.Range(-.25f, .25f), 0f, Random.Range(-.25f, .25f)));
                    }
                    j = (j + 1) % 3;
                }

            }
        }
        //HashSet<int> existedModels2 = new HashSet<int>();
        for (int i = numOfRows * numAgentsPerRow; i < numOfRows * numAgentsPerRow + numOfOtherAgents; i++)
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


            GameObject character = Instantiate(models[agents[i]], this.transform);


            character.SetActive(true);
            character.transform.position = positions[i] + origin;
            character.transform.eulerAngles = new Vector3(0, 180, 0);
            character.GetComponent<Animator>().runtimeAnimatorController = defaultController;
            character.transform.LookAt(new Vector3(Random.Range(-25f, 25f), -0.9277931f, Random.Range(-25f, 25f)));
            model_list.Add(character);

        }

        /* Universal Controller */
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

        /* Animation Setting */
        for (int r = 0; r < numOfRows; r++)
        {
            for (int i = 0; i < numOfAgents + numOfTwoPersonGroups * 2 + numOfThreePersonGroups * 3; i++)
            {
                if (GroupOrderForEachRow[r][i] == 1)
                {
                    /*
                    model_list[numAgentsPerRow * r + i].GetComponent<Animator>().runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
                    Animator anim = model_list[numAgentsPerRow * r + i].GetComponent<Animator>();
                    AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
                    anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
                    */
                    //model_list[numAgentsPerRow * r + i].GetComponent<Animator>().speed = Random.Range(.8f, 1.2f);
                    //string groupname = actions[Random.Range(0, actions.Count)];
                    //string rndWalkingClip = AnimationManager.GetGroupByName("Walking").RandomClipName();
                    //string rndActionClip = AnimationManager.GetGroupByName(groupname).RandomClipName();
                    //string[] clips = { rndActionClip, rndWalkingClip };
                    //float[] intervals = { 0f, 2.5f, };
                    //float[] fadeValues = { 1.0f, 0.05f };
                    //float[] offsets = { 0, Random.Range(0, 1) };
                    //AnimationManager.RandomizeAllParameters(numAgentsPerRow * r + i); //randomize all parameters before playing
                    //AnimationManager.PlayClipsSchduled(numAgentsPerRow * r + i, clips, intervals, fadeValues, offsets);
                    
                    model_list[numAgentsPerRow * r + i].GetComponent<Animator>().speed = Random.Range(.6f, 1.0f);
                    string groupname = actions[Random.Range(0, actions.Count)];
                    MixamoBlendTree t = AnimationManager.GetGroupByName(groupname).RandomBlendTree();
                    string[] paramNames = t.GetClipParameterNames();
                    foreach (string p in paramNames)
                    {
                        AnimationManager.SetParameterRnd(numAgentsPerRow * r + i, p);
                    }
                    AnimationManager.PlayClip(numAgentsPerRow * r + i, t.name, 0.0f, Random.Range(0f, 1f));
                }
                else
                {
                    /*
                    model_list[numAgentsPerRow * r + i].GetComponent<Animator>().runtimeAnimatorController = controllers_group[Random.Range(0, controllers_group.Count)];
                    Animator anim = model_list[numAgentsPerRow * r + i].GetComponent<Animator>();
                    AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
                    anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));*/
                    model_list[numAgentsPerRow * r + i].GetComponent<Animator>().speed = Random.Range(.6f, 1.0f);
                    string groupname = actions_group[Random.Range(0, actions_group.Count)];
                    MixamoBlendTree t = AnimationManager.GetGroupByName(groupname).RandomBlendTree();
                    string[] paramNames = t.GetClipParameterNames();
                    foreach (string p in paramNames)
                    {
                        AnimationManager.SetParameterRnd(numAgentsPerRow * r + i, p);
                    }
                    AnimationManager.PlayClip(numAgentsPerRow * r + i, t.name, 0.0f, Random.Range(0f, 1f));
                }
            }
        }
        for (int r = 1; r < numOfRows; r++)
        {
            for (int i = 0; i < numOfAgents + numOfTwoPersonGroups * 2 + numOfThreePersonGroups * 3; i++)
            {
                if (GroupOrderForEachRow[r][i] == 1 && Vector3.Distance(model_list[numAgentsPerRow * r + i].transform.position, model_list[numAgentsPerRow * (r - 1) + i].transform.position) > distance * 1.6f)
                {
                    model_list[numAgentsPerRow * r + i].GetComponent<Animator>().speed = Random.Range(.4f, .8f);
                    string[] WalkingClips = { "Walking 1", "Walking 2", "Walking 3", "Walking 6", "Walking 10", "Walking 16" };
                    string groupname1 = actions[Random.Range(0, actions.Count)];
                    string groupname2 = actions[Random.Range(0, actions.Count)];
                    string rndAction1Clip = AnimationManager.GetGroupByName(groupname1).RandomClipName();
                    string rndWalkingClip = WalkingClips[Random.Range(0, WalkingClips.Length)];
                    string rndAction2Clip = AnimationManager.GetGroupByName(groupname2).RandomClipName();
                    string[] clips = { rndAction1Clip, "Walking 10", rndAction2Clip };
                    float walk_timing = Random.Range(0.25f, 0.5f);
                    float[] intervals = { 0, walk_timing, walk_timing + 0.2f };
                    float[] fadeValues = { 0.1f, 0.0f, 0.1f };
                    float[] offsets = { Random.Range(0, 1), 0.0f, Random.Range(0, 1) };
                    AnimationManager.RandomizeAllParameters(numAgentsPerRow * r + i); //randomize all parameters before playing
                    AnimationManager.PlayClipsSchduled(numAgentsPerRow * r + i, clips, intervals, fadeValues, offsets);
                }
            }
        }
        for (int i = numOfRows * numAgentsPerRow; i < numOfRows * numAgentsPerRow + numOfOtherAgents; i++)
        {
            model_list[i].GetComponent<Animator>().runtimeAnimatorController = controllers_others[Random.Range(0, controllers_others.Count)];

            Animator anim = model_list[i].GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0); //could replace 0 by any other animation layer index
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
            change_label = model_list[i].GetComponent<UnityEngine.Perception.GroundTruth.LabelManagement.Labeling>();
            //change_label.labels[0] = model_list[i].GetComponent<Animator>().runtimeAnimatorController.name;
            string clipName = model_list[i].GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;
            change_label.labels[0] = clipName;
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
    private float Parabola(float deltax, float pb0, float pb1)
    {
        return pb0 * deltax * deltax + pb1 * deltax + zpos;
    }

    private float Curve(float deltax, float radius, float sign)
    {
        if (deltax < radius)
        {
            return sign * (Mathf.Sqrt(radius * radius - deltax * deltax) - radius) + zpos;
        }
        else
        {
            return -sign * radius + zpos;
        }
    }

    private float StraightLine(float deltax, float a)
    {
        return a * deltax + zpos;
    }

    private float OneCornerStraightLine(float deltax, float a, float thresh1, float b)
    {
        if (deltax < thresh1)
        {
            return a * deltax + zpos;
        }
        else
        {
            return b * (deltax - thresh1) + a * thresh1 + zpos;
        }
    }

    private float TwoCornerStraightLine(float deltax, float a, float thresh1, float b, float thresh2, float c)
    {
        if (deltax < thresh1)
        {
            return a * deltax + zpos;
        }
        else if (deltax < thresh2)
        {
            return b * (deltax - thresh1) + a * thresh1 + zpos;
        }
        else
        {
            return c * (deltax - thresh2) + b * (thresh2 - thresh1) + a * thresh1 + zpos;
        }
    }

    public void ShuffleGroupOrder()
    {
        GroupOrder = new List<int>();
        List<int> _GroupOrder = new List<int>();
        for (int i = 0; i < numOfAgents; i++)
        {
            GroupOrder.Add(1);
        }
        for (int i = numOfAgents; i < numOfAgents + numOfTwoPersonGroups; i++)
        {
            GroupOrder.Add(2);
        }
        for (int i = numOfAgents + numOfTwoPersonGroups; i < numOfAgents + numOfTwoPersonGroups + numOfThreePersonGroups; i++)
        {
            GroupOrder.Add(3);
        }

        for (int i = 0; i < GroupOrder.Count - 1; i++)
        {
            int rnd = Random.Range(i, GroupOrder.Count);
            var temp = GroupOrder[rnd];
            GroupOrder[rnd] = GroupOrder[i];
            GroupOrder[i] = temp;
        }

        foreach (var id in GroupOrder)
        {
            for (int j = 0; j < id; j++)
            {
                _GroupOrder.Add(id);
            }
        }

        GroupOrder = _GroupOrder;
    }
}
