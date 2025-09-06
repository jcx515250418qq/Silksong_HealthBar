using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TiltPlat : MonoBehaviour
{
	[Header("Basic")]
	[SerializeField]
	private Vector2 originOffset;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform art;

	[SerializeField]
	private Vector2 dropVector;

	[SerializeField]
	private float tiltFactor;

	[SerializeField]
	private ParticleSystem platDust;

	[SerializeField]
	private RandomAudioClipTable landAudioClipTable;

	[SerializeField]
	private RandomAudioClipTable creakAudioClipTable;

	[Header("Extras")]
	[SerializeField]
	private GameObject[] activeObjects;

	[SerializeField]
	private bool startInactive;

	[Space]
	[SerializeField]
	private UnityEvent OnLand;

	private Collider2D collider;

	private Transform heroTrans;

	private Vector2 initialArtPos;

	private Quaternion initialArtRot;

	private float sign;

	private float nextUpdateTime;

	private Coroutine dropRoutine;

	private Coroutine tinkMoveRoutine;

	private Coroutine tiltRoutine;

	private TinkEffect tinkEffect;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(originOffset, 0.1f);
	}

	private void Awake()
	{
		collider = GetComponent<Collider2D>();
		tinkEffect = GetComponent<TinkEffect>();
		if ((bool)tinkEffect)
		{
			tinkEffect.HitInDirection += OnHitInDirection;
		}
	}

	private void OnEnable()
	{
		if (!art)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		initialArtPos = art.localPosition;
		initialArtRot = art.localRotation;
		Vector3 lossyScale = base.transform.lossyScale;
		sign = Mathf.Sign(lossyScale.x) * Mathf.Sign(lossyScale.y);
		if (startInactive)
		{
			collider.enabled = false;
			activeObjects.SetAllActive(value: false);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (base.isActiveAndEnabled && other.gameObject.layer == 9 && other.GetSafeContact().Normal.y.IsWithinTolerance(0.01f, -1f))
		{
			heroTrans = other.transform;
			StopMoveRoutines();
			dropRoutine = StartCoroutine(ArtDrop());
			OnLand.Invoke();
			landAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
			if (tiltRoutine == null)
			{
				tiltRoutine = StartCoroutine(ArtTilt());
			}
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if ((bool)heroTrans && !(other.transform != heroTrans))
		{
			heroTrans = null;
			StopMoveRoutines();
			dropRoutine = StartCoroutine(ArtRaise());
		}
	}

	private void StopMoveRoutines()
	{
		if (dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
		}
		if (tinkMoveRoutine != null)
		{
			StopCoroutine(tinkMoveRoutine);
		}
	}

	private IEnumerator ArtDrop()
	{
		Vector2 targetPos = initialArtPos + dropVector;
		for (float elapsed = 0f; elapsed < 0.075f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(initialArtPos, targetPos, elapsed / 0.075f);
			art.SetLocalPosition2D(position);
			yield return null;
		}
		art.SetLocalPosition2D(targetPos);
	}

	private IEnumerator ArtRaise()
	{
		Vector2 startPos = art.localPosition;
		for (float elapsed = 0f; elapsed < 0.2f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(startPos, initialArtPos, elapsed / 0.2f);
			art.SetLocalPosition2D(position);
			yield return null;
		}
		art.SetLocalPosition2D(initialArtPos);
	}

	private IEnumerator ArtTilt()
	{
		SetArtTilt(0f);
		WaitForSeconds wait = new WaitForSeconds(1f / 18f);
		float previousTilt = 0f;
		double nextCreakTime = 0.0;
		while (true)
		{
			float num;
			if ((bool)heroTrans)
			{
				float x = heroTrans.position.x;
				num = base.transform.TransformPoint(originOffset).x - x;
				num *= tiltFactor * sign;
			}
			else
			{
				num = Mathf.Lerp(previousTilt, 0f, 4f / 9f);
				if (Math.Abs(num) < 0.001f)
				{
					break;
				}
			}
			if (Math.Abs(num - previousTilt) > 0.001f && Time.timeAsDouble >= nextCreakTime)
			{
				creakAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
				nextCreakTime = Time.timeAsDouble + 1.5;
			}
			SetArtTilt(num);
			previousTilt = num;
			yield return wait;
		}
		SetArtTilt(0f);
		tiltRoutine = null;
	}

	private void SetArtTilt(float tilt)
	{
		art.localRotation = initialArtRot * Quaternion.Euler(0f, 0f, tilt);
	}

	public void ActivateTiltPlat(bool isInstant)
	{
		startInactive = false;
		collider.enabled = true;
		activeObjects.SetAllActive(value: true);
		if (!isInstant)
		{
			OnLand.Invoke();
			art.SetLocalPosition2D(initialArtPos + dropVector);
			if (dropRoutine != null)
			{
				StopCoroutine(dropRoutine);
			}
			dropRoutine = StartCoroutine(ArtRaise());
		}
	}

	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		StopMoveRoutines();
		switch (direction)
		{
		default:
			return;
		case HitInstance.HitDirection.Left:
			tinkMoveRoutine = StartCoroutine(TinkDirectionMove(new Vector2(-0.1f, 0f)));
			break;
		case HitInstance.HitDirection.Right:
			tinkMoveRoutine = StartCoroutine(TinkDirectionMove(new Vector2(0.1f, 0f)));
			break;
		case HitInstance.HitDirection.Up:
			tinkMoveRoutine = StartCoroutine(TinkDirectionMove(new Vector2(0f, 0.1f)));
			break;
		}
		if ((bool)platDust)
		{
			platDust.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			platDust.Play(withChildren: true);
		}
	}

	private IEnumerator TinkDirectionMove(Vector2 moveVec)
	{
		Vector2 targetPos = initialArtPos + moveVec;
		for (float elapsed = 0f; elapsed < 0.05f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(initialArtPos, targetPos, elapsed / 0.05f);
			art.SetLocalPosition2D(position);
			yield return null;
		}
		for (float elapsed = 0f; elapsed < 0.1f; elapsed += Time.deltaTime)
		{
			Vector2 position2 = Vector2.Lerp(targetPos, initialArtPos, elapsed / 0.1f);
			art.SetLocalPosition2D(position2);
			yield return null;
		}
		art.SetLocalPosition2D(initialArtPos);
	}
}
