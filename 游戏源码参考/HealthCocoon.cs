using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCocoon : MonoBehaviour, IHitResponder
{
	[Serializable]
	private class FlingPrefab
	{
		public GameObject Prefab;

		public int MinAmount;

		public int MaxAmount;

		public Vector2 OriginVariation = new Vector2(0.5f, 0.5f);

		public float MinSpeed;

		public float MaxSpeed;

		public float MinAngle;

		public float MaxAngle;

		private List<GameObject> pool = new List<GameObject>();

		public void SetupPool(Transform parent)
		{
			if ((bool)Prefab)
			{
				pool.Capacity = MaxAmount;
				for (int i = pool.Count; i < MaxAmount; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Prefab, parent);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.SetActive(value: false);
					pool.Add(gameObject);
				}
			}
		}

		public GameObject Spawn()
		{
			foreach (GameObject item in pool)
			{
				if (!item.activeSelf)
				{
					item.SetActive(value: true);
					return item;
				}
			}
			return null;
		}
	}

	[Header("Behaviour")]
	[SerializeField]
	private GameObject[] slashEffects;

	[SerializeField]
	private GameObject[] spellEffects;

	[SerializeField]
	private Vector3 effectOrigin = new Vector3(0f, 0.8f, 0f);

	[Space]
	[SerializeField]
	private FlingPrefab[] flingPrefabs;

	[Space]
	[SerializeField]
	private GameObject[] enableChildren;

	[SerializeField]
	private GameObject[] disableChildren;

	[SerializeField]
	private Collider2D[] disableColliders;

	[Space]
	[SerializeField]
	private Rigidbody2D cap;

	[SerializeField]
	private float capHitForce = 10f;

	[Space]
	[SerializeField]
	private AudioClip deathSound;

	[Space]
	[SerializeField]
	private CameraShakeTarget deathCameraShake;

	[Header("Animation")]
	[SerializeField]
	private string idleAnimation = "Cocoon Idle";

	[SerializeField]
	private string sweatAnimation = "Cocoon Sweat";

	[SerializeField]
	private AudioClip moveSound;

	[SerializeField]
	private float waitMin = 2f;

	[SerializeField]
	private float waitMax = 6f;

	private bool activated;

	private Coroutine animRoutine;

	private AudioSource source;

	private tk2dSpriteAnimator animator;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		animator = GetComponent<tk2dSpriteAnimator>();
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if (!component)
		{
			return;
		}
		component.OnGetSaveState += delegate(out bool value)
		{
			value = activated;
		};
		component.OnSetSaveState += delegate(bool value)
		{
			activated = value;
			if (activated)
			{
				SetBroken();
			}
		};
	}

	private void Start()
	{
		animRoutine = StartCoroutine(Animate());
		FlingPrefab[] array = flingPrefabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetupPool(base.transform);
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if ((bool)source && (bool)clip)
		{
			source.PlayOneShot(clip);
		}
	}

	private IEnumerator Animate()
	{
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(waitMin, waitMax));
			PlaySound(moveSound);
			if ((bool)animator)
			{
				tk2dSpriteAnimationClip clipByName = animator.GetClipByName(sweatAnimation);
				animator.Play(clipByName);
				yield return new WaitForSeconds((float)clipByName.frames.Length / clipByName.fps);
				animator.Play(idleAnimation);
			}
		}
	}

	private void SetBroken()
	{
		StopCoroutine(animRoutine);
		GetComponent<MeshRenderer>().enabled = false;
		GameObject[] array = disableChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		Collider2D[] array2 = disableColliders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
	}

	private void FlingObjects(FlingPrefab fling)
	{
		if (!fling.Prefab)
		{
			return;
		}
		int num = UnityEngine.Random.Range(fling.MinAmount, fling.MaxAmount + 1);
		Vector2 linearVelocity = default(Vector2);
		for (int i = 1; i <= num; i++)
		{
			GameObject obj = fling.Spawn();
			obj.transform.position += new Vector3(fling.OriginVariation.x * UnityEngine.Random.Range(-1f, 1f), fling.OriginVariation.y * UnityEngine.Random.Range(-1f, 1f));
			float num2 = UnityEngine.Random.Range(fling.MinSpeed, fling.MaxSpeed);
			float num3 = UnityEngine.Random.Range(fling.MinAngle, fling.MaxAngle);
			float x = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
			float y = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
			linearVelocity.x = x;
			linearVelocity.y = y;
			Rigidbody2D component = obj.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.linearVelocity = linearVelocity;
			}
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (activated)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		if (damageInstance.IsNailDamage)
		{
			flag = true;
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			float z = 0f;
			Vector2 vector = new Vector2(1.5f, 1.5f);
			if (overriddenDirection < 45f)
			{
				z = UnityEngine.Random.Range(340, 380);
			}
			else if (overriddenDirection < 135f)
			{
				z = UnityEngine.Random.Range(340, 380);
			}
			else if (overriddenDirection < 225f)
			{
				vector.x *= -1f;
				z = UnityEngine.Random.Range(70, 110);
			}
			else if (overriddenDirection < 360f)
			{
				z = UnityEngine.Random.Range(250, 290);
			}
			GameObject[] array = slashEffects;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject obj = array[i].Spawn(base.transform.position + effectOrigin);
				obj.transform.eulerAngles = new Vector3(0f, 0f, z);
				obj.transform.localScale = vector;
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell)
		{
			flag = true;
			GameObject[] array = spellEffects;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject obj2 = array[i].Spawn(base.transform.position + effectOrigin);
				Vector3 position = obj2.transform.position;
				position.z = 0.0031f;
				obj2.transform.position = position;
			}
		}
		if (flag)
		{
			activated = true;
			GameObject[] array = enableChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			if ((bool)cap)
			{
				cap.gameObject.SetActive(value: true);
				Vector2 hitDirectionAsVector = damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular);
				cap.AddForce(capHitForce * hitDirectionAsVector, ForceMode2D.Impulse);
			}
			FlingPrefab[] array2 = flingPrefabs;
			foreach (FlingPrefab fling in array2)
			{
				FlingObjects(fling);
			}
			PlaySound(deathSound);
			SetBroken();
			GameManager.instance.AddToCocoonList();
			deathCameraShake.DoShake(this);
		}
		return flag ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}
}
