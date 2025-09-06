using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(AudioSource), "", false)]
	[Tooltip("Tween common AudioSource properties.")]
	public class TweenAudio : TweenComponentBase<AudioSource>
	{
		public enum AudioProperty
		{
			Volume = 0,
			Pitch = 1
		}

		[Tooltip("Audio property to tween.")]
		public AudioProperty property;

		[Tooltip("Tween To/From values set below.")]
		public TweenDirection tweenDirection;

		[Tooltip("Value for the selected property.")]
		public FsmFloat value;

		private AudioSource audio;

		private float fromFloat;

		private float toFloat;

		public override void Reset()
		{
			base.Reset();
			property = AudioProperty.Volume;
			tweenDirection = TweenDirection.To;
			value = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			audio = cachedComponent;
			if (tweenDirection == TweenDirection.From)
			{
				switch (property)
				{
				case AudioProperty.Volume:
					fromFloat = value.Value;
					toFloat = audio.volume;
					break;
				case AudioProperty.Pitch:
					fromFloat = value.Value;
					toFloat = audio.pitch;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (property)
				{
				case AudioProperty.Volume:
					fromFloat = audio.volume;
					toFloat = value.Value;
					break;
				case AudioProperty.Pitch:
					fromFloat = audio.pitch;
					toFloat = value.Value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			switch (property)
			{
			case AudioProperty.Volume:
				audio.volume = Mathf.Lerp(fromFloat, toFloat, t);
				break;
			case AudioProperty.Pitch:
				audio.pitch = Mathf.Lerp(fromFloat, toFloat, t);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
