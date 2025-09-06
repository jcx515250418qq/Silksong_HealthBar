using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PulseOpacity : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmFloat MinOpacity;

		public FsmFloat MaxOpacity;

		public FsmFloat MinSpeed;

		public FsmFloat MaxSpeed;

		public FsmAnimationCurve PulseCurve;

		public FsmFloat IncreaseDuration;

		public FsmAnimationCurve IncreaseCurve;

		private float elapsedTime;

		private tk2dSprite sprite;

		public override void Reset()
		{
			MinOpacity = 0f;
			MaxOpacity = 1f;
			MinSpeed = 1f;
			MaxSpeed = 2f;
			IncreaseDuration = 1f;
			PulseCurve = new FsmAnimationCurve
			{
				curve = new AnimationCurve
				{
					keys = new Keyframe[3]
					{
						new Keyframe(0f, 1f),
						new Keyframe(0.5f, 0f),
						new Keyframe(1f, 1f)
					},
					preWrapMode = WrapMode.Loop,
					postWrapMode = WrapMode.Loop
				}
			};
			IncreaseCurve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				sprite = safe.GetComponent<tk2dSprite>();
				if ((bool)sprite)
				{
					elapsedTime = 0f;
					UpdateOpacity();
					return;
				}
			}
			Finish();
		}

		public override void OnUpdate()
		{
			float progress = GetProgress();
			float num = Mathf.Lerp(MinSpeed.Value, MaxSpeed.Value, IncreaseCurve.curve.Evaluate(progress));
			elapsedTime += Time.deltaTime * num;
			UpdateOpacity();
		}

		private void UpdateOpacity()
		{
			float t = PulseCurve.curve.Evaluate(elapsedTime);
			float a = Mathf.Lerp(MinOpacity.Value, MaxOpacity.Value, t);
			Color color = sprite.color;
			color.a = a;
			sprite.color = color;
		}

		public override float GetProgress()
		{
			if (IncreaseDuration.Value <= 0f)
			{
				return 1f;
			}
			return base.State.StateTime / IncreaseDuration.Value;
		}
	}
}
