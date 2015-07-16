using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RecastNavMeshQuery))]
public class RecastNavMeshQueryEditor : Editor
{
    private Vector2 _scrollPosition;

    public override void OnInspectorGUI()
    {
        var catagoryStyle = new GUIStyle();
        catagoryStyle.fontStyle = FontStyle.Bold;

		RecastNavMeshQuery recastNavMeshQuery = target as RecastNavMeshQuery;

        EditorGUILayout.PrefixLabel("Debug", GUIStyle.none, catagoryStyle);
        recastNavMeshQuery.StartPosition = EditorGUILayout.ObjectField("Starting Object", recastNavMeshQuery.StartPosition, typeof(GameObject), true) as GameObject;
        recastNavMeshQuery.EndPosition = EditorGUILayout.ObjectField("Ending Object", recastNavMeshQuery.EndPosition, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Draw Path"))
        {
            recastNavMeshQuery.Initialize();
            recastNavMeshQuery.SmoothGeneratedPath();
        }

        EditorGUILayout.LabelField("Points on Path", ""+recastNavMeshQuery.SmoothPathNum);

    }

}