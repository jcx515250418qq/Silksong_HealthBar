using UnityEngine;

public class Dragonfly : MonoBehaviour
{
	public float xRange;

	public float yRange;

	public float idleMin = 0.5f;

	public float idleMax = 1.5f;

	public float flyMin = 0.15f;

	public float flyMax = 0.15f;

	private int state;

	private float timer;

	private GameObject hero;

	private MeshRenderer meshRenderer;

	private Rigidbody2D rb;

	private Vector3 initPos;

	private void Start()
	{
		initPos = base.transform.position;
		hero = GameManager.instance.hero_ctrl.gameObject;
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		if (state == 0)
		{
			if (meshRenderer.isVisible)
			{
				StartIdle();
			}
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f && state == 1)
		{
			state = 2;
		}
	}

	private void StartIdle()
	{
		rb.linearVelocity = new Vector3(0f, 0f, 0f);
		timer = Random.Range(idleMin, idleMax);
		state = 1;
	}
}
