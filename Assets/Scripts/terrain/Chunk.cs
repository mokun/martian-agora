using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{

    public Texture2D heightMap;
    public float pixelToMeterRatio = 1;
    public float heightRange = 10;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        if (heightMap != null)
            GenerateMesh();
    }

    private float GetHeightFromHeightmap(int x, int z)
    {
        if (x < 0 || z < 0 || x > heightMap.width || z > heightMap.height)
        {
            Debug.LogError("GetHeightFromHeightmap failed. (" + x + "," + z + ")");
            return 0;
        }

        int blurRadius = 5;
        int blurCount = 0;
        float ratioTotal = 0;
        for (int i = -blurRadius; i < blurRadius; i++)
        {
            for (int j = -blurRadius; j < blurRadius; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) <= blurRadius)
                {
                    blurCount++;
                    ratioTotal += GetHeightmapPixelRatio(x, z);
                }
            }
        }
        float heightRatio = ratioTotal / blurCount;
        
        return heightRatio * heightRange;
    }

    private float GetHeightmapPixelRatio(int x, int z)
    {
        Color color = heightMap.GetPixel(x, z);
        return (color.r + color.g + color.b) / 3;
    }

    public void GenerateMesh()
    {
        if (vertices != null)
        {
            Debug.LogError("Generate Chunk was called twice. Aborting.");
            return;
        }

        int xSize = heightMap.width - 1;
        int zSize = heightMap.height - 1;

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float height = GetHeightFromHeightmap(x, z);
                vertices[i] = new Vector3(x * pixelToMeterRatio, height, z * pixelToMeterRatio);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        UpdateMeshCollider();
    }

    private void UpdateMeshCollider()
    {
        if (GetComponent<MeshCollider>() == null)
            gameObject.AddComponent<MeshCollider>();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}