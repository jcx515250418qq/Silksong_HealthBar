using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackThreadCore : MonoBehaviour
{
	public abstract class TintHelper
	{
		protected Color originalColor;

		protected Color startColor;

		protected Color targetColor;

		protected bool hasRecordedOriginal;

		public abstract Color Color { get; set; }

		public Color OriginalColor => originalColor;

		protected void RecordOriginalColor()
		{
			if (!hasRecordedOriginal)
			{
				originalColor = Color;
				hasRecordedOriginal = true;
			}
		}

		public void RestoreOriginalColor()
		{
			if (hasRecordedOriginal)
			{
				Color = originalColor;
			}
		}

		private void RecordStartColor()
		{
			startColor = Color;
		}

		public void BeginLerpTo(Color newTargetColor)
		{
			RecordStartColor();
			targetColor = newTargetColor;
		}

		public void LerpToTarget(float t)
		{
			Color = Color.Lerp(startColor, targetColor, t);
		}

		public void LerpFromOriginalTo(Color newTargetColor, float t)
		{
			if (hasRecordedOriginal)
			{
				Color = Color.Lerp(originalColor, newTargetColor, t);
			}
		}
	}

	public sealed class SpriteRendererTintHelper : TintHelper
	{
		private readonly SpriteRenderer target;

		public override Color Color
		{
			get
			{
				return target.color;
			}
			set
			{
				target.color = value;
			}
		}

		public SpriteRendererTintHelper(SpriteRenderer target)
		{
			this.target = target;
			RecordOriginalColor();
		}
	}

	public sealed class Tk2dSpriteTintHelper : TintHelper
	{
		private readonly tk2dSprite target;

		public override Color Color
		{
			get
			{
				return target.color;
			}
			set
			{
				target.color = value;
			}
		}

		public Tk2dSpriteTintHelper(tk2dSprite target)
		{
			this.target = target;
			RecordOriginalColor();
		}
	}

	[SerializeField]
	private Transform core;

	private bool wasDeactivated;

	private static List<BlackThreadCore> _activeCores;

	private List<TintHelper> tintTargets = new List<TintHelper>();

	private Coroutine lerpRoutine;

	public static bool IsAnyActive
	{
		get
		{
			if (_activeCores == null)
			{
				return false;
			}
			bool result = false;
			foreach (BlackThreadCore activeCore in _activeCores)
			{
				if (activeCore.IsActive)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	private bool IsActive
	{
		get
		{
			PersistentBoolItem component = GetComponent<PersistentBoolItem>();
			if ((bool)component)
			{
				component.PreSetup();
				if (component.GetCurrentValue())
				{
					return false;
				}
			}
			if (!wasDeactivated)
			{
				return base.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (_activeCores == null)
		{
			_activeCores = new List<BlackThreadCore>();
		}
		_activeCores.Add(this);
		bool flag = core != null;
		if (!flag)
		{
			core = base.transform.Find("Core");
			flag = core != null;
		}
		if (!flag)
		{
			return;
		}
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(includeInactive: false);
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			if (!spriteRenderer.CompareTag("HeroLight"))
			{
				tintTargets.Add(new SpriteRendererTintHelper(spriteRenderer));
			}
		}
		tk2dSprite[] componentsInChildren2 = GetComponentsInChildren<tk2dSprite>();
		foreach (tk2dSprite target in componentsInChildren2)
		{
			tintTargets.Add(new Tk2dSpriteTintHelper(target));
		}
	}

	private void OnEnable()
	{
		wasDeactivated = false;
	}

	private void OnDestroy()
	{
		if (_activeCores != null)
		{
			_activeCores.Remove(this);
			if (_activeCores.Count == 0)
			{
				_activeCores = null;
			}
		}
	}

	public void Deactivate()
	{
		wasDeactivated = true;
	}

	public void LerpToColor(Color color, float duration)
	{
		if (lerpRoutine != null)
		{
			StopCoroutine(lerpRoutine);
			lerpRoutine = null;
		}
		if (duration <= 0f)
		{
			foreach (TintHelper tintTarget in tintTargets)
			{
				tintTarget.Color = color;
			}
			return;
		}
		lerpRoutine = StartCoroutine(LerpToColorRoutine(color, duration));
	}

	public void LerpToOriginal(float duration)
	{
		if (lerpRoutine != null)
		{
			StopCoroutine(lerpRoutine);
			lerpRoutine = null;
		}
		if (duration <= 0f)
		{
			foreach (TintHelper tintTarget in tintTargets)
			{
				tintTarget.RestoreOriginalColor();
			}
			return;
		}
		lerpRoutine = StartCoroutine(LerpToOriginalRoutine(duration));
	}

	private IEnumerator LerpToColorRoutine(Color color, float duration)
	{
		if (duration > 0f)
		{
			float multi = 1f / duration;
			float t = 0f;
			foreach (TintHelper tintTarget in tintTargets)
			{
				tintTarget.BeginLerpTo(color);
			}
			while (t < 1f)
			{
				t += Time.deltaTime * multi;
				for (int i = 0; i < tintTargets.Count; i++)
				{
					tintTargets[i].LerpToTarget(t);
				}
				yield return null;
			}
		}
		foreach (TintHelper tintTarget2 in tintTargets)
		{
			tintTarget2.Color = color;
		}
		lerpRoutine = null;
	}

	private IEnumerator LerpToOriginalRoutine(float duration)
	{
		if (duration > 0f)
		{
			float multi = 1f / duration;
			float t = 0f;
			foreach (TintHelper tintTarget in tintTargets)
			{
				tintTarget.BeginLerpTo(tintTarget.OriginalColor);
			}
			while (t < 1f)
			{
				t += Time.deltaTime * multi;
				for (int i = 0; i < tintTargets.Count; i++)
				{
					tintTargets[i].LerpToTarget(t);
				}
				yield return null;
			}
		}
		foreach (TintHelper tintTarget2 in tintTargets)
		{
			tintTarget2.RestoreOriginalColor();
		}
		lerpRoutine = null;
	}
}
