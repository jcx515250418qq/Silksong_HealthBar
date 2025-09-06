using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class MatchScaleSign : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmGameObject MatchTo;

		public bool EveryFrame;

		private GameObject target;

		public override void Reset()
		{
			Target = null;
			MatchTo = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			target = Target.GetSafe(this);
			if ((bool)target && (bool)MatchTo.Value)
			{
				DoAction();
			}
			else
			{
				Finish();
			}
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			Vector3 localScale = target.transform.localScale;
			Vector3 lossyScale = target.transform.lossyScale;
			Vector3 lossyScale2 = MatchTo.Value.transform.lossyScale;
			Vector3 localScale2 = localScale;
			if ((lossyScale.x > 0f && lossyScale2.x < 0f) || (lossyScale.x < 0f && lossyScale2.x > 0f))
			{
				localScale2.x = 0f - localScale2.x;
			}
			if ((lossyScale.y > 0f && lossyScale2.y < 0f) || (lossyScale.y < 0f && lossyScale2.y > 0f))
			{
				localScale2.y = 0f - localScale2.y;
			}
			target.transform.localScale = localScale2;
		}
	}
}
