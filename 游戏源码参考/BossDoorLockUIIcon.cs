using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BossDoorLockUIIcon : MonoBehaviour
{
	public Image bossIcon;

	public float unlockAnimDelay = 1f;

	public float indexOffsetDelay = 0.25f;

	public AudioSource audioSourcePrefab;

	public AudioEvent unlockSound;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void SetUnlocked(bool unlocked, bool doUnlockAnim = false, int indexAnimOffset = 0)
	{
		if (unlocked)
		{
			StartCoroutine(PlayAnimWithDelay("Unlock", doUnlockAnim, doUnlockAnim ? (unlockAnimDelay + indexOffsetDelay * (float)indexAnimOffset) : 0f));
		}
		else
		{
			animator.Play("Locked");
		}
	}

	private IEnumerator PlayAnimWithDelay(string anim, bool doAnim, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (doAnim)
		{
			animator.Play(anim);
			unlockSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		else
		{
			animator.Play(anim, 0, 1f);
		}
	}
}
