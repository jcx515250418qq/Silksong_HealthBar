using System.Collections;
using UnityEngine;

public class GrimmballControl : MonoBehaviour
{
	public ParticleSystem ptFlame;

	public ParticleSystem ptSmoke;

	public float maxLifeTime = 10f;

	private Collider2D col;

	private Rigidbody2D body;

	private Coroutine fireRoutine;

	private Coroutine shrinkRoutine;

	private float force;

	private float tweenY;

	private bool hit;

	public float Force
	{
		get
		{
			return force;
		}
		set
		{
			force = value;
		}
	}

	public float TweenY
	{
		get
		{
			return tweenY;
		}
		set
		{
			tweenY = value;
		}
	}

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		force = 0f;
		tweenY = 0f;
		col.enabled = true;
		hit = false;
		ptFlame.Play();
		ptSmoke.Play();
		base.transform.localScale = Vector3.one;
	}

	private void OnDisable()
	{
		iTween.Stop(base.gameObject);
		if (fireRoutine != null)
		{
			StopCoroutine(fireRoutine);
			fireRoutine = null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 8 && shrinkRoutine == null && !hit)
		{
			DoHit();
		}
	}

	public void DoHit()
	{
		hit = true;
		if (fireRoutine != null)
		{
			StopCoroutine(fireRoutine);
			fireRoutine = null;
		}
		shrinkRoutine = StartCoroutine(Shrink());
	}

	public void Fire()
	{
		if (fireRoutine == null)
		{
			fireRoutine = StartCoroutine(DoFire());
		}
	}

	private IEnumerator DoFire()
	{
		Vector3 vector = new Vector3(0f, tweenY + Random.Range(-0.2f, 0.2f), 0f);
		iTween.MoveBy(base.gameObject, iTween.Hash("amount", vector, "time", 0.7f, "easetype", iTween.EaseType.easeOutSine, "space", Space.World));
		for (float elapsed = 0f; elapsed < maxLifeTime; elapsed += Time.fixedDeltaTime)
		{
			body.AddForce(new Vector2(force, 0f), ForceMode2D.Force);
			yield return new WaitForFixedUpdate();
		}
		DoHit();
	}

	private IEnumerator Shrink()
	{
		ptFlame.Stop();
		ptSmoke.Stop();
		col.enabled = false;
		float time = 0.5f;
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", Vector3.zero, "time", time, "easetype", iTween.EaseType.linear));
		for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
		{
			body.linearVelocity *= 0.85f;
			yield return null;
		}
		shrinkRoutine = null;
		base.gameObject.Recycle();
	}
}
