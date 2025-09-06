using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(tk2dSprite))]
public class DebrisParticle : MonoBehaviour
{
	private Rigidbody2D body;

	private tk2dSprite sprite;

	[SerializeField]
	private string[] randomSpriteIds;

	[SerializeField]
	private float startZ;

	[SerializeField]
	private float scaleMin;

	[SerializeField]
	private float scaleMax;

	[SerializeField]
	private float blackChance;

	private bool didSpin;

	protected void Reset()
	{
		startZ = 0.019f;
		scaleMin = 1.25f;
		scaleMax = 2f;
		blackChance = 1f / 3f;
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		sprite = GetComponent<tk2dSprite>();
	}

	protected void OnEnable()
	{
		if (randomSpriteIds.Length != 0)
		{
			sprite.SetSprite(sprite.Collection, randomSpriteIds[Random.Range(0, randomSpriteIds.Length)]);
		}
		Vector3 position = base.transform.position;
		position.z = startZ;
		base.transform.position = position;
		float num = Random.Range(scaleMin, scaleMax);
		Vector3 localScale = base.transform.localScale;
		localScale.x = num;
		localScale.y = num;
		base.transform.localScale = localScale;
		if (Random.Range(0f, 1f) < blackChance)
		{
			sprite.color = Color.black;
			position.z -= 0.05f;
			base.transform.position = position;
		}
		else
		{
			sprite.color = Color.white;
		}
		didSpin = false;
	}

	protected void Update()
	{
		if (!didSpin)
		{
			didSpin = true;
			body.AddTorque(0f - body.linearVelocity.x, ForceMode2D.Force);
		}
	}
}
