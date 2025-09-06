using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[Tooltip("Randomly shakes a GameObject's position by a diminishing amount over time.")]
	public class ShakePosition : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 extents;

		public FsmFloat duration;

		public FsmBool isLooping;

		public FsmEvent stopEvent;

		private float timer;

		private Transform target;

		private Vector3 startingWorldPosition;

		public override void Reset()
		{
			base.Reset();
			gameObject = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			duration = 1f;
			isLooping = false;
			stopEvent = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			timer = 0f;
			GameObject safe = gameObject.GetSafe(this);
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
				bool value = isLooping.Value;
				float num = Mathf.Clamp01(1f - timer / duration.Value);
				Vector3 vector = Vector3.Scale(extents.Value, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
				target.position = startingWorldPosition + vector * (value ? 1f : num);
				timer += Time.deltaTime;
				if (!value && timer > duration.Value)
				{
					StopAndReset();
					base.Fsm.Event(stopEvent);
					Finish();
				}
			}
			else
			{
				StopAndReset();
				base.Fsm.Event(stopEvent);
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
