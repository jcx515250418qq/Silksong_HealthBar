using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RemoveMemoryMsg : FsmStateAction
	{
		public FsmOwnerDefault Source;

		public FsmFloat DisappearDelay;

		public override void Reset()
		{
			Source = null;
			DisappearDelay = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Source.GetSafe(this);
			if ((bool)safe)
			{
				MemoryMsgBox.RemoveText(safe, DisappearDelay.Value);
			}
			Finish();
		}
	}
}
