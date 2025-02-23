using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

public class AnimControllerManager : MonoBehaviour
{
    public string AnimatorControllerPath = "Assets/ZiyangTesting/CombinedController.controller";
    public List<AnimClipsGroup> AnimClipsGroups;
    public List<GameObject> Characters;
    public List<Animator> CharacterAnimators;
    public List<IEnumerator> CharacterRunningAnimationSequences;
    public RuntimeAnimatorController controller;

#if UNITY_EDITOR
    public AnimatorController controller1;
#endif

    #region Unity Functions

    private void Awake()
    {
        //create the animation controller
        //controller = AnimatorController.CreateAnimatorControllerAtPath(AnimatorControllerPath);
        //CharacterAnimators = new List<Animator>(new Animator[Characters.Count]);
        //CharacterRunningAnimationSequences = new List<IEnumerator>(new IEnumerator[Characters.Count]);
    }


void Start()
    {
        ////add all clips from all groups to the controller
        //LoadClipsToController();

        ////bind the runtimeAnimatorController of each character to the controller created
        //AttachControllerToAllCharacters();
    }

    void Update()
    {
        //Expample Code
        
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //play a random clip from the "Fighting" group with 0.1f fade time and a random offset
        //    PlayClip(0, GetGroupByName("Walking").RandomClipName(), 0.1f, Random.Range(0f,1f));
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    //play a random blend tree clip from the "Fighting" group with 0.3f fade time and a random offset
        //    MixamoBlendTree t = GetGroupByName("Fighting").RandomBlendTree();
        //    string[] paramNames = t.GetClipParameterNames();
        //    foreach (string p in paramNames)
        //    {
        //        SetParameterRnd(0, p);
        //    }
        //    PlayClip(0, t.name, 0.3f, Random.Range(0f,1f));
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    //Schduled clips:
        //    //  wait for 0 seconds and start a random idle clip for 1.5 seconds,
        //    //  then start a random texting clip for 2.5 seconds, then start a random walking clip
        //    //  for 2.5 seconds, and end with a random fighting clip looping.
        //    //fadeValues:
        //    //  define normalized transition time of every animation
        //    //offsets:
        //    //  define normalized offset of every animation
        //    string rndIdleClip = GetGroupByName("Standing-idle").RandomClipName();
        //    string rndTextingClip = GetGroupByName("Texting").RandomClipName();
        //    string rndWalkingClip = GetGroupByName("Walking").RandomClipName();
        //    string rndFightingClip = GetGroupByName("Fighting").RandomClipName();
        //    string[] clips = { rndIdleClip, rndTextingClip, rndWalkingClip, rndFightingClip };
        //    float[] intervals = { 0f, 1.5f, 2.5f, 7.5f };
        //    float[] fadeValues = { 0.1f, 0.1f, 0.3f, 0.3f };
        //    float[] offsets = { 0, 0, 0, 0 };
        //    RandomizeAllParameters(0); //randomize all parameters before playing
        //    PlayClipsSchduled(0, clips, intervals, fadeValues, offsets);
        //}
        
    }

#endregion

#region Public Functions

    public void RandomizeAllParameters(int characterID)
    {
        foreach (AnimatorControllerParameter acp in CharacterAnimators[characterID].parameters)
        {
            SetParameterRnd(characterID, acp.name);
        }
    }

#if UNITY_EDITOR
    public void RandomizeBlendTreeClipParameters(int characterID, string groupName, string clipName)
    {
        AnimClipsGroup group = GetGroupByName(groupName);
        group.GetBlendTreeByName(clipName)
             .GetClipParameterNames()
             .ToList()
             .ForEach(x => SetParameterRnd(characterID, x));
    }
#endif
    public void SetParameterRnd(int characterID, string parameterName)
    {
        SetParameter(characterID, parameterName, MixamoBlendTree.RandomParameterValue());
    }

    public void SetParameter(int characterID, string parameterName, float value)
    {
        CharacterAnimators[characterID].SetFloat(parameterName, value);
    }

    
    /// <summary>
    /// Play a clip on a character
    /// </summary>
    /// <param name="characterID">index number of the character in the list</param>
    /// <param name="clipName">name of the clip</param>
    /// <param name="fade">normalized transition duration</param>
    /// <param name="offset">normalized offset</param>
    public void PlayClip(int characterID, string clipName, float fade = 0.3f, float offset = 0)
    {
        CharacterAnimators[characterID].CrossFade(clipName, fade, 0, offset);
    }

    /// <summary>
    /// Play a set of clips on a character with corresponding intervals (in seconds)
    /// </summary>
    /// <param name="characterID"></param>
    /// <param name="clipNames">array of names of clips to play</param>
    /// <param name="intervals">array of intervals (in seconds) between each clip</param>
    /// <param name="fadeValues">array of normalized transition duration values between each clip</param>
    /// <param name="offsets">array of normailized offset values for each clip</param>
    public void PlayClipsSchduled(int characterID, string[] clipNames, float[] intervals, float[] fadeValues, float[] offsets = null)
    {
        if (clipNames.Length != intervals.Length || clipNames.Length != fadeValues.Length)
        {
            throw new System.Exception("clipsNames, intervals, and fadeValues should have the same size!");
        }
        //TODO: Stop the last coroutine before starting a new one
        var sq = CharacterRunningAnimationSequences;
        if(sq[characterID] != null) StopCoroutine(sq[characterID]);
        sq[characterID] = playClipsSchduled(characterID, clipNames, intervals, fadeValues, offsets);
        StartCoroutine(sq[characterID]);
    }

    public AnimClipsGroup GetGroupByID(int ID)
    {
        return AnimClipsGroups[ID];
    }

    public AnimClipsGroup GetGroupByName(string name)
    {
        return AnimClipsGroups.Find(x => x.name.Equals(name));
    }
#endregion

#region Private Helpers

    public void AttachControllerToAllCharacters()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Animator animator = Characters[i].GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
#if UNITY_EDITOR
            animator.runtimeAnimatorController = controller1;
#endif
            CharacterAnimators[i] = animator;
        }

    }

    public void LoadClipsToController()
    {
#if UNITY_EDITOR

        foreach (var clipGroup in AnimClipsGroups)
        {
            foreach (var clip in clipGroup.Clips)
            {
                controller1.AddMotion(clip);
            }

            foreach (MixamoBlendTree mbt in clipGroup.MixamoBlendTrees)
            {
                controller1.AddMotion(mbt.GenerateBlendTree());
                foreach (string param in mbt.GetClipParameterNames())
                {
                    controller1.AddParameter(param, AnimatorControllerParameterType.Float);
                }
            }
        }
#endif

    }
    /// <summary>
    /// Helper function of PlayClipsSchduled()
    /// </summary>
    public IEnumerator playClipsSchduled(int characterID, string[] clipNames, float[] intervals, float[] fadeValues, float[] offsets)
    {
        for (int i = 0; i < clipNames.Length; i++)
        {
            yield return new WaitForSeconds(intervals[i]);
            //print("playing clip: " + clipNames[i]);
            offsets = offsets ?? new float[clipNames.Length];
            PlayClip(characterID, clipNames[i], fadeValues[i], offsets[i]);
        }
    }

#endregion

    
    
}
