using System;
using UnityEngine;

[ExecuteInEditMode]
public class SetShaderPropertyRotation : MonoBehaviour
{
	[SerializeField]
	private Renderer renderer;

	[SerializeField]
	private string variableName;

	[SerializeField]
	private float offset;

	[SerializeField]
	private bool updateAtRuntime;

	private MaterialPropertyBlock block;

	private void OnEnable()
	{
		UpdateValue();
	}

	private void Update()
	{
		if (!Application.isPlaying || updateAtRuntime)
		{
			UpdateValue();
		}
	}

	private void UpdateValue()
	{
		if ((bool)renderer && !string.IsNullOrEmpty(variableName))
		{
			float value = (base.transform.eulerAngles.z + offset) * (MathF.PI / 180f);
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			block.Clear();
			renderer.GetPropertyBlock(block);
			block.SetFloat(variableName, value);
			renderer.SetPropertyBlock(block);
		}
	}
}
