using System.Collections.Generic;
using UnityEngine;

public class SpriteExtruder : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer originalDisplay;

	[SerializeField]
	private float extrusionDepth;

	[SerializeField]
	private int extrusionLayers;

	[SerializeField]
	private bool colourBack;

	[SerializeField]
	private float rotationThreshold = 1f;

	private readonly List<SpriteRenderer> extrusions = new List<SpriteRenderer>();

	private bool wasExtrusionVisible;

	private bool hasStarted;

	protected float ExtrusionDepth => extrusionDepth;

	protected SpriteRenderer OriginalDisplay => originalDisplay;

	private void OnValidate()
	{
		if (extrusionLayers < 1)
		{
			extrusionLayers = 1;
		}
		if (rotationThreshold < 0f)
		{
			rotationThreshold = 0f;
		}
	}

	protected virtual void Awake()
	{
		FullRefresh();
	}

	private void Start()
	{
		hasStarted = true;
	}

	protected virtual void OnEnable()
	{
		OnValidate();
		if (hasStarted && (bool)originalDisplay)
		{
			originalDisplay.gameObject.SetActive(value: true);
			originalDisplay.transform.Reset();
		}
	}

	private void LateUpdate()
	{
		if (!originalDisplay)
		{
			return;
		}
		bool flag;
		if (rotationThreshold > 0f)
		{
			Vector3 eulerAngles = originalDisplay.transform.eulerAngles;
			flag = !eulerAngles.x.IsWithinTolerance(rotationThreshold, 0f) || !eulerAngles.y.IsWithinTolerance(rotationThreshold, 0f);
		}
		else
		{
			flag = true;
		}
		if (flag == wasExtrusionVisible)
		{
			return;
		}
		foreach (SpriteRenderer extrusion in extrusions)
		{
			extrusion.enabled = flag;
		}
		wasExtrusionVisible = flag;
	}

	private void CreateExtrusions()
	{
		if ((bool)originalDisplay && extrusionDepth != 0f)
		{
			_ = originalDisplay.sprite;
			Transform parent = originalDisplay.transform;
			int num = extrusionLayers - 1;
			for (int i = 0; i < extrusionLayers; i++)
			{
				SpriteRenderer component = new GameObject("extrusion", typeof(SpriteRenderer))
				{
					hideFlags = HideFlags.HideAndDontSave
				}.GetComponent<SpriteRenderer>();
				component.sprite = originalDisplay.sprite;
				component.color = ((colourBack && i == num) ? originalDisplay.color : Color.black);
				component.transform.SetParentReset(parent);
				extrusions.Add(component);
			}
		}
	}

	private void DestroyExtrusions()
	{
		foreach (SpriteRenderer extrusion in extrusions)
		{
			if ((bool)extrusion)
			{
				Object.DestroyImmediate(extrusion.gameObject);
			}
		}
		extrusions.Clear();
	}

	private void LayoutExtrusions()
	{
		float num = ((extrusionDepth != 0f) ? (extrusionDepth / (float)extrusionLayers) : 0f);
		for (int i = 0; i < extrusions.Count; i++)
		{
			extrusions[i].transform.SetLocalPositionZ(num * (float)(i + 1));
		}
	}

	private void OnDestroy()
	{
		DestroyExtrusions();
	}

	[ContextMenu("Full Refresh")]
	public void FullRefresh()
	{
		DestroyExtrusions();
		CreateExtrusions();
		LayoutExtrusions();
		wasExtrusionVisible = true;
	}
}
