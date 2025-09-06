using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class ChangeScaleByLanguage : ChangeByLanguageOld<ChangeScaleByLanguage.ScaleOverride>
{
	[Serializable]
	public sealed class ScaleOverride : LanguageOverride
	{
		public OverrideFloat xScale;

		public OverrideFloat yScale;

		public OverrideFloat zScale;

		public HandHeldOverrides handHeldOverrides;
	}

	[Serializable]
	public sealed class HandHeldOverrides
	{
		public OverrideFloat xScale;

		public OverrideFloat yScale;

		public OverrideFloat zScale;
	}

	[SerializeField]
	private HandHeldOverrides handHeldOverrides = new HandHeldOverrides();

	private Vector3 originalScale;

	protected override void RecordOriginalValues()
	{
		if (!recorded)
		{
			recorded = true;
			originalScale = base.transform.localScale;
		}
	}

	protected override void DoRevertValues()
	{
		base.transform.localScale = originalScale;
	}

	public override void ApplyHandHeld()
	{
		ApplySetting(handHeldOverrides);
	}

	private void ApplySetting(HandHeldOverrides setting)
	{
		Vector3 localScale = base.transform.localScale;
		if (setting.xScale.IsEnabled)
		{
			localScale.x = setting.xScale.Value;
		}
		if (setting.yScale.IsEnabled)
		{
			localScale.y = setting.yScale.Value;
		}
		if (setting.zScale.IsEnabled)
		{
			localScale.z = setting.zScale.Value;
		}
		base.transform.localScale = localScale;
	}

	protected override void ApplySetting(ScaleOverride setting)
	{
		Vector3 scale = base.transform.localScale;
		if (setting.xScale.IsEnabled)
		{
			scale.x = setting.xScale.Value;
		}
		if (setting.yScale.IsEnabled)
		{
			scale.y = setting.yScale.Value;
		}
		if (setting.zScale.IsEnabled)
		{
			scale.z = setting.zScale.Value;
		}
		if (ShouldApplyHandHeld())
		{
			ApplySetting(setting.handHeldOverrides);
		}
		base.transform.localScale = scale;
		void ApplySetting(HandHeldOverrides setting)
		{
			if (setting.xScale.IsEnabled)
			{
				scale.x = setting.xScale.Value;
			}
			if (setting.yScale.IsEnabled)
			{
				scale.y = setting.yScale.Value;
			}
			if (setting.zScale.IsEnabled)
			{
				scale.z = setting.zScale.Value;
			}
		}
	}
}
