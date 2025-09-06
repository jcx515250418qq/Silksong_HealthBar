using System.Linq;
using UnityEngine;

public class PlayRandomAnimatorStateOnEnable : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string[] stateNames;

	private int[] stateNameHashes;

	private void Awake()
	{
		stateNameHashes = stateNames.Select((string stateName) => Animator.StringToHash(stateName)).ToArray();
	}

	private void OnEnable()
	{
		if ((bool)animator)
		{
			int num = stateNameHashes.Length;
			if (num != 0)
			{
				animator.Play(stateNameHashes[Random.Range(0, num)]);
			}
		}
	}
}
