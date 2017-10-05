using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
		[SerializeField]
		private Texture2D heightmap;

		[SerializeField]
		private float pixelWidthInMeters = 1;

		[SerializeField]
		private int chunkWidthInPixels = 10;

		[SerializeField]
		private int chunkRenderDistance = 2;

		[SerializeField]
		private Material terrainMaterial;

		[SerializeField]
		private GameObject player;

		[SerializeField]
		private float heightInMeters = 10;

		[SerializeField]
		private int noiseSampleCount=3;

		[SerializeField]
		private int smoothRadius = 3;

		private List<MeshRenderer> visibleChunks = new List<MeshRenderer> ();
		private IntVector2 playerChunkCoordinate;
		private bool isSetup=false;
		private Dictionary<IntVector2,GameObject> chunks= new Dictionary<IntVector2, GameObject>();
		private float chunkWidthInMeters;
		private float heightmapWidthInMeters;
		private GameObject chunkContainer;

		// Use this for initialization
		void Start ()
		{
				Setup ();
				RefreshPlayerChunkCoordinate (true);
		}

		/// <summary>
		/// Returns values 0 to 1 representing where the player is on the minimap.
		/// </summary>
		public Vector2 GetPlayerMinimapPosition(){
				if (!isSetup)
						return Vector2.zero;
				float x = Mathf.Clamp01 (player.transform.position.x / heightmapWidthInMeters);
				float z = Mathf.Clamp01 (player.transform.position.z / heightmapWidthInMeters);
				return new Vector2 (x, z);
		}

		private float GetValueFromImage(int x, int z){
				if (x < 0 || z < 0 || x > heightmap.width || z > heightmap.height)
				{
						Debug.LogError("GetValueFromImage failed, outside range. (" + x + "," + z + ")");
						return 0;
				}
				Color color = heightmap.GetPixel(x, z);
				return (color.r + color.g + color.b) / 3;
		}

		public Texture2D GetMinimapTexture(){
				return heightmap;
		}

		private float GetNoisyHeight(int x, int z){
				if (x % chunkWidthInPixels == 0 || z % chunkWidthInPixels == 0)
						return GetHeight (x, z);

				float heightSum = GetHeight(x,z)*noiseSampleCount;
				for (int i = 0; i < noiseSampleCount; i++) {
						heightSum += GetHeight (x + Random.Range (-1, 1), z + Random.Range (-1, 1));
				}
				return heightSum / (noiseSampleCount * 2);
		}

		public float GetHeightAtPoint(Vector3 point){
				int x = Mathf.RoundToInt(point.x / pixelWidthInMeters);
				int z = Mathf.RoundToInt(point.z / pixelWidthInMeters);
				return GetHeight (x, z);
		}

		private float GetHeight(int x, int z){
				if (x < 0)
						x = 0;
				if (z < 0)
						z = 0;
				if (x > heightmap.width)
						x = heightmap.width;
				if (z > heightmap.height)
						z = heightmap.height;
				
				return GetValueFromImage (x, z) * heightInMeters;
		}

		private void RefreshPlayerChunkCoordinate(bool forceRefresh=false){
				int x = Mathf.RoundToInt (player.transform.position.x / chunkWidthInMeters);
				int z = Mathf.RoundToInt (player.transform.position.z / chunkWidthInMeters);
				IntVector2 newPoint = new IntVector2 (x, z);
				if (forceRefresh || newPoint != playerChunkCoordinate) {
						Debug.Log ("Player moved to a new chunk coordinate: " + newPoint);
						playerChunkCoordinate = newPoint;
						RefreshVisibleChunks ();
				}
		}

		private void RefreshVisibleChunks(){
				foreach (MeshRenderer r in visibleChunks) {
						r.enabled = false;
				}

				visibleChunks = new List<MeshRenderer> ();
				for (int i = -chunkRenderDistance; i <= chunkRenderDistance; i++) {
						for (int j = -chunkRenderDistance; j <= chunkRenderDistance; j++) {
								IntVector2 point = new IntVector2 (i+playerChunkCoordinate.x, j+playerChunkCoordinate.y);
								MeshRenderer mr = GetChunkMeshRendererAtPoint (point);
								visibleChunks.Add (mr);
								mr.enabled = true;
						}
				}
		}

		public GameObject GenerateChunk(IntVector2 chunkPoint)
		{
				Vector3[] vertices;

				GameObject chunk = new GameObject ("chunk " + chunkPoint);
				chunk.transform.position = new Vector3 (chunkPoint.x * chunkWidthInMeters, 0, chunkPoint.y * chunkWidthInMeters);
				chunk.transform.parent = chunkContainer.transform;
				chunk.layer = LayerMask.NameToLayer ("Terrain");
				MeshFilter mf= chunk.AddComponent<MeshFilter> ();
				MeshRenderer mr= chunk.AddComponent<MeshRenderer> ();
				mr.material = terrainMaterial;

				Mesh mesh = new Mesh();
				mf.mesh = mesh;
				mesh.name = "Procedural Grid";

				vertices = new Vector3[(chunkWidthInPixels + 1) * (chunkWidthInPixels + 1)];
				Vector2[] uv = new Vector2[vertices.Length];
				//Vector4[] tangents = new Vector4[vertices.Length];
				//Vector3[] normals = new Vector3[vertices.Length];
				Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

				int xOffset = chunkPoint.x * chunkWidthInPixels;
				int zOffset = chunkPoint.y * chunkWidthInPixels;
				for (int i = 0, z = 0; z <= chunkWidthInPixels; z++)
				{
						for (int x = 0; x <= chunkWidthInPixels; x++, i++)
						{
								float height = GetNoisyHeight(x+xOffset, z+zOffset);
								vertices[i] = new Vector3(x * pixelWidthInMeters, height, z * pixelWidthInMeters);
								uv[i] = new Vector2((float)x / chunkWidthInPixels, (float)z / chunkWidthInPixels);
								//tangents[i]=tangent;
								//tangents [i] = new Vector4 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f), Random.Range (-1f, 1f)).normalized;

								//normals[i]=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized;

						}
				}

				mesh.vertices = vertices;
				//mesh.normals = normals;
				mesh.uv = uv;
				//mesh.tangents = tangents;

				int[] triangles = new int[chunkWidthInPixels * chunkWidthInPixels * 6];
				for (int ti = 0, vi = 0, z = 0; z < chunkWidthInPixels; z++, vi++)
				{
						for (int x = 0; x < chunkWidthInPixels; x++, ti += 6, vi++)
						{
								triangles[ti] = vi;
								triangles[ti + 3] = triangles[ti + 2] = vi + 1;
								triangles[ti + 4] = triangles[ti + 1] = vi + chunkWidthInPixels + 1;
								triangles[ti + 5] = vi + chunkWidthInPixels + 2;
						}
				}
				mesh.triangles = triangles;
				mesh.RecalculateNormals();

				if (chunk.GetComponent<MeshCollider>() == null)
						chunk.AddComponent<MeshCollider>();
				chunk.GetComponent<MeshCollider>().sharedMesh = mesh;

				return chunk;
		}

		private MeshRenderer GetChunkMeshRendererAtPoint(IntVector2 point){
				if (chunks.ContainsKey (point))
						return chunks [point].GetComponent<MeshRenderer>();
				
				GameObject chunk = GenerateChunk (point);
				chunks.Add (point, chunk);
				MeshRenderer mr = chunk.GetComponent<MeshRenderer> ();
				return mr;
		}

		private void Setup(){
				if (isSetup)
						return;

				chunkWidthInMeters = pixelWidthInMeters * chunkWidthInPixels;
				heightmapWidthInMeters = heightmap.width * pixelWidthInMeters;

				chunkContainer = new GameObject ("chunks");

				if (transform.position.magnitude > 0) {
						Debug.LogError (this + " is meant to be at (0,0,0) to work properly.");
						return;
				}

				if (heightmap == null || terrainMaterial == null || player == null) {
						Debug.LogError ("Setup failed. Null values in inspector.");
						return;
				}

				isSetup = true;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (!isSetup)
						return;
				
				RefreshPlayerChunkCoordinate ();
		}
}
