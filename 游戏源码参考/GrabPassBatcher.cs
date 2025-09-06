using System.Collections.Generic;
using UnityEngine;

public class GrabPassBatcher : MonoBehaviour
{
	private class Batch
	{
		public readonly HashSet<GrabPassBatcher> ActiveList;

		public GameObject BatchRenderer;

		public Batch()
		{
			ActiveList = new HashSet<GrabPassBatcher>();
		}
	}

	[SerializeField]
	private Material grabMaterial;

	[SerializeField]
	private float targetZ;

	private static Dictionary<Material, Batch> _activeBatches;

	private void Awake()
	{
		if (_activeBatches == null)
		{
			_activeBatches = new Dictionary<Material, Batch>();
		}
		if (!_activeBatches.TryGetValue(grabMaterial, out var value))
		{
			value = (_activeBatches[grabMaterial] = new Batch());
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			gameObject.name = "GrabPassBatcher";
			gameObject.GetComponent<MeshRenderer>().material = grabMaterial;
			Transform t = gameObject.transform;
			t.SetParentReset(GameCameras.instance.mainCamera.transform);
			t.SetPositionZ(targetZ);
			value.BatchRenderer = gameObject;
		}
		value.ActiveList.Add(this);
	}

	private void OnDestroy()
	{
		if (_activeBatches.TryGetValue(grabMaterial, out var value))
		{
			value.ActiveList.Remove(this);
			if (value.ActiveList.Count == 0)
			{
				_activeBatches.Remove(grabMaterial);
				Object.Destroy(value.BatchRenderer);
			}
		}
	}
}
