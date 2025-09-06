using System;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

[ExecuteAlways]
public class ShaderPassObjectScale : MonoBehaviour
{
	[Flags]
	private enum Axis
	{
		[UsedImplicitly]
		None = 0,
		X = 1,
		Y = 2,
		Z = 4,
		All = -1
	}

	[SerializeField]
	[EnumPickerBitmask]
	private Axis passAxis = Axis.All;

	private MaterialPropertyBlock block;

	private MeshRenderer renderer;

	private static readonly int _objectScaleId = Shader.PropertyToID("_ObjectScale");

	private void Start()
	{
		DoUpdate();
	}

	public void DoUpdate()
	{
		if (!renderer)
		{
			renderer = GetComponent<MeshRenderer>();
		}
		if ((bool)renderer)
		{
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			Vector3 lossyScale = base.transform.lossyScale;
			Vector4 value = new Vector4(1f, 1f, 1f, 1f);
			if ((passAxis & Axis.X) == Axis.X)
			{
				value.x = Mathf.Abs(lossyScale.x);
			}
			if ((passAxis & Axis.Y) == Axis.Y)
			{
				value.y = Mathf.Abs(lossyScale.y);
			}
			if ((passAxis & Axis.Z) == Axis.Z)
			{
				value.z = Mathf.Abs(lossyScale.z);
			}
			renderer.GetPropertyBlock(block);
			block.SetVector(_objectScaleId, value);
			renderer.SetPropertyBlock(block);
		}
	}
}
