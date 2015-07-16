using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Detour;
using Recast;
using Recast.Data;

// Handles the Effects Configuration
public class ServerNavMesh : AtavismFunction
{

	private Config config;
	private NavMeshSettings settings;
	
	public GameObject InputMesh;
	public float BuildTime;
	
	public int Verts { get; set; }
	public int Tris { get; set; }
	
	public float resultDuration = 5;
	float resultTimeout;
	string result;

	// Use this for initialization
	public ServerNavMesh ()
	{	
		functionName = "Nav Mesh";		
		// Init
		settings = new NavMeshSettings();
		settings.CellSize = 0.50000f;
		settings.CellHeight = 0.2f;
		settings.AgentHeight = 2.0f;
		settings.AgentRadius = .3f;
		settings.AgentMaxClimb = 0.9f;
		settings.AgentMaxSlope = 45;
		settings.RegionMinSize = 6;
		settings.RegionMergeSize = 20;
		settings.EdgeMaxLen = 12;
		settings.EdgeMaxError = 1.5f;
		settings.VertsPerPoly = 6;
		settings.DetailSampleDist = 9;
		settings.DetailSampleMaxError = 2;
		settings.TileSize = 256;
	}
	
	public void NewResult(string resultMessage) {
		result = resultMessage;
		resultTimeout = Time.realtimeSinceStartup + resultDuration;
	}
	
	public override void Draw (Rect box)
	{
		
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		
		ImagePack.DrawLabel (pos.x, pos.y, "Generate Nav Mesh");
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "1) Choose the tag of the objects you want to create a NavMesh of.");
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		settings.Tags = ImagePack.DrawMaskField(pos, "Tags", settings.Tags, UnityEditorInternal.InternalEditorUtility.tags);
		pos.y += ImagePack.fieldHeight * 1.5f;
		pos.width *=2;
		ImagePack.DrawText(pos, "2) Click Generate to generate your NavMesh. It can take a few minutes.");
		pos.y += ImagePack.fieldHeight;
		if (ImagePack.DrawButton (pos.x, pos.y, "Generate")) {
			Geometry geom = BuildConfig(settings, out config);
			// create nav mesh object and build nav data (build all tiles)
			GameObject oldgo = GameObject.Find("AtavismNavMesh");
			if (oldgo != null)
				DestroyImmediate(oldgo);
			GameObject go = new GameObject("AtavismNavMesh");
			go.tag = "EditorOnly";
			go.AddComponent<AtavismNavMesh>();
			AtavismNavMesh navMesh = go.GetComponent<AtavismNavMesh>();
			navMesh.settings = settings;
			BuildTime = navMesh.BuildAllTiles(config, geom, settings.TileWidth, settings.TileHeight, settings.MaxPolysPerTile, settings.MaxTiles);
		}
		
		pos.y += ImagePack.fieldHeight * 1.5f;
		ImagePack.DrawText(pos, "3) After the NavMesh is generated select the AtavismNavMesh object");
		pos.y += ImagePack.fieldHeight * 0.8f;
		ImagePack.DrawText(pos, "in the scene and click Export. The files are located in the NavMesh");
		pos.y += ImagePack.fieldHeight * 0.8f;
		ImagePack.DrawText(pos, "folder in your project root. ");
		pos.y += ImagePack.fieldHeight * 1.5f;
		ImagePack.DrawText(pos, "4) Copy the scene folder to the NavMesh folder on your server.");
		pos.y += ImagePack.fieldHeight;
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, result);
		}

	}
	
	public static Geometry BuildConfig(NavMeshSettings settings, out Config config)
	{
		config = new Config()
		{
			CellSize = settings.CellSize,
			CellHeight = settings.CellHeight,
			WalkableSlopeAngle = settings.AgentMaxSlope,
			WalkableHeight = (int) Math.Ceiling(settings.AgentHeight/settings.CellHeight),
			WalkableClimb = (int) Math.Floor(settings.AgentMaxClimb/settings.CellHeight),
			WalkableRadius = (int) Math.Ceiling(settings.AgentRadius/settings.CellSize),
			MaxEdgeLength = (int) (settings.EdgeMaxLen/settings.CellSize),
			MaxSimplificationError = settings.EdgeMaxError,
			MinRegionArea = (int) (settings.RegionMinSize*settings.RegionMinSize),
			MergeRegionArea = (int) (settings.RegionMergeSize*settings.RegionMergeSize),
			MaxVertexesPerPoly = (int) settings.VertsPerPoly,
			DetailSampleDistance = settings.DetailSampleDist < 0.9 ? 0 : settings.CellSize*settings.DetailSampleDist,
			DetailSampleMaxError = settings.CellHeight*settings.DetailSampleMaxError,
			BorderSize = (int) Math.Ceiling(settings.AgentRadius/settings.CellSize) + 3,
			TileSize = settings.TileSize,
		};
		
		config.Width = config.TileSize + config.BorderSize*2;
		config.Height = config.TileSize + config.BorderSize*2;
		
		Geometry geom = new Geometry();
		BuildGeometry(settings, config, geom);
		BuildTileSizeData(settings, config, geom);
		return geom;
	}
	
	public static void BuildTileSizeData(NavMeshSettings settings, Config _config, Geometry _geom)
	{
		RecastVertex bmin = _geom.MinBounds;
		RecastVertex bmax = _geom.MaxBounds;
		
		int gw = 0, gh = 0;
		
		CalcGridSize(bmin, bmax, _config.CellSize, out gw, out gh);
		
		int ts = settings.TileSize;
		int tw = (gw + ts - 1)/ts;
		int th = (gh + ts - 1)/ts;
		
		settings.TileWidth = tw;
		settings.TileHeight = th;
		
		int tileBits = Math.Min(ilog2(nextPow2(th*tw)), 14);
		if (tileBits > 14)
			tileBits = 14;
		
		int polyBits = 22 - tileBits;
		settings.MaxTiles = 1 << tileBits;
		settings.MaxPolysPerTile = 1 << polyBits;
	}
	
	private static void CalcGridSize(RecastVertex bmin, RecastVertex bmax, float cellSize, out int w, out int h)
	{
		if (bmin != null && bmax != null)
		{
			w = (int) ((bmax.X - bmin.X)/cellSize + 0.5f);
			h = (int) ((bmax.Z - bmin.Z)/cellSize + 0.5f);
		}
		else
		{
			w = 0;
			h = 0;
		}
	}
	
	private static int nextPow2(int v)
	{
		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		v++;
		return v;
	}
	
	private static int ilog2(int v)
	{
		int r;
		int shift;
		
		r = ((v > 0xffff) ? 1 : 0) << 4;
		v >>= r;
		shift = ((v > 0xff) ? 1 : 0) << 3;
		v >>= shift;
		r |= shift;
		shift = ((v > 0xf) ? 1 : 0) << 2;
		v >>= shift;
		r |= shift;
		shift = ((v > 0x3) ? 1 : 0) << 1;
		v >>= shift;
		r |= shift;
		r |= (v >> 1);
		return r;
	}
	
	/// <summary>
	/// This takes the current geometry and builds the data to go into Recast
	/// It needs to be updated to take into account scale, position, and rotation
	/// It needs to be updated to look for specific tags
	/// </summary>
	/// <param name="geom"></param>
	private static void BuildGeometry(NavMeshSettings settings, Config config, Geometry geom)
	{
		for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
		{
			if ((settings.Tags & (1 << i)) != 0)
			{
				foreach (var gameObject in GameObject.FindGameObjectsWithTag(UnityEditorInternal.InternalEditorUtility.tags[i]))
				{
					foreach (var terrainObj in gameObject.GetComponentsInChildren<Terrain>())
					{
						var terrain = terrainObj.terrainData;
						var w = terrain.heightmapWidth;
						var h = terrain.heightmapHeight;
						var meshScale = terrain.size;
						var tRes = 1;
						meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
						var tData = terrain.GetHeights(0, 0, w, h);
						
						w = (w - 1) / tRes + 1;
						h = (h - 1) / tRes + 1;
						var tVertices = new Vector3[w * h];
						var tPolys = new int[(w - 1) * (h - 1) * 6];
						
						// Build vertices and UVs
						for (int y = 0; y < h; y++)
						{
							for (int x = 0; x < w; x++)
							{
								tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(x, tData[y * tRes, x * tRes], y)) + terrainObj.transform.position;
							}
						}
						
						var index = 0;
						// Build triangle indices: 3 indices into vertex array for each triangle
						for (int y = 0; y < h - 1; y++)
						{
							for (int x = 0; x < w - 1; x++)
							{
								// For each grid cell output two triangles
								tPolys[index++] = (y * w) + x;
								tPolys[index++] = ((y + 1) * w) + x;
								tPolys[index++] = (y * w) + x + 1;
								
								tPolys[index++] = ((y + 1) * w) + x;
								tPolys[index++] = ((y + 1) * w) + x + 1;
								tPolys[index++] = (y * w) + x + 1;
							}
						}
						int subTotalVerts = geom.NumVertexes;
						foreach (var tVertex in tVertices)
						{
							geom.Vertexes.Add(new RecastVertex(tVertex.x, tVertex.y, tVertex.z));
							geom.NumVertexes++;
						}
						for (int j = 0; j < tPolys.Length; j += 3)
						{
							geom.Triangles.Add(tPolys[j] + subTotalVerts);
							geom.Triangles.Add(tPolys[j + 1] + subTotalVerts);
							geom.Triangles.Add(tPolys[j + 2] + subTotalVerts);
							geom.NumTriangles++;
						}
					}
					foreach (var componentsInChild in gameObject.GetComponentsInChildren<MeshFilter>())
					{
						int subTotalVerts = geom.NumVertexes;
						foreach (Vector3 vector3 in componentsInChild.sharedMesh.vertices)
						{
							Vector3 vec = gameObject.transform.TransformPoint(vector3);
							geom.Vertexes.Add(new RecastVertex(vec.x, vec.y, vec.z));
							geom.NumVertexes++;
						}
						for (int j = 0; j < componentsInChild.sharedMesh.triangles.Length; j += 3)
						{
							geom.Triangles.Add(componentsInChild.sharedMesh.triangles[j] + subTotalVerts);
							geom.Triangles.Add(componentsInChild.sharedMesh.triangles[j + 1] + subTotalVerts);
							geom.Triangles.Add(componentsInChild.sharedMesh.triangles[j + 2] + subTotalVerts);
							geom.NumTriangles++;
						}
					}
					foreach (var offMeshConnector in gameObject.GetComponentsInChildren<OffMeshConnector>())
					{
						RecastVertex start = new RecastVertex(offMeshConnector.StartPosition.x, offMeshConnector.StartPosition.y, offMeshConnector.StartPosition.z);
						RecastVertex end = new RecastVertex(offMeshConnector.EndPosition.x, offMeshConnector.EndPosition.y, offMeshConnector.EndPosition.z);
						geom.AddOffMeshConnection(start, end, offMeshConnector.Radius, offMeshConnector.Bidirectional, 5, 8);
					}
				}
			}
		}
		
		if (geom.NumVertexes != 0)
		{
			geom.CalculateBounds();
			config.CalculateGridSize(geom);
			geom.CreateChunkyTriMesh();
		}
	}
	
}
