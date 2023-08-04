using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
	//Data data;
	Map map;

	int xSize, ySize;
	public Vector2 chunkIndex;
	public Vector3 chunkPosition;
	
	public enum HexDirection { NE, S, SE, SW, W, NW }

	private void Awake()
	{
		//data = Data.Instance;
		map = Map.Instance;
		//DisplayChunk();
	}
	void Start()
	{
		xSize = map.chunkXSize;
		ySize = map.chunkYSize;
		//RenderMap();
	}

	//private Vector3[] vertices;
	private List<Vector3> vertices;
	private Mesh mesh;
	public GameObject labelPrefab;

	public void DisplayChunk()
    {
		Texture2D hMap = GameObject.Find("MapGenerator").GetComponent<HeightMapGenerator>().heightMaps[0];//Resources.Load("HeightMapTestTwo") as Texture2D;

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		//Bottom left section of the map, other sections are similar
		//Floor 128
		for (int i = 0; i < Mathf.CeilToInt(hMap.width / map.mapXSize) + 4.5f; i++)
		{
			for (int j = 0; j < Mathf.FloorToInt(hMap.height / map.mapYSize); j++)
			{
				//Add each new vertex in the plane
				//Floor 127
				verts.Add(new Vector3(i, hMap.GetPixel(i + ((int)chunkIndex.x * (Mathf.FloorToInt(hMap.width / map.mapXSize) - 1)), j + ((int)chunkIndex.y * (Mathf.FloorToInt(hMap.height / map.mapYSize) - 1))).grayscale * 10, j));
				//verts.Add(new Vector3(i / 2, hMap.GetPixel((int)(i * 2) + ((int)chunkIndex.x * (Mathf.FloorToInt(hMap.width / map.mapXSize) - 1)), (int)(j * 2) + ((int)chunkIndex.y * (Mathf.FloorToInt(hMap.height / map.mapYSize) - 1))).grayscale * 10, j / 2));
				//uvs.Add(new Vector2((i + (chunkIndex.x * 127)) / 512f, (j + (chunkIndex.y * 127))) / 512f);

				//if (chunkIndex.x == 0 && chunkIndex.y == 0) Debug.Log("UVS: " + uvs[j + (i * 127)]);
				//Skip if a new square on the plane hasn't been formed
				if (i == 0 || j == 0) continue;
				//Adds the index of the three vertices in order to make up each of the two tris
				//Floor 128

				tris.Add(128 * i + j); //Top right
				tris.Add(128 * i + j - 1); //Bottom right
				tris.Add(128 * (i - 1) + j - 1); //Bottom left - First triangle
				tris.Add(128 * (i - 1) + j - 1); //Bottom left 
				tris.Add(128 * (i - 1) + j); //Top left
				tris.Add(128 * i + j); //Top right - Second triangle

			}
		}

		//Vector2[] uvs = new Vector2[verts.Count];
		/*for (var i = 0; i < verts.Count; i++) //Give UV coords X,Z world coords
			uvs.Add(new Vector2(verts[i].x / 128, verts[i].z / 128));*/

					Mesh procMesh = new Mesh();
		procMesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
		//procMesh.uv = uvs.ToArray();
		procMesh.triangles = tris.ToArray();
		procMesh.RecalculateNormals(); //Determines which way the triangles are facing
		GetComponent<MeshFilter>().mesh = procMesh; //Assign Mesh object to MeshFilter


	}
	private void RenderMap()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		GetComponent<MeshCollider>().sharedMesh = mesh;

		//mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.name = "Chunk";
		mesh.Clear();

		//vertices = new Vector3[((xSize + 1) * (ySize + 1)) * 6];
		vertices = new List<Vector3>();
		//Vector2[] uv = new Vector2[vertices.Length];
		//Vector2[] uv2 = new Vector2[vertices.Length];
		List<int> triangles = new List<int>();
		List<Color> colors = new List<Color>();

		float height = 1;
		float y = 0;
		float x;
		float colNum = 0;
		   //The length of one flat side
		float offsetSide = (height / Mathf.Tan(60 * Mathf.Deg2Rad));
		   //The x length of the angled side on the left and right edges
		float offsetEdge = (height * 0.5f) * Mathf.Tan(30 * Mathf.Deg2Rad);

		float blendRegion = 0.1f;

		Color grassColor = Color.green;
		Color hillColor = new Color(0.3f, 0.3f, 0.3f);
		Color mountainColor = Color.gray;
		Color waterColor = Color.blue;

		Dictionary<string, Color> colorDict = new Dictionary<string, Color>();
		colorDict["water"] = Color.blue;
		colorDict["coast"] = new Color32(51, 196, 255, 255);
		colorDict["snow"] = new Color32(185, 235, 255, 255);
		colorDict["tundra"] = new Color32(180, 184, 205, 255);
		colorDict["desert"] = new Color32(223, 197, 76, 255);
		colorDict["grass"] = Color.green;
		colorDict["forest"] = new Color32(73, 156, 42, 255);
		colorDict["mountain"] = Color.gray;

		//mesh.subMeshCount = 2;
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.triangles = triangles.ToArray();
		//mesh.SetTriangles(triangles, 0);
		//mesh.SetTriangles(triangles, 1);
		mesh.RecalculateNormals();
		//mesh.uv = uv;
	}

	/* PERLIN NOISE CODE
		float sample = Mathf.PerlinNoise((float)((float)x / xSize), (float)((float)y / ySize));
		Debug.Log(sample);
		if (sample < 0.45f)
		{
			uv[i] = new Vector2(0.5f, 0.75f);
			uv[i + 1] = new Vector2(0.5f, 1);
			uv[i + 2] = new Vector2(0.75f, 1f);
			uv[i + 3] = new Vector2(0.75f, 0.75f);
		}
		else
		{
			uv[i] = new Vector2(0.75f, 0.75f);
			uv[i + 1] = new Vector2(0.75f, 1);
			uv[i + 2] = new Vector2(1f, 1f);
			uv[i + 3] = new Vector2(1f, 0.75f);
		}
	*/
}