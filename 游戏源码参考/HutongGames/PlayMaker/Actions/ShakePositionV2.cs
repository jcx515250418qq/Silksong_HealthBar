using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[Tooltip("Randomly shakes a GameObject's position by a diminishing amount over time.")]
	public class ShakePositionV2 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmVector3 Extents;

		public FsmFloat Duration;

		public FsmBool IsLooping;

		public FsmEvent StopEvent;

		public FsmFloat FpsLimit;

		public FsmBool IsCameraShake;

		private float timer;

		private double nextUpdateTime;

		private Transform target;

		private Vector3 startingWorldPosition;

		public override void Reset()
		{
			base.Reset();
			Target = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			Extents = null;
			Duration = 1f;
			IsLooping = false;
			StopEvent = null;
			FpsLimit = null;
			IsCameraShake = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			timer = 0f;
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				target = safe.transform;
				startingWorldPosition = target.position;
			}
			else
			{
				target = null;
			}
			UpdateShaking();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			UpdateShaking();
		}

		public override void OnExit()
		{
			StopAndReset();
			base.OnExit();
		}

		private void UpdateShaking()
		{
			if (target != null)
			{
				timer += Time.deltaTime;
				if (FpsLimit.Value > 0f)
				{
					if (Time.unscaledTimeAsDouble < nextUpdateTime)
					{
						return;
					}
					nextUpdateTime = Time.unscaledTimeAsDouble + (double)(1f / FpsLimit.Value);
				}
				bool value = IsLooping.Value;
				float num = (value ? 1f : Mathf.Clamp01(1f - timer / Duration.Value));
				if (IsCameraShake.Value)
				{
					num *= CameraShakeManager.ShakeMultiplier;
				}
				Vector3 vector = Vector3.Scale(Extents.Value, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
				target.position = startingWorldPosition + vector * num;
				if (!value && timer > Duration.Value)
				{
					StopAndReset();
					base.Fsm.Event(StopEvent);
					Finish();
				}
			}
			else
			{
				StopAndReset();
				base.Fsm.Event(StopEvent);
				Finish();
			}
		}

		private void StopAndReset()
		{
			if (target != null)
			{
				target.position = startingWorldPosition;
				target = null;
			}
		}
	}
}
