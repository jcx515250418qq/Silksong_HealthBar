using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FinishingRigidBody : MonoBehaviour
{
	private enum Conclusions
	{
		Disable = 0,
		Recycle = 1,
		Destroy = 2
	}

	private enum States
	{
		Ready = 0,
		Concluded = 1,
		Shrinking = 2
	}

	[SerializeField]
	private float waitDuration;

	[SerializeField]
	private float shrinkDuration;

	[SerializeField]
	private Conclusions conclusion;

	[SerializeField]
	private bool persistOffScreen;

	private Renderer rend;

	private Rigidbody2D body;

	private States state;

	private Vector3 shrinkStartScale;

	private float timer;

	private int framesEnabled;

	protected void Reset()
	{
		waitDuration = 8f;
		shrinkDuration = 1f;
		conclusion = Conclusions.Disable;
	}

	protected void Awake()
	{
		rend = GetComponent<Renderer>();
		body = GetComponent<Rigidbody2D>();
	}

	protected void OnEnable()
	{
		state = States.Ready;
		timer = 0f;
		framesEnabled = 0;
	}

	protected void Update()
	{
		if (state == States.Ready && !body.IsAwake())
		{
			timer += Time.deltaTime;
			if (timer > waitDuration)
			{
				timer = 0f;
				state = States.Shrinking;
				shrinkStartScale = base.transform.localScale;
			}
		}
		if (state == States.Shrinking)
		{
			timer += Time.deltaTime;
			if (timer > shrinkDuration)
			{
				Conclude();
				return;
			}
			float num = 1f - Mathf.Clamp01(timer / shrinkDuration);
			base.transform.localScale = num * shrinkStartScale;
		}
		if (!persistOffScreen && rend != null && !rend.isVisible && framesEnabled > 10)
		{
			Conclude();
		}
		else
		{
			framesEnabled++;
		}
	}

	private void Conclude()
	{
		if (state == States.Shrinking)
		{
			base.transform.localScale = shrinkStartScale;
		}
		state = States.Concluded;
		if (conclusion == Conclusions.Disable)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (conclusion == Conclusions.Recycle)
		{
			base.gameObject.Recycle();
		}
		else if (conclusion == Conclusions.Destroy)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
