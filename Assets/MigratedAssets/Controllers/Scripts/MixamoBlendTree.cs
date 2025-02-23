using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MixamoBlendTree", order = 1)]
public class MixamoBlendTree : ScriptableObject
{
    public const float PARAM_MIN = -1f;
    public const float PARAM_MAX = 1f;
    public const string PARAM_DEFAULT_LETTERS = "XYZW";

    public MixamoBlendMode blendMode;
    public AnimationClip[] Clips = new AnimationClip[8];
    public string[] ParameterNames = new string[3];

    private void OnValidate()
    {
        for(int i = 0; i < ParameterNames.Length; i++)
        {
            if (ParameterNames[i].Equals(""))
            {
                ParameterNames[i] = PARAM_DEFAULT_LETTERS[i].ToString();
            }
        }
    }

#if UNITY_EDITOR
    public BlendTree GenerateBlendTree()
    {
        BlendTree bt = null;
        switch (blendMode)
        {
            case MixamoBlendMode.Tree1D:
                bt = Generate1DBlendTree();
                break;
            case MixamoBlendMode.Tree2D:
                bt = Generate2DBlendTree();
                break;
            case MixamoBlendMode.Tree3D:
                bt = Generate3DBlendTree();
                break;
        }
        return bt;
    }
#endif

    public string[] GetClipParameterNames()
    {
        return ParameterNames.Take(blendMode.ToDimension()).Select(x => name + "_" + x).ToArray(); ;
    }
#if UNITY_EDITOR
    public BlendTree Generate1DBlendTree()
    {
        BlendTree bt = new BlendTree();
        ModifyBlendTree(ref bt, name, GetParameterNameByIndex(0), null, BlendTreeType.Simple1D, PARAM_MIN, PARAM_MAX);
        bt.AddChild(Clips[0], PARAM_MIN);
        bt.AddChild(Clips[1], PARAM_MAX);
        return bt;
    }

    public BlendTree Generate2DBlendTree()
    {
        BlendTree bt = new BlendTree();
        ModifyBlendTree(ref bt, name, GetParameterNameByIndex(0), GetParameterNameByIndex(1), BlendTreeType.FreeformCartesian2D, PARAM_MIN, PARAM_MAX);
        for(int i = 0; i < 4; i++)
        {
            float min = i < 2 ? PARAM_MIN : PARAM_MAX;
            float max = i % 2 == 0 ? PARAM_MIN : PARAM_MAX;
            bt.AddChild(Clips[i], new Vector2(min, max));
        }
        return bt;
    }

    public BlendTree Generate3DBlendTree()
    {
        BlendTree bt = new BlendTree();
        BlendTree[] childrenBlendTrees = new BlendTree[4];
        string xParam = GetParameterNameByIndex(0);
        string yParam = GetParameterNameByIndex(1);
        string zParam = GetParameterNameByIndex(2);
        ModifyBlendTree(ref bt, name, xParam, yParam, BlendTreeType.FreeformCartesian2D, PARAM_MIN, PARAM_MAX);
        for (int i = 0; i < 4; i++)
        {
            childrenBlendTrees[i] = new BlendTree();
            ModifyBlendTree(ref childrenBlendTrees[i], name + "child" + i.ToString(), zParam,
                            null, BlendTreeType.Simple1D, PARAM_MIN, PARAM_MAX);
            //i: 0 1 2 3
            //1: 0 2 4 6
            //2: 1 3 5 7
            AnimationClip clip1 = Clips[2*i];
            AnimationClip clip2 = Clips[2*i + 1];
            childrenBlendTrees[i].AddChild(clip1, PARAM_MIN);
            childrenBlendTrees[i].AddChild(clip2, PARAM_MAX);
            float x = i <= 1 ? PARAM_MIN : PARAM_MAX;
            float y = i % 2 == 0 ? PARAM_MIN : PARAM_MAX;
            bt.AddChild(childrenBlendTrees[i], new Vector2(x, y));
        }
        return bt;
    }
#endif

    public string GetParameterNameByIndex(int i)
    {
        return name + "_" + ParameterNames[i];
    }

    public static float RandomParameterValue()
    {
        return UnityEngine.Random.Range(PARAM_MIN, PARAM_MAX);
    }

    public static Tuple<int, int> IntToTupleOfBinaryDigits(int i)
    {
        string binaryStr = Convert.ToString(i, 2);
        return new Tuple<int, int>(binaryStr[0] - '0', binaryStr[1] - '0');
    }

#if UNITY_EDITOR
    public static void ModifyBlendTree(ref BlendTree bt, string name, string blendParameter,
                                        string blendParameterY, BlendTreeType blendType, float minThreshold,
                                        float maxThreshold)
    {
        bt.name = name;
        bt.blendParameter = blendParameter;
        bt.blendParameterY = blendParameterY;
        bt.useAutomaticThresholds = false;
        bt.blendType = blendType;
        bt.maxThreshold = maxThreshold;
        bt.minThreshold = minThreshold;
    }
#endif
}

