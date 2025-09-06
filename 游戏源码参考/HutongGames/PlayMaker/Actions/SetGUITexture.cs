using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITexture : FsmStateAction
	{
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmTexture texture;
	}
}
