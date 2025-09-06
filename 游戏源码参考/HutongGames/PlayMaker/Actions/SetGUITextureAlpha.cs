using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Alpha of the GUITexture attached to a Game Object. Useful for fading GUI elements in/out.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITextureAlpha : FsmStateAction
	{
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmFloat alpha;
	}
}
