using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class LerpObjectPosition : FsmStateAction
	{
		[Tooltip("Will use target's current position if not set.")]
		public FsmOwnerDefault fromPosition;

		[RequiredField]
		public FsmOwnerDefault toPosition;

		[RequiredField]
		public FsmOwnerDefault target;

		public FsmFloat duration;

		public FsmBool resetOnExit;

		public FsmBool setFinalPositionOnExit;

		public FsmBool useLocalPosition;

		private GameObject fromGO;

		private GameObject toGO;

		private GameObject targetGO;

		private Vector3 fromPositionValue;

		private float t;

		private float inverDuration;

		private bool useLocal;

		public override void Reset()
		{
			fromPosition = null;
			toPosition = null;
			target = null;
			duration = null;
			resetOnExit = null;
			setFinalPositionOnExit = null;
		}

		public override void OnEnter()
		{
			fromGO = fromPosition.GetSafe(this);
			toGO = toPosition.GetSafe(this);
			targetGO = target.GetSafe(this);
			bool flag = fromGO != null;
			bool flag2 = toGO != null;
			bool flag3 = targetGO != null;
			useLocal = useLocalPosition.Value;
			if (duration.Value <= 0f && flag2 && flag3)
			{
				if (useLocal)
				{
					targetGO.transform.localPosition = toGO.transform.localPosition;
				}
				else
				{
					targetGO.transform.position = toGO.transform.position;
				}
				return;
			}
			if (flag3)
			{
				if (useLocal)
				{
					fromPositionValue = targetGO.transform.localPosition;
				}
				else
				{
					fromPositionValue = targetGO.transform.position;
				}
			}
			if (flag)
			{
				if (useLocal)
				{
					fromPositionValue = fromGO.transform.localPosition;
				}
				else
				{
					fromPositionValue = fromGO.transform.position;
				}
			}
			if (!flag2 || !flag3)
			{
				Finish();
				return;
			}
			t = 0f;
			inverDuration = 1f / duration.Value;
		}

		public override void OnExit()
		{
			if (setFinalPositionOnExit.Value && toGO != null)
			{
				if (targetGO != null)
				{
					if (useLocal)
					{
						targetGO.transform.localPosition = toGO.transform.localPosition;
					}
					else
					{
						targetGO.transform.position = toGO.transform.position;
					}
				}
			}
			else if (resetOnExit.Value && targetGO != null)
			{
				if (useLocal)
				{
					targetGO.transform.localPosition = fromPositionValue;
				}
				else
				{
					targetGO.transform.position = fromPositionValue;
				}
			}
		}

		public override void OnUpdate()
		{
			while (t < 1f)
			{
				t += inverDuration * Time.deltaTime;
				if (useLocal)
				{
					targetGO.transform.localPosition = Vector3.Lerp(fromPositionValue, toGO.transform.localPosition, Mathf.Clamp01(t));
				}
				else
				{
					targetGO.transform.position = Vector3.Lerp(fromPositionValue, toGO.transform.position, Mathf.Clamp01(t));
				}
				if (t >= 1f)
				{
					Finish();
				}
			}
		}
	}
}
