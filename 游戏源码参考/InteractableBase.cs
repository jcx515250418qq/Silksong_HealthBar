using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : DebugDrawColliderRuntimeAdder
{
	public enum PromptLabels
	{
		Inspect = 0,
		Speak = 1,
		Listen = 2,
		Enter = 3,
		Ascend = 4,
		Rest = 5,
		Shop = 6,
		Travel = 7,
		Challenge = 8,
		Exit = 9,
		Descend = 10,
		Sit = 11,
		Trade = 12,
		Accept = 13,
		Watch = 14,
		Ascend_GG = 15,
		Consume = 16,
		Track = 17,
		TurnIn = 18,
		Attack = 19,
		Give = 20,
		Take = 21,
		Claim = 22,
		Call = 23,
		Play = 24,
		Dive = 25,
		Take_Living = 26
	}

	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private Transform promptMarker;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string promptLabel = "Speak";

	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private PromptLabels interactLabel;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private InteractPriority priority;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private TriggerEnterEvent enterDetector;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("DisplayExitDetectorField", true, true, false)]
	private TriggerEnterEvent exitDetector;

	private readonly HashSet<Collider2D> insideColliders = new HashSet<Collider2D>();

	private readonly HashSet<Collider2D> localInsideColliders = new HashSet<Collider2D>();

	private int previousInsideCount;

	private bool isShowingInteraction;

	private bool isQueueing;

	private Coroutine delayRoutine;

	private readonly List<IInteractableBlocker> blockers = new List<IInteractableBlocker>();

	private bool checkControlVersion;

	private int expectedControlVersion;

	public Transform PromptMarker
	{
		get
		{
			return promptMarker;
		}
		set
		{
			promptMarker = value;
		}
	}

	public PromptLabels InteractLabel
	{
		get
		{
			return interactLabel;
		}
		set
		{
			PromptLabels promptLabels = interactLabel;
			interactLabel = value;
			if (interactLabel != promptLabels && Application.isPlaying && HideInteraction())
			{
				ShowInteraction();
			}
		}
	}

	public virtual string InteractLabelDisplay
	{
		get
		{
			UpgradeOldSerializedFields();
			return interactLabel.ToString();
		}
	}

	public InteractPriority Priority => priority;

	public bool IsDisabled { get; private set; }

	public bool IsQueued { get; private set; }

	protected virtual bool IsQueueingHandled => false;

	protected virtual bool AutoQueueOnDeactivate => false;

	public TriggerEnterEvent EnterDetector
	{
		get
		{
			return enterDetector;
		}
		set
		{
			if ((bool)enterDetector)
			{
				enterDetector.OnTriggerEntered -= DetectorTriggerEnter;
			}
			if (ExitDetector == null || enterDetector == exitDetector)
			{
				ExitDetector = value;
			}
			enterDetector = value;
			if ((bool)enterDetector)
			{
				enterDetector.OnTriggerEntered += DetectorTriggerEnter;
			}
			else if (!HasDefinedDetector() && previousInsideCount == 0 && localInsideColliders.Count != 0)
			{
				previousInsideCount = 1;
				if (base.enabled)
				{
					ShowInteraction();
				}
				previousInsideCount = 0;
			}
		}
	}

	public TriggerEnterEvent ExitDetector
	{
		get
		{
			return exitDetector;
		}
		set
		{
			if ((bool)exitDetector)
			{
				exitDetector.OnTriggerExited -= DetectorTriggerExit;
			}
			exitDetector = value;
			if ((bool)exitDetector)
			{
				exitDetector.OnTriggerExited += DetectorTriggerExit;
			}
		}
	}

	public bool IsBlocked
	{
		get
		{
			foreach (IInteractableBlocker blocker in blockers)
			{
				if (blocker.IsBlocking)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected virtual bool EnableInteractableFields()
	{
		return true;
	}

	private bool DisplayExitDetectorField()
	{
		if (EnableInteractableFields())
		{
			return HasDefinedDetector();
		}
		return false;
	}

	private bool HasDefinedDetector()
	{
		return enterDetector;
	}

	protected virtual void OnValidate()
	{
		UpgradeOldSerializedFields();
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		ExitDetector = exitDetector;
		EnterDetector = enterDetector;
	}

	protected virtual void OnDisable()
	{
		if (!HasDefinedDetector())
		{
			insideColliders.Clear();
			previousInsideCount = 0;
		}
		InteractManager.RemoveInteractible(this);
	}

	private void UpgradeOldSerializedFields()
	{
		if (!string.IsNullOrEmpty(promptLabel))
		{
			try
			{
				interactLabel = (PromptLabels)Enum.Parse(typeof(PromptLabels), promptLabel, ignoreCase: true);
				promptLabel = null;
			}
			catch
			{
				Debug.LogError("Could not upgrade serialized string promptLabel (" + promptLabel + ") to PromptLabels enum");
			}
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		LocalAddInside(collision);
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		LocalRemoveInside(collision);
	}

	private void DetectorTriggerEnter(Collider2D collision, GameObject sender)
	{
		AddInside(collision);
	}

	private void DetectorTriggerExit(Collider2D collision, GameObject sender)
	{
		RemoveInside(collision);
	}

	private void AddInside(Collider2D col)
	{
		if (col.gameObject.layer == 9)
		{
			insideColliders.Add(col);
			previousInsideCount = insideColliders.Count;
			if (base.enabled)
			{
				ShowInteraction();
			}
		}
	}

	private void RemoveInside(Collider2D col)
	{
		insideColliders.Remove(col);
		int count = insideColliders.Count;
		if (count <= 0 && previousInsideCount > 0)
		{
			HideInteraction();
		}
		previousInsideCount = count;
	}

	private void LocalAddInside(Collider2D col)
	{
		if (col.gameObject.layer == 9 && localInsideColliders.Add(col) && !HasDefinedDetector())
		{
			int num = previousInsideCount;
			previousInsideCount = localInsideColliders.Count;
			if (base.enabled)
			{
				ShowInteraction();
			}
			previousInsideCount = num;
		}
	}

	private void LocalRemoveInside(Collider2D col)
	{
		if (localInsideColliders.Remove(col) && !HasDefinedDetector() && localInsideColliders.Count <= 0)
		{
			HideInteraction();
		}
	}

	private void ShowInteraction()
	{
		if (!IsDisabled && !isShowingInteraction && (previousInsideCount > 0 || (!HasDefinedDetector() && localInsideColliders.Count > 0)))
		{
			isShowingInteraction = true;
			InteractManager.AddInteractible(this, promptMarker ? promptMarker : base.transform, Vector3.zero);
		}
	}

	private bool HideInteraction()
	{
		isShowingInteraction = false;
		return InteractManager.RemoveInteractible(this);
	}

	protected void DisableInteraction()
	{
		InteractManager.BlockingInteractable = this;
		HeroController.instance.RelinquishControl();
		HideInteraction();
		checkControlVersion = false;
	}

	protected void RecordControlVersion()
	{
		checkControlVersion = true;
		expectedControlVersion = HeroController.ControlVersion;
	}

	protected void EnableInteraction()
	{
		if (InteractManager.BlockingInteractable == this)
		{
			InteractManager.BlockingInteractable = null;
			if (!checkControlVersion || expectedControlVersion == HeroController.ControlVersion)
			{
				HeroController.instance.RegainControl(allowInput: false);
			}
		}
		checkControlVersion = false;
		ShowInteraction();
	}

	public void Activate()
	{
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
		}
		IsDisabled = false;
		isQueueing = false;
		ShowInteraction();
		FSMUtility.SetBoolOnGameObjectFSMs(base.gameObject, "Was Interactable Active", value: true);
		FSMUtility.SendEventToGameObject(base.gameObject, "INTERACTIBLE ENABLED", isRecursive: true);
		if (IsQueued)
		{
			IsQueued = false;
			if (!IsQueueingHandled)
			{
				Interact();
			}
		}
		OnActivated();
	}

	protected virtual void OnActivated()
	{
	}

	public void ActivateDelayed(float delay)
	{
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
		}
		delayRoutine = this.ExecuteDelayed(delay, Activate);
	}

	public void Deactivate(bool allowQueueing)
	{
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
		}
		isQueueing = allowQueueing;
		if (!allowQueueing)
		{
			IsDisabled = true;
			HideInteraction();
		}
		else
		{
			IsDisabled = false;
			if (isQueueing && !IsQueued && AutoQueueOnDeactivate)
			{
				IsQueued = true;
			}
		}
		FSMUtility.SetBoolOnGameObjectFSMs(base.gameObject, "Was Interactable Active", value: false);
	}

	public void QueueInteraction()
	{
		if (isQueueing)
		{
			IsQueued = true;
		}
		if (isQueueing && !IsQueueingHandled)
		{
			HideInteraction();
		}
		else
		{
			Interact();
		}
	}

	public abstract void Interact();

	public virtual void CanInteract()
	{
	}

	public virtual void CanNotInteract()
	{
	}

	protected static bool TrySendStateChangeEvent(PlayMakerFSM fsm, string eventName, bool logError = true)
	{
		if (!string.IsNullOrEmpty(eventName) && (bool)fsm)
		{
			string activeStateName = fsm.ActiveStateName;
			fsm.SendEvent(eventName);
			if (fsm.ActiveStateName.Equals(activeStateName))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void AddBlocker(IInteractableBlocker blocker)
	{
		blockers.AddIfNotPresent(blocker);
	}

	public void RemoveBlocker(IInteractableBlocker blocker)
	{
		blockers.Remove(blocker);
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}
