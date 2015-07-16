using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Detour;
using Recast;
using Recast.Data;
using RecastNavCSharp.Recast.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using NavMesh = Detour.NavMesh;

public class NavMeshSettings {
	public int Tags;
	
	public float CellSize;
	public float CellHeight;
	public float AgentHeight;
	public float AgentRadius;
	public float AgentMaxClimb;
	public float EdgeMaxError;
	
	public int AgentMaxSlope;
	public int RegionMinSize;
	public int RegionMergeSize;
	public int EdgeMaxLen;
	public int VertsPerPoly;
	public int DetailSampleDist;
	public int DetailSampleMaxError;
	
	public int TileSize;
	private int OldTileSize;
	public int TileWidth;
	public int TileHeight;
	public int MaxTiles;
	public int MaxPolysPerTile;
}

public class RecastNavTile {
	public int x;
	public int y;
	public Detour.AtavismNavTile builder;
	public List<Vector3> NavMeshVerts = new List<Vector3>();
	public List<int> NavMeshTriangles = new List<int>();
	public List<Color> NavMeshColors = new List<Color>();
	public List<Vector2> NavMeshUVs = new List<Vector2>();
	
	public GameObject MeshObject;
	public MeshFilter MeshFilter;
	public MeshRenderer MeshRenderer;
	public Mesh RecastMesh;
	public Material Mat { get; set; }
}

[AddComponentMenu("Recast/NavMesh")]
[ExecuteInEditMode]
[Serializable]
public class AtavismNavMesh : MonoBehaviour {

	public NavMeshSettings settings;

    public bool IsBuilding = false;
    public int Progress = 0;
    public int Total = 0;

    public NavMesh NavMesh { get; set; }
    public Material Material { get; set; }

    public List<Vector3> NavMeshVerts { get; set; }
    public List<int> NavMeshTriangles { get; set; }
    public List<Color> NavMeshColors { get; set; }
    public List<Vector2> NavMeshUVs { get; set; }

    public float AgentHeight;
    public float AgentRadius;
    public float AgentMaxClimb;

    public bool MeshActive = false;

    public GameObject MeshObject;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;
    public Mesh RecastMesh;
    public Material Mat { get; set; }

    public Config Config { get; set; }
    public Geometry Geometry { get; set; }

    private int TileWidth { get; set; }
    private int TileHeight { get; set; }

    public int NumTiles { get { return NavMesh._tiles.Length; } }
    
	List<RecastNavTile> recastNavTiles = new List<RecastNavTile>();


    // Multi threading values
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    TaskScheduler scheduler = TaskScheduler.Default;
    List<Task> tasks = new List<Task>();


    // Use this for initialization
    void Start ()
    {
        Mat = Resources.Load("vertexMat") as Material;
        if (MeshObject != null && MeshObject.GetComponent<MeshRenderer>() != null)
        {
            MeshObject.GetComponent<MeshRenderer>().materials = new []{Mat};
        }
    }

    int bit(int a, int b)
    {
        return (a & (1 << b)) >> b;
    }

    Color duIntToCol(int i, int a)
    {
        int r = bit(i, 1) + bit(i, 3) * 2 + 1;
        int g = bit(i, 2) + bit(i, 4) * 2 + 1;
        int b = bit(i, 0) + bit(i, 5) * 2 + 1;
        return duRGBA(r * 63, g * 63, b * 63, a);
    }

    void OnDestroy()
    {
        DestroyImmediate(RecastMesh);
        if (MeshObject != null && MeshObject.GetComponent<MeshRenderer>() != null)
        {
            MeshObject.GetComponent<MeshRenderer>().materials = new[] { Mat };
        }
    }

    public void BuildGeometry()
    {
        BuildNavMeshGeometry();
        DrawNavMeshGeometry();
    }

    public void RebuildTiles()
    {
        Progress = 0;
        IsBuilding = true;
        RecastVertex bmin = Geometry.MinBounds;
        RecastVertex bmax = Geometry.MaxBounds;
        RecastVertex tileBMin = new RecastVertex();
        RecastVertex tileBMax = new RecastVertex();
        float tcs = Config.TileSize * Config.CellSize;
        Total = TileWidth * TileHeight;
        for (int y = 0; y < TileHeight; y++)
        {
            for (int x = 0; x < TileWidth; x++)
            {
                Progress = y * TileWidth + x;
                tileBMin.X = bmin.X + x * tcs;
                tileBMin.Y = bmin.Y;
                tileBMin.Z = bmin.Z + y * tcs;

                tileBMax.X = bmin.X + (x + 1) * tcs;
                tileBMax.Y = bmax.Y;
                tileBMax.Z = bmin.Z + (y + 1) * tcs;
				#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Generating...", "Generating Tile " + Progress + " of " + Total, Progress / (float)Total);
                #endif
                var builder = BuildTileMesh(x, y, tileBMin, tileBMax);

                // remove/add new tile?
                if (builder != null)
                {
					Detour.AtavismNavTile outBuilder;
                    // nav mesh remove tile
                    NavMesh.RemoveTile(NavMesh.GetTileRefAt(x, y, 0), out outBuilder);
                    // nav mesh add tile
                    long result = 0;
                    NavMesh.AddTile(builder, NavMesh.TileFreeData, 0, ref result);
                }
            }
        }
		#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
		#endif
        IsBuilding = false;
        BuildGeometry();
    }

    /// <summary>
    /// Builds the entire NavMesh from the Data gathered by BuildGeometry through the Detail Mesh
    /// Then it creates a GameObject that has the RecastNavMesh.
    /// </summary>
    /// <returns></returns>
    public long BuildAllTiles(Config config, Geometry geom, int tileWidth, int tileHeight, int maxPolysPerTile, int maxTiles)
    {
        NavMesh = new Detour.NavMesh();
        NavMeshParams param = new NavMeshParams()
                                  {
                                      Orig = geom.MinBounds.ToArray(),
                                      MaxPolys = maxPolysPerTile,
                                      MaxTiles = maxTiles,
                                      TileWidth = config.TileSize*config.CellSize,
                                      TileHeight = config.TileSize*config.CellSize

                                  };
        NavMesh.Init(param);
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Config = config;
        Geometry = geom;
        Progress = 0;
        IsBuilding = true;
        Stopwatch timer = new Stopwatch();
        timer.Start();
        RecastVertex bmin = geom.MinBounds;
        RecastVertex bmax = geom.MaxBounds;
        float tcs = config.TileSize * config.CellSize;
        Total = TileWidth * TileHeight;
        bool canceled = false;
        for (int y = 0; y < TileHeight; y++)
        {
            YLoop(y, tcs, bmin, bmax);
        }

		#if UNITY_EDITOR
        if (!canceled)
        {
            while (Progress != Total)
            {
            
                canceled = EditorUtility.DisplayCancelableProgressBar("Generating...",
                                                                      "Generating Tile " + Progress + " of " + Total,
                                                                      Progress/(float) Total);
                if (canceled)
                {
                    tokenSource.Cancel();
                    break;
                }
            }
        }

        Task.WaitAll(tasks.ToArray());
        timer.Stop();

        EditorUtility.ClearProgressBar();
		#endif
        IsBuilding = false;
        BuildGeometry();
        return timer.ElapsedMilliseconds;
    }

    private void YLoop(int y, float tcs, RecastVertex bmin, RecastVertex bmax)
    {
        bool canceled = false;
		#if UNITY_EDITOR
        for (int x = 0; x < TileWidth; x++)
        {
            canceled = EditorUtility.DisplayCancelableProgressBar("Generating...",
                                                                  "Generating Tile " + Progress + " of " + Total,
                                                                  Progress / (float)Total);
            if (canceled)
            {
                tokenSource.Cancel();
                break;
            }
            Xloop(y, x, tcs, bmin, bmax);
        }
        #endif
    }

    private void Xloop(int y, int x, float tcs, RecastVertex bmin, RecastVertex bmax)
    {
        RecastVertex tileBMin = new RecastVertex();
        RecastVertex tileBMax = new RecastVertex();
        tileBMin.X = bmin.X + x*tcs;
        tileBMin.Y = bmin.Y;
        tileBMin.Z = bmin.Z + y*tcs;

        tileBMax.X = bmin.X + (x + 1)*tcs;
        tileBMax.Y = bmax.Y;
        tileBMax.Z = bmin.Z + (y + 1)*tcs;
        #if UNITY_EDITOR
        bool canceled = EditorUtility.DisplayCancelableProgressBar("Generating...",
                                                              "Generating Tile " + Progress + " of " + Total,
                                                              Progress / (float)Total);
        if (canceled)
        {
            tokenSource.Cancel();
        }
        #endif
        var t = Task.Factory.StartNew(() => BuildTile(x, y, tileBMin, tileBMax), tokenSource.Token, TaskCreationOptions.LongRunning, scheduler);
        tasks.Add(t);
    }

    public void BuildTile(int x, int y, RecastVertex tileBMin, RecastVertex tileBMax)
    {
            var builder = BuildTileMesh(x, y, tileBMin, tileBMax);

            // remove/add new tile?
            if (builder != null)
            {
                lock (this)
                {
					Detour.AtavismNavTile outBuilder;
                    // nav mesh remove tile
                    NavMesh.RemoveTile(NavMesh.GetTileRefAt(x, y, 0), out outBuilder);
                    // nav mesh add tile
                    long result = 0;
                    NavMesh.AddTile(builder, NavMesh.TileFreeData, 0, ref result);
                }
            }
            Progress++;

    }

    public void BuildTile(int x, int y)
    {
        RecastVertex bmin = Geometry.MinBounds;
        RecastVertex bmax = Geometry.MaxBounds;
        float tcs = Config.TileSize * Config.CellSize;
        RecastVertex tileBMin = new RecastVertex();
        RecastVertex tileBMax = new RecastVertex();
        tileBMin.X = bmin.X + x * tcs;
        tileBMin.Y = bmin.Y;
        tileBMin.Z = bmin.Z + y * tcs;

        tileBMax.X = bmin.X + (x + 1) * tcs;
        tileBMax.Y = bmax.Y;
        tileBMax.Z = bmin.Z + (y + 1) * tcs;

        var builder = BuildTileMesh(x, y, tileBMin, tileBMax);

        // remove/add new tile?
        if (builder != null)
        {
			Detour.AtavismNavTile outBuilder;
            // nav mesh remove tile
            NavMesh.RemoveTile(NavMesh.GetTileRefAt(x, y, 0), out outBuilder);
            // nav mesh add tile
            long result = 0;
            NavMesh.AddTile(builder, NavMesh.TileFreeData, 0, ref result);
        }
    }

    public void RemoveTile(int x, int y)
    {
		Detour.AtavismNavTile outBuilder;
        // nav mesh remove tile
        NavMesh.RemoveTile(NavMesh.GetTileRefAt(x, y, 0), out outBuilder);
    }

	private Detour.AtavismNavTile BuildTileMesh(int tx, int ty, RecastVertex min, RecastVertex max)
    {
        Config.Width = Config.TileSize + Config.BorderSize * 2;
        Config.Height = Config.TileSize + Config.BorderSize * 2;
        Config.MinBounds = min;
        Config.MaxBounds = max;
        Config.MinBounds.X -= Config.BorderSize * Config.CellSize;
        Config.MinBounds.Z -= Config.BorderSize * Config.CellSize;

        Config.MaxBounds.X += Config.BorderSize * Config.CellSize;
        Config.MaxBounds.Z += Config.BorderSize * Config.CellSize;

        HeightField heightfield = new HeightField(Config.Width, Config.Height, Config.MinBounds.ToArray(), Config.MaxBounds.ToArray(), Config.CellSize, Config.CellHeight);


        short[] triAreas = new short[Geometry.ChunkyTriMesh.MaxTrisPerChunk];

        float[] tbmin = new float[2], tbmax = new float[2];
        tbmin[0] = Config.MinBounds.X;
        tbmin[1] = Config.MinBounds.Z;

        tbmax[0] = Config.MaxBounds.X;
        tbmax[1] = Config.MaxBounds.Z;

        int[] cid = new int[512];

        int ncid = Geometry.ChunkyTriMesh.GetChunksOverlappingRect(tbmin, tbmax, ref cid, 512);

        if (ncid == 0)
            return null;

        for (int i = 0; i < ncid; i++)
        {
            ChunkyTriMeshNode node = Geometry.ChunkyTriMesh.Nodes[cid[i]];
            int[] tris = new int[node.n * 3];
            Array.Copy(Geometry.ChunkyTriMesh.Tris, node.i * 3, tris, 0, node.n * 3);
            List<int> ctris = new List<int>(tris);
            int nctris = node.n;

            Array.Clear(triAreas, 0, triAreas.Length);
            Geometry.MarkWalkableTriangles(Config.WalkableSlopeAngle, ctris, nctris, ref triAreas);

            heightfield.RasterizeTriangles(Geometry, ctris, nctris, triAreas, Config.WalkableClimb);
        }

        heightfield.FilterLowHangingWalkableObstacles(Config.WalkableClimb);
        heightfield.FilterLedgeSpans(Config.WalkableHeight, Config.WalkableClimb);
        heightfield.FilterWalkableLowHeightSpans(Config.WalkableHeight);


        CompactHeightfield compactHeightfield = new CompactHeightfield(Config.WalkableHeight, Config.WalkableClimb, heightfield);
        compactHeightfield.ErodeWalkableArea(Config.WalkableRadius);

        // optional convex volumes

        compactHeightfield.BuildDistanceField();
        compactHeightfield.BuildRegions(Config.BorderSize, Config.MinRegionArea, Config.MergeRegionArea);


        ContourSet contourSet = new ContourSet(compactHeightfield, Config.MaxSimplificationError, Config.MaxEdgeLength);

        if (contourSet.NConts == 0)
            return null;

        PolyMesh polyMesh = new PolyMesh(contourSet, Config.MaxVertexesPerPoly);

        DetailPolyMesh detailPolyMesh = new DetailPolyMesh(polyMesh, compactHeightfield, Config.DetailSampleDistance,
                                                            Config.DetailSampleMaxError);

        // Convert the Areas and Flags for path weighting
        for (int i = 0; i < polyMesh.NPolys; i++)
        {

            if (polyMesh.Areas[i] == Geometry.WalkableArea)
            {
                polyMesh.Areas[i] = 0; // Sample_polyarea_ground
                polyMesh.Flags[i] = 1; // Samply_polyflags_walk
            }
        }
        NavMeshCreateParams param = new NavMeshCreateParams
        {
            Verts = polyMesh.Verts,
            VertCount = polyMesh.NVerts,
            Polys = polyMesh.Polys,
            PolyAreas = polyMesh.Areas,
            PolyFlags = polyMesh.Flags,
            PolyCount = polyMesh.NPolys,
            Nvp = polyMesh.Nvp,
            DetailMeshes = detailPolyMesh.Meshes,
            DetailVerts = detailPolyMesh.Verts,
            DetailVertsCount = detailPolyMesh.NVerts,
            DetailTris = detailPolyMesh.Tris,
            DetailTriCount = detailPolyMesh.NTris,

            // Off Mesh data
            OffMeshConVerts = Geometry.OffMeshConnectionVerts.ToArray(),
            OffMeshConRad = Geometry.OffMeshConnectionRadii.ToArray(),
            OffMeshConDir = Geometry.OffMeshConnectionDirections.ToArray(),
            OffMeshConAreas = Geometry.OffMeshConnectionAreas.ToArray(),
            OffMeshConFlags = Geometry.OffMeshConnectionFlags.ToArray(),
            OffMeshConUserId = Geometry.OffMeshConnectionIds.ToArray(),
            OffMeshConCount = (int)Geometry.OffMeshConnectionCount,
            // end off mesh data

            WalkableHeight = Config.WalkableHeight,
            WalkableRadius = Config.WalkableRadius,
            WalkableClimb = Config.WalkableClimb,
            BMin = new float[] { polyMesh.BMin[0], polyMesh.BMin[1], polyMesh.BMin[2] },
            BMax = new float[] { polyMesh.BMax[0], polyMesh.BMax[1], polyMesh.BMax[2] },
            Cs = polyMesh.Cs,
            Ch = polyMesh.Ch,
            BuildBvTree = true,
            TileX = tx,
            TileY = ty,
            TileLayer = 0
        };
		return new Detour.AtavismNavTile(param);
    }

    #region NavMesh Geometry Functions

    /// <summary>
    /// Builds the NavMesh geometry into a Unity3d Mesh to display for debug
    /// </summary>
    public void BuildNavMeshGeometry()
    {
    	foreach(RecastNavTile navTile in recastNavTiles) {
			DestroyImmediate(navTile.MeshObject);
    	}
    	recastNavTiles.Clear();
        if (NavMesh != null)
        {
            NavMeshTriangles = new List<int>();
            NavMeshVerts = new List<Vector3>();
            NavMeshUVs = new List<Vector2>();
            NavMeshColors = new List<Color>();
            for (int i = 0; i < NavMesh.GetMaxTiles(); i++)
            {
                MeshTile tile = NavMesh.GetTile(i);
                if (tile.Header == null) 
                	continue;
				RecastNavTile navTile = new RecastNavTile();
                long baseId = NavMesh.GetPolyRefBase(tile);
                for (int j = 0; j < tile.Header.PolyCount; j++)
                {
					BuildNavMeshPoly(baseId | (uint)j, duRGBA(0, 0, 64, 128), NavMeshVerts, NavMeshColors, NavMeshUVs, NavMeshTriangles, navTile);
                }
                navTile.x = tile.Header.X;
				navTile.y = tile.Header.Y;
				navTile.builder = tile.Data;
				recastNavTiles.Add(navTile);
            }
        }
    }

    /// <summary>
    /// Builds a Single polygon out of the NavMesh, called by BuildNavMeshGeometry
    /// </summary>
    /// <param name="refId"></param>
    /// <param name="color"></param>
    /// <param name="verts"></param>
    /// <param name="colors"></param>
    /// <param name="uvs"></param>
    /// <param name="tris"></param>
    private void BuildNavMeshPoly(long refId, Color color, List<Vector3> verts, List<Color> colors, List<Vector2> uvs, List<int> tris, RecastNavTile navTile)
    {

        MeshTile tile = null;
        Poly poly = null;
        if ((NavMesh.GetTileAndPolyByRef(refId, ref tile, ref poly) & Status.Failure) != 0)
            return;

        long ip = 0;
        for (int i = 0; i < tile.Polys.Length; i++)
        {
            if (poly == tile.Polys[i])
                ip = i;
        }
		if (poly.Type == Detour.AtavismNavTile.PolyTypeOffMeshConnection)
        {
        }
        else
        {
            PolyDetail pd = tile.DetailMeshes[ip];
            for (int i = 0; i < pd.TriCount; i++)
            {
                int t = ((int)pd.TriBase + i) * 4;
                for (int j = 0; j < 3; j++)
                {
                    if (tile.DetailTris[t + j] < poly.VertCount)
                    {
						Vector3 newVerts = new Vector3(tile.Verts[poly.Verts[tile.DetailTris[t + j]] * 3 + 0], tile.Verts[poly.Verts[tile.DetailTris[t + j]] * 3 + 1], tile.Verts[poly.Verts[tile.DetailTris[t + j]] * 3 + 2]);
						verts.Add(newVerts);
						navTile.NavMeshVerts.Add(newVerts);
                    }
                    else
                    {
						Vector3 newVerts = new Vector3(tile.DetailVerts[(pd.VertBase + tile.DetailTris[t + j] - poly.VertCount) * 3 + 0],
						                              tile.DetailVerts[(pd.VertBase + tile.DetailTris[t + j] - poly.VertCount) * 3 + 1],
						                              tile.DetailVerts[(pd.VertBase + tile.DetailTris[t + j] - poly.VertCount) * 3 + 2]);
						verts.Add(newVerts);
						navTile.NavMeshVerts.Add(newVerts);
                    }
                    uvs.Add(new Vector2());
					navTile.NavMeshUVs.Add(new Vector2());
                    colors.Add(color);//duIntToCol((int)ip, 192));
                    navTile.NavMeshColors.Add(color);
                    tris.Add(tris.Count);
					navTile.NavMeshTriangles.Add(navTile.NavMeshTriangles.Count);
                }
            }
        }

    }

    /// <summary>
    /// Converts an RGBA of intgers into a Unity3d Color
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    Color duRGBA(int r, int g, int b, int a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <summary>
    /// Creates a Mesh Object as a child of this object and builds the mesh from the data built by BuildNavMeshGeometry
    /// </summary>
    public void DrawNavMeshGeometry()
    {
        
        foreach(RecastNavTile navTile in recastNavTiles) {
			navTile.MeshObject = new GameObject("Mesh_" + navTile.x + "_" + navTile.y);
			navTile.MeshObject.transform.parent = this.transform;
			navTile.MeshObject.AddComponent<MeshFilter>();
			navTile.MeshObject.AddComponent<MeshRenderer>();
			navTile.MeshObject.AddComponent<NavTile>();
			
			navTile.MeshFilter = navTile.MeshObject.GetComponent<MeshFilter>();
			navTile.RecastMesh = new Mesh();
			navTile.RecastMesh.vertices = navTile.NavMeshVerts.ToArray();
			navTile.RecastMesh.triangles = navTile.NavMeshTriangles.ToArray();
			navTile.RecastMesh.colors = navTile.NavMeshColors.ToArray();
			navTile.RecastMesh.uv = navTile.NavMeshUVs.ToArray();
			navTile.RecastMesh.RecalculateNormals();
			navTile.MeshFilter.sharedMesh = navTile.RecastMesh;
			navTile.MeshObject.GetComponent<MeshRenderer>().sharedMaterial = Mat;
			
			navTile.MeshObject.GetComponent<NavTile>().Builder = navTile.builder;
        }
		MeshActive = true;
    }

    public void Toggle()
    {
		MeshActive = !MeshActive;
		foreach(RecastNavTile navTile in recastNavTiles) {
			navTile.MeshObject.SetActive(MeshActive);
		}
    }

    #endregion

}
