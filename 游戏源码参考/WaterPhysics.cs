using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
	public enum SplashDirection
	{
		Down = 0,
		Up = 1,
		Right = 2,
		Left = 3
	}

	public float spring = 0.02f;

	public float damping = 0.04f;

	public float spread = 0.05f;

	[Space]
	public float detectorHeight = 2f;

	[Space]
	public float velocityUpMultiplier = 1f;

	public float velocityDownMultiplier = 1f;

	[Space]
	[Tooltip("Amount of nodes per unit width.")]
	public int resolution = 5;

	public float lineZ = -0.1f;

	[Header("Reflections")]
	public bool reflections;

	public float zOffset = -5f;

	public float depth = 10f;

	[Space]
	public int reflectionResolution = 256;

	private GameObject cameraObj;

	private int edgeCount;

	private Vector2[] positions;

	private float[] velocities;

	private float[] accelerations;

	private GameObject meshObject;

	private Mesh mesh;

	private Vector3[] vertices;

	private float top;

	private float left;

	private float bottom;

	private GameObject[] colliders;

	private LineRenderer lineRenderer;

	private RenderTexture texture;

	private void Awake()
	{
		lineRenderer = base.gameObject.GetComponent<LineRenderer>();
	}

	private void Start()
	{
		Bounds bounds = GetComponent<MeshRenderer>().bounds;
		float num = bounds.max.x - bounds.min.x;
		top = bounds.max.y;
		left = bounds.min.x;
		bottom = bounds.min.y;
		edgeCount = Mathf.RoundToInt(num) * resolution;
		int num2 = edgeCount + 1;
		lineRenderer.positionCount = num2;
		lineRenderer.useWorldSpace = true;
		positions = new Vector2[num2];
		velocities = new float[num2];
		accelerations = new float[num2];
		colliders = new GameObject[num2];
		mesh = new Mesh();
		mesh.name = "WaterPhysicsMesh";
		meshObject = new GameObject("Water Mesh");
		meshObject.transform.SetParent(base.transform);
		meshObject.transform.localPosition = Vector2.zero;
		meshObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		Material material = GetComponent<MeshRenderer>().material;
		meshObject.AddComponent<MeshRenderer>().sharedMaterial = material;
		Object.Destroy(GetComponent<MeshFilter>());
		Object.Destroy(GetComponent<MeshRenderer>());
		for (int i = 0; i < num2; i++)
		{
			positions[i] = new Vector2(left + num * ((float)i / (float)edgeCount), top);
			accelerations[i] = 0f;
			velocities[i] = 0f;
			lineRenderer.SetPosition(i, positions[i]);
			colliders[i] = new GameObject("Trigger");
			colliders[i].AddComponent<BoxCollider2D>().isTrigger = true;
			colliders[i].transform.position = new Vector3(left + num * (float)i / (float)edgeCount, top - detectorHeight / 2f, 0f);
			colliders[i].transform.localScale = new Vector3(num / (float)edgeCount, detectorHeight, 1f);
			colliders[i].AddComponent<WaterDetector>();
			colliders[i].transform.SetParent(base.transform);
		}
		vertices = new Vector3[edgeCount * 4];
		Vector2[] array = new Vector2[edgeCount * 4];
		int[] array2 = new int[edgeCount * 6];
		for (int j = 0; j < edgeCount; j++)
		{
			int num3 = j * 4;
			float x = ((j == 0) ? 0f : ((float)(j - 1) / (float)edgeCount));
			float x2 = (float)j / (float)edgeCount;
			array[num3] = new Vector2(x, 1f);
			array[num3 + 1] = new Vector2(x2, 1f);
			array[num3 + 2] = new Vector2(x, 0f);
			array[num3 + 3] = new Vector2(x2, 0f);
			int num4 = j * 6;
			array2[num4] = num3;
			array2[num4 + 1] = num3 + 1;
			array2[num4 + 2] = num3 + 3;
			array2[num4 + 3] = num3 + 3;
			array2[num4 + 4] = num3 + 2;
			array2[num4 + 5] = num3;
		}
		UpdateVertexPositions();
		mesh.uv = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		if (reflections)
		{
			float orthographicSize = base.transform.localScale.y / 2f;
			cameraObj = new GameObject("WaterCamera");
			cameraObj.transform.SetParent(base.transform);
			cameraObj.transform.localScale = Vector3.one;
			cameraObj.transform.localPosition = new Vector3(0f, 1f, zOffset);
			Camera camera = cameraObj.AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = orthographicSize;
			camera.nearClipPlane = 0f;
			camera.farClipPlane = depth;
			int num5 = reflectionResolution;
			int width = Mathf.RoundToInt((float)num5 * (base.transform.localScale.x / base.transform.localScale.y));
			texture = new RenderTexture(width, num5, 32, RenderTextureFormat.ARGB32);
			texture.name = "WaterPhysics" + GetInstanceID();
			camera.targetTexture = texture;
			material.SetTexture("_ReflectionTex", texture);
			cameraObj.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (texture != null)
		{
			texture.Release();
			Object.Destroy(texture);
			texture = null;
		}
		if ((bool)mesh)
		{
			Object.Destroy(mesh);
			mesh = null;
		}
	}

	private void Update()
	{
		UpdateVertexPositions();
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < positions.Length; i++)
		{
			float num = spring * (positions[i].y - top) + velocities[i] * damping;
			accelerations[i] = 0f - num;
			positions[i].y += velocities[i];
			velocities[i] += accelerations[i];
			lineRenderer.SetPosition(i, new Vector3(positions[i].x, positions[i].y, base.transform.position.z + lineZ));
		}
		float[] array = new float[positions.Length];
		float[] array2 = new float[positions.Length];
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < positions.Length; k++)
			{
				if (k > 0)
				{
					array[k] = spread * (positions[k].y - positions[k - 1].y);
					velocities[k - 1] += array[k];
				}
				if (k < positions.Length - 1)
				{
					array2[k] = spread * (positions[k].y - positions[k + 1].y);
					velocities[k + 1] += array2[k];
				}
			}
		}
		for (int l = 0; l < positions.Length; l++)
		{
			if (l > 0)
			{
				positions[l - 1].y += array[l];
			}
			if (l < positions.Length - 1)
			{
				positions[l + 1].y += array2[l];
			}
		}
	}

	private void UpdateVertexPositions()
	{
		for (int i = 0; i < edgeCount; i++)
		{
			int num = i * 4;
			vertices[num] = new Vector2(positions[i].x, positions[i].y) - (Vector2)meshObject.transform.position;
			vertices[num + 1] = new Vector2(positions[i + 1].x, positions[i + 1].y) - (Vector2)meshObject.transform.position;
			vertices[num + 2] = new Vector2(positions[i].x, bottom) - (Vector2)meshObject.transform.position;
			vertices[num + 3] = new Vector2(positions[i + 1].x, bottom) - (Vector2)meshObject.transform.position;
		}
		mesh.vertices = vertices;
	}

	public void Splash(float xPos, float velocity)
	{
		if (xPos >= positions[0].x && xPos <= positions[positions.Length - 1].x)
		{
			xPos -= positions[0].x;
			int num = Mathf.RoundToInt((float)(positions.Length - 1) * (xPos / (positions[positions.Length - 1].x - positions[0].x)));
			_ = velocities[num];
			velocities[num] = velocity * ((velocity > 0f) ? velocityUpMultiplier : velocityDownMultiplier);
		}
	}
}
