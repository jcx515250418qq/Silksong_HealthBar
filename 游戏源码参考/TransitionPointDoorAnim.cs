using System.Collections;
using UnityEngine;

public class TransitionPointDoorAnim : MonoBehaviour, ITransitionPointDoorAnim
{
	private static readonly int _openAnim = Animator.StringToHash("Open");

	private static readonly int _closeAnim = Animator.StringToHash("Close");

	[SerializeField]
	private TransitionPoint transitionPoint;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioEvent openSound;

	[SerializeField]
	private AudioEvent closeSound;

	private void Awake()
	{
		transitionPoint.DoorAnimHandler = this;
	}

	private void Start()
	{
		GameManager gm = GameManager.instance;
		string text = transitionPoint.gameObject.name;
		string entryGateName = gm.GetEntryGateName();
		if (!gm.HasFinishedEnteringScene && entryGateName == text)
		{
			animator.Play(_openAnim, 0, 1f);
			GameManager.EnterSceneEvent temp = null;
			temp = delegate
			{
				closeSound.SpawnAndPlayOneShot(base.transform.position);
				animator.Play(_closeAnim, 0, 0f);
				gm.OnFinishedEnteringScene -= temp;
			};
			gm.OnFinishedEnteringScene += temp;
		}
		else
		{
			animator.Play(_closeAnim, 0, 1f);
		}
	}

	public Coroutine GetDoorAnimRoutine()
	{
		return StartCoroutine(DoorEnterAnimRoutine());
	}

	private IEnumerator DoorEnterAnimRoutine()
	{
		openSound.SpawnAndPlayOneShot(base.transform.position);
		animator.Play(_openAnim, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}
}
