using System;
using TMProOld;
using UnityEngine;

public sealed class ChangeSpacingByLanguage : ChangeByLanguageOld<ChangeSpacingByLanguage.SpaceOverride>
{
	[Serializable]
	public sealed class SpaceOverride : LanguageOverride
	{
		public float lineSpacing;
	}

	[SerializeField]
	private TextMeshPro text;

	private float originalValue;

	private bool hasText;

	protected override void DoAwake()
	{
		hasText = text != null;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (text == null)
		{
			text = GetComponent<TextMeshPro>();
		}
		hasText = text != null;
	}

	protected override void RecordOriginalValues()
	{
		if (!recorded && hasText)
		{
			originalValue = text.lineSpacing;
			recorded = true;
		}
	}

	protected override void DoRevertValues()
	{
		if (hasText)
		{
			text.lineSpacing = originalValue;
		}
	}

	protected override void ApplySetting(SpaceOverride setting)
	{
		if (hasText)
		{
			text.lineSpacing = setting.lineSpacing;
		}
	}
}
