using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

class EditorServer
{
    static void Play()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
#endif
    }
}