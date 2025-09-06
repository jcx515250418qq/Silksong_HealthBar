using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(tk2dSpriteAnimator))]
[RequireComponent(typeof(AudioSource))]
public class WaterDrip : MonoBehaviour
{
	public float idleTimeMin = 2f;

	public float idleTimeMax = 8f;

	public float fallVelocity = -7f;

	public RandomAudioClipTable impactAudioClipTable;

	public float impactTranslation = -0.5f;

	private bool impacted;

	private Vector2 startPos;

	private Collider2D col;

	private Rigidbody2D body;

	private tk2dSpriteAnimator anim;

	private AudioSource source;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<tk2dSpriteAnimator>();
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		startPos = base.transform.position;
		StartCoroutine(Drip());
	}

	private IEnumerator Drip()
	{
		while (true)
		{
			anim.Play("Idle");
			body.gravityScale = 0f;
			body.linearVelocity = Vector2.zero;
			col.enabled = false;
			yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
			col.enabled = true;
			yield return StartCoroutine(anim.PlayAnimWait("Drip"));
			anim.Play("Fall");
			body.gravityScale = 1f;
			body.linearVelocity = new Vector2(0f, fallVelocity);
			impacted = false;
			while (!impacted)
			{
				yield return null;
			}
			body.gravityScale = 0f;
			body.linearVelocity = Vector2.zero;
			col.enabled = false;
			impactAudioClipTable.PlayOneShot(source);
			base.transform.position += new Vector3(0f, impactTranslation, 0f);
			yield return StartCoroutine(anim.PlayAnimWait("Impact"));
			base.transform.position = startPos;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		impacted = true;
	}
}
