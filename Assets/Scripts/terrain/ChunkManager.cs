using UnityEngine;
using System.Collections;

public class ChunkManager : MonoBehaviour
{
    public Texture2D heightMap;
    public float pixelToMeterRatio = 1;
    public float heightRange = 10;
    public Material terrainMaterial;

    private GameObject[,] chunks;
    private int chunkRowCount = 128;

    private void Start()
    {
        int rows = heightMap.width / chunkRowCount;
        int columns = heightMap.height / chunkRowCount;
        chunks = new GameObject[rows, columns];

        Debug.Log((float)heightMap.width + " " + (float)chunkRowCount);

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                GameObject go = new GameObject("chunk (" + x + "," + z + ")");
                go.transform.parent = gameObject.transform;
                go.transform.position = new Vector3(x * chunkRowCount * pixelToMeterRatio, 0, z * chunkRowCount * pixelToMeterRatio);
                Chunk newChunk = go.AddComponent<Chunk>();
                newChunk.pixelToMeterRatio = pixelToMeterRatio;
                newChunk.heightRange = heightRange;
                go.GetComponent<MeshRenderer>().materials = new Material[1] { terrainMaterial };

                int addToX = x == rows - 1 ? 0 : 1;
                int addToZ = z == columns - 1 ? 0 : 1;
                Texture2D newHeightMap = new Texture2D(chunkRowCount + addToX, chunkRowCount + addToZ);
                newHeightMap.SetPixels(heightMap.GetPixels(x * chunkRowCount,
                    z * chunkRowCount,
                    chunkRowCount + addToX,
                    chunkRowCount + addToZ));

                newChunk.heightMap = newHeightMap;
                newChunk.GenerateMesh();
            }
        }
    }
}
