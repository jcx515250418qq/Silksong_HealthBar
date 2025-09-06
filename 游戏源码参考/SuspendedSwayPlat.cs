using System.Collections;
using UnityEngine;

public class SuspendedSwayPlat : SuspendedPlatformBase
{
	[SerializeField]
	private GameObject hangingParent;

	[SerializeField]
	private CollisionEnterEvent landDetector;

	[SerializeField]
	private float landBreakDelay;

	[SerializeField]
	private JitterSelf landBreakJitter;

	[SerializeField]
	private GameObject[] landEnable;

	[SerializeField]
	private CameraShakeTarget landShake;

	[SerializeField]
	private AudioEvent landOnAudio;

	[Space]
	[SerializeField]
	private GameObject fallingParent;

	[SerializeField]
	private CameraShakeTarget fallShake;

	[SerializeField]
	private AudioEvent fallAudio;

	[SerializeField]
	private float fallLerpAcceleration;

	[SerializeField]
	private float fallLerpMaxSpeed;

	[SerializeField]
	private GameObject[] fallEnable;

	[Space]
	[SerializeField]
	private GameObject landedParent;

	[SerializeField]
	private GameObject[] landedEnable;

	[SerializeField]
	private CameraShakeTarget impactShake;

	[SerializeField]
	private AudioEvent impactAudio;

	private void OnValidate()
	{
		if (fallLerpAcceleration < 0f)
		{
			fallLerpAcceleration = 0f;
		}
	}

	protected override void Awake()
	{
		if ((bool)hangingParent)
		{
			hangingParent.SetActive(value: true);
		}
		if ((bool)fallingParent)
		{
			fallingParent.SetActive(value: false);
		}
		if ((bool)landedParent)
		{
			landedParent.SetActive(value: false);
		}
		landEnable.SetAllActive(value: false);
		fallEnable.SetAllActive(value: false);
		landedEnable.SetAllActive(value: false);
		base.Awake();
		if (!landDetector)
		{
			return;
		}
		landDetector.CollisionEntered += delegate(Collision2D collision)
		{
			if (!activated && collision.gameObject.layer == 9 && collision.GetSafeContact().Normal.y < 0f)
			{
				StartCoroutine(BreakDrop(landBreakDelay));
			}
		};
	}

	public override void CutDown()
	{
		if (!activated)
		{
			base.CutDown();
			StartCoroutine(BreakDrop(0f));
		}
	}

	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		if ((bool)hangingParent)
		{
			hangingParent.SetActive(value: false);
		}
		if ((bool)landedParent)
		{
			landedParent.SetActive(value: true);
		}
	}

	private IEnumerator BreakDrop(float delay)
	{
		activated = true;
		landShake.DoShake(this);
		landOnAudio.SpawnAndPlayOneShot(base.transform.position);
		landEnable.SetAllActive(value: true);
		if (delay > 0f)
		{
			if ((bool)landBreakJitter)
			{
				landBreakJitter.StartJitter();
			}
			yield return new WaitForSeconds(delay);
			if ((bool)landBreakJitter)
			{
				landBreakJitter.StopJitter();
			}
		}
		if ((bool)hangingParent)
		{
			hangingParent.SetActive(value: false);
		}
		if ((bool)fallingParent)
		{
			if ((bool)hangingParent)
			{
				fallingParent.transform.SetPosition2D(hangingParent.transform.position);
			}
			fallingParent.SetActive(value: true);
			fallEnable.SetAllActive(value: true);
			fallShake.DoShake(this);
			fallAudio.SpawnAndPlayOneShot(base.transform.position);
			if ((bool)landedParent)
			{
				Vector2 initialPos = fallingParent.transform.position;
				Quaternion initialRotation = fallingParent.transform.rotation;
				Vector2 targetPos = landedParent.transform.position;
				Quaternion targetRotation = landedParent.transform.rotation;
				_ = (targetPos - initialPos).normalized;
				float num = Vector2.Distance(targetPos, initialPos);
				float speed = 0f;
				float speedMultiplier = 1f / num;
				float t = 0f;
				while (t <= 1f)
				{
					speed += fallLerpAcceleration * Time.deltaTime;
					if (fallLerpMaxSpeed > 0f && speed > fallLerpMaxSpeed)
					{
						speed = fallLerpMaxSpeed;
					}
					t += speed * speedMultiplier * Time.deltaTime;
					Vector2 position = Vector2.Lerp(initialPos, targetPos, t);
					Quaternion rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
					fallingParent.transform.SetPosition2D(position);
					fallingParent.transform.rotation = rotation;
					yield return null;
				}
				fallingParent.transform.SetPosition2D(targetPos);
				fallingParent.transform.rotation = targetRotation;
			}
			fallingParent.SetActive(value: false);
		}
		if ((bool)landedParent)
		{
			landedParent.SetActive(value: true);
			impactAudio.SpawnAndPlayOneShot(landedParent.transform.position);
		}
		else
		{
			impactAudio.SpawnAndPlayOneShot(base.transform.position);
		}
		landedEnable.SetAllActive(value: true);
		impactShake.DoShake(this);
	}
}
