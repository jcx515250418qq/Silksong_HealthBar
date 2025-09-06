using System;
using TMProOld;
using TeamCherry.SharedUtils;
using UnityEngine;

public class ChangeFontStyleByLanguage : ChangeByLanguageNew<ChangeFontStyleByLanguage.OverrideSetting>
{
	[Serializable]
	public class OverrideStyle : OverrideMaskValue<FontStyles>
	{
	}

	[Serializable]
	public class OverrideSetting : OverrideValue
	{
		public OverrideStyle fontStyle;
	}

	[SerializeField]
	private TextMeshPro textMeshPro;

	private bool hasTextMeshPro;

	private FontStyles originalFontStyle;

	protected override void DoAwake()
	{
		base.DoAwake();
		hasTextMeshPro = textMeshPro != null;
		if (!hasTextMeshPro)
		{
			textMeshPro = GetComponent<TextMeshPro>();
			hasTextMeshPro = textMeshPro != null;
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (textMeshPro == null)
		{
			textMeshPro = GetComponent<TextMeshPro>();
		}
		hasTextMeshPro = textMeshPro != null;
	}

	protected override void RecordOriginalValues()
	{
		if (!recorded && hasTextMeshPro)
		{
			originalFontStyle = textMeshPro.fontStyle;
			recorded = true;
		}
	}

	protected override void DoRevertValues()
	{
		if (hasTextMeshPro)
		{
			textMeshPro.fontStyle = originalFontStyle;
		}
	}

	protected override void ApplySetting(OverrideSetting setting)
	{
		if (hasTextMeshPro && setting.fontStyle.IsEnabled)
		{
			textMeshPro.fontStyle = setting.fontStyle.Value;
		}
	}
}
