using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
	public Action<bool> DepressedChange;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TriggerEnterEvent trigger;

	[SerializeField]
	private Transform art;

	[SerializeField]
	private Vector2 artOffset;

	[SerializeField]
	private float riseDuration;

	[SerializeField]
	private AudioEvent riseAudio;

	[SerializeField]
	private bool startLocked;

	[Space]
	public UnityEvent OnDepress;

	private Vector2 initialArtPos;

	private bool isDepressed;

	private bool isLocked;

	private Vector2 animateStartPos;

	private Vector2 animateEndPos;

	private Coroutine animatePosRoutine;

	private bool playAudio;

	public bool IsDepressed => isDepressed;

	private void Awake()
	{
		trigger.OnTriggerEntered += OnTriggerEntered;
		trigger.OnTriggerExited += OnTriggerExited;
		if ((bool)art)
		{
			initialArtPos = art.localPosition;
		}
		if (startLocked)
		{
			SetLocked(value: true);
		}
	}

	private void OnTriggerEntered(Collider2D other, GameObject sender)
	{
		SetDepressed(value: true);
	}

	private void OnTriggerExited(Collider2D other, GameObject sender)
	{
		SetDepressed(value: false);
	}

	private void SetDepressed(bool value)
	{
		if (isDepressed == value)
		{
			return;
		}
		isDepressed = value;
		if (!isLocked)
		{
			playAudio = true;
			SetDepressedPosition(value);
			playAudio = false;
			if (DepressedChange != null)
			{
				DepressedChange(isDepressed);
			}
			if (isDepressed)
			{
				OnDepress.Invoke();
			}
		}
	}

	private void SetDepressedPosition(bool value)
	{
		if (animatePosRoutine != null)
		{
			StopCoroutine(animatePosRoutine);
			animatePosRoutine = null;
		}
		if (!art)
		{
			return;
		}
		animateStartPos = art.localPosition;
		animateEndPos = (value ? (initialArtPos + artOffset) : initialArtPos);
		if (!value)
		{
			if (playAudio)
			{
				riseAudio.SpawnAndPlayOneShot(base.transform.position);
			}
			animatePosRoutine = this.StartTimerRoutine(0f, riseDuration, delegate(float t)
			{
				Vector2 position = Vector2.Lerp(animateStartPos, animateEndPos, t);
				art.SetLocalPosition2D(position);
			});
		}
		else
		{
			art.SetLocalPosition2D(animateEndPos);
		}
	}

	public void SetLocked(bool value)
	{
		isLocked = value;
		SetDepressedPosition(isLocked || isDepressed);
	}

	public void UnlockActivate()
	{
		SetLocked(value: false);
		if (!isDepressed && DepressedChange != null)
		{
			DepressedChange(isDepressed);
		}
	}
}
