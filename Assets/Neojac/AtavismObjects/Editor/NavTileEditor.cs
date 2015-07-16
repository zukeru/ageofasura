using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Detour;

[CustomEditor(typeof(NavTile))]
public class NavTileEditor: Editor {

	// Use this for initialization
	void Start () {
	
	}
	
	public override void OnInspectorGUI()
	{
		NavTile navTile = target as NavTile;
		AtavismNavMesh recastNavMesh = navTile.transform.parent.GetComponent<AtavismNavMesh>();
		if (GUILayout.Button("Remove Tile"))
		{
			recastNavMesh.RemoveTile(navTile.Builder.Header.X, navTile.Builder.Header.Y);
			recastNavMesh.BuildGeometry();
			DestroyImmediate (navTile.gameObject);
		}
		if (GUILayout.Button("Rebuild Tile"))
		{
			recastNavMesh.BuildTile(navTile.Builder.Header.X, navTile.Builder.Header.Y);
			recastNavMesh.BuildGeometry();
		}
		if (GUILayout.Button("Export Tile"))
		{
			// Choose base filename
			string instance = EditorApplication.currentScene;
			string[] split = instance.Split(char.Parse("/"));
			instance = split[split.Length -1];
			split = instance.Split(char.Parse("."));
			instance = split[0];
			string save_path_for_export = Directory.GetCurrentDirectory() + "/NavMesh/" + instance;
			var base_path = save_path_for_export + "/" + instance;
			//var path = EditorUtility.SaveFilePanel("Export NavMesh", save_path_for_export, EditorApplication.currentScene + ".xml", "xml");
			
			// Builders, loop through all tiles
			XmlSerializer xml_single_build_serializer = new XmlSerializer(typeof(Detour.AtavismNavTile));
			int X_for_name = navTile.Builder.Header.X;
			int Y_for_name = navTile.Builder.Header.Y;
			string save_builder_path = base_path + "_tile" + X_for_name.ToString() + "_" + Y_for_name.ToString() + ".xml";
				
			if (save_builder_path.Length > 0)
			{
				FileStream f = null;
				if (File.Exists(save_builder_path))
				{
					File.Delete(save_builder_path);
				}
				f = File.Create(save_builder_path);
				xml_single_build_serializer.Serialize(f, navTile.Builder);
				f.Close();
			}
		}
	}
}
