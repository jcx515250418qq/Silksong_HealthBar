using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GrassSprite : MonoBehaviour
{
	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private Color color;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private Mesh mesh;

	private bool appliedFlip;

	private bool flipX;

	private bool flipY;

	private static readonly int _mainTexProp = Shader.PropertyToID("_MainTex");

	private void OnValidate()
	{
		UpdateMesh();
	}

	private void Awake()
	{
		Upgrade();
		UpdateMesh();
	}

	private void OnEnable()
	{
		if (base.enabled)
		{
			meshRenderer.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (!base.enabled)
		{
			meshRenderer.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh);
		}
	}

	public void Upgrade()
	{
		int sortingLayerID;
		int sortingOrder;
		if ((bool)meshRenderer)
		{
			sortingLayerID = meshRenderer.sortingLayerID;
			sortingOrder = meshRenderer.sortingOrder;
		}
		else
		{
			sortingLayerID = 0;
			sortingOrder = 0;
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			sprite = component.sprite;
			color = component.color;
			sortingLayerID = component.sortingLayerID;
			sortingOrder = component.sortingOrder;
			DestroyComponent(component);
		}
		meshFilter = base.gameObject.AddComponentIfNotPresent<MeshFilter>();
		meshFilter.hideFlags |= HideFlags.HideInInspector;
		meshRenderer = base.gameObject.AddComponentIfNotPresent<MeshRenderer>();
		meshRenderer.hideFlags |= HideFlags.HideInInspector;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;
		meshRenderer.sortingLayerID = sortingLayerID;
		meshRenderer.sortingOrder = sortingOrder;
	}

	public void RemoveAndAddSpriteRenderer()
	{
		int sortingLayerID = meshRenderer.sortingLayerID;
		int sortingOrder = meshRenderer.sortingOrder;
		DestroyComponent(meshFilter);
		DestroyComponent(meshRenderer);
		SpriteRenderer spriteRenderer = base.gameObject.AddComponentIfNotPresent<SpriteRenderer>();
		if ((bool)spriteRenderer)
		{
			spriteRenderer.sprite = sprite;
			spriteRenderer.color = color;
			spriteRenderer.sortingLayerID = sortingLayerID;
			spriteRenderer.sortingOrder = sortingOrder;
		}
		DestroyComponent(this);
	}

	private void DestroyComponent(Component component)
	{
		if ((bool)component)
		{
			UnityEngine.Object.DestroyImmediate(component, allowDestroyingAssets: true);
		}
	}

	public void UpdateMesh()
	{
		if (sprite == null || meshFilter == null)
		{
			return;
		}
		Animator componentInParent = GetComponentInParent<Animator>();
		if (Application.isPlaying && !componentInParent && !appliedFlip)
		{
			Transform transform = base.transform;
			Vector3 lossyScale = transform.lossyScale;
			appliedFlip = true;
			flipX = Mathf.Sign(lossyScale.x) < 0f;
			flipY = Mathf.Sign(lossyScale.y) < 0f;
			bool flag = lossyScale.z < 0f;
			if (flipX || flipY || flag)
			{
				transform.FlipLocalScale(flipX, flipY, flag);
			}
		}
		Vector3[] array = sprite.vertices.Select((Vector2 v) => v.ToVector3(0f)).ToArray();
		int[] triangles = sprite.triangles.Select(Convert.ToInt32).ToArray();
		List<Vector2> source = sprite.uv.ToList();
		Color[] array2 = new Color[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = color;
		}
		Vector3 position = base.transform.position;
		Vector4 vector = new Vector4(position.x, position.y, position.z);
		Vector4[] array3 = new Vector4[array.Length];
		for (int j = 0; j < array3.Length; j++)
		{
			Vector3 vector2 = array[j];
			if (flipX)
			{
				vector2.x *= -1f;
			}
			if (flipY)
			{
				vector2.y *= -1f;
			}
			float y = vector2.y;
			vector.w = y;
			array[j] = vector2;
			array3[j] = vector;
		}
		mesh = meshFilter.sharedMesh;
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh);
		}
		mesh = new Mesh();
		mesh.name = "GrassSprite";
		mesh.hideFlags |= HideFlags.DontSave;
		meshFilter.sharedMesh = mesh;
		mesh.vertices = array;
		mesh.triangles = triangles;
		mesh.colors = array2;
		mesh.SetUVs(0, source.ToList());
		mesh.SetUVs(3, array3.ToList());
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		meshRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetTexture(_mainTexProp, sprite.texture);
		meshRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
