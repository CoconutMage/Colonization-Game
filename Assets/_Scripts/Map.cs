using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	public static Map Instance { get; private set; }

	[SerializeField]
	public int mapXSize, mapYSize, chunkXSize, chunkYSize;
	//public int chunkIndex;

	public GameObject chunkPrefab;

	//Data data;
	Chunk chunkScript;
	//EarthMapValues emv;
	public float noiseOffsetX, noiseOffsetY;
	int[,] mapData;

	private void Start()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}
		Instance = this;

		//data = Data.Instance;
		//emv = EarthMapValues.Instance;

		noiseOffsetX = Random.Range(0, 9999);
		noiseOffsetY = Random.Range(0, 9999);

		mapData = new int[chunkXSize * mapXSize, chunkYSize * mapYSize];

		PerlinGenerate();
		//ReadCivMap();
		//ReadMapFile();
	}
	void PerlinGenerate()
	{
		float height = 1;
		//The length of one flat side
		float offsetSide = (height / Mathf.Tan(60 * Mathf.Deg2Rad));
		//The x length of the angled side on the left and right edges
		float offsetEdge = (height * 0.5f) * Mathf.Tan(30 * Mathf.Deg2Rad);
		int chunkIndex = 0;

		for (int w = 0; w < mapYSize; w++)
		{
			for (int z = 0; z < mapXSize; z++, chunkIndex++)
			{
				Vector3 chunkPos = new Vector3(z * 127, 0, w * 127);
				//PopulateChunkData(chunkIndex, chunkPos);

				float y = 0;
				float x;
				float colNum = 0;

				for (int i = 0, ti = 0, index = 0; y < chunkYSize; y++)
				{
					colNum = 0;
					for (x = 0; colNum < chunkXSize; x += offsetSide + offsetEdge, i += 25, ti += 12, index++)
					{
						if (colNum % 2 != 0) y += (0.5f * height);

						int r = Random.Range(0, 101);
						int tileType = -1;

						//Debug.Log("Perlin: " + Mathf.PerlinNoise(x / (float)xSize, y / (float)ySize));
						//Debug.Log("Index: " + chunkIndex + " : " + x + " : " + (xSize * (chunkIndex % data.map.xSize)) * (offsetSide + offsetEdge) + " : " + (float)(xSize * data.map.xSize * (offsetSide + offsetEdge)));

						//--------------------------------------------------------------------------------------------------------------------------- I think these cords are wrong

						//These numbers I pulled out of my ass, so edit for your pleasure. Except for coords and data.map size, dont edit those
						float xCord = x + (chunkXSize * (chunkIndex % mapXSize) * (offsetSide + offsetEdge)), yCord = y + (chunkYSize * (chunkIndex / mapXSize) * height);
						float mapSizeX = (float)(chunkXSize * mapXSize * (offsetSide + offsetEdge)), mapSizeY = (float)(chunkYSize * mapYSize * height);
						float offsetX = noiseOffsetX, offsetY = noiseOffsetY;
						//How many iterations of less impactfull noise functions are layered on
						int octave = 3;
						//How much detail is added for each octave. Basically increases the frequency for each successive octave
						int lacunarity = 2;
						//How bunched together the hills are. Think frequency of a sound wave
						float frequency = 5.25f;
						//How high and low the peaks and valleys are
						float amplitude = 8.5f;
						//How much less impactful each successive octave is. Basically reduces the amplitutude of each octave
						float persistance = 0.5f;
						float offsetZ = -2.5f;
						float perlinVal = 0;

						//-----------------------------------------------------------------------------

						for (int k = 0; k < octave; k++)
						{
							perlinVal += (Mathf.PerlinNoise((offsetX + xCord) / mapSizeX * frequency * (Mathf.Pow(lacunarity, k)), (offsetY + yCord) / mapSizeY * frequency * (Mathf.Pow(lacunarity, k)))) * Mathf.Pow(persistance, k);
						}

						perlinVal *= amplitude;
						perlinVal += offsetZ;
						//Debug.Log("Index: " + chunkIndex + " : " + x + " : " + perlinVal);

						if (perlinVal <= 4.25f) tileType = 0;// "water";
						else if (perlinVal > 4.25f && perlinVal <= 5.25f) tileType = 1;//"grass";
						else if (perlinVal > 5.25f && perlinVal <= 7.0f) tileType = 2;// "hill";
						else tileType = 3;//"mountain";

						if (colNum % 2 != 0) y -= (0.5f * height);

						//tileType = 1;

						mapData[(int)colNum + (chunkXSize * (chunkIndex % mapXSize)), (int)(y + (chunkYSize * (chunkIndex / mapXSize)))] = tileType;

						colNum++;
					}
				}

				//Physical Characteristics
				/*GameObject chunk = Instantiate(chunkPrefab);
				chunk.transform.parent = transform;
				chunk.transform.localPosition = chunkPos;

				//Data Management
				chunkScript = chunk.GetComponent<Chunk>();
				chunkScript.chunkIndex = new Vector2(z, w);
				chunkScript.chunkPosition = chunkPos;
				chunkScript.DisplayChunk();*/
			}
		}
		GetComponent<HeightMapGenerator>().GenerateHeightMap(mapData);
		/*for (int w = 0; w < mapYSize; w++)
		{
			for (int z = 0; z < mapXSize; z++, chunkIndex++)
			{
				Vector3 chunkPos = new Vector3(z * 128f, 0, w * 127);
				//PopulateChunkData(chunkIndex, chunkPos);

				//Physical Characteristics
				GameObject chunk = Instantiate(chunkPrefab);
				chunk.transform.parent = transform;
				chunk.transform.localPosition = chunkPos;

				//Data Management
				chunkScript = chunk.GetComponent<Chunk>();
				chunkScript.chunkIndex = new Vector2(z, w);
				chunkScript.chunkPosition = chunkPos;
				chunkScript.DisplayChunk();
			}
		}*/
	}
	void ReadCivMap()
	{
		float height = 1;
		//The length of one flat side
		float offsetSide = (height / Mathf.Tan(60 * Mathf.Deg2Rad));
		//The x length of the angled side on the left and right edges
		float offsetEdge = (height * 0.5f) * Mathf.Tan(30 * Mathf.Deg2Rad);
		int chunkIndex = 0;

		for (int w = 0; w < mapYSize; w++)
		{
			for (int z = 0; z < mapXSize; z++, chunkIndex++)
			{
				Vector2 chunkPos = new Vector2(z * chunkXSize * (offsetSide + offsetEdge), w * chunkYSize * height);
				//PopulateChunkData(chunkIndex, chunkPos);

				float y = 0;
				float x;
				float colNum = 0;

				for (int i = 0, ti = 0, index = 0; y < chunkYSize; y++)
				{
					colNum = 0;
					for (x = 0; colNum < chunkXSize; x += /*offsetSide + offsetEdge*/ 1, i += 25, ti += 12, index++)
					{
						//int r = Random.Range(0, 101);
						string tileType = "";

						//These numbers I pulled out of my ass, so edit for your pleasure. Except for coords and data.map size, dont edit those
						float xCord = x + (chunkXSize * (chunkIndex % mapXSize))/* * (offsetSide + offsetEdge))*/, yCord = y + (chunkYSize * (chunkIndex / mapXSize)/* * height*/);

						/*if (emv.tiles[(int)xCord, (int)yCord] == 16) tileType = "water";
						else if (emv.tiles[(int)xCord, (int)yCord] == 15) tileType = "coast";
						else if (emv.tiles[(int)xCord, (int)yCord] == 14) tileType = "snow";
						else if (emv.tiles[(int)xCord, (int)yCord] == 13) tileType = "snow";
						else if (emv.tiles[(int)xCord, (int)yCord] == 12) tileType = "snow";
						else if (emv.tiles[(int)xCord, (int)yCord] == 11) tileType = "tundra";
						else if (emv.tiles[(int)xCord, (int)yCord] == 10) tileType = "tundra";
						else if (emv.tiles[(int)xCord, (int)yCord] == 9) tileType = "tundra";
						else if (emv.tiles[(int)xCord, (int)yCord] == 8) tileType = "desert";
						else if (emv.tiles[(int)xCord, (int)yCord] == 7) tileType = "desert";
						else if (emv.tiles[(int)xCord, (int)yCord] == 6) tileType = "desert";
						else if (emv.tiles[(int)xCord, (int)yCord] == 5) tileType = "forest";
						else if (emv.tiles[(int)xCord, (int)yCord] == 4) tileType = "forest";
						else if (emv.tiles[(int)xCord, (int)yCord] == 3) tileType = "forest";
						else tileType = "grass";*/

						//PopulateChunkTileData(chunkIndex, index, new Vector2(0, 0), tileType);
						colNum++;
					}
				}

				//Physical Characteristics
				GameObject chunk = Instantiate(chunkPrefab);
				chunk.transform.parent = transform;
				chunk.transform.localPosition = chunkPos;

				//Data Management
				chunkScript = chunk.GetComponent<Chunk>();
				//chunkScript.chunkIndex = chunkIndex;
				chunkScript.chunkPosition = chunkPos;
			}
		}
	}
}
