using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Creates an FSM at runtime from a saved {{Template}}. The FSM is only active while the state is active. This lets you nest FSMs inside states.\nThis is a very powerful action! It allows you to create a library of FSM Templates that can be re-used in your project. You can edit the template in one place and the changes are reflected everywhere.\nNOTE: You can also specify a template in the {{FSM Inspector}}.")]
	public class RunFSM : RunFSMAction
	{
		[Tooltip("The Template to use. You can drag and drop, use the Unity object browser, or the categorized popup browser to select a template.")]
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

		[Tooltip("Event to send when the FSM has finished (usually because it ran a {{Finish FSM}} action).")]
		public FsmEvent finishEvent;

		[ActionSection("")]
		[Tooltip("Repeat every frame. Waits for the sub Fsm to finish before calling it again.")]
		public bool everyFrame;

		private bool restart;

		public override void Reset()
		{
			fsmTemplateControl = new FsmTemplateControl();
			runFsm = null;
			everyFrame = false;
		}

		public override void Awake()
		{
			base.HandlesOnEvent = true;
			fsmTemplateControl.Init();
			if (fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				runFsm = base.Fsm.CreateSubFsm(fsmTemplateControl);
			}
		}

		public override void OnEnter()
		{
			if (runFsm == null)
			{
				Finish();
				return;
			}
			fsmTemplateControl.UpdateValues();
			fsmTemplateControl.ApplyOverrides(runFsm);
			runFsm.OnEnable();
			Fsm obj = runFsm;
			obj.OnOutputEvent = (Action<FsmEvent>)Delegate.Combine(obj.OnOutputEvent, new Action<FsmEvent>(OnOutputEvent));
			if (!runFsm.Started)
			{
				runFsm.Start();
			}
			fsmTemplateControl.UpdateOutput(base.Fsm);
			CheckIfFinished();
		}

		public override void OnExit()
		{
			if (runFsm != null)
			{
				Fsm obj = runFsm;
				obj.OnOutputEvent = (Action<FsmEvent>)Delegate.Remove(obj.OnOutputEvent, new Action<FsmEvent>(OnOutputEvent));
			}
		}

		private void OnOutputEvent(FsmEvent fsmEvent)
		{
			fsmTemplateControl.UpdateOutput(base.Fsm);
			FsmEvent fsmEvent2 = fsmTemplateControl.MapEvent(fsmEvent);
			if (fsmEvent2 != null)
			{
				base.Fsm.Event(fsmEvent2);
			}
		}

		public override void OnUpdate()
		{
			if (restart)
			{
				OnEnter();
				restart = false;
			}
			else if (runFsm != null)
			{
				runFsm.Update();
				fsmTemplateControl.UpdateOutput(base.Fsm);
				CheckIfFinished();
			}
			else
			{
				Finish();
			}
		}

		public override void OnLateUpdate()
		{
			if (runFsm != null)
			{
				runFsm.LateUpdate();
				fsmTemplateControl.UpdateOutput(base.Fsm);
				CheckIfFinished();
			}
			else
			{
				Finish();
			}
		}

		protected override void CheckIfFinished()
		{
			if (runFsm == null)
			{
				Finish();
			}
			else if (runFsm.Finished)
			{
				if (!everyFrame)
				{
					Finish();
					base.Fsm.Event(finishEvent);
				}
				else
				{
					restart = true;
				}
			}
		}
	}
}
