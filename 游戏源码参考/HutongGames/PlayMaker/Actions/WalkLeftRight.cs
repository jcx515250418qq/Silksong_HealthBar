using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class WalkLeftRight : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public float walkSpeed = 4f;

		public bool spriteFacesLeft;

		public FsmString groundLayer = "Terrain";

		public float turnDelay = 1f;

		public float wallLookAhead;

		private double nextTurnTime;

		[Header("Animation")]
		public FsmString walkAnimName;

		public FsmString turnAnimName;

		public FsmBool moveWhileTurning;

		public FsmBool startLeft;

		public FsmBool startRight;

		public FsmBool keepDirection;

		public FsmBool flipBeforeTurn;

		private float scaleX_pos;

		private float scaleX_neg;

		private const float wallRayHeight = 0.5f;

		private const float wallRayLength = 0.1f;

		private const float groundRayLength = 1f;

		private GameObject target;

		private Rigidbody2D body;

		private tk2dSpriteAnimator spriteAnimator;

		private Collider2D collider;

		private Coroutine walkRoutine;

		private Coroutine turnRoutine;

		private bool shouldTurn;

		private UnityEngine.Bounds bounds;

		private int cycle;

		private int boundsVer;

		private float Direction
		{
			get
			{
				if ((bool)target)
				{
					return Mathf.Sign(target.transform.localScale.x) * (float)((!spriteFacesLeft) ? 1 : (-1));
				}
				return 0f;
			}
		}

		public override void OnEnter()
		{
			cycle = 0;
			boundsVer = -1;
			UpdateIfTargetChanged();
			SetupStartingDirection();
			walkRoutine = StartCoroutine(Walk());
			if (walkAnimName.Value == "")
			{
				walkAnimName.Value = "Walk";
			}
		}

		public override void OnExit()
		{
			if (walkRoutine != null)
			{
				StopCoroutine(walkRoutine);
				walkRoutine = null;
			}
			if (turnRoutine != null)
			{
				StopCoroutine(turnRoutine);
				turnRoutine = null;
			}
		}

		private void UpdateIfTargetChanged()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != target)
			{
				target = ownerDefaultTarget;
				body = target.GetComponent<Rigidbody2D>();
				spriteAnimator = target.GetComponent<tk2dSpriteAnimator>();
				collider = target.GetComponent<Collider2D>();
			}
		}

		private IEnumerator Walk()
		{
			if ((bool)spriteAnimator)
			{
				spriteAnimator.Play(walkAnimName.Value);
			}
			while (true)
			{
				if ((bool)body)
				{
					Vector2 linearVelocity = body.linearVelocity;
					linearVelocity.x = walkSpeed * Direction;
					body.linearVelocity = linearVelocity;
					cycle++;
					if (shouldTurn || (Time.timeAsDouble >= nextTurnTime && CheckIsGrounded() && (CheckWall() || CheckFloor())))
					{
						shouldTurn = false;
						nextTurnTime = Time.timeAsDouble + (double)turnDelay;
						yield return Turn();
					}
				}
				yield return new WaitForFixedUpdate();
			}
		}

		private IEnumerator Turn()
		{
			if (!moveWhileTurning.Value)
			{
				Vector2 linearVelocity = body.linearVelocity;
				linearVelocity.x = 0f;
				body.linearVelocity = linearVelocity;
				if (flipBeforeTurn.Value)
				{
					FlipScale();
				}
				tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName(turnAnimName.Value);
				if (clipByName != null)
				{
					float seconds = (float)clipByName.frames.Length / clipByName.fps;
					spriteAnimator.Play(clipByName);
					yield return new WaitForSeconds(seconds);
				}
				if (!flipBeforeTurn.Value)
				{
					FlipScale();
				}
				if ((bool)spriteAnimator)
				{
					spriteAnimator.Play(walkAnimName.Value);
				}
				turnRoutine = null;
			}
			else
			{
				tk2dSpriteAnimationClip clipByName2 = spriteAnimator.GetClipByName(turnAnimName.Value);
				Vector3 localScale = target.transform.localScale;
				localScale.x *= -1f;
				target.transform.localScale = localScale;
				Vector2 linearVelocity2 = body.linearVelocity;
				linearVelocity2.x = 0f - linearVelocity2.x;
				body.linearVelocity = linearVelocity2;
				float seconds2 = (float)clipByName2.frames.Length / clipByName2.fps;
				spriteAnimator.Play(clipByName2);
				yield return new WaitForSeconds(seconds2);
				if ((bool)spriteAnimator)
				{
					spriteAnimator.Play(walkAnimName.Value);
				}
				turnRoutine = null;
			}
		}

		private void FlipScale()
		{
			Vector3 localScale = target.transform.localScale;
			localScale.x *= -1f;
			target.transform.localScale = localScale;
		}

		private void UpdateBounds()
		{
			if (boundsVer != cycle)
			{
				boundsVer = cycle;
				bounds = collider.bounds;
			}
		}

		private bool CheckWall()
		{
			UpdateBounds();
			Vector2 origin = (Vector2)bounds.center + new Vector2(0f, 0f - bounds.size.y / 2f + 0.5f);
			Vector2 direction = Vector2.right * Direction;
			float distance = bounds.size.x / 2f + 0.1f + wallLookAhead;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, direction, distance, 33024);
			if (raycastHit2D.collider != null && !raycastHit2D.collider.isTrigger)
			{
				return true;
			}
			return false;
		}

		private bool CheckFloor()
		{
			UpdateBounds();
			if (Helper.Raycast2D((Vector2)bounds.center + new Vector2((bounds.size.x / 2f + 0.1f) * Direction, 0f - bounds.size.y / 2f + 0.5f), Vector2.down, 1f, 256).collider != null)
			{
				return false;
			}
			return true;
		}

		private bool CheckIsGrounded()
		{
			UpdateBounds();
			if (Helper.Raycast2D((Vector2)bounds.center + new Vector2(0f, 0f - bounds.size.y / 2f + 0.5f), Vector2.down, 1f, 256).collider != null)
			{
				return true;
			}
			return false;
		}

		private void SetupStartingDirection()
		{
			if (target.transform.localScale.x < 0f)
			{
				if (!spriteFacesLeft && startRight.Value)
				{
					shouldTurn = true;
				}
				if (spriteFacesLeft && startLeft.Value)
				{
					shouldTurn = true;
				}
			}
			else
			{
				if (spriteFacesLeft && startRight.Value)
				{
					shouldTurn = true;
				}
				if (!spriteFacesLeft && startLeft.Value)
				{
					shouldTurn = true;
				}
			}
			if (!startLeft.Value && !startRight.Value && !keepDirection.Value && Random.Range(0f, 100f) <= 50f)
			{
				shouldTurn = true;
			}
			startLeft.Value = false;
			startRight.Value = false;
		}
	}
}
