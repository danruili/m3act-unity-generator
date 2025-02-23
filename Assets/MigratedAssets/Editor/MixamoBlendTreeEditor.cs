using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(MixamoBlendTree))]
public class MixamoBlendTreeEditor : Editor
{
    public const string INFO_MSG = "X, Y and Z are corresponding to the 3 parameters in a Mixamo animation. " +
                            "The number 0 or 1 after is corresponding to 0% or 100% of the parameter" +
                            "on the Mixamo animation page. \n\n" +
                            "Please make sure the correct animation is imported" +
                            "or this blend tree will not work!";

    int currentParamNum = 1;

    private SerializedProperty Clips;
    private SerializedProperty ParameterNames;

    private void OnEnable()
    {
        Clips = serializedObject.FindProperty("Clips");
        ParameterNames = serializedObject.FindProperty("ParameterNames");
    }

    public override void OnInspectorGUI()
    {
        Debug.Log("on inpector gui");

        MixamoBlendTree script = (MixamoBlendTree)target;

        drawTreeModeSelection(script);

        drawClipsAndParameterNameField(script);

        //if(GUILayout.Button("Import .fbx From Folder"))
        //{
        //    string path = EditorUtility.OpenFolderPanel("Select Clips Folder", "", "");
            
        //    List<AnimationClip> clipsInFolder = UnpackAllFbxInFolder(path);

        //    for (int i = 0; i < clipsInFolder.Count - 1; i++)
        //    {
        //        script.Clips[i + 1] = clipsInFolder[i];
        //    }

        //    script.Clips[0] = clipsInFolder[clipsInFolder.Count - 1];

        //    //List<string> failed = new List<string>();
        //    //AssetDatabase.DeleteAssets(files, failed);
        //}

        EditorGUILayout.HelpBox(INFO_MSG, MessageType.Info, true);
    }

    public static List<AnimationClip> UnpackAllFbxInFolder(string path)
    {
        string[] files = Directory.GetFiles(path);
        files = files.Where(x => x.EndsWith(".fbx")).Select(x => x.Substring(x.IndexOf("Assets"))).ToArray();
        List<AnimationClip> clipsInFolder = new List<AnimationClip>();
        foreach (string file in files)
        {
            AnimationClip clip1 = AssetDatabase.LoadAssetAtPath(file, typeof(AnimationClip)) as AnimationClip;
            AnimationClipSettings u = AnimationUtility.GetAnimationClipSettings(clip1);
            u.loopTime = true;
            AnimationClip clip_copy = Instantiate(clip1);
            AnimationUtility.SetAnimationClipSettings(clip_copy, u);
            AssetDatabase.CreateAsset(clip_copy, file.Substring(0, file.LastIndexOf('.')) + ".asset");
            clipsInFolder.Add(clip_copy);
        }
        return clipsInFolder;
    }

    private void drawTreeModeSelection(MixamoBlendTree script)
    {
        using (new GUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Mixamo Tree Type");
            script.blendMode = (MixamoBlendMode)EditorGUILayout.EnumPopup(script.blendMode);
            currentParamNum = script.blendMode.ToNumOfParams();
        }
    }

    private void drawClipsAndParameterNameField(MixamoBlendTree script)
    {
        serializedObject.Update();
        int treeDimension = script.blendMode.ToDimension();
        for(int i = 0; i < treeDimension; i++)
        {
            EditorGUILayout.PropertyField(ParameterNames.GetArrayElementAtIndex(i), new GUIContent($"Parameter {MixamoBlendTree.PARAM_DEFAULT_LETTERS[i]} Name"));
        }
        for (int i = 0; i < currentParamNum; i++)
        {
            //TODO: put this in a function
            string order = Convert.ToString(i, 2).PadLeft(treeDimension, '0');
            string result = "";
            for (int ii = 0; ii < order.Length; ii++)
            {
                result += "_" + MixamoBlendTree.PARAM_DEFAULT_LETTERS[ii] + order[ii];
            }
            EditorGUILayout.PropertyField(Clips.GetArrayElementAtIndex(i), new GUIContent("Clip" + result));
        }
        serializedObject.ApplyModifiedProperties();
    }


    [MenuItem("Assets/Generate Mixamo Blend Tree")]
    private static void GenerateMixamoBlendTree()
    {
        UnityEngine.Object[] allFolderObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        foreach (UnityEngine.Object folder in allFolderObjs)
        {
            string path = AssetDatabase.GetAssetPath(folder);
            string name = folder.name;

            List<AnimationClip> clipsInFolder = UnpackAllFbxInFolder(path);

            if (clipsInFolder.Count == 0) throw new Exception("No fbx found in given folder: " + path);

            int dimension = Convert.ToInt16(Math.Log(clipsInFolder.Count, 2));
            MixamoBlendMode mode = MixamoBlendModeExtension.DimensionToMode(dimension);
            string parentPath = Directory.GetParent(path).ToString();

            MixamoBlendTree tree = CreateInstance(typeof(MixamoBlendTree)) as MixamoBlendTree;
            tree.blendMode = mode;
            for (int i = 0; i < clipsInFolder.Count - 1; i++)
            {
                tree.Clips[i + 1] = clipsInFolder[i];
            }
            tree.Clips[0] = clipsInFolder[clipsInFolder.Count - 1];

            AssetDatabase.CreateAsset(tree, parentPath + "/" + name + ".asset");
        }
    }

    [MenuItem("Assets/Generate Mixamo Blend Tree", true)]
    private static bool GenerateMixamoBlendTreeValidate()
    {
        foreach(UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)){
            if (!Directory.Exists(AssetDatabase.GetAssetPath(obj))) return false;
            if (!obj is UnityEditor.DefaultAsset) return false;
        }
        return true;
    }

}
