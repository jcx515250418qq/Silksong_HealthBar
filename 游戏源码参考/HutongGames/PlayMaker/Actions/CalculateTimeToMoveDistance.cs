using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class CalculateTimeToMoveDistance : FsmStateAction
	{
		public FsmFloat Speed;

		public FsmFloat Distance;

		[HideIf("IsDistanceSpecified")]
		public FsmVector3 FromPositon;

		[HideIf("IsDistanceSpecified")]
		public FsmVector3 ToPositon;

		[UIHint(UIHint.Variable)]
		public FsmFloat Time;

		public override void Reset()
		{
			Speed = null;
			Distance = new FsmFloat
			{
				UseVariable = true
			};
			FromPositon = null;
			ToPositon = null;
			Time = null;
		}

		public bool IsDistanceSpecified()
		{
			return !Distance.IsNone;
		}

		public override void OnEnter()
		{
			float num = ((!Distance.IsNone) ? Distance.Value : Vector3.Distance(FromPositon.Value, ToPositon.Value));
			Time.Value = num / Speed.Value;
			Finish();
		}
	}
}
