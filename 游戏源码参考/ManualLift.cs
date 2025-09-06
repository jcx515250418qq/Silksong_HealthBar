using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class ManualLift : MonoBehaviour, HeroPlatformStick.IMoveHooks, HeroPlatformStick.ITouchHooks
{
	private enum UnlockSide
	{
		None = 0,
		Left = 1,
		Right = 2
	}

	[SerializeField]
	private SimpleButton buttonLeft;

	[SerializeField]
	private SimpleButton buttonRight;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform moveTransform;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float acceleration;

	[SerializeField]
	private float deceleration;

	[SerializeField]
	private float moveDelay;

	[SerializeField]
	private PlayMakerFSM tiltPlatFsm;

	[Space]
	[SerializeField]
	private DamageEnemies movingDamager;

	[SerializeField]
	private GameObject heroSquasherLeft;

	[SerializeField]
	private float heroSquashXLeft;

	[SerializeField]
	private GameObject heroSquasherRight;

	[SerializeField]
	private float heroSquashXRight;

	[Space]
	[SerializeField]
	private CameraShakeTarget endImpactShakeSmall;

	[SerializeField]
	private CameraShakeTarget endImpactShakeBig;

	[SerializeField]
	private PlayableDirector supportTimeline;

	[SerializeField]
	private float supportTimelineSpeed;

	[Space]
	[SerializeField]
	private AudioSource movingAudioLoop;

	[SerializeField]
	private AnimationCurve movingAudioPitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve movingAudioVolumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AudioEvent takeOffAudio;

	[SerializeField]
	private AudioEvent arriveAudio;

	[Space]
	[SerializeField]
	private Vector2 leftTargetPos;

	[SerializeField]
	private Vector2 rightTargetPos;

	[SerializeField]
	private TempPressurePlate callPlateLeft;

	[SerializeField]
	private TempPressurePlate callPlateRight;

	[SerializeField]
	private UnlockSide unlockFromSide;

	[SerializeField]
	private PersistentBoolItem unlockPersistent;

	[SerializeField]
	private Animator unlockAnimator;

	[Space]
	[SerializeField]
	private string smallImpactLeftEvent;

	[SerializeField]
	private string bigImpactLeftEvent;

	[SerializeField]
	private string smallImpactRightEvent;

	[SerializeField]
	private string bigImpactRightEvent;

	[Space]
	public UnityEvent OnUnlock;

	[Space]
	public UnityEvent OnStartedMoving;

	public UnityEvent OnStoppedMoving;

	private FsmFloat tiltPlatXFloat;

	private bool isLeftPressed;

	private bool isRightPressed;

	private int calledDirection;

	private int previousInputDirection;

	private float moveDelayLeft;

	private bool wasMoving;

	private float currentVelocity;

	private float currentPosT;

	private float targetTDirection;

	private float speedFactor;

	private bool isUnlocked;

	private Coroutine unlockRoutine;

	private void OnDrawGizmos()
	{
		float z = base.transform.position.z;
		Vector3 vector = leftTargetPos.ToVector3(z);
		Vector3 vector2 = rightTargetPos.ToVector3(z);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawWireSphere(vector, 0.3f);
		Gizmos.DrawWireSphere(vector2, 0.3f);
	}

	private void Awake()
	{
		switch (unlockFromSide)
		{
		case UnlockSide.Left:
			if ((bool)callPlateLeft)
			{
				callPlateLeft.Activated += OnUnlockPlateActivated;
			}
			break;
		case UnlockSide.Right:
			if ((bool)callPlateRight)
			{
				callPlateRight.Activated += OnUnlockPlateActivated;
			}
			break;
		default:
			isUnlocked = true;
			break;
		}
		if ((bool)buttonLeft)
		{
			SimpleButton simpleButton = buttonLeft;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, (Action<bool>)delegate(bool isDepressed)
			{
				isLeftPressed = isDepressed;
				UpdateDirection(overrideCall: true);
			});
		}
		if ((bool)buttonRight)
		{
			SimpleButton simpleButton2 = buttonRight;
			simpleButton2.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton2.DepressedChange, (Action<bool>)delegate(bool isDepressed)
			{
				isRightPressed = isDepressed;
				UpdateDirection(overrideCall: true);
			});
		}
		if ((bool)movingDamager)
		{
			movingDamager.gameObject.SetActive(value: false);
		}
		if ((bool)callPlateLeft)
		{
			callPlateLeft.Activated += delegate
			{
				OnCallPlateActivated(-1);
			};
		}
		if ((bool)callPlateRight)
		{
			callPlateRight.Activated += delegate
			{
				OnCallPlateActivated(1);
			};
		}
		if (!unlockPersistent)
		{
			return;
		}
		unlockPersistent.OnGetSaveState += delegate(out bool value)
		{
			value = isUnlocked;
		};
		unlockPersistent.OnSetSaveState += delegate(bool value)
		{
			isUnlocked = value;
			unlockAnimator.enabled = true;
			unlockAnimator.Play("Unlock", 0, isUnlocked ? 1f : 0f);
			unlockAnimator.Update(0f);
			unlockAnimator.enabled = false;
			if (isUnlocked)
			{
				OnUnlockPlateActivated();
			}
			UpdateButtons();
			SetStartPos();
		};
	}

	private void Start()
	{
		if ((bool)tiltPlatFsm)
		{
			tiltPlatXFloat = tiltPlatFsm.FsmVariables.FindFsmFloat("Self X");
		}
		HeroController instance = HeroController.instance;
		if ((bool)instance && !instance.isHeroInPosition)
		{
			HeroController.HeroInPosition temp = null;
			temp = delegate
			{
				SetStartPos();
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
		}
		else
		{
			SetStartPos();
		}
		UpdateButtons();
		UpdatePlates();
		if ((bool)unlockAnimator)
		{
			unlockAnimator.enabled = true;
			unlockAnimator.Update(0f);
			unlockAnimator.enabled = false;
		}
		if ((bool)supportTimeline)
		{
			supportTimeline.Evaluate();
		}
	}

	private void Update()
	{
		bool flag = false;
		float num = targetTDirection;
		if (moveDelayLeft > 0f)
		{
			moveDelayLeft -= Time.deltaTime;
			if (moveDelayLeft > 0f)
			{
				num = 0f;
				flag = true;
			}
		}
		float num2 = ((num != 0f) ? (moveSpeed * num) : 0f);
		float num3 = ((Mathf.Abs(num2) >= Mathf.Abs(currentVelocity) || (num2 > 0f && currentVelocity <= 0f) || (num2 < 0f && currentVelocity >= 0f)) ? acceleration : deceleration);
		currentVelocity = Mathf.Lerp(currentVelocity, num2, Time.deltaTime * num3);
		float num4 = currentVelocity / moveSpeed;
		if ((bool)supportTimeline)
		{
			supportTimeline.time += Time.deltaTime * num4 * supportTimelineSpeed;
			if (supportTimeline.time < 0.0)
			{
				supportTimeline.time = supportTimeline.duration;
			}
			else if (supportTimeline.time > supportTimeline.duration)
			{
				supportTimeline.time = 0.0;
			}
			supportTimeline.Evaluate();
		}
		if ((bool)movingAudioLoop)
		{
			if (Mathf.Abs(num4) > 0.01f)
			{
				if (!movingAudioLoop.isPlaying && previousInputDirection != 0)
				{
					movingAudioLoop.Play();
				}
				float time = Mathf.Abs(num4);
				movingAudioLoop.volume = movingAudioVolumeCurve.Evaluate(time);
				movingAudioLoop.pitch = movingAudioPitchCurve.Evaluate(time);
			}
			else if (movingAudioLoop.isPlaying)
			{
				movingAudioLoop.Stop();
			}
		}
		if (Mathf.Abs(currentVelocity) <= 0.001f)
		{
			if (wasMoving)
			{
				OnStoppedMoving.Invoke();
				wasMoving = false;
			}
			return;
		}
		if (!wasMoving)
		{
			OnStartedMoving.Invoke();
			wasMoving = true;
		}
		currentPosT = Mathf.Clamp01(currentPosT + speedFactor * currentVelocity * Time.deltaTime);
		UpdatePosition();
		if (!flag && ((currentPosT <= 0f && num < 0f) || (currentPosT >= 1f && num > 0f)))
		{
			StopMoving(hardStop: true);
		}
	}

	private void SetStartPos()
	{
		HeroController instance = HeroController.instance;
		if (isUnlocked)
		{
			Vector3 position = instance.transform.position;
			currentPosT = ((Vector2.Distance(position, leftTargetPos) < Vector2.Distance(position, rightTargetPos)) ? 0f : 1f);
		}
		else
		{
			switch (unlockFromSide)
			{
			case UnlockSide.Left:
				currentPosT = 0f;
				break;
			case UnlockSide.Right:
				currentPosT = 1f;
				break;
			default:
				Debug.LogError("Should not be in this state!");
				break;
			}
		}
		UpdatePosition();
		UpdatePlates();
	}

	private void UpdateDirection(bool overrideCall)
	{
		if (!isUnlocked)
		{
			return;
		}
		if (calledDirection != 0 && overrideCall)
		{
			calledDirection = 0;
		}
		int num;
		if (calledDirection != 0)
		{
			num = calledDirection;
		}
		else
		{
			num = 0;
			if (isLeftPressed)
			{
				num--;
			}
			if (isRightPressed)
			{
				num++;
			}
		}
		if (num != previousInputDirection)
		{
			moveDelayLeft = moveDelay;
			if (num != 0)
			{
				takeOffAudio.SpawnAndPlayOneShot(movingAudioLoop.transform.position);
			}
		}
		previousInputDirection = num;
		if (num == 0)
		{
			StopMoving(hardStop: false);
			return;
		}
		targetTDirection = ((num >= 0) ? 1 : (-1));
		speedFactor = 1f / Vector2.Distance(leftTargetPos, rightTargetPos);
		if ((bool)movingDamager)
		{
			GameObject gameObject = movingDamager.gameObject;
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(value: true);
			}
			Vector2 normalized = ((num > 0) ? (rightTargetPos - leftTargetPos) : (leftTargetPos - rightTargetPos)).normalized;
			float direction = Vector2.Angle(Vector2.right, normalized);
			movingDamager.direction = direction;
		}
	}

	private void StopMoving(bool hardStop)
	{
		targetTDirection = 0f;
		calledDirection = 0;
		if (hardStop)
		{
			float num = Mathf.Abs(currentVelocity) / moveSpeed;
			if (num > 0.8f)
			{
				endImpactShakeBig.DoShake(this);
				if (currentPosT < 0.5f)
				{
					if (!string.IsNullOrEmpty(bigImpactLeftEvent))
					{
						EventRegister.SendEvent(bigImpactLeftEvent);
					}
				}
				else if (!string.IsNullOrEmpty(bigImpactRightEvent))
				{
					EventRegister.SendEvent(bigImpactRightEvent);
				}
			}
			else if (num > 0.3f)
			{
				endImpactShakeSmall.DoShake(this);
				if (currentPosT < 0.5f)
				{
					if (!string.IsNullOrEmpty(smallImpactLeftEvent))
					{
						EventRegister.SendEvent(smallImpactLeftEvent);
					}
				}
				else if (!string.IsNullOrEmpty(smallImpactRightEvent))
				{
					EventRegister.SendEvent(smallImpactRightEvent);
				}
			}
			currentVelocity = 0f;
			movingAudioLoop.Stop();
			arriveAudio.SpawnAndPlayOneShot(movingAudioLoop.transform.position);
			OnStoppedMoving.Invoke();
			wasMoving = false;
		}
		if ((bool)movingDamager)
		{
			movingDamager.gameObject.SetActive(value: false);
		}
		if ((bool)heroSquasherLeft)
		{
			heroSquasherLeft.SetActive(value: false);
		}
		if ((bool)heroSquasherRight)
		{
			heroSquasherRight.SetActive(value: false);
		}
		UpdatePlates();
	}

	private void UpdatePosition()
	{
		Vector2 position = Vector2.Lerp(leftTargetPos, rightTargetPos, currentPosT);
		moveTransform.SetPosition2D(position);
		if (tiltPlatXFloat != null)
		{
			tiltPlatXFloat.Value = moveTransform.position.x;
		}
		Vector3 localPosition = moveTransform.localPosition;
		bool flag = Mathf.Abs(currentVelocity) > 0.01f;
		if ((bool)heroSquasherLeft)
		{
			heroSquasherLeft.SetActive(flag && localPosition.x <= heroSquashXLeft);
		}
		if ((bool)heroSquasherRight)
		{
			heroSquasherRight.SetActive(flag && localPosition.x >= heroSquashXRight);
		}
	}

	private void OnUnlockPlateActivated()
	{
		switch (unlockFromSide)
		{
		case UnlockSide.Left:
			if ((bool)callPlateLeft)
			{
				callPlateLeft.Activated -= OnUnlockPlateActivated;
			}
			break;
		case UnlockSide.Right:
			if ((bool)callPlateRight)
			{
				callPlateRight.Activated -= OnUnlockPlateActivated;
			}
			break;
		default:
			Debug.LogError("Could not unsubscribe", this);
			break;
		}
		if (!isUnlocked && unlockRoutine == null)
		{
			unlockRoutine = StartCoroutine(Unlock());
		}
	}

	private IEnumerator Unlock()
	{
		isUnlocked = true;
		if ((bool)unlockAnimator)
		{
			unlockAnimator.enabled = true;
			unlockAnimator.Play("Unlock", 0, 0f);
			yield return null;
			yield return new WaitForSeconds(unlockAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		OnUnlock.Invoke();
		UpdateButtons();
		UpdatePlates();
	}

	private void OnCallPlateActivated(int direction)
	{
		if (isUnlocked)
		{
			calledDirection = direction;
			UpdateDirection(overrideCall: false);
		}
	}

	private void UpdatePlates()
	{
		if (isUnlocked)
		{
			if ((bool)callPlateRight)
			{
				if (currentPosT >= 0.99f)
				{
					callPlateRight.ActivateSilent();
				}
				else
				{
					callPlateRight.Deactivate();
				}
			}
			if ((bool)callPlateLeft)
			{
				if (currentPosT <= 0.01f)
				{
					callPlateLeft.ActivateSilent();
				}
				else
				{
					callPlateLeft.Deactivate();
				}
			}
			return;
		}
		switch (unlockFromSide)
		{
		case UnlockSide.Left:
			if ((bool)callPlateRight)
			{
				callPlateRight.ActivateSilent();
			}
			break;
		case UnlockSide.Right:
			if ((bool)callPlateLeft)
			{
				callPlateLeft.ActivateSilent();
			}
			break;
		}
	}

	private void UpdateButtons()
	{
		if ((bool)buttonLeft)
		{
			buttonLeft.SetLocked(!isUnlocked);
		}
		if ((bool)buttonRight)
		{
			buttonRight.SetLocked(!isUnlocked);
		}
	}

	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		OnStartedMoving.AddListener(delegate
		{
			onStartMove();
		});
		OnStoppedMoving.AddListener(delegate
		{
			onStopMove();
		});
	}

	public void AddTouchHooks(Action onStartTouching, Action onStopTouching)
	{
		bool wasTouching = false;
		if ((bool)buttonLeft)
		{
			SimpleButton simpleButton = buttonLeft;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, (Action<bool>)delegate
			{
				CheckTouching();
			});
		}
		if ((bool)buttonRight)
		{
			SimpleButton simpleButton2 = buttonRight;
			simpleButton2.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton2.DepressedChange, (Action<bool>)delegate
			{
				CheckTouching();
			});
		}
		void CheckTouching()
		{
			bool flag = ((bool)buttonLeft && buttonLeft.IsDepressed) || (((bool)buttonRight && buttonRight.IsDepressed) ? true : false);
			if (flag)
			{
				if (!wasTouching)
				{
					onStartTouching();
				}
			}
			else if (wasTouching)
			{
				onStopTouching();
			}
			wasTouching = flag;
		}
	}
}
