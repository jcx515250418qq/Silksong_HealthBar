using UnityEngine;

public class NpcAnimatorParams : MonoBehaviour
{
	private static readonly int InConvoParam = Animator.StringToHash("In Conversation");

	private static readonly int TalkingParam = Animator.StringToHash("Is Talking");

	[SerializeField]
	private NPCControlBase control;

	[SerializeField]
	private Animator animator;

	private void Awake()
	{
		if ((bool)control && (bool)animator)
		{
			control.StartedDialogue += delegate
			{
				animator.SetBool(InConvoParam, value: true);
			};
			control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
			{
				animator.SetBool(TalkingParam, !line.IsPlayer);
			};
			control.EndingDialogue += delegate
			{
				animator.SetBool(TalkingParam, value: false);
			};
			control.EndedDialogue += delegate
			{
				animator.SetBool(InConvoParam, value: false);
			};
		}
	}
}
