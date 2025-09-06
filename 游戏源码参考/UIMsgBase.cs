using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

public abstract class UIMsgBase<TTargetObject> : UIMsgProxy
{
	protected const float VIBRATION_FADE_DURATION = 0.25f;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float startPauseTime;

	[SerializeField]
	private GameObject stop;

	private static readonly int _appearSpeedProp = Animator.StringToHash("Appear Speed");

	private static readonly int _fadeUpProp = Animator.StringToHash("Fade Up");

	private static readonly int _showProp = Animator.StringToHash("Show");

	private static readonly int _hideProp = Animator.StringToHash("Hide");

	protected virtual int GetHideAnimId()
	{
		return _hideProp;
	}

	protected static UIMsgBase<TTargetObject> Spawn(TTargetObject item, UIMsgBase<TTargetObject> prefab, Action afterMsg)
	{
		if (!prefab)
		{
			return null;
		}
		UIMsgBase<TTargetObject> uIMsgBase = UnityEngine.Object.Instantiate(prefab);
		uIMsgBase.StartCoroutine(uIMsgBase.DoMsg(item, afterMsg));
		return uIMsgBase;
	}

	private IEnumerator DoMsg(TTargetObject item, Action afterMsg)
	{
		SetIsInMsg(value: true);
		Setup(item);
		VibrationManager.FadeVibration(0f, 0.25f);
		if ((bool)stop)
		{
			stop.SetActive(value: false);
		}
		Color colour = ScreenFaderUtils.GetColour();
		bool flag = colour.r < 0.01f && colour.g < 0.01f && colour.b < 0.01f && colour.a > 0.99f;
		if ((bool)animator)
		{
			if (!flag && animator.HasState(0, _fadeUpProp))
			{
				animator.Play(_fadeUpProp);
				yield return null;
				if (startPauseTime > 0f)
				{
					animator.SetFloat(_appearSpeedProp, 0f);
					yield return new WaitForSeconds(startPauseTime);
					animator.SetFloat(_appearSpeedProp, 1f);
					yield return null;
				}
				float length = animator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
			}
			animator.Play(_showProp);
			yield return null;
			float length2 = animator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length2);
		}
		if ((bool)stop)
		{
			stop.SetActive(value: true);
		}
		bool wasPressed = false;
		while (!wasPressed)
		{
			wasPressed = GameManager.instance.inputHandler.WasSkipButtonPressed;
			yield return null;
		}
		Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if ((bool)stop)
		{
			stop.SetActive(value: false);
		}
		VibrationManager.FadeVibration(1f, 0.25f);
		if ((bool)animator)
		{
			animator.Play(GetHideAnimId());
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		SetIsInMsg(value: false);
		afterMsg?.Invoke();
		base.gameObject.SetActive(value: false);
	}

	protected abstract void Setup(TTargetObject targetObject);
}
