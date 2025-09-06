using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterEvent : EventBase
{
	public delegate void CollisionEvent(Collider2D collider, GameObject sender);

	[SerializeField]
	private PersistentBoolItem readPersistent;

	[SerializeField]
	private bool waitForHeroInPosition = true;

	[SerializeField]
	private bool callEventOnExit;

	[SerializeField]
	private bool delayUpdateGrounded;

	[Space]
	[SerializeField]
	private LayerMask checkLayer;

	[SerializeField]
	private string[] checkTags;

	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	[SerializeField]
	private string fsmEvent;

	[SerializeField]
	private bool enableFsmOnSend;

	private GameManager gm;

	private HeroController hc;

	private HeroController subscribedHc;

	private bool active;

	private bool isDelayingProcessList;

	private List<Collider2D> enteredWhileInactive;

	private List<Collider2D> exitedWhileDelayed;

	private Coroutine delayRoutine;

	private static readonly HashSet<Collider2D> processedColliders = new HashSet<Collider2D>();

	public override string InspectorInfo => "Trigger Enter";

	public bool DelayUpdateGrounded
	{
		get
		{
			return delayUpdateGrounded;
		}
		set
		{
			delayUpdateGrounded = value;
		}
	}

	private bool ShouldDelay
	{
		get
		{
			if (!delayUpdateGrounded || hc.cState.onGround)
			{
				return gm.IsInSceneTransition;
			}
			return true;
		}
	}

	public event CollisionEvent OnTriggerEntered;

	public event CollisionEvent OnTriggerExited;

	private event CollisionEvent _OnTriggerStayed;

	public event CollisionEvent OnTriggerStayed
	{
		add
		{
			if (this._OnTriggerStayed == null)
			{
				base.gameObject.AddComponentIfNotPresent<TriggerStaySubEvent>().OnTriggerStayed += CallOnTriggerStay2D;
			}
			_OnTriggerStayed += value;
		}
		remove
		{
			_OnTriggerStayed -= value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		gm = GameManager.instance;
	}

	protected virtual void Start()
	{
		active = false;
		hc = HeroController.instance;
		if (waitForHeroInPosition)
		{
			if (hc.isHeroInPosition)
			{
				HeroInPosition();
				return;
			}
			hc.heroInPosition += OnHeroInPosition;
			subscribedHc = hc;
		}
		else
		{
			HeroInPosition();
		}
	}

	private void OnDisable()
	{
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
		}
		enteredWhileInactive?.Clear();
		exitedWhileDelayed?.Clear();
	}

	private void OnDestroy()
	{
		UnSubHeroInPosition();
	}

	private void OnHeroInPosition(bool forceDirect)
	{
		HeroInPosition();
	}

	private void UnSubHeroInPosition()
	{
		if ((bool)subscribedHc)
		{
			subscribedHc.heroInPosition -= OnHeroInPosition;
			subscribedHc = null;
		}
	}

	private void HeroInPosition()
	{
		UnSubHeroInPosition();
		if ((bool)readPersistent)
		{
			readPersistent.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					Activate();
				}
			};
			RefreshActivate();
		}
		else
		{
			Activate();
		}
	}

	public void RefreshActivate()
	{
		if (readPersistent.GetCurrentValue())
		{
			Activate();
		}
	}

	private void Activate()
	{
		if (active)
		{
			return;
		}
		active = true;
		if (enteredWhileInactive != null && enteredWhileInactive.Count != 0 && !ShouldDelay)
		{
			ProcessList(enteredWhileInactive, OnTriggerEnter2D);
			if (isDelayingProcessList)
			{
				isDelayingProcessList = false;
				ProcessList(exitedWhileDelayed, OnTriggerExit2D);
			}
		}
	}

	private static void ProcessList(List<Collider2D> list, Action<Collider2D> action)
	{
		if (list == null)
		{
			return;
		}
		processedColliders.Clear();
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			Collider2D collider2D = list[index];
			list.RemoveAt(index);
			if ((bool)collider2D && processedColliders.Add(collider2D))
			{
				action(collider2D);
			}
		}
		processedColliders.Clear();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Particle") || ((int)checkLayer != 0 && ((int)checkLayer & (1 << other.gameObject.layer)) == 0))
		{
			return;
		}
		string[] array = checkTags;
		if (array != null && array.Length > 0)
		{
			array = checkTags;
			foreach (string text in array)
			{
				if (!other.CompareTag(text))
				{
					return;
				}
			}
		}
		if (!active || ShouldDelay)
		{
			List<Collider2D> list = exitedWhileDelayed;
			if (list == null || !list.Remove(other))
			{
				if (enteredWhileInactive == null)
				{
					enteredWhileInactive = new List<Collider2D>();
				}
				enteredWhileInactive.Add(other);
				if (ShouldDelay && delayRoutine == null)
				{
					delayRoutine = StartCoroutine(WaitForNotDelayed());
				}
			}
			return;
		}
		this.OnTriggerEntered?.Invoke(other, base.gameObject);
		if ((bool)fsmTarget)
		{
			if (enableFsmOnSend && !fsmTarget.enabled)
			{
				fsmTarget.enabled = true;
			}
			fsmTarget.SendEvent(fsmEvent);
		}
		if (!callEventOnExit)
		{
			CallReceivedEvent();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (isDelayingProcessList)
		{
			List<Collider2D> list = enteredWhileInactive;
			if (list != null && list.Remove(other))
			{
				return;
			}
		}
		if (!active || ShouldDelay)
		{
			List<Collider2D> list2 = enteredWhileInactive;
			if ((list2 == null || !list2.Remove(other)) && ShouldDelay)
			{
				if (exitedWhileDelayed == null)
				{
					exitedWhileDelayed = new List<Collider2D>();
				}
				exitedWhileDelayed.Add(other);
				if (base.gameObject.activeInHierarchy && delayRoutine == null)
				{
					delayRoutine = StartCoroutine(WaitForNotDelayed());
				}
			}
		}
		else
		{
			this.OnTriggerExited?.Invoke(other, base.gameObject);
			if (callEventOnExit)
			{
				CallReceivedEvent();
			}
		}
	}

	public void CallOnTriggerStay2D(Collider2D other)
	{
		if (active)
		{
			this._OnTriggerStayed?.Invoke(other, base.gameObject);
		}
	}

	private IEnumerator WaitForNotDelayed()
	{
		isDelayingProcessList = true;
		while (ShouldDelay)
		{
			yield return null;
		}
		if (!active)
		{
			if (readPersistent != null && !readPersistent.GetCurrentValue())
			{
				yield break;
			}
			while (!active)
			{
				yield return null;
			}
		}
		if (isDelayingProcessList)
		{
			isDelayingProcessList = false;
			ProcessList(enteredWhileInactive, OnTriggerEnter2D);
			ProcessList(exitedWhileDelayed, OnTriggerExit2D);
		}
		delayRoutine = null;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}
