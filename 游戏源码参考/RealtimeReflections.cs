using UnityEngine;
using UnityEngine.Rendering;

public class RealtimeReflections : MonoBehaviour
{
	public int cubemapSize = 128;

	public float nearClip = 0.01f;

	public float farClip = 500f;

	public bool oneFacePerFrame;

	public Material[] materials;

	public ReflectionProbe[] reflectionProbes;

	public LayerMask layerMask = -1;

	private Camera cam;

	private RenderTexture renderTexture;

	private void OnEnable()
	{
		layerMask.value = -1;
	}

	private void Start()
	{
		ReflectionProbe[] array = reflectionProbes;
		foreach (ReflectionProbe obj in array)
		{
			obj.mode = ReflectionProbeMode.Realtime;
			obj.boxProjection = true;
			obj.resolution = cubemapSize;
			obj.transform.parent = base.transform.parent;
			obj.transform.localPosition = Vector3.zero;
		}
		if (materials.Length != 0)
		{
			UpdateCubemap(63);
		}
	}

	private void LateUpdate()
	{
		if (materials.Length != 0)
		{
			if (oneFacePerFrame)
			{
				int num = Time.frameCount % 6;
				int faceMask = 1 << num;
				UpdateCubemap(faceMask);
			}
			else
			{
				UpdateCubemap(63);
			}
		}
	}

	private void UpdateCubemap(int faceMask)
	{
		if (!cam)
		{
			GameObject gameObject = new GameObject("CubemapCamera", typeof(Camera));
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = Quaternion.identity;
			cam = gameObject.GetComponent<Camera>();
			cam.cullingMask = layerMask;
			cam.nearClipPlane = nearClip;
			cam.farClipPlane = farClip;
			cam.enabled = false;
		}
		if (!renderTexture)
		{
			renderTexture = new RenderTexture(cubemapSize, cubemapSize, 16);
			renderTexture.name = "RealtimeReflections" + GetInstanceID();
			renderTexture.isPowerOfTwo = true;
			renderTexture.dimension = TextureDimension.Cube;
			renderTexture.hideFlags = HideFlags.HideAndDontSave;
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (material.HasProperty("_Cube"))
					{
						material.SetTexture("_Cube", renderTexture);
					}
				}
			}
			ReflectionProbe[] array = reflectionProbes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].customBakedTexture = renderTexture;
			}
		}
		cam.transform.position = base.transform.position;
		cam.RenderToCubemap(renderTexture, faceMask);
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(cam);
		Object.DestroyImmediate(renderTexture);
	}
}
