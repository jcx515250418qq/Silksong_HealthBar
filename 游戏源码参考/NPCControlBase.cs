using System;
using System.Collections;
using UnityEngine;

public abstract class NPCControlBase : InteractableBase
{
	public enum TalkPositions
	{
		Any = 0,
		Left = 1,
		Right = 2
	}

	public enum OutsideRangeBehaviours
	{
		None = 0,
		ForceToDistance = 1,
		FaceDirection = 2
	}

	public enum HeroAnimBeginTypes
	{
		Auto = 0,
		Manual = 1
	}

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private TalkPositions talkPosition;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private float centreOffset;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private float targetDistance = 2f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private OutsideRangeBehaviours outsideRangeBehaviour;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool forceToDistance;

	[SerializeField]
	private bool checkGround;

	[SerializeField]
	private HeroTalkAnimation.AnimationTypes heroAnimation;

	[SerializeField]
	private HeroAnimBeginTypes heroAnimBegin;

	[SerializeField]
	private bool overrideHeroHurtAnim;

	private bool isWaitingToBegin;

	private Coroutine moveHeroRoutine;

	private bool isLeftValid = true;

	private bool isRightValid = true;

	private bool blockedHeroInput;

	private bool nextSceneRegistered;

	private bool didChangedSpriteDirection;

	private bool originalFacingDirection;

	public virtual bool AutoEnd => true;

	protected virtual bool AutoCallEndAction => true;

	protected override bool IsQueueingHandled => true;

	protected override bool AutoQueueOnDeactivate => isWaitingToBegin;

	protected virtual bool AllowMovePlayer => true;

	private bool DoMovePlayer
	{
		get
		{
			if (!AllowMovePlayer || !(Math.Abs(targetDistance) > 0.01f))
			{
				return outsideRangeBehaviour != OutsideRangeBehaviours.None;
			}
			return true;
		}
	}

	public bool OverrideHeroHurtAnim
	{
		get
		{
			if (!overrideHeroHurtAnim)
			{
				HeroTalkAnimation.AnimationTypes animationTypes = HeroAnimation;
				return animationTypes == HeroTalkAnimation.AnimationTypes.Kneeling || animationTypes == HeroTalkAnimation.AnimationTypes.Custom;
			}
			return true;
		}
	}

	public float CentreOffset
	{
		get
		{
			return centreOffset;
		}
		set
		{
			centreOffset = value;
		}
	}

	public float TargetDistance
	{
		get
		{
			return targetDistance;
		}
		set
		{
			targetDistance = value;
		}
	}

	public TalkPositions TalkPosition
	{
		get
		{
			return talkPosition;
		}
		set
		{
			talkPosition = value;
		}
	}

	public HeroTalkAnimation.AnimationTypes HeroAnimation
	{
		get
		{
			return heroAnimation;
		}
		set
		{
			heroAnimation = value;
		}
	}

	public OutsideRangeBehaviours OutsideRangeBehaviour
	{
		get
		{
			return outsideRangeBehaviour;
		}
		set
		{
			outsideRangeBehaviour = value;
		}
	}

	public HeroAnimBeginTypes HeroAnimBegin
	{
		get
		{
			return heroAnimBegin;
		}
		set
		{
			heroAnimBegin = value;
		}
	}

	public event Action StartingDialogue;

	public event Action StartedDialogue;

	public event Action<DialogueBox.DialogueLine> OpeningDialogueBox;

	public event Action<DialogueBox.DialogueLine> StartedNewLine;

	public event Action<DialogueBox.DialogueLine> LineEnded;

	public event Action EndingDialogue;

	public event Action EndedDialogue;

	protected virtual void OnEnable()
	{
		RegisterSceneEvents();
	}

	protected override void OnDisable()
	{
		UnregisterSceneEvents();
		base.OnDisable();
		if (!blockedHeroInput && moveHeroRoutine == null)
		{
			return;
		}
		HeroController instance = HeroController.instance;
		if (!(instance != null))
		{
			return;
		}
		if (moveHeroRoutine != null)
		{
			moveHeroRoutine = null;
			EnableInteraction();
			if (!instance.HasAnimationControl)
			{
				instance.StartAnimationControl();
			}
		}
		if (blockedHeroInput)
		{
			instance.RemoveInputBlocker(this);
			blockedHeroInput = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (DoMovePlayer)
		{
			Gizmos.DrawWireSphere(base.transform.position + new Vector3(centreOffset, 0f, 0f), 0.25f);
			if (talkPosition != TalkPositions.Left)
			{
				Gizmos.DrawWireSphere(base.transform.position + new Vector3(centreOffset + targetDistance, 0f, 0f), 0.1f);
			}
			if (talkPosition != TalkPositions.Right)
			{
				Gizmos.DrawWireSphere(base.transform.position + new Vector3(centreOffset - targetDistance, 0f, 0f), 0.1f);
			}
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (forceToDistance)
		{
			outsideRangeBehaviour = OutsideRangeBehaviours.ForceToDistance;
			forceToDistance = false;
		}
	}

	protected virtual void Start()
	{
		CheckTalkSide();
	}

	private void RegisterSceneEvents()
	{
		if (!nextSceneRegistered)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				nextSceneRegistered = true;
				instance.HeroLeavingScene += OnNextSceneWillActivate;
			}
		}
	}

	private void UnregisterSceneEvents()
	{
		if (nextSceneRegistered)
		{
			nextSceneRegistered = false;
			HeroController silentInstance = HeroController.SilentInstance;
			if ((bool)silentInstance)
			{
				silentInstance.HeroLeavingScene -= OnNextSceneWillActivate;
			}
		}
	}

	private void OnNextSceneWillActivate()
	{
		CancelHeroMove();
	}

	public void StartDialogueMove()
	{
		SendStartingDialogue();
		isWaitingToBegin = true;
		if (DoMovePlayer)
		{
			moveHeroRoutine = StartCoroutine(MovePlayer(StartDialogue));
		}
		else
		{
			StartDialogue();
		}
	}

	protected void CancelHeroMove()
	{
		if (moveHeroRoutine == null)
		{
			return;
		}
		StopCoroutine(moveHeroRoutine);
		moveHeroRoutine = null;
		HeroController instance = HeroController.instance;
		if (instance == null)
		{
			return;
		}
		if (didChangedSpriteDirection)
		{
			didChangedSpriteDirection = false;
			if (originalFacingDirection)
			{
				instance.FaceRight();
			}
			else
			{
				instance.FaceLeft();
			}
		}
		if (blockedHeroInput)
		{
			instance.RemoveInputBlocker(this);
			blockedHeroInput = false;
		}
	}

	public void StartDialogueImmediately()
	{
		SendStartingDialogue();
		StartDialogue();
	}

	private void SendStartingDialogue()
	{
		this.StartingDialogue?.Invoke();
		OnStartingDialogue();
	}

	private void StartDialogue()
	{
		isWaitingToBegin = false;
		if (heroAnimBegin == HeroAnimBeginTypes.Auto)
		{
			HeroTalkAnimation.EnterConversation(this);
		}
		this.StartedDialogue?.Invoke();
		OnStartDialogue();
	}

	public void BeginHeroAnimManual()
	{
		if (heroAnimBegin == HeroAnimBeginTypes.Manual)
		{
			HeroTalkAnimation.EnterConversation(this);
		}
	}

	public void BeginHeroTalkAnimation()
	{
		HeroTalkAnimation.EnterConversation(this);
	}

	public void NewLineStarted(DialogueBox.DialogueLine line)
	{
		HeroTalkAnimation.SetTalking(line.IsPlayer, line.IsPlayer);
		this.StartedNewLine?.Invoke(line);
		OnNewLineStarted(line);
	}

	public void NewLineEnded(DialogueBox.DialogueLine line)
	{
		if (line.IsPlayer)
		{
			HeroTalkAnimation.SetTalking(setTalking: true, setAnimating: false);
		}
		this.LineEnded?.Invoke(line);
		OnLineEnded(line);
	}

	public void EndDialogue()
	{
		CancelHeroMove();
		HeroTalkAnimation.SetTalking(setTalking: false, setAnimating: false);
		this.EndingDialogue?.Invoke();
		if (AutoCallEndAction)
		{
			CallEndAction();
		}
		OnEndDialogue();
	}

	protected void CallEndAction()
	{
		HeroTalkAnimation.ExitConversation();
		this.EndedDialogue?.Invoke();
		GameManager.instance.DoQueuedSaveGame();
	}

	public void OnOpeningDialogueBox(DialogueBox.DialogueLine firstLine)
	{
		this.OpeningDialogueBox?.Invoke(firstLine);
	}

	protected virtual void OnStartingDialogue()
	{
	}

	protected virtual void OnStartDialogue()
	{
	}

	protected virtual void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
	}

	protected virtual void OnLineEnded(DialogueBox.DialogueLine line)
	{
	}

	public virtual void OnDialogueBoxEnded()
	{
	}

	protected virtual void OnEndDialogue()
	{
	}

	public override void Interact()
	{
		StartDialogueMove();
	}

	private IEnumerator MovePlayer(Action onEnd)
	{
		HeroController hc = HeroController.instance;
		if ((bool)hc)
		{
			DisableInteraction();
			TalkPositions talkPositions = talkPosition;
			if (!isLeftValid)
			{
				talkPositions = TalkPositions.Right;
			}
			else if (!isRightValid)
			{
				talkPositions = TalkPositions.Left;
			}
			float num = base.transform.position.x + centreOffset;
			int dir = 0;
			switch (talkPositions)
			{
			case TalkPositions.Any:
				dir = (int)Mathf.Sign(hc.transform.position.x - num);
				break;
			case TalkPositions.Left:
				dir = -1;
				break;
			case TalkPositions.Right:
				dir = 1;
				break;
			}
			float targetX = num + targetDistance * (float)dir;
			float f = targetX - hc.transform.position.x;
			bool didForce = false;
			if (outsideRangeBehaviour == OutsideRangeBehaviours.ForceToDistance)
			{
				int num2 = (int)Mathf.Sign(f);
				if (num2 != dir)
				{
					dir = num2;
					didForce = true;
				}
			}
			Func<bool> shouldMove;
			switch (outsideRangeBehaviour)
			{
			case OutsideRangeBehaviours.None:
				if (Mathf.Abs(f) < 0.3f)
				{
					shouldMove = () => false;
					break;
				}
				goto case OutsideRangeBehaviours.ForceToDistance;
			case OutsideRangeBehaviours.ForceToDistance:
				shouldMove = delegate
				{
					if (dir < 0)
					{
						if (hc.transform.position.x <= targetX)
						{
							return false;
						}
					}
					else if (hc.transform.position.x >= targetX)
					{
						return false;
					}
					return true;
				};
				break;
			case OutsideRangeBehaviours.FaceDirection:
				shouldMove = () => false;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			tk2dSpriteAnimator animator = hc.GetComponent<tk2dSpriteAnimator>();
			HeroAnimationController heroAnim = hc.GetComponent<HeroAnimationController>();
			originalFacingDirection = hc.cState.facingRight;
			if (shouldMove())
			{
				hc.transform.SetScaleX(-dir);
				didChangedSpriteDirection = true;
				Rigidbody2D body = hc.GetComponent<Rigidbody2D>();
				if ((bool)body)
				{
					body.linearVelocity = body.linearVelocity.Where(hc.GetWalkSpeed() * (float)dir, null);
				}
				if ((bool)animator)
				{
					hc.StopAnimationControl();
					if ((hc.cState.facingRight && dir < 0) || (!hc.cState.facingRight && dir > 0))
					{
						float timeLeft = animator.PlayAnimGetTime(heroAnim.GetClip("TurnWalk"));
						while (timeLeft > 0f && shouldMove())
						{
							yield return null;
							timeLeft -= Time.deltaTime;
						}
					}
					animator.Play(heroAnim.GetClip("Walk"));
					hc.ForceWalkingSound = true;
				}
				Vector3 lastPosition = hc.transform.position;
				lastPosition.x += 100f;
				int unchangedFrames = 0;
				float elapsed = 0f;
				WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
				while (elapsed < 3f && shouldMove())
				{
					yield return fixedUpdate;
					elapsed += Time.deltaTime;
					Vector3 position = hc.transform.position;
					if (lastPosition == position)
					{
						unchangedFrames++;
						if (unchangedFrames > 1)
						{
							break;
						}
					}
					else
					{
						unchangedFrames = 0;
						lastPosition = position;
					}
				}
				if ((bool)body)
				{
					body.linearVelocity = body.linearVelocity.Where(0f, null);
				}
				if (outsideRangeBehaviour == OutsideRangeBehaviours.ForceToDistance)
				{
					hc.transform.SetPositionX(targetX);
				}
			}
			int num3 = (int)Mathf.Sign(hc.transform.localScale.x);
			didChangedSpriteDirection = false;
			if ((didForce && dir < 0) || (!didForce && dir > 0))
			{
				hc.FaceLeft();
			}
			else
			{
				hc.FaceRight();
			}
			if ((int)Mathf.Sign(hc.transform.localScale.x) != num3 && (bool)animator && !didForce)
			{
				hc.StopAnimationControl();
				yield return StartCoroutine(animator.PlayAnimWait(heroAnim.GetClip("TurnWalk")));
			}
			if (!hc.HasAnimationControl)
			{
				hc.AnimCtrl.PlayIdle();
			}
			hc.ForceWalkingSound = false;
			hc.AddInputBlocker(this);
			blockedHeroInput = true;
			yield return null;
			EnableInteraction();
			if (!hc.HasAnimationControl)
			{
				hc.StartAnimationControl();
			}
			hc.RemoveInputBlocker(this);
			blockedHeroInput = false;
		}
		if (base.IsQueued)
		{
			DisableInteraction();
			yield return new WaitUntil(() => !base.IsQueued);
			EnableInteraction();
		}
		moveHeroRoutine = null;
		onEnd();
	}

	public void SetTalkPositionLeft()
	{
		talkPosition = TalkPositions.Left;
	}

	public void SetTalkPositionRight()
	{
		talkPosition = TalkPositions.Right;
	}

	public void SetTalkPositionAny()
	{
		talkPosition = TalkPositions.Any;
	}

	protected override void OnActivated()
	{
		CheckTalkSide();
	}

	private void CheckTalkSide()
	{
		isLeftValid = true;
		isRightValid = true;
		if (checkGround && DoMovePlayer)
		{
			isRightValid = IsGroundSideValid(isRight: true);
			isLeftValid = IsGroundSideValid(isRight: false);
			if (!isLeftValid && !isRightValid)
			{
				Deactivate(allowQueueing: false);
			}
		}
	}

	private bool IsGroundSideValid(bool isRight)
	{
		if (!Helper.IsRayHittingNoTriggers(base.transform.position + Vector3.up * 2f, Vector2.down, 5f, 256, out var closestHit))
		{
			return false;
		}
		Vector2 point = closestHit.point;
		point.y += 0.1f;
		if (Helper.IsRayHittingNoTriggers(point, isRight ? Vector2.right : Vector2.left, targetDistance, 256, out closestHit))
		{
			point = closestHit.point;
		}
		else
		{
			point += (isRight ? Vector2.right : Vector2.left) * targetDistance;
		}
		point.x += (isRight ? (-0.1f) : 0.1f);
		return Helper.IsRayHittingNoTriggers(point, Vector2.down, 0.2f, 256);
	}
}
