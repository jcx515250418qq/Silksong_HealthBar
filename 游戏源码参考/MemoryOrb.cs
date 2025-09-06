using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class MemoryOrb : MonoBehaviour
{
	[SerializeField]
	private Color flashColour;

	[SerializeField]
	private GameObject idleFx;

	[SerializeField]
	private NestedFadeGroup idleFxFadeGroup;

	[SerializeField]
	private GameObject memoryOrbSmall;

	[SerializeField]
	private GameObject memoryOrbSmallInactive;

	[SerializeField]
	private GameObject memoryOrbLarge;

	[SerializeField]
	private GameObject memoryOrbLargeInactive;

	[SerializeField]
	private NestedFadeGroup timeAlertFadeGroup;

	[SerializeField]
	private NestedFadeGroupCurveAnimator timeAlertAnimator;

	[SerializeField]
	private GameObject appearFx;

	[SerializeField]
	private GameObject activateFx;

	[SerializeField]
	private GameObject collectFx;

	[SerializeField]
	private GameObject collectFxLarge;

	[SerializeField]
	private GameObject dissipateFx;

	[SerializeField]
	private GameObject returnObj;

	[SerializeField]
	private MemoryOrb nextOrb;

	[SerializeField]
	private GameObject nextOrbTrail;

	[SerializeField]
	private Transform spawnPoint;

	[Space]
	[SerializeField]
	private Transform returnTarget;

	[SerializeField]
	private float returnSpeed;

	[SerializeField]
	private float spawnSpeed;

	[SerializeField]
	private AudioClip collectSmall;

	[SerializeField]
	private AudioClip collectLarge;

	[SerializeField]
	private AudioClip dissipateAntic;

	[SerializeField]
	private AudioClip dissipate;

	private bool active;

	private bool collected;

	private bool isSmall;

	private Coroutine collectRoutine;

	private GameObject silkflyCloud;

	private MemoryOrbGroup orbGroup;

	private int orbIndex;

	private CircleCollider2D circleCollider;

	private AudioSource audioSource;

	private double collectableTime;

	private const float TIME_UNTIL_COLLECTABLE = 0.4f;

	public bool InstantOrb { get; set; }

	public bool SkipAppear { get; set; }

	private void Awake()
	{
		circleCollider = GetComponent<CircleCollider2D>();
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (!nextOrb && !InstantOrb)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null)
			{
				Transform transform = base.transform.parent.parent.Find("Silkfly Cloud");
				if (transform != null)
				{
					silkflyCloud = transform.gameObject;
				}
			}
		}
		else if (!InstantOrb)
		{
			isSmall = true;
		}
		circleCollider.enabled = true;
		activateFx.SetActive(value: false);
		idleFx.SetActive(value: true);
		collectFx.SetActive(value: false);
		collectFxLarge.SetActive(value: false);
		dissipateFx.SetActive(value: false);
		returnObj.SetActive(value: false);
		idleFx.transform.localPosition = new Vector3(0f, 0f, 0f);
		collected = false;
		if (collectRoutine != null)
		{
			StopCoroutine(collectRoutine);
			collectRoutine = null;
		}
		collectableTime = Time.timeAsDouble + 0.4000000059604645;
		active = false;
		collected = false;
		if (isSmall)
		{
			memoryOrbSmall.SetActive(value: false);
			memoryOrbSmallInactive.SetActive(value: true);
		}
		else
		{
			memoryOrbLarge.SetActive(value: false);
			memoryOrbLargeInactive.SetActive(value: true);
		}
		timeAlertAnimator.ForceStop();
		timeAlertFadeGroup.FadeTo(1f, 0f);
		if (InstantOrb)
		{
			if (SkipAppear)
			{
				SetActive();
			}
			else
			{
				StartCoroutine(InstantOrbSpawn());
			}
			appearFx.SetActive(value: false);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Collect();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Collect();
	}

	private void Collect()
	{
		if (!active || collected || Time.timeAsDouble < collectableTime)
		{
			return;
		}
		collected = true;
		circleCollider.enabled = false;
		idleFx.SetActive(value: false);
		if (isSmall)
		{
			collectFx.SetActive(value: true);
		}
		else
		{
			collectFxLarge.SetActive(value: true);
		}
		if (isSmall)
		{
			audioSource.volume = 1f;
			audioSource.PlayOneShot(collectSmall);
		}
		else
		{
			audioSource.volume = 1f;
			audioSource.PlayOneShot(collectLarge);
		}
		if ((bool)nextOrb)
		{
			nextOrb.SetActive();
			Transform obj = nextOrb.transform;
			float y = obj.position.y - base.transform.position.y;
			float x = obj.position.x - base.transform.position.x;
			float num;
			for (num = Mathf.Atan2(y, x) * (180f / MathF.PI); num < 0f; num += 360f)
			{
			}
			nextOrbTrail.transform.localEulerAngles = new Vector3(0f, 0f, num);
			nextOrbTrail.SetActive(value: true);
		}
		if ((bool)GameManager.instance && (bool)GameManager.instance.cameraCtrl)
		{
			GameManager.instance.cameraCtrl.ScreenFlash(flashColour);
		}
		if ((bool)orbGroup)
		{
			orbGroup.CollectedOrb(orbIndex);
		}
		if (!isSmall && !InstantOrb && silkflyCloud != null)
		{
			silkflyCloud.GetComponent<PlayMakerFSM>().SendEvent("COMPLETED");
		}
		StartCoroutine(CollectRoutine());
	}

	private IEnumerator CollectRoutine()
	{
		PlayerData instance = PlayerData.instance;
		int orbCount = instance.CollectedCloverMemoryOrbs;
		yield return new WaitForSeconds(0.3f);
		if (!isSmall)
		{
			returnObj.SetActive(value: true);
			Transform returnObjTrans = returnObj.transform;
			Vector2 startPos = returnObjTrans.position;
			Vector2 endPos = returnTarget.position;
			float num = Vector2.Distance(startPos, endPos);
			float duration = num / returnSpeed;
			Vector2 to = endPos - startPos;
			float rotation = Vector2.SignedAngle(Vector2.right, to);
			returnObjTrans.SetRotation2D(rotation);
			for (float elapsed = 0f; elapsed < duration && elapsed < 0.75f; elapsed += Time.deltaTime)
			{
				Vector2 position = Vector2.Lerp(startPos, endPos, elapsed / duration);
				returnObjTrans.SetPosition2D(position);
				yield return null;
			}
			if ((bool)orbGroup)
			{
				orbGroup.LargeOrbReturned();
			}
		}
		else
		{
			yield return new WaitForSeconds(0.75f);
		}
		yield return new WaitForSeconds(0.5f);
		if ((bool)orbGroup)
		{
			orbGroup.OrbReturned(orbCount);
		}
		base.gameObject.SetActive(value: false);
		collectRoutine = null;
	}

	private IEnumerator InstantOrbSpawn()
	{
		Vector3 destination = base.transform.position;
		base.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, base.transform.position.z);
		Vector2 startPos = base.transform.position;
		Vector2 endPos = destination;
		float num = Vector2.Distance(startPos, endPos);
		float duration = num / spawnSpeed;
		_ = endPos - startPos;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(startPos, endPos, elapsed / duration);
			base.transform.SetPosition2D(position);
			yield return null;
		}
		base.transform.position = destination;
		SetActive();
	}

	public void SetActive()
	{
		if (isSmall)
		{
			memoryOrbSmall.SetActive(value: true);
			memoryOrbSmallInactive.SetActive(value: false);
		}
		else
		{
			memoryOrbLarge.SetActive(value: true);
			memoryOrbLargeInactive.SetActive(value: false);
		}
		activateFx.SetActive(value: true);
		active = true;
	}

	public void Setup(MemoryOrbGroup group, int index)
	{
		orbGroup = group;
		orbIndex = index;
	}

	public void StartTimeAlert()
	{
		if (timeAlertAnimator.gameObject.activeInHierarchy)
		{
			timeAlertAnimator.StartAnimation();
			audioSource.volume = 0.25f;
			audioSource.PlayOneShot(dissipateAntic);
		}
	}

	public void StopTimeAlert()
	{
		if (timeAlertAnimator.gameObject.activeInHierarchy)
		{
			timeAlertAnimator.ForceStop();
			timeAlertFadeGroup.FadeTo(1f, 0f);
		}
	}

	public void Dissipate()
	{
		if (!collected)
		{
			StartCoroutine(DoDissipate());
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator DoDissipate()
	{
		circleCollider.enabled = false;
		audioSource.volume = 0.3f;
		audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
		dissipateFx.SetActive(value: true);
		audioSource.PlayOneShot(dissipate);
		idleFx.SetActive(value: false);
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(value: false);
	}
}
