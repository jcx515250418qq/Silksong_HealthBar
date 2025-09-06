using InControl;
using UnityEngine;

[RequireComponent(typeof(InControlInputModule))]
public class PreMenuInputModuleActionAdaptor : MonoBehaviour
{
	public class PreMenuInputModuleActions : PlayerActionSet
	{
		public PlayerAction Submit;

		public PlayerAction Cancel;

		public PlayerAction Left;

		public PlayerAction Right;

		public PlayerAction Up;

		public PlayerAction Down;

		public PlayerTwoAxisAction Move;

		public PreMenuInputModuleActions()
		{
			Submit = CreatePlayerAction("Submit");
			Cancel = CreatePlayerAction("Cancel");
			Left = CreatePlayerAction("Move Left");
			Right = CreatePlayerAction("Move Right");
			Up = CreatePlayerAction("Move Up");
			Down = CreatePlayerAction("Move Down");
			Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
		}
	}

	private PreMenuInputModuleActions actions;

	private void OnEnable()
	{
		CreateActions();
		InControlInputModule component = GetComponent<InControlInputModule>();
		if (component != null)
		{
			component.SubmitAction = actions.Submit;
			component.CancelAction = actions.Cancel;
			component.MoveAction = actions.Move;
		}
	}

	private void OnDisable()
	{
		DestroyActions();
	}

	private void CreateActions()
	{
		actions = new PreMenuInputModuleActions();
		actions.Submit.AddDefaultBinding(InputControlType.Action1);
		actions.Submit.AddDefaultBinding(Key.Space);
		actions.Submit.AddDefaultBinding(Key.Return);
		actions.Cancel.AddDefaultBinding(InputControlType.Action2);
		actions.Cancel.AddDefaultBinding(Key.Escape);
		actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		actions.Up.AddDefaultBinding(InputControlType.DPadUp);
		actions.Up.AddDefaultBinding(Key.UpArrow);
		actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		actions.Down.AddDefaultBinding(InputControlType.DPadDown);
		actions.Down.AddDefaultBinding(Key.DownArrow);
		actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		actions.Left.AddDefaultBinding(Key.LeftArrow);
		actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		actions.Right.AddDefaultBinding(InputControlType.DPadRight);
		actions.Right.AddDefaultBinding(Key.RightArrow);
	}

	private void DestroyActions()
	{
		actions.Destroy();
	}
}
