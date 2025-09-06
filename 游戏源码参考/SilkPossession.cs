using UnityEngine;

public class SilkPossession : MonoBehaviour
{
	public GameObject idleAnim;

	public GameObject possessAnim;

	public GameObject yankAnim;

	public GameObject possessEffect;

	public GameObject possessedEnemy;

	public RandomAudioClipTable possessionClipTable;

	public float yankWait;

	public float offset_y;

	private float timer;

	private AudioSource audioSource;

	private bool canResetEnemy;

	private void Awake()
	{
		audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		offset_y = 0f;
		possessEffect.SetActive(value: false);
		possessAnim.SetActive(value: false);
		idleAnim.SetActive(value: true);
		audioSource.pitch = Random.Range(0.85f, 1.15f);
		audioSource.Play();
	}

	private void OnDisable()
	{
		possessedEnemy = null;
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				possessEffect.SetActive(value: true);
			}
			base.transform.position = new Vector3(possessedEnemy.transform.position.x, possessedEnemy.transform.position.y + offset_y, base.transform.position.z);
			base.transform.parent = null;
		}
		if (possessedEnemy == null)
		{
			base.gameObject.Recycle();
		}
	}

	public void PlayPossess()
	{
		audioSource.Stop();
		idleAnim.SetActive(value: false);
		possessAnim.SetActive(value: true);
		possessionClipTable.SpawnAndPlayOneShot(base.transform.position);
		timer = yankWait;
	}

	public void SetEnemy(GameObject enemy)
	{
		possessedEnemy = enemy;
	}

	public void SetOffsetY(float newOffset)
	{
		offset_y = newOffset;
	}
}
