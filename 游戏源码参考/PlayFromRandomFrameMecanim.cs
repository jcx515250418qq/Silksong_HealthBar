using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayFromRandomFrameMecanim : MonoBehaviour
{
	public string stateToPlay;

	public int layerToPlay;

	public bool onEnable;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if (!onEnable)
		{
			DoPlay();
		}
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			DoPlay();
		}
	}

	private void DoPlay()
	{
		StartCoroutine(DelayStart());
	}

	private IEnumerator DelayStart()
	{
		yield return null;
		if (string.IsNullOrEmpty(stateToPlay))
		{
			int shortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			animator.Play(shortNameHash, layerToPlay, Random.Range(0f, 1f));
		}
		else
		{
			animator.Play(stateToPlay, layerToPlay, Random.Range(0f, 1f));
		}
	}
}
