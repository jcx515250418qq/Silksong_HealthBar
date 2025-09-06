using UnityEngine;

public class BossThreadAnimator : MonoBehaviour
{
	private const int CYCLES_TOTAL = 11;

	private const float THREAD_ANIM_TIME = 0.3f;

	public GameObject threadObject;

	private bool animating;

	private int cyclesRemaining;

	private float animTimer;

	private float startPause;

	private float startTimer;

	private void OnEnable()
	{
		animTimer = 0f;
		startTimer = 0f;
		cyclesRemaining = 11;
		startPause = Random.Range(0f, 0.2f);
		animating = true;
	}

	private void Update()
	{
		if (startTimer < startPause)
		{
			startTimer += Time.deltaTime;
		}
		else if (animating)
		{
			animTimer -= Time.deltaTime;
			if (animTimer <= 0f)
			{
				cyclesRemaining--;
				threadObject.SetActive(value: false);
				float x = 1f;
				if (Random.Range(1, 100) >= 50)
				{
					x = -1f;
				}
				threadObject.transform.localScale = new Vector3(x, 1f, 1f);
				base.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0, 360));
				threadObject.SetActive(value: true);
				animTimer += 0.3f;
			}
		}
		if (cyclesRemaining <= 0)
		{
			threadObject.SetActive(value: false);
			animating = false;
		}
	}
}
