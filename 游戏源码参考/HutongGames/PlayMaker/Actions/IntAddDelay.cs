using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to an Integer Variable.")]
	public class IntAddDelay : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[RequiredField]
		public FsmInt add;

		public float delay;

		public bool repeat;

		private float timer;

		public override void Reset()
		{
			intVariable = null;
			add = null;
			timer = 0f;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < delay)
			{
				timer += Time.deltaTime;
				return;
			}
			intVariable.Value += add.Value;
			if (repeat)
			{
				timer -= delay;
			}
			else
			{
				Finish();
			}
		}
	}
}
