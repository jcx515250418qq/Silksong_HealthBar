using System.Collections;
using UnityEngine;

public class MegaJellyZap : MonoBehaviour
{
	public enum Type
	{
		Zap = 0,
		MultiZap = 1
	}

	public Type type;

	public float delay;

	public float shift;

	public ParticleSystem ptAttack;

	public ParticleSystem ptAntic;

	public GameObject anticRing;

	public AudioSource audioSourcePrefab;

	public AudioEvent zapBugPt1;

	public AudioEvent zapBugPt2;

	public tk2dSpriteAnimator anim;

	public bool yDelay;

	public float yDelay_start;

	public float yDelay_factor;

	private MeshRenderer animMesh;

	private CircleCollider2D col;

	private ColorFader fade;

	private Coroutine routine;

	private float originX;

	private float originY;

	private void Awake()
	{
		col = GetComponent<CircleCollider2D>();
		fade = GetComponentInChildren<ColorFader>();
		if ((bool)anim)
		{
			animMesh = anim.GetComponent<MeshRenderer>();
		}
		originX = base.transform.position.x;
		originY = base.transform.position.y;
	}

	private void OnEnable()
	{
		routine = StartCoroutine((type == Type.Zap) ? ZapSequence() : MultiZapSequence());
		if (shift != 0f)
		{
			base.transform.position = new Vector3(originX + Random.Range(0f - shift, shift), originY + Random.Range(0f - shift, shift), base.transform.position.z);
		}
	}

	private void OnDisable()
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
	}

	private IEnumerator ZapSequence()
	{
		col.enabled = false;
		anticRing.SetActive(value: false);
		yield return new WaitForSeconds(delay);
		float seconds = 0f;
		if (yDelay)
		{
			seconds = (yDelay_start - base.transform.position.y) * yDelay_factor;
		}
		yield return new WaitForSeconds(seconds);
		ptAttack.Stop();
		ptAntic.Play();
		yield return new WaitForSeconds(0.15f);
		if ((bool)fade)
		{
			fade.Fade(up: true);
		}
		anticRing.SetActive(value: true);
		zapBugPt1.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		yield return new WaitForSeconds(1.2f);
		anticRing.SetActive(value: false);
		zapBugPt2.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		ptAttack.Play();
		yield return new WaitForSeconds(0.1f);
		col.enabled = true;
		yield return new WaitForSeconds(0.3f);
		if ((bool)fade)
		{
			fade.Fade(up: false);
		}
		ptAttack.Stop();
		ptAntic.Stop();
		col.enabled = false;
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator MultiZapSequence()
	{
		animMesh.enabled = false;
		col.enabled = false;
		ptAttack.Stop();
		base.transform.SetScaleX((Random.Range(0, 2) == 0) ? 1 : (-1));
		base.transform.SetRotation2D(Random.Range(0f, 360f));
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		anim.Play("Zap Antic");
		animMesh.enabled = true;
		yield return new WaitForSeconds(0.8f);
		col.enabled = true;
		ptAttack.Play();
		anim.Play("Zap");
		yield return new WaitForSeconds(1f);
		ptAttack.Stop();
		col.enabled = false;
		yield return StartCoroutine(anim.PlayAnimWait("Zap End"));
		animMesh.enabled = false;
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(value: false);
	}
}
