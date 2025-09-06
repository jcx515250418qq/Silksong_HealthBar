using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public enum TargetMode
	{
		FOLLOW_HERO = 0,
		LOCK_ZONE = 1,
		BOSS = 2,
		FREE = 3
	}

	private bool verboseMode;

	[HideInInspector]
	public GameManager gm;

	[HideInInspector]
	public HeroController hero_ctrl;

	private Transform heroTransform;

	public CameraController cameraCtrl;

	public TargetMode mode;

	private bool ignoreXOffset;

	public Vector3 destination;

	private Vector3 velocityX;

	private Vector3 velocityY;

	public Vector3 heroPosition_prev;

	public float xOffset;

	public float dashOffset;

	public float wallSprintOffset;

	public float fallOffset;

	public float fallOffset_multiplier;

	public float fallStickOffset;

	public float xLockMin;

	public float xLockMax;

	public float yLockMin;

	public float yLockMax;

	public bool enteredLeft;

	public bool enteredRight;

	public bool enteredTop;

	public bool enteredBot;

	public bool exitedLeft;

	public bool exitedRight;

	public bool exitedTop;

	public bool exitedBot;

	public bool superDashing;

	public bool sprinting;

	public bool harpooning;

	public bool sliding;

	public bool ridingUpdraft;

	public bool wallSprinting;

	public bool superJumping;

	public bool camRising;

	public bool umbrellaFloating;

	public float umbrellaFloatTimer;

	public float superJumpDestinationY;

	public float risingDestinationY;

	public float slowTime = 0.5f;

	public float slowTimeShort = 0.25f;

	public float detachTimeShort = 0.25f;

	public float dampTimeNormal;

	public float dampTimeSlow;

	public float dampTimeSlower;

	public float xLookAhead;

	public float runLookAhead;

	public float dashLookAhead;

	public float superDashLookAhead;

	public float sprintLookAhead;

	public float harpoonLookAhead;

	public float slidingLookAhead;

	public float slidingLookAheadVertical;

	public float updraftLookahead = 12f;

	public float superJumpLookahead = 12f;

	public float risingLookAhead;

	public float wallSprintLookAhead;

	private Vector3 heroPrevPosition;

	private float previousTargetX;

	private float dampTime;

	public float dampTimeX;

	public float dampTimeY;

	private float slowTimer;

	private float detachTimer;

	private float snapDistance = 0.15f;

	public float fallCatcher;

	public bool detachedFromHero;

	public bool stickToHeroX;

	public bool stickToHeroY;

	public bool enteredFromLockZone;

	public bool fallStick;

	private bool isGameplayScene;

	private List<CameraOffsetArea> cameraOffsetAreas = new List<CameraOffsetArea>();

	private Vector3 previousPosition;

	private Vector3 previousHeroPosition;

	public bool IsFreeModeManual { get; private set; }

	private bool IsIgnoringXOffset
	{
		get
		{
			if (!ignoreXOffset)
			{
				return hero_ctrl.playerData?.atBench ?? false;
			}
			return true;
		}
	}

	public void GameInit()
	{
		gm = GameManager.instance;
		if (cameraCtrl == null)
		{
			cameraCtrl = base.transform.parent.GetComponent<CameraController>();
		}
	}

	public void SceneInit()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (gm.IsGameplayScene())
		{
			isGameplayScene = true;
			hero_ctrl = HeroController.instance;
			heroTransform = hero_ctrl.transform;
			mode = TargetMode.FOLLOW_HERO;
			IsFreeModeManual = false;
			xLockMin = 0f;
			xLockMax = cameraCtrl.xLimit;
			yLockMin = 0f;
			yLockMax = cameraCtrl.yLimit;
			StopCamRising();
		}
		else
		{
			isGameplayScene = false;
			mode = TargetMode.FREE;
		}
	}

	public void Update()
	{
		if (hero_ctrl == null || !isGameplayScene)
		{
			mode = TargetMode.FREE;
		}
		else
		{
			if (Time.timeScale <= Mathf.Epsilon || !isGameplayScene)
			{
				return;
			}
			float x = base.transform.position.x;
			float y = base.transform.position.y;
			float z = base.transform.position.z;
			_ = heroTransform.position.x;
			float y2 = heroTransform.position.y;
			y2 += GetCameraOffset().y;
			if (mode == TargetMode.FOLLOW_HERO || mode == TargetMode.LOCK_ZONE)
			{
				SetDampTime();
				destination = heroTransform.position;
				if (IsIgnoringXOffset)
				{
					xOffset = 0f;
				}
				else if (hero_ctrl.cState.transitioning)
				{
					xOffset = (hero_ctrl.cState.facingRight ? 1f : (-1f));
				}
				else if (hero_ctrl.cState.facingRight)
				{
					if (xOffset < xLookAhead)
					{
						xOffset += Time.deltaTime * 6f;
					}
				}
				else if (xOffset > 0f - xLookAhead)
				{
					xOffset -= Time.deltaTime * 6f;
				}
				if (xOffset > 1f)
				{
					xOffset = 1f;
				}
				if (xOffset < -1f)
				{
					xOffset = -1f;
				}
				if (hero_ctrl.cState.dashing && (hero_ctrl.current_velocity.x > 5f || hero_ctrl.current_velocity.x < -5f))
				{
					if (hero_ctrl.cState.facingRight)
					{
						dashOffset = dashLookAhead;
					}
					else
					{
						dashOffset = 0f - dashLookAhead;
					}
				}
				else if (sprinting)
				{
					if (hero_ctrl.cState.facingRight)
					{
						dashOffset = sprintLookAhead;
					}
					else
					{
						dashOffset = 0f - sprintLookAhead;
					}
				}
				else if (sliding)
				{
					if (hero_ctrl.cState.facingRight)
					{
						dashOffset = slidingLookAhead;
					}
					else
					{
						dashOffset = 0f - slidingLookAhead;
					}
				}
				else if (harpooning)
				{
					if (hero_ctrl.cState.facingRight)
					{
						dashOffset = harpoonLookAhead;
					}
					else
					{
						dashOffset = 0f - harpoonLookAhead;
					}
				}
				else
				{
					dashOffset = 0f;
				}
				if (mode == TargetMode.FREE)
				{
					dashOffset = 0f;
				}
				if (umbrellaFloating)
				{
					umbrellaFloatTimer += Time.deltaTime;
				}
				else
				{
					umbrellaFloatTimer = 0f;
				}
				if (ridingUpdraft)
				{
					wallSprintOffset = updraftLookahead;
				}
				else if (superJumping)
				{
					cameraCtrl.isRising = true;
					wallSprintOffset = superJumpLookahead;
					if (y + wallSprintOffset > superJumpDestinationY)
					{
						wallSprintOffset = superJumpDestinationY - y;
					}
				}
				else if (camRising)
				{
					wallSprintOffset = risingLookAhead;
					if (y + wallSprintOffset > risingDestinationY)
					{
						wallSprintOffset = risingDestinationY - y;
					}
				}
				else if (sliding)
				{
					wallSprintOffset = slidingLookAheadVertical;
				}
				else if (hero_ctrl.cState.falling && hero_ctrl.current_velocity.y < -22f)
				{
					cameraCtrl.isFalling = true;
					wallSprintOffset -= Time.deltaTime * 4f;
					if (wallSprintOffset < -1f)
					{
						wallSprintOffset = -1f;
					}
				}
				else if (umbrellaFloatTimer > 1.25f)
				{
					wallSprintOffset -= Time.deltaTime * 1f;
					if (wallSprintOffset < -1.5f)
					{
						wallSprintOffset = -1.5f;
					}
				}
				else
				{
					if (cameraCtrl.isFalling)
					{
						cameraCtrl.isFalling = false;
					}
					if (cameraCtrl.isRising)
					{
						cameraCtrl.isRising = false;
					}
					wallSprintOffset = 0f;
				}
				if (mode == TargetMode.FREE)
				{
					wallSprintOffset = 0f;
				}
				destination = new Vector2(destination.x + xOffset + dashOffset, destination.y + wallSprintOffset);
				float num;
				float num2;
				float num3;
				float num4;
				if (mode == TargetMode.FOLLOW_HERO)
				{
					num = 0f;
					num2 = 9999f;
					num3 = 0f;
					num4 = 9999f;
				}
				else
				{
					num = xLockMin;
					num2 = xLockMax;
					num3 = yLockMin;
					num4 = yLockMax;
				}
				if (destination.x < num)
				{
					destination.x = num;
				}
				if (destination.x > num2)
				{
					destination.x = num2;
				}
				if (destination.y < num3)
				{
					destination.y = num3;
				}
				if (destination.y > num4)
				{
					destination.y = num4;
				}
				Vector3 vector = heroTransform.position - heroPosition_prev;
				if (hero_ctrl.cState.transitioning)
				{
					base.transform.position = destination;
				}
				else if (detachTimer <= 0f && !detachedFromHero)
				{
					if (vector.x < -0.001f && base.transform.position.x > destination.x)
					{
						base.transform.SetPositionX(base.transform.position.x + vector.x);
						if (base.transform.position.x < destination.x)
						{
							base.transform.SetPositionX(destination.x);
						}
					}
					if (vector.x > 0.001f && base.transform.position.x < destination.x)
					{
						base.transform.SetPositionX(base.transform.position.x + vector.x);
						if (base.transform.position.x > destination.x)
						{
							base.transform.SetPositionX(destination.x);
						}
					}
					if (vector.y < -0.001f && base.transform.position.y > destination.y)
					{
						base.transform.SetPositionY(base.transform.position.y + vector.y);
						if (base.transform.position.y < destination.y)
						{
							base.transform.SetPositionY(destination.y);
						}
					}
					if (vector.y > 0.001f && base.transform.position.y < destination.y)
					{
						base.transform.SetPositionY(base.transform.position.y + vector.y);
						if (base.transform.position.y > destination.y)
						{
							base.transform.SetPositionY(destination.y);
						}
					}
				}
				heroPosition_prev = heroTransform.position;
				base.transform.position = new Vector3(Vector3.SmoothDamp(base.transform.position, new Vector3(destination.x, y, z), ref velocityX, dampTimeX).x, Vector3.SmoothDamp(base.transform.position, new Vector3(x, destination.y, z), ref velocityY, dampTimeY).y, z);
			}
			heroPrevPosition = heroTransform.position;
			if (!cameraCtrl.AllowExitingSceneBounds)
			{
				if (base.transform.position.x < 14.6f)
				{
					base.transform.SetPositionX(14.6f);
				}
				if (base.transform.position.x > cameraCtrl.xLimit)
				{
					base.transform.SetPositionX(cameraCtrl.xLimit);
				}
				if (base.transform.position.y < 8.3f)
				{
					base.transform.SetPositionY(8.3f);
				}
				if (base.transform.position.y > cameraCtrl.yLimit)
				{
					base.transform.SetPositionY(cameraCtrl.yLimit);
				}
				if (base.transform.position.x + xOffset < 14.6f)
				{
					xOffset = 14.6f - base.transform.position.x - 0.001f;
				}
				if (base.transform.position.x + xOffset > cameraCtrl.xLimit)
				{
					xOffset = cameraCtrl.xLimit - base.transform.position.x + 0.001f;
				}
			}
		}
	}

	public void EnterLockZone(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
	{
		if (this == null || IsFreeModeManual)
		{
			return;
		}
		float num = xLockMin;
		float num2 = xLockMax;
		float num3 = yLockMin;
		float num4 = yLockMax;
		xLockMin = xLockMin_var;
		xLockMax = xLockMax_var;
		yLockMin = yLockMin_var;
		yLockMax = yLockMax_var;
		mode = TargetMode.LOCK_ZONE;
		_ = base.transform.position;
		_ = base.transform.position;
		float x = heroTransform.position.x;
		float y = heroTransform.position.y;
		float num5 = x + xOffset + dashOffset;
		if (num5 < xLockMin)
		{
			num5 = xLockMin;
		}
		if (num5 > xLockMax)
		{
			num5 = xLockMax;
		}
		float num6 = x + xOffset + dashOffset;
		if (num6 < num)
		{
			num6 = num;
		}
		if (num6 > num2)
		{
			num6 = num2;
		}
		if (num5 != num6)
		{
			if (num6 - num5 < -9f || num6 - num5 > 9f)
			{
				dampTimeX = dampTimeSlower;
			}
			else
			{
				dampTimeX = dampTimeSlow;
			}
			slowTimer = slowTime;
		}
		float num7 = y + wallSprintOffset;
		if (num7 < yLockMin)
		{
			num7 = yLockMin;
		}
		if (num7 > yLockMax)
		{
			num7 = yLockMax;
		}
		float num8 = y + wallSprintOffset;
		if (num8 < num3)
		{
			num8 = num3;
		}
		if (num8 > num4)
		{
			num8 = num4;
		}
		if (num7 != num8)
		{
			if (num8 - num7 < -9f || num8 - num7 > 9f)
			{
				dampTimeY = dampTimeSlower;
			}
			else
			{
				dampTimeY = dampTimeSlow;
			}
			slowTimer = slowTime;
		}
	}

	public void EnterLockZoneInstant(float xLockMin_var, float xLockMax_var, float yLockMin_var, float yLockMax_var)
	{
		if (!IsFreeModeManual)
		{
			xLockMin = xLockMin_var;
			xLockMax = xLockMax_var;
			yLockMin = yLockMin_var;
			yLockMax = yLockMax_var;
			mode = TargetMode.LOCK_ZONE;
			if (base.transform.position.x < xLockMin)
			{
				base.transform.SetPositionX(xLockMin);
			}
			if (base.transform.position.x > xLockMax)
			{
				base.transform.SetPositionX(xLockMax);
			}
			if (base.transform.position.y < yLockMin)
			{
				base.transform.SetPositionY(yLockMin);
			}
			if (base.transform.position.y > yLockMax)
			{
				base.transform.SetPositionY(yLockMax);
			}
			stickToHeroX = true;
			stickToHeroY = true;
		}
	}

	public void ExitLockZone()
	{
		if (hero_ctrl == null || mode == TargetMode.FREE)
		{
			return;
		}
		if (hero_ctrl.cState.hazardDeath || hero_ctrl.cState.dead || (hero_ctrl.transitionState != 0 && hero_ctrl.transitionState != HeroTransitionState.WAITING_TO_ENTER_LEVEL))
		{
			mode = TargetMode.FREE;
		}
		else
		{
			mode = TargetMode.FOLLOW_HERO;
		}
		float num = xLockMin;
		float num2 = xLockMax;
		float num3 = yLockMin;
		float num4 = yLockMax;
		_ = base.transform.position;
		_ = base.transform.position;
		_ = base.transform.position;
		float x = heroTransform.position.x;
		float y = heroTransform.position.y;
		xLockMin = 0f;
		xLockMax = cameraCtrl.xLimit;
		yLockMin = 0f;
		yLockMax = cameraCtrl.yLimit;
		float num5 = x + xOffset + dashOffset;
		if (num5 < xLockMin)
		{
			num5 = xLockMin;
		}
		if (num5 > xLockMax)
		{
			num5 = xLockMax;
		}
		float num6 = x + xOffset + dashOffset;
		if (num6 < num)
		{
			num6 = num;
		}
		if (num6 > num2)
		{
			num6 = num2;
		}
		if (num5 != num6)
		{
			if (num6 - num5 < -9f || num6 - num5 > 9f)
			{
				dampTimeX = dampTimeSlower;
			}
			else
			{
				dampTimeX = dampTimeSlow;
			}
			slowTimer = slowTime;
		}
		float num7 = y + wallSprintOffset;
		if (num7 < yLockMin)
		{
			num7 = yLockMin;
		}
		if (num7 > yLockMax)
		{
			num7 = yLockMax;
		}
		float num8 = y + wallSprintOffset;
		if (num8 < num3)
		{
			num8 = num3;
		}
		if (num8 > num4)
		{
			num8 = num4;
		}
		if (num7 != num8)
		{
			if (num8 - num7 < -9f || num8 - num7 > 9f)
			{
				dampTimeY = dampTimeSlower;
			}
			else
			{
				dampTimeY = dampTimeSlow;
			}
			slowTimer = slowTime;
		}
	}

	private void SetDampTime()
	{
		if (slowTimer > 0f)
		{
			slowTimer -= Time.deltaTime;
		}
		else
		{
			if (dampTimeX > dampTimeNormal)
			{
				dampTimeX -= 0.4f * Time.deltaTime;
			}
			else if (dampTimeX < dampTimeNormal)
			{
				dampTimeX = dampTimeNormal;
			}
			if (dampTimeY > dampTimeNormal)
			{
				dampTimeY -= 0.4f * Time.deltaTime;
			}
			else if (dampTimeY < dampTimeNormal)
			{
				dampTimeY = dampTimeNormal;
			}
		}
		if (detachTimer > 0f)
		{
			detachTimer -= Time.deltaTime;
		}
	}

	public void SetSuperDash(bool active)
	{
		superDashing = active;
	}

	public void SetSprint(bool active)
	{
		sprinting = active;
	}

	public void SetSliding(bool active)
	{
		sliding = active;
	}

	public void SetHarpooning(bool active)
	{
		harpooning = active;
	}

	public void SetUpdraft(bool active)
	{
		ridingUpdraft = active;
	}

	public void SetSuperJump(bool active, float destinationY)
	{
		superJumping = active;
		superJumpDestinationY = destinationY;
	}

	public void StartCamRising(float lookAhead, float destinationY)
	{
		camRising = true;
		risingLookAhead = lookAhead;
		risingDestinationY = destinationY;
	}

	public void StopCamRising()
	{
		camRising = false;
	}

	public void SetWallSprint(bool active)
	{
		wallSprinting = active;
	}

	public void FreezeInPlace()
	{
		mode = TargetMode.FREE;
	}

	public void StartFreeMode(bool useXOffset)
	{
		mode = TargetMode.FREE;
		IsFreeModeManual = true;
		ignoreXOffset = !useXOffset;
	}

	public void SetIgnoringXOffset(bool value)
	{
		ignoreXOffset = value;
	}

	public void EndFreeMode()
	{
		mode = TargetMode.FOLLOW_HERO;
		IsFreeModeManual = false;
		if ((bool)cameraCtrl.CurrentLockArea)
		{
			cameraCtrl.LockToArea(cameraCtrl.CurrentLockArea);
		}
	}

	public void StartLockMode()
	{
		mode = TargetMode.LOCK_ZONE;
	}

	public void SetUmbrellaFloating(bool isFloating)
	{
		umbrellaFloating = isFloating;
	}

	public void EndFallStick()
	{
		fallStick = false;
	}

	public void ShortDamp()
	{
		dampTimeX = dampTimeSlow;
		dampTimeY = dampTimeSlow;
		slowTimer = slowTimeShort;
		detachTimer = detachTimeShort;
	}

	public void ShortDetach()
	{
		detachTimer = detachTimeShort;
	}

	public void ShorterDetach()
	{
		detachTimer = 0.1f;
	}

	public void SetDetachedFromHero(bool detach)
	{
		detachedFromHero = detach;
	}

	public void PositionToStart()
	{
		EndFallStick();
		float x = base.transform.position.x;
		_ = base.transform.position;
		float x2 = heroTransform.position.x;
		float y = heroTransform.position.y;
		Vector2 cameraOffset = GetCameraOffset();
		x2 += cameraOffset.x;
		y += cameraOffset.y;
		velocityX = Vector3.zero;
		velocityY = Vector3.zero;
		destination = heroTransform.position;
		if (IsIgnoringXOffset)
		{
			xOffset = 0f;
		}
		else
		{
			xOffset = (hero_ctrl.cState.facingRight ? 1f : (-1f));
		}
		if (mode == TargetMode.LOCK_ZONE)
		{
			if (x2 < xLockMin && hero_ctrl.cState.facingRight)
			{
				xOffset = x2 - x + 1f;
			}
			if (x2 > xLockMax && !hero_ctrl.cState.facingRight)
			{
				xOffset = x2 - x - 1f;
			}
			if (x + xOffset > xLockMax)
			{
				xOffset = xLockMax - x;
			}
			if (x + xOffset < xLockMin)
			{
				xOffset = xLockMin - x;
			}
		}
		if (xOffset < 0f - xLookAhead)
		{
			xOffset = 0f - xLookAhead;
		}
		if (xOffset > xLookAhead)
		{
			xOffset = xLookAhead;
		}
		destination.x += xOffset;
		if (verboseMode)
		{
			Debug.LogFormat("CT PTS - xOffset: {0} HeroPos: {1}, {2}", xOffset, x2, y);
		}
		if (mode == TargetMode.FOLLOW_HERO)
		{
			if (verboseMode)
			{
				Debug.LogFormat("CT PTS - Follow Hero - CT Pos: {0}", base.transform.position);
			}
			base.transform.position = cameraCtrl.KeepWithinSceneBounds(destination);
		}
		else if (mode == TargetMode.LOCK_ZONE)
		{
			if (destination.x < xLockMin)
			{
				destination.x = xLockMin;
			}
			if (destination.x > xLockMax)
			{
				destination.x = xLockMax;
			}
			if (destination.y < yLockMin)
			{
				destination.y = yLockMin;
			}
			if (destination.y > yLockMax)
			{
				destination.y = yLockMax;
			}
			base.transform.position = destination;
			if (verboseMode)
			{
				Debug.LogFormat("CT PTS - Lock Zone - CT Pos: {0}", base.transform.position);
			}
		}
		if (verboseMode)
		{
			Debug.LogFormat("CT - PTS: HeroPos: {0} Mode: {1} Dest: {2}", heroTransform.position, mode, destination);
		}
		heroPrevPosition = heroTransform.position;
	}

	public void AddOffsetArea(CameraOffsetArea offsetArea)
	{
		int num = cameraOffsetAreas.IndexOf(offsetArea);
		if (num >= 0)
		{
			cameraOffsetAreas.RemoveAt(num);
		}
		cameraOffsetAreas.Add(offsetArea);
	}

	public void RemoveOffsetArea(CameraOffsetArea offsetArea)
	{
		cameraOffsetAreas.Remove(offsetArea);
	}

	public Vector2 GetCameraOffset()
	{
		if (cameraOffsetAreas.Count == 0)
		{
			return Vector2.zero;
		}
		return (Vector3)cameraOffsetAreas[cameraOffsetAreas.Count - 1].Offset;
	}
}
