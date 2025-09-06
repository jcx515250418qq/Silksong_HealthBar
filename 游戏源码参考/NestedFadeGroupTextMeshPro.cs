using System;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshPro))]
[NestedFadeGroupBridge(new Type[] { typeof(TextMeshPro) })]
public class NestedFadeGroupTextMeshPro : NestedFadeGroupBase
{
	private TextMeshPro textMesh;

	private MeshRenderer meshRenderer;

	public Color Color
	{
		get
		{
			GetMissingReferences();
			if (!textMesh)
			{
				return Color.black;
			}
			return textMesh.color;
		}
		set
		{
			GetMissingReferences();
			base.AlphaSelf = value.a;
		}
	}

	public string Text
	{
		get
		{
			GetMissingReferences();
			if (!textMesh)
			{
				return string.Empty;
			}
			return textMesh.text;
		}
		set
		{
			GetMissingReferences();
			if ((bool)textMesh)
			{
				textMesh.text = value;
			}
		}
	}

	protected override void GetMissingReferences()
	{
		if (!textMesh)
		{
			textMesh = GetComponent<TextMeshPro>();
			textMesh.enabled = true;
		}
		if (!meshRenderer)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}
	}

	protected override void OnComponentAdded()
	{
		if ((bool)textMesh)
		{
			base.AlphaSelf = textMesh.color.a;
		}
	}

	protected override void OnAlphaChanged(float alpha)
	{
		Color color = textMesh.color;
		color.a = alpha;
		textMesh.color = color;
		meshRenderer.enabled = alpha > Mathf.Epsilon;
	}
}
