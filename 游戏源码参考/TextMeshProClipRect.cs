using System.Collections.Generic;
using TMProOld;
using UnityEngine;

[ExecuteInEditMode]
public class TextMeshProClipRect : MonoBehaviour
{
	[SerializeField]
	private Transform relativeTo;

	[SerializeField]
	private Vector2 min;

	[SerializeField]
	private Vector2 max;

	private Renderer renderer;

	private MaterialPropertyBlock block;

	private static readonly int _clipRectProp = Shader.PropertyToID("_ClipRect");

	private List<TMP_SubMesh> subMeshes = new List<TMP_SubMesh>();

	private void OnEnable()
	{
		renderer = GetComponent<Renderer>();
		if (block == null)
		{
			block = new MaterialPropertyBlock();
		}
	}

	private void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 obj = (relativeTo ? relativeTo.position : transform.position);
		Vector3 position = obj + min.ToVector3(0f);
		Vector3 position2 = obj + max.ToVector3(0f);
		Vector3 vector = transform.InverseTransformPoint(position);
		Vector3 vector2 = transform.InverseTransformPoint(position2);
		Vector4 bounds = new Vector4(vector.x, vector.y, vector2.x, vector2.y);
		ApplyBounds(renderer, bounds);
		foreach (TMP_SubMesh subMesh in subMeshes)
		{
			ApplyBounds(subMesh.renderer, bounds);
		}
	}

	private void ApplyBounds(Renderer renderer, Vector4 bounds)
	{
		block.Clear();
		renderer.GetPropertyBlock(block);
		block.SetVector(_clipRectProp, bounds);
		renderer.SetPropertyBlock(block);
	}

	public void AddSubMesh(TMP_SubMesh subMesh)
	{
		subMeshes.Add(subMesh);
	}

	public void RemoveSubMesh(TMP_SubMesh subMesh)
	{
		subMeshes.Remove(subMesh);
	}
}
