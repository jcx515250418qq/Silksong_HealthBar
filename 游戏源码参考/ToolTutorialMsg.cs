using System;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ToolTutorialMsg : UIMsgBase<ToolItem>
{
	[SerializeField]
	private NestedFadeGroupBase rootFader;

	[SerializeField]
	private SpriteRenderer toolIcon;

	[SerializeField]
	private SpriteRenderer ring;

	private void Awake()
	{
		rootFader.AlphaSelf = 0f;
	}

	public static void Spawn(ToolItem tool, Action afterMsg = null)
	{
		ToolTutorialMsg msg = null;
		msg = UIMsgBase<ToolItem>.Spawn(tool, UI.ToolTutorialMsgPrefab, AfterMsg) as ToolTutorialMsg;
		if ((bool)msg)
		{
			GameCameras.instance.HUDOut();
			HeroController.instance.AddInputBlocker(msg);
		}
		void AfterMsg()
		{
			HeroController.instance.RemoveInputBlocker(msg);
			if (afterMsg != null)
			{
				afterMsg();
			}
			else
			{
				GameCameras.instance.HUDIn();
			}
		}
	}

	protected override void Setup(ToolItem tool)
	{
		if ((bool)toolIcon)
		{
			toolIcon.sprite = tool.InventorySpriteBase;
		}
		if ((bool)ring)
		{
			ring.color = UI.GetToolTypeColor(tool.Type);
		}
	}
}
