using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class MatchScaleSignV2 : FsmStateAction
	{
		public enum MatchType
		{
			None = 0,
			MatchScaleSign = 1,
			InvertScaleSign = 2
		}

		public FsmOwnerDefault Target;

		public FsmOwnerDefault MatchTo;

		[ObjectType(typeof(MatchType))]
		public FsmEnum MatchX;

		[ObjectType(typeof(MatchType))]
		public FsmEnum MatchY;

		[ObjectType(typeof(MatchType))]
		public FsmEnum MatchZ;

		public bool EveryFrame;

		private GameObject target;

		private GameObject matchTo;

		public override void Reset()
		{
			Target = null;
			MatchTo = null;
			EveryFrame = false;
			MatchX = null;
			MatchY = null;
			MatchZ = null;
		}

		public override void OnEnter()
		{
			target = Target.GetSafe(this);
			matchTo = MatchTo.GetSafe(this);
			if ((bool)target && (bool)matchTo)
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
			Vector3 lossyScale2 = matchTo.transform.lossyScale;
			Vector3 localScale2 = localScale;
			MatchType matchType = (MatchType)(object)MatchX.Value;
			if (matchType != 0)
			{
				if (matchType == MatchType.InvertScaleSign)
				{
					lossyScale.x *= -1f;
				}
				if ((lossyScale.x > 0f && lossyScale2.x < 0f) || (lossyScale.x < 0f && lossyScale2.x > 0f))
				{
					localScale2.x = 0f - localScale2.x;
				}
			}
			MatchType matchType2 = (MatchType)(object)MatchY.Value;
			if (matchType2 != 0)
			{
				if (matchType2 == MatchType.InvertScaleSign)
				{
					lossyScale.y *= -1f;
				}
				if ((lossyScale.y > 0f && lossyScale2.y < 0f) || (lossyScale.y < 0f && lossyScale2.y > 0f))
				{
					localScale2.y = 0f - localScale2.y;
				}
			}
			MatchType matchType3 = (MatchType)(object)MatchZ.Value;
			if (matchType3 != 0)
			{
				if (matchType3 == MatchType.InvertScaleSign)
				{
					lossyScale.z *= -1f;
				}
				if ((lossyScale.z > 0f && lossyScale2.z < 0f) || (lossyScale.z < 0f && lossyScale2.z > 0f))
				{
					localScale2.z = 0f - localScale2.z;
				}
			}
			target.transform.localScale = localScale2;
		}
	}
}
