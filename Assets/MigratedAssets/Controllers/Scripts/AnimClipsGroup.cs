using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AnimClipsGroup", order = 1)]
public class AnimClipsGroup : ScriptableObject
{
    public List<AnimationClip> Clips;
    //public List<BlendTreeClipInfo> BlendTreeClipsInfos;
    public List<MixamoBlendTree> MixamoBlendTrees;
    
    /// <summary>
    /// return a random clip name from all the clips and blendtree clips in the group
    /// </summary>
    /// <returns></returns>
    public string RandomClipName()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        int clipLen = Clips == null ? 0 : Clips.Count;
        int btLen = MixamoBlendTrees == null ? 0 : MixamoBlendTrees.Count;
        if(clipLen == 0 && btLen == 0)
        {
            throw new Exception("RandomClip() is called on a empty group!");
        }
        if (btLen == 0) rnd = 0;
        if (clipLen == 0) rnd = 1;

        if(rnd == 0)
        {
            return Clips[UnityEngine.Random.Range(0, Clips.Count)].name;
        }
        else
        {
            return MixamoBlendTrees[UnityEngine.Random.Range(0, MixamoBlendTrees.Count)].name;
        }
    }

    public MixamoBlendTree RandomBlendTree()
    {
        if(MixamoBlendTrees == null) throw new Exception("RandomClip() is called on a empty group!");
        if(MixamoBlendTrees.Count < 1) throw new Exception("RandomClip() is called on a empty group!");
        return MixamoBlendTrees[UnityEngine.Random.Range(0, MixamoBlendTrees.Count)];
    }

    public MixamoBlendTree GetBlendTreeByName(string clipName)
    {
        return MixamoBlendTrees.Find(x => x.name.Equals(clipName));
    }
}
