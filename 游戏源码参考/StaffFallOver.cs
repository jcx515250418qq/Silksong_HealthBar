using System.Collections;
using UnityEngine;

public class StaffFallOver : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject front;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform frontRotator;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject frontCollider;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject side;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject sideCollider;

	[SerializeField]
	private float landSpeedThreshold;

	[SerializeField]
	private float landAngularThreshold;

	[SerializeField]
	private float landWaitTime;

	[SerializeField]
	private float fallDuration;

	[SerializeField]
	private AnimationCurve fallCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float bounceUpForce;

	private Coroutine fallRoutine;

	private Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		front.SetActive(value: true);
		frontCollider.SetActive(value: true);
		side.SetActive(value: false);
		sideCollider.SetActive(value: false);
		frontRotator.localEulerAngles = Vector3.zero;
	}

	private void OnDisable()
	{
		if (fallRoutine != null)
		{
			StopCoroutine(fallRoutine);
			fallRoutine = null;
		}
	}

	private void OnCollisionEnter2D()
	{
		if (fallRoutine == null)
		{
			fallRoutine = StartCoroutine(Fall());
		}
	}

	private IEnumerator Fall()
	{
		WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();
		float elapsedStill = 0f;
		while (elapsedStill < landWaitTime)
		{
			yield return fixedWait;
			float magnitude = body.linearVelocity.magnitude;
			elapsedStill = ((!(Mathf.Abs(body.angularVelocity) > landAngularThreshold) && !(magnitude > landSpeedThreshold)) ? (elapsedStill + Time.fixedDeltaTime) : 0f);
		}
		frontCollider.SetActive(value: false);
		sideCollider.SetActive(value: true);
		float targetRotation = ((base.transform.localEulerAngles.z < 0f) ? 90f : (-90f));
		for (float elapsed = 0f; elapsed < fallDuration; elapsed += Time.deltaTime)
		{
			float t = fallCurve.Evaluate(elapsed / fallDuration);
			float y = Mathf.Lerp(0f, targetRotation, t);
			frontRotator.localEulerAngles = new Vector3(0f, y, 0f);
			yield return null;
		}
		front.SetActive(value: false);
		side.SetActive(value: true);
		body.AddForce(new Vector2(0f, bounceUpForce), ForceMode2D.Impulse);
	}
}
