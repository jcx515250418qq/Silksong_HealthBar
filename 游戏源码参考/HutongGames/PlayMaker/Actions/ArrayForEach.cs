using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Loop through all items in an {{Array}} and run an FSM Template on each item.\\nNOTE: This is an advanced/experimental action, still in beta. Please report any issues you run into.")]
	public class ArrayForEach : RunFSMAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Array to iterate through.")]
		public FsmArray array;

		[HideTypeFilter]
		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the item in a variable")]
		public FsmVar storeItem;

		[ActionSection("Run FSM")]
		[Tooltip("The Template to run on each item in the array.<ul><li>The Template should expose a variable in the Inspector.</li><li>Use this Input variable to input the stored item.</li><li>The Template should use {{Finish FSM}} when finished.</li></ul>")]
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

		[Tooltip("Event to send after iterating through all items in the Array.")]
		public FsmEvent finishEvent;

		private int currentIndex;

		public override void Reset()
		{
			array = null;
			fsmTemplateControl = new FsmTemplateControl();
			runFsm = null;
		}

		public override void Awake()
		{
			if (array != null && fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				runFsm = base.Fsm.CreateSubFsm(fsmTemplateControl);
			}
		}

		public override void OnEnter()
		{
			if (array == null || runFsm == null)
			{
				Finish();
				return;
			}
			currentIndex = 0;
			StartFsm();
		}

		public override void OnUpdate()
		{
			runFsm.Update();
			if (runFsm.Finished)
			{
				StartNextFsm();
			}
		}

		public override void OnFixedUpdate()
		{
			runFsm.FixedUpdate();
			if (runFsm.Finished)
			{
				StartNextFsm();
			}
		}

		public override void OnLateUpdate()
		{
			runFsm.LateUpdate();
			if (runFsm.Finished)
			{
				StartNextFsm();
			}
		}

		private void StartNextFsm()
		{
			currentIndex++;
			StartFsm();
		}

		private void StartFsm()
		{
			while (currentIndex < array.Length)
			{
				DoStartFsm();
				if (!runFsm.Finished)
				{
					return;
				}
				currentIndex++;
			}
			base.Fsm.Event(finishEvent);
			Finish();
		}

		private void DoStartFsm()
		{
			storeItem.SetValue(array.Values[currentIndex]);
			fsmTemplateControl.UpdateValues();
			fsmTemplateControl.ApplyOverrides(runFsm);
			runFsm.OnEnable();
			if (!runFsm.Started)
			{
				runFsm.Start();
			}
		}

		protected override void CheckIfFinished()
		{
		}
	}
}
