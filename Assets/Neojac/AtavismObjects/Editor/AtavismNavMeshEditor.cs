using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Detour;
using UnityEditor;
using UnityEngine;
using System.Collections;
using Formatting = System.Xml.Formatting;

[CustomEditor(typeof(AtavismNavMesh))]
public class AtavismNavMeshEditor : Editor {

    private Vector2 _scrollPosition;
    private bool[] fold;
        
    /// <summary>
    /// Sets up the GUI so users can export the NavMesh data into XML or Binary formats
    /// </summary>
	public override void OnInspectorGUI()
	{
        var catagoryStyle = new GUIStyle();
        catagoryStyle.fontStyle = FontStyle.Bold;
		AtavismNavMesh recastNavMesh = target as AtavismNavMesh;
        if (recastNavMesh == null || recastNavMesh.NavMesh == null)
            return;
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false); //Last two bools are if we want to show the scroll bar always.

        EditorGUILayout.BeginVertical();
        /*if (fold == null)
        {
            fold = new bool[recastNavMesh.NavMesh._tiles.Length];
        }
        if (fold.Length != recastNavMesh.NavMesh._tiles.Length)
        {
            var temp = new bool[recastNavMesh.NavMesh._tiles.Length];
            Array.Copy(fold, 0, temp, 0, fold.Length);
            fold = temp;
        }
        EditorGUILayout.Separator();*/

        recastNavMesh.Mat = EditorGUILayout.ObjectField("Material", recastNavMesh.Mat, typeof(Material), true) as Material;
        if (GUILayout.Button("Toggle Geometry"))
        {
            recastNavMesh.Toggle();
        }

	    EditorGUILayout.Separator();
        //EditorGUILayout.PrefixLabel("Output", GUIStyle.none, catagoryStyle);
        if (GUILayout.Button("Export Full Nav Mesh"))
        {
			// Choose base filename
			string instance = EditorApplication.currentScene;
			string[] split = instance.Split(char.Parse("/"));
			instance = split[split.Length -1];
			split = instance.Split(char.Parse("."));
			instance = split[0];
			string filePath = Directory.GetCurrentDirectory() + "/NavMesh/" + instance + "/";
			
			FileInfo file = new FileInfo(filePath);
			file.Directory.Create(); // If the directory already exists, this method does nothing.
			var base_path = filePath + instance;

			var params_path = base_path + ".xml";
			
			// Export params
			XmlSerializer xmlSerializer_params = new XmlSerializer(typeof(Detour.NavMeshParams));
			
			if (params_path.Length > 0)
			{
				FileStream f = null;
				if (File.Exists(params_path))
				{
					File.Delete(params_path);
				}
				f = File.Create(params_path);
				xmlSerializer_params.Serialize(f,recastNavMesh.NavMesh.Param);
				f.Close();
			}
			
			// Builders, loop through all tiles
			for (int pc = 0; pc < recastNavMesh.Total; pc++)  //recastNavMesh.Total = total number of tiles I think, SR
			{
				XmlSerializer xml_single_build_serializer = new XmlSerializer(typeof(Detour.AtavismNavTile));
				int X_for_name = recastNavMesh.NavMesh._tiles[pc].Data.Header.X;
				int Y_for_name = recastNavMesh.NavMesh._tiles[pc].Data.Header.Y;
				String save_builder_path = base_path + "_tile" + X_for_name.ToString() + "_" + Y_for_name.ToString() + ".xml";
				
				if (save_builder_path.Length > 0)
				{
					FileStream f = null;
					if (File.Exists(save_builder_path))
					{
						File.Delete(save_builder_path);
					}
					f = File.Create(save_builder_path);
					xml_single_build_serializer.Serialize(f, recastNavMesh.NavMesh._tiles[pc].Data);
					f.Close();
				}
			}
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

	}
}
