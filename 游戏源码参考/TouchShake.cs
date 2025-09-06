using System.Collections;
using UnityEngine;

public class TouchShake : MonoBehaviour
{
	[Header("If using SpriteRenderer")]
	public SpriteRenderer spriteRenderer;

	public BasicSpriteAnimator anim;

	[Header("If using tk2D")]
	public tk2dSpriteAnimator tk2dAnim;

	private Coroutine animateRoutine;

	[Header("General")]
	public ParticleSystem particles;

	public int emitParticles = 20;

	public AudioSource audioSourcePrefab;

	public RandomAudioClipTable audioTable;

	private void Start()
	{
		if ((bool)anim)
		{
			anim.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.tag == "HeroBox") && !(collision.tag == "Player"))
		{
			return;
		}
		if ((bool)anim)
		{
			if (anim.gameObject.activeInHierarchy && (bool)anim && animateRoutine == null)
			{
				animateRoutine = StartCoroutine(DoAnimation());
			}
		}
		else if ((bool)tk2dAnim && tk2dAnim.gameObject.activeInHierarchy && (bool)tk2dAnim && animateRoutine == null)
		{
			animateRoutine = StartCoroutine(DoAnimation());
		}
	}

	private IEnumerator DoAnimation()
	{
		if ((bool)particles)
		{
			particles.Emit(emitParticles);
		}
		if ((bool)audioTable)
		{
			audioTable.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		if ((bool)spriteRenderer && (bool)anim)
		{
			yield return StartCoroutine(SpriteAnimation());
		}
		else if ((bool)tk2dAnim)
		{
			yield return StartCoroutine(tk2dAnimation());
		}
		animateRoutine = null;
	}

	private IEnumerator SpriteAnimation()
	{
		Sprite sprite = spriteRenderer.sprite;
		anim.enabled = true;
		yield return null;
		yield return new WaitForSeconds(anim.Length);
		anim.enabled = false;
		spriteRenderer.sprite = sprite;
	}

	private IEnumerator tk2dAnimation()
	{
		tk2dAnim.PlayFromFrame(0);
		yield return new WaitForSeconds(tk2dAnim.CurrentClip.Duration);
	}
}
