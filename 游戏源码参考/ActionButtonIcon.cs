using System;
using GlobalEnums;
using InControl;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActionButtonIcon : ActionButtonIconBase
{
	[SerializeField]
	private HeroActionButton action;

	public bool showForControllerOnly;

	private Vector3? initialScale;

	public override HeroActionButton Action => action;

	protected override void OnEnable()
	{
		base.OnIconUpdate += CheckHideIcon;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnIconUpdate -= CheckHideIcon;
		base.OnDisable();
	}

	private void CheckHideIcon()
	{
		if (showForControllerOnly && (bool)sr)
		{
			if (!initialScale.HasValue)
			{
				initialScale = base.transform.localScale;
			}
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			if (instance.lastActiveController == BindingSourceType.KeyBindingSource || instance.lastActiveController == BindingSourceType.None)
			{
				base.transform.localScale = Vector3.zero;
			}
			else if (instance.lastActiveController == BindingSourceType.DeviceBindingSource)
			{
				base.transform.localScale = initialScale.Value;
			}
		}
	}

	public void SetAction(HeroActionButton action)
	{
		this.action = action;
		GetButtonIcon(action);
	}

	public void SetActionString(string action)
	{
		object obj = Enum.Parse(typeof(HeroActionButton), action);
		if (obj != null)
		{
			this.action = (HeroActionButton)obj;
			GetButtonIcon((HeroActionButton)obj);
		}
		else
		{
			Debug.LogError("SetAction couldn't convert " + action);
		}
	}
}
