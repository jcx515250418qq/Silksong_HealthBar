using UnityEngine;

public class SetCullingInState : StateMachineBehaviour
{
	[SerializeField]
	private AnimatorCullingMode cullingMode;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.cullingMode = cullingMode;
	}
}
