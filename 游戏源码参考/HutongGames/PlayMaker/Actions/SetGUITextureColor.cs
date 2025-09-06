using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITextureColor : FsmStateAction
	{
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmColor color;
	}
}
