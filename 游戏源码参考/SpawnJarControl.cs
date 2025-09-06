using System.Collections;
using UnityEngine;

public class SpawnJarControl : MonoBehaviour
{
	public float spawnY = 106.52f;

	public float breakY = 94.55f;

	public ParticleSystem readyDust;

	public ParticleSystem dustTrail;

	public ParticleSystem ptBreakS;

	public ParticleSystem ptBreakL;

	public GameObject strikeNailR;

	public AudioEventRandom breakSound;

	public AudioSource audioSourcePrefab;

	private GameObject enemyToSpawn;

	private int enemyHealth;

	private CircleCollider2D col;

	private Rigidbody2D body;

	private SpriteRenderer sprite;

	private void Awake()
	{
		col = GetComponent<CircleCollider2D>();
		body = GetComponent<Rigidbody2D>();
		sprite = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		col.enabled = false;
		sprite.enabled = false;
		StartCoroutine(Behaviour());
	}

	private IEnumerator Behaviour()
	{
		base.transform.SetPositionY(spawnY);
		base.transform.SetPositionZ(0.01f);
		readyDust.Play();
		yield return new WaitForSeconds(0.5f);
		col.enabled = true;
		body.linearVelocity = new Vector2(0f, -25f);
		body.angularVelocity = ((Random.Range(0, 2) > 0) ? (-300) : 300);
		readyDust.Stop();
		dustTrail.Play();
		sprite.enabled = true;
		while (base.transform.position.y > breakY)
		{
			yield return null;
		}
		base.transform.SetPositionY(breakY);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		dustTrail.Stop();
		ptBreakS.Play();
		ptBreakL.Play();
		strikeNailR.Spawn(base.transform.position);
		col.enabled = false;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		sprite.enabled = false;
		breakSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		if ((bool)enemyToSpawn)
		{
			GameObject obj = enemyToSpawn.Spawn(base.transform.position);
			HealthManager component = obj.GetComponent<HealthManager>();
			if ((bool)component)
			{
				component.hp = enemyHealth;
			}
			obj.tag = "Boss";
		}
		yield return new WaitForSeconds(2f);
		base.gameObject.Recycle();
	}

	public void SetEnemySpawn(GameObject prefab, int health)
	{
		enemyToSpawn = prefab;
		enemyHealth = health;
	}
}
