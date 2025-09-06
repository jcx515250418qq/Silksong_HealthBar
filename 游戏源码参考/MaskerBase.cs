using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

[ExecuteInEditMode]
public class MaskerBase : NestedFadeGroup
{
	private enum Types
	{
		Mask = 0,
		Other = 1
	}

	[SerializeField]
	private Types type;

	public static float EditorTestingAlpha = 1f;

	public static bool ApplyToInverseMasks;

	public static bool UseTestingAlphaInPlayMode;

	private static readonly List<MaskerBase> _maskerList = new List<MaskerBase>();

	protected override float ExtraAlpha
	{
		get
		{
			if (type != 0)
			{
				return 1f;
			}
			if (!UseTestingAlphaInPlayMode && Application.isPlaying)
			{
				return 1f;
			}
			return EditorTestingAlpha;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_maskerList.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_maskerList.Remove(this);
	}

	public static void RefreshAll()
	{
		foreach (MaskerBase masker in _maskerList)
		{
			masker.RefreshAlpha();
		}
	}
}
