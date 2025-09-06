using System;
using GlobalEnums;
using UnityEngine;

public class JumpEffects : MonoBehaviour
{
	[SerializeField]
	private GameObject dustEffects;

	[SerializeField]
	private GameObject grassEffects;

	[SerializeField]
	private GameObject boneEffects;

	[SerializeField]
	private GameObject peakPuffEffects;

	[SerializeField]
	private SpriteRenderer splash;

	[SerializeField]
	private GameObject jumpPuff;

	[SerializeField]
	private GameObject dustTrail;

	[SerializeField]
	private GameObject spatterWhitePrefab;

	private GameObject ownerObject;

	private Vector3 ownerOffset;

	private Vector3 ownerPos;

	private Vector3 previousOwnerPos;

	private tk2dSpriteAnimator jumpPuffAnimator;

	private float recycleTimer;

	private float fallTimer;

	private float dripTimer;

	private float dripEndTimer;

	private bool dripping;

	private bool checkForFall;

	private bool trailAttached;

	private void OnEnable()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
		recycleTimer = 0f;
		fallTimer = 0.1f;
		dripTimer = 0f;
		dripEndTimer = 0f;
		dripping = false;
		checkForFall = false;
		trailAttached = false;
	}

	public void Play(GameObject owner, Vector2 velocity, Vector3 posOffset)
	{
		recycleTimer = 2f;
		ownerObject = owner;
		ownerOffset = posOffset;
		previousOwnerPos = ownerObject.transform.position;
		switch (owner.GetComponent<EnviroRegionListener>().CurrentEnvironmentType)
		{
		case EnvironmentTypes.Grass:
			grassEffects.SetActive(value: true);
			checkForFall = true;
			PlayJumpPuff(velocity);
			PlayTrail();
			break;
		case EnvironmentTypes.Bone:
			boneEffects.SetActive(value: true);
			checkForFall = true;
			PlayJumpPuff(velocity);
			PlayTrail();
			break;
		case EnvironmentTypes.PeakPuff:
			peakPuffEffects.SetActive(value: true);
			checkForFall = true;
			PlayJumpPuff(velocity);
			PlayTrail();
			break;
		case EnvironmentTypes.ShallowWater:
			SplashOut();
			break;
		case EnvironmentTypes.Moss:
		case EnvironmentTypes.WetMetal:
		case EnvironmentTypes.WetWood:
		case EnvironmentTypes.RunningWater:
			PlaySplash();
			break;
		default:
			dustEffects.SetActive(value: true);
			checkForFall = true;
			PlayJumpPuff(velocity);
			PlayTrail();
			break;
		}
	}

	private void Update()
	{
		if (recycleTimer > 0f)
		{
			recycleTimer -= Time.deltaTime;
			if (recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
				return;
			}
		}
		if (!ownerObject)
		{
			base.gameObject.Recycle();
			return;
		}
		ownerPos = ownerObject.transform.TransformPoint(ownerOffset);
		if (checkForFall)
		{
			if (fallTimer >= 0f)
			{
				fallTimer -= Time.deltaTime;
			}
			else
			{
				CheckForFall();
			}
		}
		if (trailAttached)
		{
			dustTrail.transform.position = new Vector3(ownerPos.x, ownerPos.y - 1.5f, ownerPos.z + 0.001f);
		}
		if (dripping)
		{
			if (dripTimer <= 0f)
			{
				ObjectPoolExtensions.Spawn(position: new Vector3(ownerPos.x + UnityEngine.Random.Range(-0.25f, 0.25f), ownerPos.y + UnityEngine.Random.Range(-0.5f, 0.5f), ownerPos.z), prefab: spatterWhitePrefab);
				dripTimer += 0.025f;
			}
			else
			{
				dripTimer -= Time.deltaTime;
			}
			if (dripEndTimer <= 0f)
			{
				dripping = false;
			}
			else
			{
				dripEndTimer -= Time.deltaTime;
			}
		}
		previousOwnerPos = ownerPos;
	}

	private void CheckForFall()
	{
		if (!((ownerPos.y - previousOwnerPos.y) / Time.deltaTime > 0f))
		{
			jumpPuff.SetActive(value: false);
			dustTrail.GetComponent<ParticleSystem>().Stop();
			checkForFall = false;
		}
	}

	private void PlayTrail()
	{
		dustTrail.SetActive(value: true);
		trailAttached = true;
	}

	private void PlayJumpPuff(Vector2 velocity)
	{
		float z = velocity.x * -3f + 2.6f;
		jumpPuff.transform.localEulerAngles = new Vector3(0f, 0f, z);
		jumpPuff.SetActive(value: true);
		if (jumpPuffAnimator == null)
		{
			jumpPuffAnimator = jumpPuff.GetComponent<tk2dSpriteAnimator>();
		}
		jumpPuffAnimator.PlayFromFrame(0);
	}

	private void SplashOut()
	{
		dripEndTimer = 0.4f;
		dripping = true;
		Vector3 position = ownerObject.transform.position;
		Vector2 linearVelocity = default(Vector2);
		for (int i = 1; i <= 11; i++)
		{
			GameObject obj = spatterWhitePrefab.Spawn(position);
			float num = UnityEngine.Random.Range(5f, 12f);
			float num2 = UnityEngine.Random.Range(80f, 110f);
			float x = num * Mathf.Cos(num2 * (MathF.PI / 180f));
			float y = num * Mathf.Sin(num2 * (MathF.PI / 180f));
			linearVelocity.x = x;
			linearVelocity.y = y;
			obj.GetComponent<Rigidbody2D>().linearVelocity = linearVelocity;
		}
	}

	private void PlaySplash()
	{
		AreaEffectTint.IsActive(base.transform.position, out var tintColor);
		splash.color = tintColor;
		splash.gameObject.SetActive(value: true);
		Transform transform = splash.transform;
		Vector3 localScale = transform.localScale;
		if (UnityEngine.Random.Range(1, 100) > 50)
		{
			transform.localScale = new Vector3(0f - localScale.x, localScale.y, localScale.z);
		}
	}
}
