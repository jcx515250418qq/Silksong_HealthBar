using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[Tooltip("Scales a transform to a level.")]
	public class ScaleTo : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 target;

		public FsmFloat duration;

		public FsmFloat delay;

		public ScaleToCurves curve;

		private float timer;

		private Transform transform;

		private Vector3 startScale;

		public override void Reset()
		{
			base.Reset();
			gameObject = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			target = null;
			duration = 1f;
			delay = 0f;
			curve = ScaleToCurves.Linear;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			timer = 0f;
			GameObject safe = gameObject.GetSafe(this);
			if (safe != null)
			{
				transform = safe.transform;
				startScale = transform.localScale;
			}
			else
			{
				transform = null;
			}
			UpdateScaling();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			UpdateScaling();
		}

		private void UpdateScaling()
		{
			if (transform != null)
			{
				timer += Time.deltaTime;
				float curved = GetCurved(Mathf.Clamp01((timer - delay.Value) / duration.Value), curve);
				transform.localScale = Vector3.Lerp(startScale, target.Value, curved);
				if (timer > duration.Value + delay.Value)
				{
					transform.localScale = target.Value;
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		private static float GetCurved(float val, ScaleToCurves curve)
		{
			return curve switch
			{
				ScaleToCurves.QuadraticOut => QuadraticOut(val), 
				ScaleToCurves.SinusoidalOut => SinusoidalOut(val), 
				_ => Linear(val), 
			};
		}

		private static float Linear(float val)
		{
			return val;
		}

		private static float QuadraticOut(float val)
		{
			return val * (2f - val);
		}

		private static float SinusoidalOut(float val)
		{
			return Mathf.Sin(val * MathF.PI * 0.5f);
		}
	}
}
