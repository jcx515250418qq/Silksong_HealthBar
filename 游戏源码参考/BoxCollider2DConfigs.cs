using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoxCollider2DConfigs : MonoBehaviour
{
	[Serializable]
	private struct Config
	{
		public Vector2 Offset;

		public Vector2 Size;
	}

	[SerializeField]
	private Config[] configs;

	private Config initialConfig;

	private BoxCollider2D box;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		if (configs != null)
		{
			Config[] array = configs;
			for (int i = 0; i < array.Length; i++)
			{
				Config config = array[i];
				Gizmos.DrawWireCube(config.Offset, config.Size);
			}
		}
	}

	private void Awake()
	{
		GetBox();
	}

	private void GetBox()
	{
		if (!box)
		{
			box = GetComponent<BoxCollider2D>();
			initialConfig = new Config
			{
				Offset = box.offset,
				Size = box.size
			};
		}
	}

	public void SetConfig(int index)
	{
		GetBox();
		Config config = ((index >= 0 && index < configs.Length) ? configs[index] : initialConfig);
		box.offset = config.Offset;
		box.size = config.Size;
	}
}
