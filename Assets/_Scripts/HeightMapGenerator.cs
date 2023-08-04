using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightMapGenerator : MonoBehaviour
{
    public List<Texture2D> heightMaps;
    public Texture2D blankHex;
    public Texture2D mountainHex;
    public Texture2D hillHex;
    public void GenerateHeightMap(int[,] mapData)
    {
        float height = 64;
        int width = 74;
        //The length of one flat side
        float offsetSide = (height / Mathf.Tan(60 * Mathf.Deg2Rad));
        //The x length of the angled side on the left and right edges
        float offsetEdge = (height * 0.5f) * Mathf.Tan(30 * Mathf.Deg2Rad);
        //Vector2 heightMapSize = new Vector2(518, 512);
        Vector2 heightMapSize = new Vector2(1016, 1024);
        //18x16 tilemaps
        //450x272
        //25x17 num of tilemaps
        heightMaps = new List<Texture2D>();

        for (int j = 0; j < mapData.GetLength(1) / 16; j++)
        {
            for (int i = 0; i < mapData.GetLength(0) / 18; i++)
            {
                Color32[] solidBlack = new Color32[(int)(heightMapSize.x * heightMapSize.y)];
                for (int w = 0; w < solidBlack.Length; w++) solidBlack[w] = Color.black;
                heightMaps.Add(new Texture2D((int)heightMapSize.x, (int)heightMapSize.y));
                heightMaps[i + (j * mapData.GetLength(0) / 18)].SetPixels32(solidBlack, 0);
                for (int y = 0; y < /*Map.Instance.mapYSize * Map.Instance.chunkYSize*/Mathf.FloorToInt(heightMaps[i + (j * mapData.GetLength(0) / 18)].height / height); y++)
                {
                    //if (y == 1) break;
                    float colNum = 0;
                    for (int x = 0; x < /*Map.Instance.mapXSize * Map.Instance.chunkXSize*/Mathf.FloorToInt(heightMaps[i + (j * mapData.GetLength(0) / 18)].width / (offsetEdge + offsetSide)) + 1; x++)
                    {
                        float yPos = y * height;
                        if (colNum % 2 != 0)
                        {
                            yPos += (0.5f * height);
                        }
                        //float xPos = x * (offsetSide + offsetEdge), yPos = (height / 2) * y;

                        if (x + (i * 18) == mapData.GetLength(0)) break;

                        int mapDataAtPixel;// = mapData[x + (i * 18), y + (j * 16)];
                        if (j != 0 && y == 0 && colNum % 2 == 1)
                        {
                            mapDataAtPixel = mapData[x + (i * 18), y + ((j - 1) * 16) + 15];
                            if (mapDataAtPixel != 0)
                            {
                                yPos = -(0.5f * height);
                                for (int w = 0; w < blankHex.height; w++)
                                {
                                    for (int z = 0; z < blankHex.width; z++)
                                    {
                                        if (blankHex.GetPixel(z, w).a != 0)
                                        {
                                            if (heightMaps[i + (j * mapData.GetLength(0) / 18)].width < z + Mathf.CeilToInt(x * (offsetEdge + offsetSide))) break;
                                            Color c = blankHex.GetPixel(z, w);
                                            if (mapDataAtPixel == 3)
                                            {
                                                Color d = mountainHex.GetPixel(z, w);// - new Color(0.1f, 0.1f, 0.1f, 0);
                                                c += (d * d.a);
                                            }
                                            else if (mapDataAtPixel == 2)
                                            {
                                                Color d = hillHex.GetPixel(z, w);// - new Color(0.1f, 0.1f, 0.1f, 0);
                                                c += (d * d.a);
                                            }
                                            heightMaps[i + (j * mapData.GetLength(0) / 18)].SetPixel(z + Mathf.CeilToInt(x * (offsetEdge + offsetSide)), w + (int)yPos, c);
                                        }
                                    }
                                    if (w + (int)yPos + 2 < 0) continue;
                                }
                                yPos = y * height;
                                yPos += (0.5f * height);
                            }
                        }

                        mapDataAtPixel = mapData[x + (i * 18), y + (j * 16)];
                        if (mapDataAtPixel != 0)
                        {
                            for (int w = 0; w < blankHex.height; w++)
                            {
                                for (int z = 0; z < blankHex.width; z++)
                                {
                                    if (blankHex.GetPixel(z, w).a != 0)
                                    {
                                        if (heightMaps[i + (j * mapData.GetLength(0) / 18)].width < z + Mathf.CeilToInt(x * (offsetEdge + offsetSide))) break;
                                        Color c = blankHex.GetPixel(z, w);
                                        if (mapDataAtPixel == 3)
                                        {
                                            Color d = mountainHex.GetPixel(z, w);// - new Color(0.1f, 0.1f, 0.1f, 0);
                                            c += (d * d.a);
                                        }
                                        else if (mapDataAtPixel == 2)
                                        {
                                            Color d = hillHex.GetPixel(z, w);// - new Color(0.1f, 0.1f, 0.1f, 0);
                                            c += (d * d.a);
                                        }
                                        heightMaps[i + (j * mapData.GetLength(0) / 18)].SetPixel(z + Mathf.CeilToInt(x * (offsetEdge + offsetSide)), w + (int)yPos, c);
                                    }
                                    /*else if (heightMap.GetPixel(z + Mathf.CeilToInt(x * (offsetEdge + offsetSide)), w + (int)yPos) != Color.b)
                                    {
                                        Debug.Log("Color: " + heightMap.GetPixel(z + Mathf.CeilToInt(x * (offsetEdge + offsetSide)), w + (int)yPos));
                                        heightMap.SetPixel(z + Mathf.CeilToInt(x * (offsetEdge + offsetSide)), w + (int)yPos, Color.black);
                                    }*/
                                }
                                if (heightMaps[i + (j * mapData.GetLength(0) / 18)].height < w + (int)yPos + 2 || (j == (mapData.GetLength(1) / 16) - 1 && y >= Mathf.FloorToInt(heightMaps[i + (j * mapData.GetLength(0) / 18)].height / height) - 1 && colNum % 2 == 1)) break;
                            }
                        }

                        //break;
                        if (colNum % 2 != 0) yPos -= (0.5f * height);
                        colNum++;
                        //if (x == 14) break;
                    }
                    //break;
                }
                heightMaps[i + (j * mapData.GetLength(0) / 18)].Apply();
                transform.GetChild(i + (j * mapData.GetLength(0) / 18)).GetComponent<MeshRenderer>().material.mainTexture = heightMaps[i + (j * mapData.GetLength(0) / 18)];
            }
        }
    }
}
