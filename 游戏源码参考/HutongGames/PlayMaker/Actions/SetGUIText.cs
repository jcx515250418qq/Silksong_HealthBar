using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Text used by the GUIText Component attached to a Game Object.")]
	[Obsolete("GUIText is part of the legacy UI system removed in 2019.3")]
	public class SetGUIText : FsmStateAction
	{
		[ActionSection("Obsolete. Use Unity UI instead.")]
		[UIHint(UIHint.TextArea)]
		public FsmString text;
	}
}
