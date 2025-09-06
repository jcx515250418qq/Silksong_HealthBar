using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dSprite))]
public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;

	private GameObject spawnedEnemy;

	[Range(0f, 1f)]
	public float spawnChance = 0.75f;

	public iTween.EaseType easeType = iTween.EaseType.easeOutSine;

	public Vector3 moveBy = new Vector3(0f, -8f, -16.98f);

	public float easeTime = 1f;

	private float elapsed;

	private bool isComplete;

	public Color startColor = Color.black;

	public Color endColor = Color.white;

	public EventRegister killEvent;

	private tk2dSprite sprite;

	public event Action<GameObject> OnEnemySpawned;

	private void Awake()
	{
		sprite = GetComponent<tk2dSprite>();
		sprite.color = startColor;
		if ((bool)enemyPrefab)
		{
			spawnedEnemy = UnityEngine.Object.Instantiate(enemyPrefab);
			spawnedEnemy.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (UnityEngine.Random.Range(0f, 1f) <= spawnChance)
		{
			if ((bool)killEvent)
			{
				killEvent.ReceivedEvent += delegate
				{
					base.gameObject.SetActive(value: false);
				};
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", moveBy);
			hashtable.Add("time", easeTime);
			hashtable.Add("easetype", easeType);
			hashtable.Add("space", Space.World);
			iTween.MoveBy(base.gameObject, hashtable);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (isComplete)
		{
			return;
		}
		elapsed += Time.deltaTime;
		sprite.color = Color.Lerp(startColor, endColor, Mathf.Clamp(elapsed / easeTime, 0f, 1f));
		if (elapsed > easeTime)
		{
			isComplete = true;
			spawnedEnemy.transform.position = base.transform.position;
			spawnedEnemy.transform.localScale = base.transform.localScale;
			spawnedEnemy.SetActive(value: true);
			if (this.OnEnemySpawned != null)
			{
				this.OnEnemySpawned(spawnedEnemy);
			}
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(spawnedEnemy, "chaser");
			if ((bool)playMakerFSM)
			{
				playMakerFSM.FsmVariables.FindFsmBool("Start Alert").Value = true;
			}
			base.gameObject.SetActive(value: false);
		}
	}
}
