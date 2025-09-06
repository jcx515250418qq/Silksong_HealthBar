using UnityEngine;

[ExecuteAlways]
public class ShaderPassObjectPos : MonoBehaviour
{
	private MaterialPropertyBlock block;

	private MeshRenderer renderer;

	private static readonly int _objectScaleId = Shader.PropertyToID("_ObjectPos");

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
			renderer.GetPropertyBlock(block);
			block.SetVector(_objectScaleId, base.transform.position);
			renderer.SetPropertyBlock(block);
		}
	}
}
