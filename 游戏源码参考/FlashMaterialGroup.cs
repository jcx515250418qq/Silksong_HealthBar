using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlashMaterialGroup : MonoBehaviour
{
	[Range(0f, 1f)]
	public float flashAmount;

	private Renderer renderer;

	private Material material;

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		List<Renderer> list = new List<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer != this.renderer && renderer.sharedMaterial == this.renderer.sharedMaterial)
			{
				list.Add(renderer);
			}
		}
		material = new Material(this.renderer.sharedMaterial);
		this.renderer.sharedMaterial = material;
		foreach (Renderer item in list)
		{
			item.material = material;
		}
	}

	private void OnDestroy()
	{
		if (material != null)
		{
			Object.Destroy(material);
		}
	}

	private void Update()
	{
		if ((bool)material)
		{
			material.SetFloat("_FlashAmount", flashAmount);
		}
	}
}
