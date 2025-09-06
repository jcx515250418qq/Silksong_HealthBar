using System;
using UnityEngine;

public class BreakablePole : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Sprite brokenSprite;

	[SerializeField]
	private float inertBackgroundThreshold;

	[SerializeField]
	private float inertForegroundThreshold;

	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private RandomAudioClipTable hitClip;

	[SerializeField]
	private GameObject slashImpactPrefab;

	[SerializeField]
	private Rigidbody2D top;

	protected void Reset()
	{
		inertBackgroundThreshold = -1f;
		inertForegroundThreshold = 1f;
	}

	protected void Start()
	{
		float z = base.transform.position.z;
		if (z < inertBackgroundThreshold || z > inertForegroundThreshold)
		{
			base.enabled = false;
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		if (cardinalDirection != 2 && cardinalDirection != 0)
		{
			return IHitResponder.Response.None;
		}
		spriteRenderer.sprite = brokenSprite;
		Transform obj = slashImpactPrefab.Spawn().transform;
		obj.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(340f, 380f));
		Vector3 localScale = obj.localScale;
		localScale.x = ((cardinalDirection == 2) ? (-1f) : 1f);
		localScale.y = 1f;
		hitClip.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		top.gameObject.SetActive(value: true);
		float num = ((cardinalDirection == 2) ? UnityEngine.Random.Range(120, 140) : UnityEngine.Random.Range(40, 60));
		top.linearVelocity = new Vector2(Mathf.Cos(num * (MathF.PI / 180f)), Mathf.Sin(num * (MathF.PI / 180f))) * 17f;
		base.enabled = false;
		return IHitResponder.Response.GenericHit;
	}
}
