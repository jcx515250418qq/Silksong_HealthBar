using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class WaveEffectControl : MonoBehaviour
{
	private float timer;

	public Color colour;

	public SpriteRenderer spriteRenderer;

	public float accel;

	public float accelStart = 5f;

	public bool doNotRecycle;

	public bool doNotPositionZ;

	public bool blackWave;

	public bool otherColour;

	public float scaleMultiplier = 1f;

	private NestedFadeGroupSpriteRenderer groupBridge;

	private bool started;

	private bool hasGroupBridge;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		UpdateEffect();
	}

	private void Start()
	{
		groupBridge = spriteRenderer.GetComponent<NestedFadeGroupSpriteRenderer>();
		hasGroupBridge = groupBridge != null;
		started = true;
	}

	private void OnEnable()
	{
		timer = 0f;
		if (blackWave)
		{
			colour = new Color(0f, 0f, 0f, 1f);
		}
		else if (!otherColour)
		{
			colour = new Color(1f, 1f, 1f, 1f);
		}
		accel = accelStart;
		if (!doNotPositionZ)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0.1f);
		}
		if (started)
		{
			UpdateEffect();
		}
	}

	private void Update()
	{
		timer += Time.deltaTime * accel;
		UpdateEffect();
		if (timer > 1f)
		{
			if (!doNotRecycle)
			{
				base.gameObject.Recycle();
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateEffect()
	{
		float num = (1f + timer * 4f) * scaleMultiplier;
		base.transform.localScale = new Vector3(num, num, num);
		Color color = spriteRenderer.color;
		color.a = 1f - timer;
		if (hasGroupBridge)
		{
			groupBridge.Color = color;
		}
		else
		{
			spriteRenderer.color = color;
		}
	}

	private void FixedUpdate()
	{
		accel *= 0.95f;
		if (accel < 0.5f)
		{
			accel = 0.5f;
		}
	}
}
