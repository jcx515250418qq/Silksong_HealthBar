using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Extensions
{
	public enum IntTest
	{
		Equal = 0,
		LessThan = 1,
		MoreThan = 2,
		LessThanOrEqual = 3,
		MoreThanOrEqual = 4
	}

	public static Selectable GetFirstInteractable(this Selectable start)
	{
		if (start == null)
		{
			return null;
		}
		if (start.interactable)
		{
			return start;
		}
		return start.navigation.selectOnDown.GetFirstInteractable();
	}

	public static void PlayOnSource(this AudioClip self, AudioSource source, float pitchMin = 1f, float pitchMax = 1f)
	{
		if ((bool)self && (bool)source)
		{
			source.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
			source.PlayOneShot(self);
		}
	}

	public static void SetActiveChildren(this GameObject self, bool value)
	{
		int childCount = self.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			self.transform.GetChild(i).gameObject.SetActive(value);
		}
	}

	public static void SetActiveWithChildren(this MeshRenderer self, bool value)
	{
		if (self.transform.childCount > 0)
		{
			MeshRenderer[] componentsInChildren = self.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = value;
			}
		}
		else
		{
			self.enabled = value;
		}
	}

	private static AnimatorControllerParameter[] GetAnimatorParameters(Animator animator)
	{
		if (0 == 0 && Application.isPlaying)
		{
			return animator.parameters;
		}
		return null;
	}

	public static bool HasParameter(this Animator self, string paramName, AnimatorControllerParameterType? type = null)
	{
		AnimatorControllerParameter[] animatorParameters = GetAnimatorParameters(self);
		if (animatorParameters == null)
		{
			return false;
		}
		AnimatorControllerParameter[] array = animatorParameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in array)
		{
			if (animatorControllerParameter.name == paramName && (!type.HasValue || animatorControllerParameter.type == type))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasParameter(this Animator self, int paramID, AnimatorControllerParameterType? type = null)
	{
		AnimatorControllerParameter[] animatorParameters = GetAnimatorParameters(self);
		if (animatorParameters == null)
		{
			return false;
		}
		AnimatorControllerParameter[] array = animatorParameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in array)
		{
			if (animatorControllerParameter.nameHash == paramID && (!type.HasValue || animatorControllerParameter.type == type))
			{
				return true;
			}
		}
		return false;
	}

	public static bool SetFloatIfExists(this Animator self, int id, float value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, AnimatorControllerParameterType.Float))
		{
			self.SetFloat(id, value);
			return true;
		}
		return false;
	}

	public static bool SetIntIfExists(this Animator self, int id, int value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, AnimatorControllerParameterType.Int))
		{
			self.SetInteger(id, value);
			return true;
		}
		return false;
	}

	public static bool SetBoolIfExists(this Animator self, int id, bool value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, AnimatorControllerParameterType.Bool))
		{
			self.SetBool(id, value);
			return true;
		}
		return false;
	}

	public static bool SetTriggerIfExists(this Animator self, int id)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, AnimatorControllerParameterType.Trigger))
		{
			self.SetTrigger(id);
			return true;
		}
		return false;
	}

	public static IEnumerator PlayAnimWait(this tk2dSpriteAnimator self, string anim, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> triggerAction = null)
	{
		return self.PlayAnimWait(self.GetClipByName(anim), triggerAction);
	}

	public static IEnumerator PlayAnimWait(this tk2dSpriteAnimator self, tk2dSpriteAnimationClip clip, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> triggerAction = null)
	{
		if (clip != null)
		{
			self.Play(clip);
			if (triggerAction != null)
			{
				self.AnimationEventTriggered = triggerAction;
			}
			WaitForTk2dAnimatorClipFinish waitForTk2dAnimatorClipFinish = new WaitForTk2dAnimatorClipFinish(self);
			while (self.CurrentClip == clip && self.IsPlaying(clip) && waitForTk2dAnimatorClipFinish.keepWaiting)
			{
				yield return null;
			}
		}
	}

	public static float PlayAnimGetTime(this tk2dSpriteAnimator self, string anim)
	{
		return self.PlayAnimGetTime(self.GetClipByName(anim));
	}

	public static float PlayAnimGetTime(this tk2dSpriteAnimator self, tk2dSpriteAnimationClip clip)
	{
		if (clip == null)
		{
			return 0f;
		}
		self.Play(clip);
		return clip.Duration;
	}

	public static bool TryPlay(this tk2dSpriteAnimator self, string anim)
	{
		if (string.IsNullOrEmpty(anim))
		{
			return false;
		}
		tk2dSpriteAnimationClip clipByName = self.GetClipByName(anim);
		if (clipByName == null)
		{
			return false;
		}
		self.Play(clipByName);
		return true;
	}

	public static void SetPositionX(this Transform t, float newX)
	{
		Vector3 position = t.position;
		position = new Vector3(newX, position.y, position.z);
		t.position = position;
	}

	public static void SetPositionY(this Transform t, float newY)
	{
		Vector3 position = t.position;
		position = new Vector3(position.x, newY, position.z);
		t.position = position;
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		Vector3 position = t.position;
		position = new Vector3(position.x, position.y, newZ);
		t.position = position;
	}

	public static void SetLocalPositionX(this Transform t, float newX)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(newX, localPosition.y, localPosition.z);
		t.localPosition = localPosition;
	}

	public static void SetLocalPositionY(this Transform t, float newY)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(localPosition.x, newY, localPosition.z);
		t.localPosition = localPosition;
	}

	public static void SetLocalPositionZ(this Transform t, float newZ)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(localPosition.x, localPosition.y, newZ);
		t.localPosition = localPosition;
	}

	public static void SetPosition2D(this Transform t, float x, float y)
	{
		t.position = new Vector3(x, y, t.position.z);
	}

	public static void SetPosition2D(this Transform t, Vector2 position)
	{
		t.position = new Vector3(position.x, position.y, t.position.z);
	}

	public static void SetLocalPosition2D(this Transform t, float x, float y)
	{
		t.localPosition = new Vector3(x, y, t.localPosition.z);
	}

	public static void SetLocalPosition2D(this Transform t, Vector2 position)
	{
		t.localPosition = new Vector3(position.x, position.y, t.localPosition.z);
	}

	public static void SetPosition3D(this Transform t, float x, float y, float z)
	{
		t.position = new Vector3(x, y, z);
	}

	public static void SetScaleX(this Transform t, float newXScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(newXScale, localScale.y, localScale.z);
		t.localScale = localScale;
	}

	public static void SetScaleY(this Transform t, float newYScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(localScale.x, newYScale, localScale.z);
		t.localScale = localScale;
	}

	public static void SetScaleZ(this Transform t, float newZScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(localScale.x, localScale.y, newZScale);
		t.localScale = localScale;
	}

	public static void SetScale2D(this Transform t, Vector2 newScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(newScale.x, newScale.y, localScale.z);
		t.localScale = localScale;
	}

	public static void SetRotationZ(this Transform t, float newZRotation)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, newZRotation);
		t.localEulerAngles = localEulerAngles;
	}

	public static void SetScaleMatching(this Transform t, float newScale)
	{
		t.localScale = new Vector3(newScale, newScale, newScale);
	}

	public static void FlipLocalScale(this Transform t, bool x = false, bool y = false, bool z = false)
	{
		Vector3 localScale = t.localScale;
		if (x)
		{
			localScale.x = 0f - localScale.x;
		}
		if (y)
		{
			localScale.y = 0f - localScale.y;
		}
		if (z)
		{
			localScale.z = 0f - localScale.z;
		}
		t.localScale = localScale;
	}

	public static void SetParentReset(this Transform t, Transform parent)
	{
		t.SetParent(parent);
		t.Reset();
	}

	public static void Reset(this Transform t)
	{
		t.localScale = Vector3.one;
		t.localRotation = Quaternion.identity;
		t.localPosition = Vector3.zero;
	}

	public static bool IsOnHeroPlane(this Transform transform)
	{
		return Mathf.Abs(transform.position.z - 0.004f) <= 1.8f;
	}

	public static float GetPositionX(this Transform t)
	{
		return t.position.x;
	}

	public static float GetPositionY(this Transform t)
	{
		return t.position.y;
	}

	public static float GetPositionZ(this Transform t)
	{
		return t.position.z;
	}

	public static float GetScaleX(this Transform t)
	{
		return t.localScale.x;
	}

	public static float GetScaleY(this Transform t)
	{
		return t.localScale.y;
	}

	public static float GetScaleZ(this Transform t)
	{
		return t.localScale.z;
	}

	public static float GetLocalRotation2D(this Transform t)
	{
		return t.localEulerAngles.z;
	}

	public static float GetRotation2D(this Transform t)
	{
		return t.eulerAngles.z;
	}

	public static void SetLocalRotation2D(this Transform t, float rotation)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		localEulerAngles.z = rotation;
		t.localEulerAngles = localEulerAngles;
	}

	public static void SetRotation2D(this Transform t, float rotation)
	{
		Vector3 eulerAngles = t.eulerAngles;
		eulerAngles.z = rotation;
		t.eulerAngles = eulerAngles;
	}

	public static float TransformRadius(this Transform t, float radius)
	{
		Vector3 lossyScale = t.lossyScale;
		float num = Mathf.Abs(Mathf.Max(lossyScale.x, lossyScale.y));
		return radius * num;
	}

	public static bool IsWithinGameCameraBoundsDistance(this Transform transform, Vector2 margins)
	{
		Vector3 position = GameCameras.instance.mainCamera.transform.position;
		Vector3 position2 = transform.position;
		float num = position2.x - position.x;
		float num2 = position2.y - position.y;
		if (num < 0f)
		{
			num *= -1f;
		}
		if (num2 < 0f)
		{
			num2 *= -1f;
		}
		if (num <= 15f + margins.x)
		{
			return num2 <= 9f + margins.y;
		}
		return false;
	}

	public static bool IsAny(this string value, params string[] others)
	{
		foreach (string value2 in others)
		{
			if (value.Equals(value2))
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsAny(this string value, params string[] others)
	{
		foreach (string value2 in others)
		{
			if (value.Contains(value2))
			{
				return true;
			}
		}
		return false;
	}

	public static bool TryFormat(this string text, out string outText, params object[] formatItems)
	{
		try
		{
			outText = string.Format(text, formatItems);
			return true;
		}
		catch (FormatException)
		{
			outText = text;
			return false;
		}
	}

	public static string ToSingleLine(this string multilineText)
	{
		LanguageCode languageCode = Language.CurrentLanguage();
		string text = ((languageCode != LanguageCode.JA && languageCode != LanguageCode.ZH) ? " " : "");
		string newValue = text;
		return multilineText.Replace("\n", newValue);
	}

	public static T GetBy2DIndexes<T>(this List<T> list, int width, int x, int y, T def = default(T))
	{
		int num = y * width + x;
		if (num < 0 || num >= list.Count || x >= width || x < 0)
		{
			return def;
		}
		return list[num];
	}

	public static void SendEventSafe(this PlayMakerFSM fsm, string eventName)
	{
		if (fsm != null)
		{
			fsm.SendEvent(eventName);
		}
	}

	public static Color Where(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
	{
		return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
	}

	public static Vector3 Where(this Vector3 original, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
	}

	public static Vector2 Where(this Vector2 original, float? x = null, float? y = null)
	{
		return new Vector2(x ?? original.x, y ?? original.y);
	}

	public static Vector3 ToVector3(this Vector2 original, float z)
	{
		return new Vector3(original.x, original.y, z);
	}

	public static Coroutine StartTimerRoutine(this MonoBehaviour self, float delay, float duration, Action<float> handler, Action onAfterDelay = null, Action onTimerEnd = null, bool isRealtime = false)
	{
		if (duration <= 0f && delay <= 0f)
		{
			TimerRoutine(delay, duration, handler, onAfterDelay, onTimerEnd, isRealtime).MoveNext();
			return null;
		}
		return self.StartCoroutine(TimerRoutine(delay, duration, handler, onAfterDelay, onTimerEnd, isRealtime));
	}

	private static IEnumerator TimerRoutine(float delay, float duration, Action<float> handler, Action onAfterDelay, Action onTimerEnd, bool isRealtime)
	{
		handler?.Invoke(0f);
		if (delay > 0f)
		{
			if (isRealtime)
			{
				yield return new WaitForSecondsRealtime(delay);
			}
			else
			{
				yield return new WaitForSeconds(delay);
			}
		}
		onAfterDelay?.Invoke();
		if (handler != null)
		{
			for (float elapsed = 0f; elapsed < duration; elapsed = ((!isRealtime) ? (elapsed + Time.deltaTime) : (elapsed + Time.unscaledDeltaTime)))
			{
				handler(elapsed / duration);
				yield return null;
			}
			handler(1f);
		}
		else if (duration > 0f)
		{
			if (isRealtime)
			{
				yield return new WaitForSecondsRealtime(duration);
			}
			else
			{
				yield return new WaitForSeconds(duration);
			}
		}
		onTimerEnd?.Invoke();
	}

	public static Coroutine ExecuteDelayed(this MonoBehaviour runner, float delay, Action handler)
	{
		if (handler == null)
		{
			return null;
		}
		return runner.StartCoroutine(DelayRoutine(delay, handler));
	}

	private static IEnumerator DelayRoutine(float delay, Action handler)
	{
		yield return new WaitForSeconds(delay);
		handler();
	}

	public static void SetFsmBoolIfExists(this PlayMakerFSM fsm, string boolName, bool value)
	{
		FsmBool fsmBool = fsm.FsmVariables.FindFsmBool(boolName);
		if (fsmBool != null)
		{
			fsmBool.Value = value;
		}
	}

	public static bool GetFsmBoolIfExists(this PlayMakerFSM fsm, string boolName)
	{
		return fsm.FsmVariables.FindFsmBool(boolName)?.Value ?? false;
	}

	public static bool IsEventValid(this PlayMakerFSM fsm, string eventName)
	{
		bool? flag = fsm.IsEventValid(eventName, isRequired: true);
		if (flag.HasValue)
		{
			return flag.Value;
		}
		return false;
	}

	public static bool? IsEventValid(this PlayMakerFSM fsmComponent, string eventName, bool isRequired)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			if (!isRequired)
			{
				return null;
			}
			return false;
		}
		if (!fsmComponent)
		{
			return null;
		}
		return IsEventInFsmRecursive((fsmComponent.FsmTemplate == null) ? fsmComponent.Fsm : fsmComponent.FsmTemplate.fsm, eventName);
	}

	private static bool IsEventInFsmRecursive(Fsm fsm, string eventName)
	{
		FsmEvent[] events = fsm.Events;
		for (int i = 0; i < events.Length; i++)
		{
			if (events[i].Name == eventName)
			{
				return true;
			}
		}
		foreach (Fsm subFsm in fsm.SubFsmList)
		{
			if (IsEventInFsmRecursive(subFsm, eventName))
			{
				return true;
			}
		}
		return false;
	}

	public static bool SendEventRecursive(this PlayMakerFSM fsm, string eventName)
	{
		if (!fsm)
		{
			return false;
		}
		return fsm.Fsm.SendEventRecursive(eventName);
	}

	public static bool SendEventRecursive(this Fsm fsm, string eventName)
	{
		if (!fsm.Started)
		{
			return false;
		}
		foreach (Fsm subFsm in fsm.SubFsmList)
		{
			if (subFsm.Active && subFsm.SendEventRecursive(eventName))
			{
				return true;
			}
		}
		string activeStateName = fsm.ActiveStateName;
		FsmState activeState = fsm.ActiveState;
		int num;
		int num2;
		if (activeState != null)
		{
			num = activeState.ActiveActionIndex;
			num2 = activeState.loopCount;
		}
		else
		{
			num = -1;
			num2 = -1;
		}
		fsm.Event(eventName);
		FsmState activeState2 = fsm.ActiveState;
		if (activeState2 != null)
		{
			if (activeState2.loopCount != num2)
			{
				return true;
			}
			if (activeState2.ActiveActionIndex != num)
			{
				return true;
			}
		}
		return !fsm.ActiveStateName.Equals(activeStateName);
	}

	public static bool? IsAnimValid(this tk2dSpriteAnimator animator, string animName, bool isRequired)
	{
		if (string.IsNullOrEmpty(animName))
		{
			if (!isRequired)
			{
				return null;
			}
			return false;
		}
		if (!animator)
		{
			return false;
		}
		return animator.GetClipByName(animName) != null;
	}

	public static bool? IsVariableValid(this PlayMakerFSM fsm, string variableName, bool isRequired)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			if (!isRequired)
			{
				return null;
			}
			return false;
		}
		if (!fsm)
		{
			return null;
		}
		return ((fsm.FsmTemplate == null) ? fsm.FsmVariables : fsm.FsmTemplate.fsm.Variables).Contains(variableName);
	}

	public static bool IsWithinTolerance(this float original, float tolerance, float target)
	{
		return Mathf.Abs(target - original) <= tolerance;
	}

	public static bool IsAngleWithinTolerance(this float original, float tolerance, float target)
	{
		original = (original + 360f) % 360f;
		target = (target + 360f) % 360f;
		float num = Mathf.Abs(target - original);
		num = Mathf.Min(num, 360f - num);
		return num <= tolerance;
	}

	public static float DirectionToAngle(this Vector2 direction)
	{
		return Mathf.Atan2(direction.x, 0f - direction.y) * 180f / MathF.PI - 90f;
	}

	public static Vector2 AngleToDirection(this float angle)
	{
		float f = angle * (MathF.PI / 180f);
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
	}

	public static Vector3 MultiplyElements(this Vector3 self, Vector3 other)
	{
		Vector3 result = self;
		result.x *= other.x;
		result.y *= other.y;
		result.z *= other.z;
		return result;
	}

	public static Vector3 MultiplyElements(this Vector3 self, float? x = null, float? y = null, float? z = null)
	{
		Vector3 result = self;
		result.x *= x ?? 1f;
		result.y *= y ?? 1f;
		result.z *= z ?? 1f;
		return result;
	}

	public static Vector2 MultiplyElements(this Vector2 self, Vector2 other)
	{
		Vector2 result = self;
		result.x *= other.x;
		result.y *= other.y;
		return result;
	}

	public static Vector2 MultiplyElements(this Vector2 self, float? x = null, float? y = null)
	{
		Vector2 result = self;
		result.x *= x ?? 1f;
		result.y *= y ?? 1f;
		return result;
	}

	public static Vector2 ClampVector2(this Vector2 self, Vector2 min, Vector2 max)
	{
		Vector2 result = default(Vector2);
		result.x = Mathf.Clamp(self.x, min.x, max.x);
		result.y = Mathf.Clamp(self.y, min.y, max.y);
		return result;
	}

	public static Vector4 MultiplyElements(this Vector4 self, Vector4 other)
	{
		Vector4 result = self;
		result.x *= other.x;
		result.y *= other.y;
		result.z *= other.z;
		result.w += other.w;
		return result;
	}

	public static Vector4 MultiplyElements(this Vector4 self, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		Vector4 result = self;
		result.x *= x ?? 1f;
		result.y *= y ?? 1f;
		result.z *= z ?? 1f;
		result.w *= w ?? 1f;
		return result;
	}

	public static Vector3 DivideElements(this Vector3 self, Vector3 other)
	{
		Vector3 result = self;
		result.x /= other.x;
		result.y /= other.y;
		result.z /= other.z;
		return result;
	}

	public static Vector3 DivideElements(this Vector3 self, float? x = null, float? y = null, float? z = null)
	{
		Vector3 result = self;
		result.x /= x ?? 1f;
		result.y /= y ?? 1f;
		result.z /= z ?? 1f;
		return result;
	}

	public static Vector2 DivideElements(this Vector2 self, Vector2 other)
	{
		Vector2 result = self;
		result.x /= other.x;
		result.y /= other.y;
		return result;
	}

	public static Vector2 DivideElements(this Vector2 self, float? x = null, float? y = null)
	{
		Vector2 result = self;
		result.x /= x ?? 1f;
		result.y /= y ?? 1f;
		return result;
	}

	public static Vector4 DivideElements(this Vector4 self, Vector4 other)
	{
		Vector4 result = self;
		result.x /= other.x;
		result.y /= other.y;
		result.z /= other.z;
		result.w /= other.w;
		return result;
	}

	public static Vector4 DivideElements(this Vector4 self, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		Vector4 result = self;
		result.x /= x ?? 1f;
		result.y /= y ?? 1f;
		result.z /= z ?? 1f;
		result.w /= w ?? 1f;
		return result;
	}

	public static Vector3 Abs(this Vector3 self)
	{
		return new Vector3(Mathf.Abs(self.x), Mathf.Abs(self.y), Mathf.Abs(self.z));
	}

	public static Vector2 Abs(this Vector2 self)
	{
		return new Vector2(Mathf.Abs(self.x), Mathf.Abs(self.y));
	}

	public static Color MultiplyElements(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
	{
		return original * (r ?? 1f) * (g ?? 1f) * (b ?? 1f) * (a ?? 1f);
	}

	public static Color MultiplyElements(this Color original, Color other)
	{
		Color result = original;
		result.r *= other.r;
		result.g *= other.g;
		result.b *= other.b;
		result.a *= other.a;
		return result;
	}

	public static bool AddIfNotPresent<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
			return true;
		}
		return false;
	}

	public static void RemoveNulls<T>(this List<T> list)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == null)
			{
				list.RemoveAt(num);
			}
		}
	}

	public static bool IsRayHittingLocal(this Transform transform, Vector2 originLocal, Vector2 directionLocal, float length, int layerMask)
	{
		Vector2 origin = transform.TransformPoint(originLocal);
		Vector2 direction = transform.TransformVector(directionLocal);
		return Helper.Raycast2D(origin, direction, length, layerMask).collider != null;
	}

	public static bool IsRayHittingLocalNoTriggers(this Transform transform, Vector2 originLocal, Vector2 directionLocal, float length, int layerMask)
	{
		Vector2 origin = transform.TransformPoint(originLocal);
		Vector2 direction = transform.TransformVector(directionLocal);
		return Helper.IsRayHittingNoTriggers(origin, direction, length, layerMask);
	}

	public static void SetVelocity(this Rigidbody2D body, float? x = null, float? y = null)
	{
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity.x = x ?? linearVelocity.x;
		linearVelocity.y = y ?? linearVelocity.y;
		body.linearVelocity = linearVelocity;
	}

	public static T GetRandomElement<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	public static T GetRandomElement<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T GetAndRemoveRandomElement<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static Vector2 RandomInRange(this Vector2 original)
	{
		return new Vector2(UnityEngine.Random.Range(0f - original.x, original.x), UnityEngine.Random.Range(0f - original.y, original.y));
	}

	public static Vector3 RandomInRange(this Vector3 original)
	{
		return new Vector3(UnityEngine.Random.Range(0f - original.x, original.x), UnityEngine.Random.Range(0f - original.y, original.y), UnityEngine.Random.Range(0f - original.z, original.z));
	}

	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		if (collection != null)
		{
			return collection.Count == 0;
		}
		return true;
	}

	public static void SetAllActive(this ICollection<GameObject> collection, bool value)
	{
		if (collection == null)
		{
			return;
		}
		foreach (GameObject item in collection)
		{
			if (!(item == null))
			{
				item.SetActive(value);
			}
		}
	}

	public static void PushIfNotNull<T>(this Stack<T> stack, T value)
	{
		if (value != null)
		{
			stack.Push(value);
		}
	}

	public static T PushReturn<T>(this Stack<T> stack, T value)
	{
		stack.Push(value);
		return value;
	}

	public static T PushIfNotNullReturn<T>(this Stack<T> stack, T value)
	{
		if (value != null)
		{
			stack.Push(value);
		}
		return value;
	}

	public static void ForceUpdateLayoutNoCanvas(this LayoutGroup layoutGroup)
	{
		layoutGroup.enabled = false;
		layoutGroup.CalculateLayoutInputVertical();
		layoutGroup.SetLayoutVertical();
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.SetLayoutHorizontal();
		layoutGroup.enabled = true;
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
	}

	public static float GetArea(this Vector2 vector)
	{
		return Mathf.Abs(vector.x) * Mathf.Abs(vector.y);
	}

	public static float GetArea(this Vector3 vector)
	{
		return Mathf.Abs(vector.x) * Mathf.Abs(vector.y) * Mathf.Abs(vector.z);
	}

	public static Vector3 GetSign(this Vector3 vector3)
	{
		return new Vector3(Mathf.Sign(vector3.x), Mathf.Sign(vector3.y), Mathf.Sign(vector3.z));
	}

	public static bool Test(this int lhs, IntTest test, int rhs)
	{
		return test switch
		{
			IntTest.Equal => lhs == rhs, 
			IntTest.LessThan => lhs < rhs, 
			IntTest.LessThanOrEqual => lhs <= rhs, 
			IntTest.MoreThan => lhs > rhs, 
			IntTest.MoreThanOrEqual => lhs >= rhs, 
			_ => throw new NotImplementedException(), 
		};
	}

	public static void Shuffle<T>(this IList<T> ts)
	{
		int count = ts.Count;
		int num = count - 1;
		for (int i = 0; i < num; i++)
		{
			int num2 = UnityEngine.Random.Range(i, count);
			int index = i;
			int index2 = num2;
			T val = ts[num2];
			T val2 = ts[i];
			T val4 = (ts[index] = val);
			val4 = (ts[index2] = val2);
		}
	}

	public static InventoryItemManager.SelectionDirection Opposite(this InventoryItemManager.SelectionDirection direction)
	{
		return direction switch
		{
			InventoryItemManager.SelectionDirection.Up => InventoryItemManager.SelectionDirection.Down, 
			InventoryItemManager.SelectionDirection.Down => InventoryItemManager.SelectionDirection.Up, 
			InventoryItemManager.SelectionDirection.Left => InventoryItemManager.SelectionDirection.Right, 
			InventoryItemManager.SelectionDirection.Right => InventoryItemManager.SelectionDirection.Left, 
			_ => throw new NotImplementedException(), 
		};
	}

	public static void CopyFrom(this Collider2D self, Collider2D other)
	{
		if (self is PolygonCollider2D)
		{
			if (other is PolygonCollider2D)
			{
				((PolygonCollider2D)self).CopyFrom((PolygonCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
		if (self is BoxCollider2D)
		{
			if (other is BoxCollider2D)
			{
				((BoxCollider2D)self).CopyFrom((BoxCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
		if (self is CircleCollider2D)
		{
			if (other is CircleCollider2D)
			{
				((CircleCollider2D)self).CopyFrom((CircleCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
		throw new NotImplementedException();
	}

	private static void CopyShared(Collider2D to, Collider2D from)
	{
		to.offset = from.offset;
		to.isTrigger = from.isTrigger;
		to.usedByEffector = from.usedByEffector;
		to.usedByComposite = from.usedByComposite;
		to.sharedMaterial = from.sharedMaterial;
		to.includeLayers = from.includeLayers;
		to.excludeLayers = from.excludeLayers;
		to.forceSendLayers = from.forceSendLayers;
		to.forceReceiveLayers = from.forceReceiveLayers;
		to.contactCaptureLayers = from.contactCaptureLayers;
		to.callbackLayers = from.callbackLayers;
	}

	public static void CopyFrom(this BoxCollider2D self, BoxCollider2D other)
	{
		CopyShared(self, other);
		self.offset = other.offset;
		self.size = other.size;
	}

	public static void CopyFrom(this CircleCollider2D self, CircleCollider2D other)
	{
		CopyShared(self, other);
		self.offset = other.offset;
		self.radius = other.radius;
	}

	public static void CopyFrom(this PolygonCollider2D self, PolygonCollider2D other)
	{
		CopyShared(self, other);
		self.offset = other.offset;
		self.pathCount = other.pathCount;
		for (int i = 0; i < self.pathCount; i++)
		{
			self.SetPath(i, other.GetPath(i));
		}
	}

	public static T AddComponentIfNotPresent<T>(this GameObject obj) where T : Component
	{
		T component = obj.GetComponent<T>();
		if (!component)
		{
			return obj.AddComponent<T>();
		}
		return component;
	}

	public static bool IsBitSet(this int bitmask, int index)
	{
		int num = 1 << index;
		return (bitmask & num) == num;
	}

	public static int SetBitAtIndex(this int bitMask, int index)
	{
		bitMask |= 1 << index;
		return bitMask;
	}

	public static int ResetBitAtIndex(this int bitMask, int index)
	{
		bitMask &= ~(1 << index);
		return bitMask;
	}

	public static bool IsBitSet(this uint bitmask, int index)
	{
		int num = 1 << index;
		return (bitmask & num) == num;
	}

	public static uint SetBitAtIndex(this uint bitMask, int index)
	{
		bitMask |= (uint)(1 << index);
		return bitMask;
	}

	public static uint ResetBitAtIndex(this uint bitMask, int index)
	{
		bitMask &= (uint)(~(1 << index));
		return bitMask;
	}

	public static bool IsBitSet(this long bitmask, int index)
	{
		long num = 1L << index;
		return (bitmask & num) == num;
	}

	public static long SetBitAtIndex(this long bitMask, int index)
	{
		bitMask |= 1L << index;
		return bitMask;
	}

	public static long ResetBitAtIndex(this long bitMask, int index)
	{
		bitMask &= ~(1L << index);
		return bitMask;
	}

	public static bool IsBitSet(this ulong bitmask, int index)
	{
		ulong num = (ulong)(1L << index);
		return (bitmask & num) == num;
	}

	public static ulong SetBitAtIndex(this ulong bitMask, int index)
	{
		bitMask |= (ulong)(1L << index);
		return bitMask;
	}

	public static ulong ResetBitAtIndex(this ulong bitMask, int index)
	{
		bitMask &= (ulong)(~(1L << index));
		return bitMask;
	}

	public static int CountSetBits(this ulong value)
	{
		int num = 0;
		while (value != 0L)
		{
			num++;
			value &= value - 1;
		}
		return num;
	}

	public static void EmptyArray<T>(this T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = default(T);
		}
	}

	private static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
	{
		if (type == typeof(object))
		{
			return new List<FieldInfo>();
		}
		List<FieldInfo> allFields = type.BaseType.GetAllFields(flags);
		allFields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
		return allFields;
	}

	private static T DeepCopy<T>(T obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("Object cannot be null");
		}
		return (T)DoCopy(obj);
	}

	private static object DoCopy(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Type type = obj.GetType();
		if (type.IsValueType || type == typeof(string))
		{
			return obj;
		}
		if (type.IsArray)
		{
			Type elementType = type.GetElementType();
			Array array = obj as Array;
			Array array2 = Array.CreateInstance(elementType, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(DoCopy(array.GetValue(i)), i);
			}
			return Convert.ChangeType(array2, obj.GetType());
		}
		if (typeof(UnityEngine.Object).IsAssignableFrom(type))
		{
			return obj;
		}
		if (type.IsClass)
		{
			object obj2 = Activator.CreateInstance(obj.GetType());
			{
				foreach (FieldInfo allField in type.GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object value = allField.GetValue(obj);
					if (value != null)
					{
						allField.SetValue(obj2, DoCopy(value));
					}
				}
				return obj2;
			}
		}
		throw new ArgumentException("Unknown type");
	}

	public static T Clone<T>(this T ev) where T : UnityEventBase
	{
		return DeepCopy(ev);
	}
}
