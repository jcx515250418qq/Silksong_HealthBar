using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoPassBlackThreadState : FsmStateAction
	{
		[CheckForComponent(typeof(PassBlackThreadState))]
		public FsmOwnerDefault Source;

		public FsmGameObject Target;

		public override void Reset()
		{
			Source = null;
			Target = null;
		}

		public override void OnEnter()
		{
			PassBlackThreadState component = Source.GetSafe(this).GetComponent<PassBlackThreadState>();
			GameObject value = Target.Value;
			if ((bool)value)
			{
				value.GetComponent<BlackThreadState>().PassState(component);
			}
			Finish();
		}
	}
}
