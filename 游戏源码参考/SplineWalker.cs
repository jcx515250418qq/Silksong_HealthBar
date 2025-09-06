using System;
using GlobalEnums;
using TeamCherry.Splines;
using UnityEngine;

public class SplineWalker : MonoBehaviour, IInteractableBlocker
{
	private enum MoveStates
	{
		None = -1,
		Walking = 0,
		Flying = 1
	}

	[Serializable]
	private class WalkAudio
	{
		public EnvironmentTypes Environment;

		public AudioClip Clip;
	}

	private const string FLY_UP_ANIM = "Fly Up";

	private const string FLY_DOWN_ANIM = "Fly Down";

	private const string FLY_ANTIC_ANIM = "Fly Antic";

	private const string FLY_LAND_ANIM = "Land";

	private const string WALK_ANIM = "Walk";

	private const string WALK_TURNED_ANIM = "TurnToWalk";

	[SerializeField]
	private HermiteSplinePath path;

	[SerializeField]
	private float walkingZ;

	[SerializeField]
	private InteractableBase blockInteractable;

	[Space]
	[SerializeField]
	private float walkSpeed;

	[SerializeField]
	private float flySpeed;

	[SerializeField]
	private float flyAcceleration;

	[SerializeField]
	private float flyMaxSpeed;

	[SerializeField]
	private bool spriteFacesRight;

	[SerializeField]
	private float groundDistanceThreshold;

	[SerializeField]
	private float targetDistance;

	[Space]
	[SerializeField]
	private AudioSource loopSource;

	[SerializeField]
	private WalkAudio[] walkAudios;

	[Space]
	[SerializeField]
	private AudioSource audioLoopFly;

	[SerializeField]
	private PlayMakerFSM walkVoiceController;

	[SerializeField]
	private RandomAudioClipTable landAudioClipTable;

	[SerializeField]
	private RandomAudioClipTable takeoffVoiceAudioClipTable;

	[SerializeField]
	private RandomAudioClipTable landVoiceAudioClipTable;

	private Vector3 targetPos;

	private Vector3 adjustedTargetPos;

	private Vector2 runnerVelocity;

	private float splineDirection;

	private float currentFlySpeed;

	private bool isFollowingPath;

	private bool isInCustomAnim;

	private float splineDistance;

	private bool isTargetAtEnd;

	private MoveStates currentMoveState;

	private MoveStates prevMoveState;

	private bool wasGroundSnapped;

	private float groundSnapLerpMultiplier;

	private float groundSnapT;

	private float previousGroundSnappedY;

	private BoxCollider2D box;

	private tk2dSpriteAnimator animator;

	private EnviroRegionListener enviroListener;

	private HeroPerformanceSingReaction heroPerformanceSingReaction;

	private bool hasHeroPerformanceSingReaction;

	public bool IsBlocking
	{
		get
		{
			if (currentMoveState != MoveStates.Flying)
			{
				return isInCustomAnim;
			}
			return true;
		}
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(targetPos, 0.15f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(adjustedTargetPos, 0.2f);
		}
	}

	private void Awake()
	{
		box = GetComponent<BoxCollider2D>();
		animator = GetComponent<tk2dSpriteAnimator>();
		enviroListener = base.gameObject.AddComponent<EnviroRegionListener>();
		EnviroRegionListener enviroRegionListener = enviroListener;
		enviroRegionListener.CurrentEnvironmentTypeChanged = (Action<EnvironmentTypes>)Delegate.Combine(enviroRegionListener.CurrentEnvironmentTypeChanged, new Action<EnvironmentTypes>(OnCurrentEnvironmentTypeChanged));
		animator.AnimationCompleted = OnAnimationCompleted;
		heroPerformanceSingReaction = GetComponent<HeroPerformanceSingReaction>();
		hasHeroPerformanceSingReaction = heroPerformanceSingReaction != null;
		if ((bool)blockInteractable)
		{
			blockInteractable.AddBlocker(this);
		}
	}

	private void Update()
	{
		UpdateState();
		prevMoveState = currentMoveState;
	}

	private void UpdateState()
	{
		if (!isFollowingPath || isInCustomAnim)
		{
			return;
		}
		UpdateTarget();
		UpdateRunner();
		if (!isFollowingPath || isInCustomAnim)
		{
			return;
		}
		switch (currentMoveState)
		{
		case MoveStates.Walking:
			if (prevMoveState != 0)
			{
				if (hasHeroPerformanceSingReaction)
				{
					heroPerformanceSingReaction.enabled = true;
				}
				if (!animator.IsPlaying("Walk") && !animator.IsPlaying("TurnToWalk"))
				{
					animator.Play("Walk");
				}
				if ((bool)loopSource)
				{
					loopSource.Play();
				}
				if ((bool)audioLoopFly)
				{
					audioLoopFly.Stop();
				}
				if ((bool)walkVoiceController)
				{
					walkVoiceController.SendEventSafe("WALK START");
				}
				base.transform.SetPositionZ(walkingZ);
				box.enabled = true;
			}
			break;
		case MoveStates.Flying:
		{
			string text = ((runnerVelocity.y > 0f) ? "Fly Up" : "Fly Down");
			if (!animator.IsPlaying(text))
			{
				animator.Play(text);
			}
			box.enabled = CanCollide(targetPos.z);
			float z = targetPos.z;
			if (z < walkingZ)
			{
				z = walkingZ;
			}
			base.transform.SetPositionZ(z);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private bool CanCollide(float zPos)
	{
		return zPos.IsWithinTolerance(0.1f, walkingZ);
	}

	private void UpdateTarget()
	{
		if (isTargetAtEnd)
		{
			return;
		}
		Vector2 vector = base.transform.position;
		targetPos = path.GetPositionAlongSpline(splineDistance);
		while (!(((Vector2)targetPos - vector).magnitude >= targetDistance))
		{
			splineDistance += 0.1f * splineDirection;
			targetPos = path.GetPositionAlongSpline(splineDistance);
			if (splineDirection > 0f)
			{
				if (splineDistance < path.TotalDistance)
				{
					continue;
				}
			}
			else if (splineDistance > 0f)
			{
				continue;
			}
			isTargetAtEnd = true;
			break;
		}
	}

	private void UpdateRunner()
	{
		TrySnapToGround(targetPos, out adjustedTargetPos, isTarget: true);
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Vector2 vector = (Vector2)adjustedTargetPos - (Vector2)position;
		Vector2 vector2 = vector;
		float num;
		if (currentMoveState == MoveStates.Walking)
		{
			vector2.y = 0f;
			num = walkSpeed;
		}
		else
		{
			if (vector2.y > 0f)
			{
				currentFlySpeed += flyAcceleration * Time.deltaTime;
			}
			else
			{
				currentFlySpeed = flySpeed;
			}
			if (flyMaxSpeed > flySpeed && currentFlySpeed > flyMaxSpeed)
			{
				currentFlySpeed = flyMaxSpeed;
			}
			num = currentFlySpeed;
		}
		vector2 = ((vector2.magnitude > 0f) ? vector2.normalized : Vector2.zero);
		runnerVelocity = vector2 * num;
		if (isTargetAtEnd && ShouldTurn(runnerVelocity.x))
		{
			StopWalking();
			return;
		}
		position += runnerVelocity.ToVector3(0f) * Time.deltaTime;
		Vector3 vector3 = position;
		Vector3 newPos;
		float groundDistance;
		if (Vector2.Dot(vector.normalized, Vector2.up) > 0.5f)
		{
			StartFlying(doLaunch: true);
		}
		else if (TrySnapToGround(position, out newPos, isTarget: false, out groundDistance))
		{
			if (wasGroundSnapped)
			{
				if (newPos.y + 0.2f < previousGroundSnappedY)
				{
					wasGroundSnapped = false;
				}
				else if (newPos.y - 0.2f > previousGroundSnappedY && animator.IsPlaying("Walk"))
				{
					animator.PlayFromFrame(0);
				}
			}
			if (groundDistance < 0.1f)
			{
				if (currentMoveState == MoveStates.Flying && (bool)landAudioClipTable)
				{
					landAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				}
				if (currentMoveState == MoveStates.Flying && (bool)landVoiceAudioClipTable)
				{
					landVoiceAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				}
				currentMoveState = MoveStates.Walking;
				wasGroundSnapped = true;
				groundSnapT = 1f;
				groundSnapLerpMultiplier = 1f;
				vector3 = newPos;
			}
			else
			{
				if (!wasGroundSnapped)
				{
					wasGroundSnapped = true;
					groundSnapLerpMultiplier = 1f;
					groundSnapT = 0f;
				}
				groundSnapLerpMultiplier += Time.deltaTime * Mathf.Abs(Physics2D.gravity.y);
				groundSnapT += Time.deltaTime * groundSnapLerpMultiplier;
				if (groundSnapT > 1f)
				{
					groundSnapT = 1f;
				}
				vector3 = Vector2.Lerp(position, newPos, groundSnapT);
			}
			previousGroundSnappedY = newPos.y;
		}
		else
		{
			StartFlying(vector.y > 0f);
		}
		if (ShouldTurn(runnerVelocity.x))
		{
			animator.transform.FlipLocalScale(x: true);
			if (currentMoveState == MoveStates.Walking)
			{
				animator.Play("TurnToWalk");
			}
		}
		transform.SetPosition2D(vector3);
	}

	private void StartFlying(bool doLaunch)
	{
		wasGroundSnapped = false;
		if ((bool)loopSource)
		{
			loopSource.Stop();
		}
		if (currentMoveState != MoveStates.Flying)
		{
			if ((bool)audioLoopFly)
			{
				audioLoopFly.Play();
			}
			if ((bool)takeoffVoiceAudioClipTable)
			{
				takeoffVoiceAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
			}
			if ((bool)walkVoiceController)
			{
				walkVoiceController.SendEventSafe("WALK STOP");
			}
			if (doLaunch)
			{
				animator.Play("Fly Antic");
				isInCustomAnim = true;
			}
			currentMoveState = MoveStates.Flying;
			if (hasHeroPerformanceSingReaction)
			{
				heroPerformanceSingReaction.enabled = false;
			}
			currentFlySpeed = flySpeed;
		}
	}

	private bool ShouldTurn(float moveDir)
	{
		float num = base.transform.localScale.x * (float)(spriteFacesRight ? 1 : (-1));
		if (!(num < 0f) || !(moveDir > 0f))
		{
			if (num > 0f)
			{
				return moveDir < 0f;
			}
			return false;
		}
		return true;
	}

	private void TrySnapToGround(Vector3 initialPos, out Vector3 newPos, bool isTarget)
	{
		TrySnapToGround(initialPos, out newPos, isTarget, out var _);
	}

	private bool TrySnapToGround(Vector3 initialPos, out Vector3 newPos, bool isTarget, out float groundDistance)
	{
		Vector2 vector = (Vector2)initialPos + box.offset;
		Vector2 vector2 = box.size * 0.5f;
		Vector2 vector3 = vector - vector2;
		float num = vector.y - vector3.y;
		RaycastHit2D raycastHit2D;
		if (CanCollide(initialPos.z))
		{
			float distance = num + groundDistanceThreshold;
			raycastHit2D = Helper.Raycast2D(vector, Vector2.down, distance, 256);
		}
		else
		{
			raycastHit2D = default(RaycastHit2D);
		}
		bool result;
		if ((bool)raycastHit2D)
		{
			newPos = raycastHit2D.point;
			newPos.y += vector2.y - box.offset.y;
			result = true;
			groundDistance = vector3.y - raycastHit2D.point.y;
		}
		else
		{
			newPos = initialPos;
			if (isTarget)
			{
				newPos.y += num;
			}
			result = false;
			groundDistance = float.MaxValue;
		}
		return result;
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator spriteAnimator, tk2dSpriteAnimationClip clip)
	{
		if (isFollowingPath)
		{
			string text = clip.name;
			if (text == "Fly Antic" || text == "Land")
			{
				isInCustomAnim = false;
			}
		}
	}

	[ContextMenu("TEST FORWARD", true)]
	[ContextMenu("TEST BACKWARD", true)]
	public bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("TEST FORWARD")]
	public void TestForward()
	{
		StartWalking(0f, 1f);
	}

	[ContextMenu("TEST BACKWARD")]
	public void TestBackward()
	{
		StartWalking(1f, -1f);
	}

	public void StartWalking(float percentage, float moveDirection)
	{
		StopWalking();
		isInCustomAnim = false;
		isTargetAtEnd = false;
		splineDirection = moveDirection;
		prevMoveState = MoveStates.None;
		splineDistance = path.TotalDistance * percentage;
		Vector3 newPos = path.GetPositionAlongSpline(splineDistance);
		newPos.y += 1f;
		TrySnapToGround(newPos, out newPos, isTarget: false);
		base.transform.position = newPos;
		float moveDir = path.GetPositionAlongSpline(splineDistance + 0.1f * splineDirection).x - newPos.x;
		if (ShouldTurn(moveDir))
		{
			base.transform.FlipLocalScale(x: true);
		}
		ResumeWalking();
	}

	public void StopWalking()
	{
		isFollowingPath = false;
		if ((bool)loopSource)
		{
			loopSource.Stop();
		}
		if ((bool)audioLoopFly)
		{
			audioLoopFly.Stop();
		}
		if ((bool)walkVoiceController)
		{
			walkVoiceController.SendEventSafe("WALK STOP");
		}
	}

	public void ResumeWalking()
	{
		isFollowingPath = true;
		prevMoveState = MoveStates.None;
	}

	private void OnCurrentEnvironmentTypeChanged(EnvironmentTypes newEnviroType)
	{
		if (!loopSource)
		{
			return;
		}
		AudioClip audioClip = null;
		WalkAudio[] array = walkAudios;
		foreach (WalkAudio walkAudio in array)
		{
			if (walkAudio.Environment == newEnviroType)
			{
				audioClip = walkAudio.Clip;
				break;
			}
		}
		if (!audioClip)
		{
			loopSource.clip = null;
			loopSource.Stop();
			Debug.LogWarning($"{base.gameObject.name} does not have clip for {newEnviroType} enviro type");
			return;
		}
		loopSource.clip = audioClip;
		if (currentMoveState == MoveStates.Walking)
		{
			loopSource.Play();
		}
	}
}
