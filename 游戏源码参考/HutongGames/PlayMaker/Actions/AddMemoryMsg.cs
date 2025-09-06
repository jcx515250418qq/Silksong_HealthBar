using TeamCherry.Localization;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AddMemoryMsg : FsmStateAction
	{
		public FsmOwnerDefault Source;

		public LocalisedFsmString Text;

		public override void Reset()
		{
			Source = null;
			Text = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Source.GetSafe(this);
			if ((bool)safe)
			{
				MemoryMsgBox.AddText(safe, Text);
			}
			Finish();
		}
	}
}
