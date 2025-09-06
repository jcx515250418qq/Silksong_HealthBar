using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Ease base action - don't use!")]
	public abstract class EaseFsmAction : FsmStateAction
	{
		protected delegate float EasingFunction(float start, float end, float value);

		public enum EaseType
		{
			easeInQuad = 0,
			easeOutQuad = 1,
			easeInOutQuad = 2,
			easeInCubic = 3,
			easeOutCubic = 4,
			easeInOutCubic = 5,
			easeInQuart = 6,
			easeOutQuart = 7,
			easeInOutQuart = 8,
			easeInQuint = 9,
			easeOutQuint = 10,
			easeInOutQuint = 11,
			easeInSine = 12,
			easeOutSine = 13,
			easeInOutSine = 14,
			easeInExpo = 15,
			easeOutExpo = 16,
			easeInOutExpo = 17,
			easeInCirc = 18,
			easeOutCirc = 19,
			easeInOutCirc = 20,
			linear = 21,
			spring = 22,
			bounce = 23,
			easeInBack = 24,
			easeOutBack = 25,
			easeInOutBack = 26,
			elastic = 27,
			punch = 28
		}

		[RequiredField]
		[Tooltip("How long the ease should take in seconds")]
		public FsmFloat time;

		[Tooltip("Optionally, use speed instead of time.")]
		public FsmFloat speed;

		[Tooltip("Optional delay in seconds before starting to ease.")]
		public FsmFloat delay;

		[Tooltip("The easing function to use.")]
		public EaseType easeType = EaseType.linear;

		[Tooltip("Reverse the ease.")]
		public FsmBool reverse;

		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		protected EasingFunction ease;

		protected float runningTime;

		protected float lastTime;

		protected float startTime;

		protected float deltaTime;

		protected float delayTime;

		protected float percentage;

		protected float[] fromFloats = new float[0];

		protected float[] toFloats = new float[0];

		protected float[] resultFloats = new float[0];

		protected bool finishAction;

		protected bool start;

		protected bool finished;

		protected bool isRunning;

		public override void Reset()
		{
			easeType = EaseType.linear;
			time = new FsmFloat
			{
				Value = 1f
			};
			delay = new FsmFloat
			{
				UseVariable = true
			};
			speed = new FsmFloat
			{
				UseVariable = true
			};
			reverse = new FsmBool
			{
				Value = false
			};
			realTime = false;
			finishEvent = null;
			ease = null;
			runningTime = 0f;
			lastTime = 0f;
			percentage = 0f;
			fromFloats = new float[0];
			toFloats = new float[0];
			resultFloats = new float[0];
			finishAction = false;
			start = false;
			finished = false;
			isRunning = false;
		}

		public override void OnEnter()
		{
			finished = false;
			isRunning = false;
			SetEasingFunction();
			runningTime = 0f;
			percentage = (reverse.IsNone ? 0f : (reverse.Value ? 1f : 0f));
			finishAction = false;
			startTime = FsmTime.RealtimeSinceStartup;
			lastTime = FsmTime.RealtimeSinceStartup - startTime;
			delayTime = (delay.IsNone ? 0f : (delayTime = delay.Value));
			start = true;
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			if (start && !isRunning)
			{
				if (delayTime >= 0f)
				{
					if (realTime)
					{
						deltaTime = FsmTime.RealtimeSinceStartup - startTime - lastTime;
						lastTime = FsmTime.RealtimeSinceStartup - startTime;
						delayTime -= deltaTime;
					}
					else
					{
						delayTime -= Time.deltaTime;
					}
				}
				else
				{
					isRunning = true;
					start = false;
					startTime = FsmTime.RealtimeSinceStartup;
					lastTime = FsmTime.RealtimeSinceStartup - startTime;
				}
			}
			if (!isRunning || finished)
			{
				return;
			}
			if (reverse.IsNone || !reverse.Value)
			{
				UpdatePercentage();
				if (percentage < 1f)
				{
					for (int i = 0; i < fromFloats.Length; i++)
					{
						resultFloats[i] = ease(fromFloats[i], toFloats[i], percentage);
					}
				}
				else
				{
					finishAction = true;
					finished = true;
					isRunning = false;
				}
				return;
			}
			UpdatePercentage();
			if (percentage > 0f)
			{
				for (int j = 0; j < fromFloats.Length; j++)
				{
					resultFloats[j] = ease(fromFloats[j], toFloats[j], percentage);
				}
			}
			else
			{
				finishAction = true;
				finished = true;
				isRunning = false;
			}
		}

		protected void UpdatePercentage()
		{
			if (realTime)
			{
				deltaTime = FsmTime.RealtimeSinceStartup - startTime - lastTime;
				lastTime = FsmTime.RealtimeSinceStartup - startTime;
				if (!speed.IsNone)
				{
					runningTime += deltaTime * speed.Value;
				}
				else
				{
					runningTime += deltaTime;
				}
			}
			else if (!speed.IsNone)
			{
				runningTime += Time.deltaTime * speed.Value;
			}
			else
			{
				runningTime += Time.deltaTime;
			}
			if (!reverse.IsNone && reverse.Value)
			{
				percentage = 1f - Mathf.Clamp01(runningTime / time.Value);
			}
			else
			{
				percentage = Mathf.Clamp01(runningTime / time.Value);
			}
		}

		protected void SetEasingFunction()
		{
			switch (easeType)
			{
			case EaseType.easeInQuad:
				ease = easeInQuad;
				break;
			case EaseType.easeOutQuad:
				ease = easeOutQuad;
				break;
			case EaseType.easeInOutQuad:
				ease = easeInOutQuad;
				break;
			case EaseType.easeInCubic:
				ease = easeInCubic;
				break;
			case EaseType.easeOutCubic:
				ease = easeOutCubic;
				break;
			case EaseType.easeInOutCubic:
				ease = easeInOutCubic;
				break;
			case EaseType.easeInQuart:
				ease = easeInQuart;
				break;
			case EaseType.easeOutQuart:
				ease = easeOutQuart;
				break;
			case EaseType.easeInOutQuart:
				ease = easeInOutQuart;
				break;
			case EaseType.easeInQuint:
				ease = easeInQuint;
				break;
			case EaseType.easeOutQuint:
				ease = easeOutQuint;
				break;
			case EaseType.easeInOutQuint:
				ease = easeInOutQuint;
				break;
			case EaseType.easeInSine:
				ease = easeInSine;
				break;
			case EaseType.easeOutSine:
				ease = easeOutSine;
				break;
			case EaseType.easeInOutSine:
				ease = easeInOutSine;
				break;
			case EaseType.easeInExpo:
				ease = easeInExpo;
				break;
			case EaseType.easeOutExpo:
				ease = easeOutExpo;
				break;
			case EaseType.easeInOutExpo:
				ease = easeInOutExpo;
				break;
			case EaseType.easeInCirc:
				ease = easeInCirc;
				break;
			case EaseType.easeOutCirc:
				ease = easeOutCirc;
				break;
			case EaseType.easeInOutCirc:
				ease = easeInOutCirc;
				break;
			case EaseType.linear:
				ease = linear;
				break;
			case EaseType.spring:
				ease = spring;
				break;
			case EaseType.bounce:
				ease = bounce;
				break;
			case EaseType.easeInBack:
				ease = easeInBack;
				break;
			case EaseType.easeOutBack:
				ease = easeOutBack;
				break;
			case EaseType.easeInOutBack:
				ease = easeInOutBack;
				break;
			case EaseType.elastic:
				ease = elastic;
				break;
			case EaseType.punch:
				ease = elastic;
				break;
			}
		}

		protected float linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		protected float clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((num2 - num) / 2f);
			float num4 = 0f;
			float num5 = 0f;
			if (end - start < 0f - num3)
			{
				num5 = (num2 - start + end) * value;
				return start + num5;
			}
			if (end - start > num3)
			{
				num5 = (0f - (num2 - end + start)) * value;
				return start + num5;
			}
			return start + (end - start) * value;
		}

		protected float spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * MathF.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		protected float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		protected float easeOutQuad(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * value * (value - 2f) + start;
		}

		protected float easeInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value + start;
			}
			value -= 1f;
			return (0f - end) / 2f * (value * (value - 2f) - 1f) + start;
		}

		protected float easeInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		protected float easeOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		protected float easeInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value + 2f) + start;
		}

		protected float easeInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		protected float easeOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return (0f - end) * (value * value * value * value - 1f) + start;
		}

		protected float easeInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value + start;
			}
			value -= 2f;
			return (0f - end) / 2f * (value * value * value * value - 2f) + start;
		}

		protected float easeInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		protected float easeOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		protected float easeInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end / 2f * (value * value * value * value * value + 2f) + start;
		}

		protected float easeInSine(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * Mathf.Cos(value / 1f * (MathF.PI / 2f)) + end + start;
		}

		protected float easeOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value / 1f * (MathF.PI / 2f)) + start;
		}

		protected float easeInOutSine(float start, float end, float value)
		{
			end -= start;
			return (0f - end) / 2f * (Mathf.Cos(MathF.PI * value / 1f) - 1f) + start;
		}

		protected float easeInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
		}

		protected float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (0f - Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
		}

		protected float easeInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end / 2f * (0f - Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		protected float easeInCirc(float start, float end, float value)
		{
			end -= start;
			return (0f - end) * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		protected float easeOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		protected float easeInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return (0f - end) / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		protected float bounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 21f / 22f;
			return end * (7.5625f * value * value + 63f / 64f) + start;
		}

		protected float easeInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		protected float easeOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value = value / 1f - 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		protected float easeInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		protected float punch(float amplitude, float value)
		{
			float num = 9f;
			if (value == 0f)
			{
				return 0f;
			}
			if (value == 1f)
			{
				return 0f;
			}
			float num2 = 0.3f;
			num = num2 / (MathF.PI * 2f) * Mathf.Asin(0f);
			return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num) * (MathF.PI * 2f) / num2);
		}

		protected float elastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			if (num4 == 0f || num4 < Mathf.Abs(end))
			{
				num4 = end;
				num3 = num2 / 4f;
			}
			else
			{
				num3 = num2 / (MathF.PI * 2f) * Mathf.Asin(end / num4);
			}
			return num4 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num3) * (MathF.PI * 2f) / num2) + end + start;
		}
	}
}
