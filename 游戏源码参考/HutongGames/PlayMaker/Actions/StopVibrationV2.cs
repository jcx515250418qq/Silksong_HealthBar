using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class StopVibrationV2 : FsmStateAction
	{
		public FsmString tag;

		public FsmFloat fadeTime;

		public FsmBool waitUntilFinish;

		private List<VibrationEmission> emissions = new List<VibrationEmission>();

		public override void Reset()
		{
			base.Reset();
			tag = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			bool flag = true;
			if (fadeTime.Value > 0f)
			{
				emissions.Clear();
				VibrationManager.GetVibrationsWithTag(tag.Value, emissions);
				if (emissions.Count > 0)
				{
					if (waitUntilFinish.Value)
					{
						flag = false;
					}
					StartCoroutine(FadeEmissions());
				}
			}
			else if (tag == null || tag.IsNone || string.IsNullOrEmpty(tag.Value))
			{
				VibrationManager.StopAllVibration();
			}
			else
			{
				VibrationManager.GetMixer()?.StopAllEmissionsWithTag(tag.Value);
			}
			if (flag)
			{
				Finish();
			}
		}

		private IEnumerator FadeEmissions()
		{
			float t = 0f;
			float inverse = 1f / fadeTime.Value;
			while (t < 1f)
			{
				t += Time.deltaTime * inverse;
				float strength = Mathf.Clamp01(1f - t);
				for (int num = emissions.Count - 1; num >= 0; num--)
				{
					VibrationEmission vibrationEmission = emissions[num];
					if (!vibrationEmission.IsPlaying)
					{
						emissions.RemoveAt(num);
					}
					else
					{
						vibrationEmission.SetStrength(strength);
					}
				}
				yield return null;
			}
			for (int num2 = emissions.Count - 1; num2 >= 0; num2--)
			{
				emissions[num2].Stop();
			}
			emissions.Clear();
			if (waitUntilFinish.Value)
			{
				Finish();
			}
		}
	}
}
