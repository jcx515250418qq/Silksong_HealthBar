using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BlurPlane : MonoBehaviour
{
	public delegate void BlurPlanesChangedDelegate();

	private MeshRenderer meshRenderer;

	private Material originalMaterial;

	private static List<BlurPlane> _blurPlanes;

	private static readonly int _blurPlaneVibranceOffset = Shader.PropertyToID("_BlurPlaneVibranceOffset");

	public static int BlurPlaneCount => _blurPlanes.Count;

	public static BlurPlane ClosestBlurPlane
	{
		get
		{
			if (_blurPlanes.Count <= 0)
			{
				return null;
			}
			return _blurPlanes[0];
		}
	}

	public float PlaneZ => base.transform.position.z;

	public static event BlurPlanesChangedDelegate BlurPlanesChanged;

	public static BlurPlane GetBlurPlane(int index)
	{
		return _blurPlanes[index];
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		_blurPlanes = new List<BlurPlane>();
	}

	protected void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		originalMaterial = meshRenderer.sharedMaterial;
	}

	protected void OnEnable()
	{
		int i;
		for (i = 0; i < _blurPlanes.Count; i++)
		{
			BlurPlane blurPlane = _blurPlanes[i];
			if (PlaneZ <= blurPlane.PlaneZ)
			{
				break;
			}
		}
		_blurPlanes.Insert(i, this);
		if (BlurPlane.BlurPlanesChanged != null)
		{
			BlurPlane.BlurPlanesChanged();
		}
	}

	protected void OnDisable()
	{
		_blurPlanes.Remove(this);
		if (BlurPlane.BlurPlanesChanged != null)
		{
			BlurPlane.BlurPlanesChanged();
		}
	}

	public void SetPlaneVisibility(bool isVisible)
	{
		meshRenderer.enabled = isVisible;
	}

	public void SetPlaneMaterial(Material material)
	{
		meshRenderer.sharedMaterial = ((material == null) ? originalMaterial : material);
	}

	public static void SetVibranceOffset(float offset)
	{
		Shader.SetGlobalFloat(_blurPlaneVibranceOffset, offset);
	}
}
