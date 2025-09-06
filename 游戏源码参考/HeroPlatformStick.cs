using System;
using System.Collections;
using UnityEngine;

public class HeroPlatformStick : MonoBehaviour
{
	public interface IMoveHooks
	{
		void AddMoveHooks(Action onStartMove, Action onStopMove);
	}

	public interface ITouchHooks
	{
		void AddTouchHooks(Action onStartTouching, Action onStopTouching);
	}

	[Header("Optional")]
	[SerializeField]
	private TrackTriggerObjects insideTracker;

	[SerializeField]
	private Transform setParent;

	private bool isInactive;

	private bool wasInside;

	private bool isForcedTouching;

	private static Coroutine _waitRoutine;

	public Transform SetParent
	{
		get
		{
			if (!setParent)
			{
				return base.transform;
			}
			return setParent;
		}
	}

	public bool IsActive
	{
		get
		{
			return !isInactive;
		}
		set
		{
			isInactive = !value;
			Refresh();
		}
	}

	private void Awake()
	{
		Transform obj = base.transform;
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		Vector3 lossyScale = obj.lossyScale;
		obj.localScale = Vector3.one;
		Vector3 lossyScale2 = obj.lossyScale;
		Vector3 vector = lossyScale.DivideElements(lossyScale2);
		component.size = component.size.MultiplyElements((Vector2)vector);
		component.offset = component.offset.MultiplyElements((Vector2)vector);
		IMoveHooks componentInParent = GetComponentInParent<IMoveHooks>();
		if (componentInParent != null)
		{
			isInactive = true;
			componentInParent.AddMoveHooks(delegate
			{
				isInactive = false;
				Refresh();
			}, delegate
			{
				isInactive = true;
				Refresh();
			});
		}
		ITouchHooks componentInParent2 = GetComponentInParent<ITouchHooks>();
		if (componentInParent2 != null)
		{
			isInactive = true;
			componentInParent2.AddTouchHooks(delegate
			{
				isForcedTouching = true;
				Refresh();
			}, delegate
			{
				isForcedTouching = false;
				Refresh();
			});
		}
	}

	private void OnEnable()
	{
		if ((bool)insideTracker)
		{
			insideTracker.InsideStateChanged += OnInsideStateChanged;
		}
	}

	private void OnDisable()
	{
		if ((bool)insideTracker)
		{
			insideTracker.InsideStateChanged -= OnInsideStateChanged;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((bool)insideTracker)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 9)
		{
			return;
		}
		HeroController component = gameObject.GetComponent<HeroController>();
		if ((bool)component && !(collision.GetSafeContact().Normal.y >= 0f))
		{
			if (_waitRoutine != null)
			{
				StopCoroutine(_waitRoutine);
				_waitRoutine = null;
			}
			if (component.cState.onGround)
			{
				wasInside = true;
				Refresh();
			}
			else
			{
				_waitRoutine = StartCoroutine(WaitForGrounded(component));
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if ((bool)insideTracker)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer == 9 && (bool)gameObject.GetComponent<HeroController>())
		{
			if (_waitRoutine != null)
			{
				StopCoroutine(_waitRoutine);
				_waitRoutine = null;
			}
			wasInside = false;
			Refresh();
		}
	}

	private IEnumerator WaitForGrounded(HeroController heroController)
	{
		while (!heroController.cState.onGround)
		{
			yield return null;
		}
		wasInside = true;
		Refresh();
	}

	private void DoParent(HeroController heroController)
	{
		heroController.SetHeroParent(SetParent);
		Rigidbody2D body = heroController.Body;
		if (body != null)
		{
			body.interpolation = RigidbodyInterpolation2D.None;
		}
	}

	private void DoDeparent(HeroController heroController)
	{
		heroController.SetHeroParent(null);
		Rigidbody2D component = heroController.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			component.interpolation = RigidbodyInterpolation2D.Interpolate;
		}
	}

	private void OnInsideStateChanged(bool isInside)
	{
		wasInside = isInside;
		Refresh();
	}

	private void Refresh()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			if ((wasInside || isForcedTouching) && !isInactive)
			{
				DoParent(silentInstance);
			}
			else if (!(silentInstance.transform.parent != SetParent))
			{
				DoDeparent(silentInstance);
			}
		}
	}
}
