using System;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
	[Serializable]
	public class Direction
	{
		public GameObject effectPrefab;

		public Vector3 scale = Vector3.one;

		public Vector3 rotation;

		[Space]
		public float minFlingSpeed = 4f;

		public float maxFlingSpeed = 4f;

		public float minFlingAngle = 5f;

		public float maxFlingAngle = 5f;
	}

	[Serializable]
	public class FlingObject
	{
		public GameObject referenceObject;

		[Space]
		public int spawnMin = 25;

		public int spawnMax = 30;

		public float speedMin = 9f;

		public float speedMax = 20f;

		public float angleMin = 20f;

		public float angleMax = 160f;

		public Vector2 originVariation = new Vector2(0.5f, 0.5f);

		public void Fling(Vector3 origin)
		{
			if (!referenceObject)
			{
				return;
			}
			int num = UnityEngine.Random.Range(spawnMin, spawnMax + 1);
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = referenceObject.Spawn();
				if ((bool)gameObject)
				{
					gameObject.transform.position = origin + new Vector3(UnityEngine.Random.Range(0f - originVariation.x, originVariation.x), UnityEngine.Random.Range(0f - originVariation.y, originVariation.y), 0f);
					float num2 = UnityEngine.Random.Range(speedMin, speedMax);
					float num3 = UnityEngine.Random.Range(angleMin, angleMax);
					float x = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
					float y = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
					Vector2 force = new Vector2(x, y);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if ((bool)component)
					{
						component.AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
		}
	}

	public GameObject[] flingDebris;

	public float attackMagnitude = 6f;

	[Space]
	public Direction right;

	public Direction left;

	public Direction up;

	public Direction down;

	[Space]
	public Probability.ProbabilityGameObject[] containingParticles;

	public FlingObject[] flingObjectRegister;

	[Space]
	public GameObject objectNailEffectPrefab;

	public GameObject midpointNailEffectPrefab;

	public GameObject spellHitEffectPrefab;

	[Space]
	public AudioClip[] cutSound;

	public float pitchMin = 0.9f;

	public float pitchMax = 1.1f;

	private AudioSource source;

	private bool activated;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1f)
		{
			if ((bool)source)
			{
				source.enabled = false;
			}
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			base.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (activated)
		{
			return;
		}
		bool flag = false;
		int num = (int)Mathf.Sign(base.transform.position.x - collision.transform.position.x);
		float num2 = 1f;
		Direction direction = null;
		if (collision.tag == "Nail Attack")
		{
			flag = true;
			num2 = attackMagnitude;
			if ((bool)objectNailEffectPrefab)
			{
				GameObject obj = objectNailEffectPrefab.Spawn(base.transform.position);
				Vector3 localScale = obj.transform.localScale;
				localScale.x = Mathf.Abs(localScale.x) * (float)num;
				obj.transform.localScale = localScale;
			}
			if ((bool)midpointNailEffectPrefab)
			{
				GameObject obj2 = midpointNailEffectPrefab.Spawn((collision.transform.position + base.transform.position) / 2f);
				Vector3 localScale2 = obj2.transform.localScale;
				localScale2.x = Mathf.Abs(localScale2.x) * (float)num;
				obj2.transform.localScale = localScale2;
			}
			float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
			if (value < 45f)
			{
				direction = right;
			}
			else if (value < 135f)
			{
				direction = up;
			}
			else if (value < 225f)
			{
				direction = left;
			}
			else if (value < 360f)
			{
				direction = down;
			}
			if (direction != null && (bool)direction.effectPrefab)
			{
				GameObject gameObject = direction.effectPrefab.Spawn(base.transform.position);
				if ((bool)gameObject)
				{
					gameObject.transform.localScale = direction.scale;
					gameObject.transform.localEulerAngles = direction.rotation;
				}
			}
		}
		else if (collision.tag == "Hero Spell")
		{
			flag = true;
			if ((bool)spellHitEffectPrefab)
			{
				spellHitEffectPrefab.Spawn(base.transform.position);
			}
		}
		if (!flag)
		{
			return;
		}
		if (containingParticles.Length != 0)
		{
			GameObject gameObject2 = Probability.GetRandomGameObjectByProbability(containingParticles);
			if ((bool)gameObject2)
			{
				if (gameObject2.transform.parent != base.transform)
				{
					FlingObject flingObject = null;
					FlingObject[] array = flingObjectRegister;
					foreach (FlingObject flingObject2 in array)
					{
						if (flingObject2.referenceObject == gameObject2)
						{
							flingObject = flingObject2;
							break;
						}
					}
					if (flingObject != null)
					{
						flingObject.Fling(base.transform.position);
					}
					else
					{
						gameObject2 = gameObject2.Spawn(base.transform.position);
					}
				}
				gameObject2.SetActive(value: true);
			}
		}
		GameObject[] array2 = flingDebris;
		foreach (GameObject gameObject3 in array2)
		{
			if ((bool)gameObject3)
			{
				gameObject3.SetActive(value: true);
				float num3 = UnityEngine.Random.Range(direction.minFlingSpeed, direction.maxFlingSpeed) * num2;
				float num4 = UnityEngine.Random.Range(direction.minFlingAngle, direction.maxFlingAngle);
				float x = num3 * Mathf.Cos(num4 * (MathF.PI / 180f));
				float y = num3 * Mathf.Sin(num4 * (MathF.PI / 180f));
				Vector2 force = new Vector2(x, y);
				Rigidbody2D component = gameObject3.GetComponent<Rigidbody2D>();
				if ((bool)component)
				{
					component.AddForce(force, ForceMode2D.Impulse);
				}
			}
		}
		if ((bool)source && cutSound.Length != 0)
		{
			source.clip = cutSound[UnityEngine.Random.Range(0, cutSound.Length)];
			source.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
			source.Play();
		}
		GameCameras gameCameras = UnityEngine.Object.FindObjectOfType<GameCameras>();
		if ((bool)gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		SpriteRenderer component2 = GetComponent<SpriteRenderer>();
		if ((bool)component2)
		{
			component2.enabled = false;
		}
		activated = true;
	}
}
