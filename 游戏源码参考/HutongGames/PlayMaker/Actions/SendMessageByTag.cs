using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SendMessageByTag : FsmStateAction
	{
		public FsmString tag;

		public FsmString message;

		public override void OnEnter()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag.Value);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SendMessage(message.Value);
			}
		}

		public override void OnExit()
		{
		}
	}
}
