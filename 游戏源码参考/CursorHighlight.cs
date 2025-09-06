using UnityEngine;

public class CursorHighlight : MonoBehaviour
{
	private RectTransform myRect;

	private Vector2 startPos;

	private Vector2 targetPos;

	[Tooltip("The time it takes for the cursor to move from one option to another.")]
	public float lerpTime = 1f;

	[Tooltip("The wait period between the cursor moving from one option to another.")]
	public float cursorCooldown = 0.1f;

	private float lerpTimer;

	private float cooldownTimer;

	private bool coolingDown;

	public void Awake()
	{
		myRect = GetComponent<RectTransform>();
	}

	private void Start()
	{
		lerpTimer = lerpTime;
		cooldownTimer = 0f;
		startPos = myRect.anchoredPosition;
		targetPos = startPos;
		coolingDown = false;
	}

	private void Update()
	{
		if (coolingDown)
		{
			if (cooldownTimer > cursorCooldown)
			{
				coolingDown = false;
				cooldownTimer = 0f;
			}
			else
			{
				cooldownTimer += Time.deltaTime;
			}
			return;
		}
		if (lerpTimer > lerpTime)
		{
			lerpTimer = lerpTime;
			coolingDown = true;
		}
		else if (lerpTimer < lerpTime)
		{
			lerpTimer += Time.deltaTime;
		}
		float num = lerpTimer / lerpTime;
		num = num * num * (3f - 2f * num);
		myRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, num);
	}

	public void MoveCursor(RectTransform buttonRect)
	{
		if (!coolingDown)
		{
			startPos = myRect.anchoredPosition;
			targetPos = buttonRect.anchoredPosition;
			lerpTimer = 0f;
		}
	}
}
