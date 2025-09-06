using UnityEngine;

public class FeatherPhysics : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private float transitionTime;

	[Space]
	[SerializeField]
	private float curveTime = 1f;

	[SerializeField]
	private AnimationCurve velocityCurveX;

	[SerializeField]
	private float velocityMagnitudeX = 1f;

	[SerializeField]
	private AnimationCurve velocityCurveY;

	[SerializeField]
	private float velocityMagnitudeY = 1f;

	[SerializeField]
	private bool doNotAnimateY;

	[Space]
	[SerializeField]
	private Vector2 groundRayOrigin;

	[SerializeField]
	private float groundRayLength = 0.5f;

	private bool recordedGravity;

	private float elapsedTime;

	private float transitionTimeLeft;

	private float gravityScale;

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(groundRayOrigin), 0.2f);
	}

	private void Awake()
	{
		SurfaceWaterFloater component = GetComponent<SurfaceWaterFloater>();
		if ((bool)component)
		{
			component.OnLandInWater.AddListener(delegate
			{
				base.enabled = false;
			});
			component.OnExitWater.AddListener(delegate
			{
				base.enabled = true;
			});
		}
		if ((bool)body)
		{
			recordedGravity = true;
			gravityScale = body.gravityScale;
		}
	}

	private void OnEnable()
	{
		if (!body)
		{
			base.enabled = false;
		}
		else if (recordedGravity)
		{
			body.gravityScale = gravityScale;
		}
		elapsedTime = Random.Range(0f, curveTime);
	}

	private void OnDisable()
	{
		if ((bool)body)
		{
			body.linearVelocity = Vector2.zero;
		}
	}

	private void FixedUpdate()
	{
		if (!(body.linearVelocity.y > 0f))
		{
			if (body.gravityScale > 0f)
			{
				body.gravityScale = 0f;
				transitionTimeLeft = transitionTime;
			}
			float num = 1f;
			Vector2 origin = (Vector2)base.transform.TransformPoint(groundRayOrigin) + new Vector2(0f, 0.1f);
			Vector2 down = Vector2.down;
			float distance = groundRayLength + 0.1f;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, down, distance, 256);
			if (raycastHit2D.collider != null)
			{
				num = Mathf.Clamp01((raycastHit2D.distance - 1f - Physics2D.defaultContactOffset * 2f) / groundRayLength);
			}
			float time = elapsedTime / curveTime;
			float x = velocityCurveX.Evaluate(time) * velocityMagnitudeX;
			float y = ((!doNotAnimateY) ? (velocityCurveY.Evaluate(time) * velocityMagnitudeY) : body.linearVelocity.y);
			float num2;
			if (transitionTimeLeft > 0f)
			{
				num2 = (transitionTime - transitionTimeLeft) / transitionTime;
				transitionTimeLeft -= Time.deltaTime;
			}
			else
			{
				num2 = 1f;
			}
			body.linearVelocity = new Vector2(x, y) * (num * num2 * Time.timeScale);
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= curveTime)
			{
				elapsedTime %= curveTime;
			}
		}
	}
}
