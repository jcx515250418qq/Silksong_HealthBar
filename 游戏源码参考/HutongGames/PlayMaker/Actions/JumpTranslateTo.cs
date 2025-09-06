using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[UsedImplicitly]
	public class JumpTranslateTo : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault TranslateObject;

		public FsmVector3 StartPos;

		public FsmVector3 EndPos;

		public FsmFloat JumpHeight;

		public FsmAnimationCurve JumpCurve;

		public FsmFloat Duration;

		private Transform translateObject;

		private double enterTime;

		private Vector3 startPos;

		private Vector3 endPos;

		public override void Reset()
		{
			TranslateObject = null;
			StartPos = null;
			EndPos = null;
			JumpHeight = 1f;
			JumpCurve = new FsmAnimationCurve
			{
				curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 3f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f, -3f, 0f))
			};
			Duration = 1f;
		}

		public override void OnEnter()
		{
			GameObject safe = TranslateObject.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			enterTime = Time.timeAsDouble;
			translateObject = safe.transform;
			startPos = StartPos.Value;
			endPos = EndPos.Value;
			DoAction();
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			float num = (float)(Time.timeAsDouble - enterTime);
			float value = Duration.Value;
			bool flag;
			if (num > value)
			{
				num = value;
				flag = true;
			}
			else
			{
				flag = false;
			}
			float num2 = num / value;
			Vector3 position = Vector3.Lerp(startPos, endPos, num2);
			float num3 = JumpHeight.Value * JumpCurve.curve.Evaluate(num2);
			position.y += num3;
			translateObject.position = position;
			if (flag)
			{
				Finish();
			}
		}
	}
}
