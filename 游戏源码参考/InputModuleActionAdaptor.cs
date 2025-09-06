using InControl;
using UnityEngine;

[RequireComponent(typeof(HollowKnightInputModule))]
public class InputModuleActionAdaptor : MonoBehaviour
{
	private InputHandler inputHandler;

	private HollowKnightInputModule inputModule;

	private void Start()
	{
		inputHandler = GameManager.instance.inputHandler;
		inputModule = GetComponent<HollowKnightInputModule>();
		if (inputHandler != null && inputModule != null)
		{
			inputModule.MoveAction = inputHandler.inputActions.MoveVector;
			inputModule.SubmitAction = inputHandler.inputActions.MenuSubmit;
			inputModule.CancelAction = inputHandler.inputActions.MenuCancel;
			inputModule.JumpAction = inputHandler.inputActions.Jump;
			inputModule.AttackAction = inputHandler.inputActions.Attack;
			inputModule.CastAction = inputHandler.inputActions.Cast;
			inputModule.MoveAction = inputHandler.inputActions.MoveVector;
		}
		else
		{
			Debug.LogError("Unable to bind player action set to Input Module.");
		}
		InputHandler.OnUpdateHeroActions += OnInputHandlerOnOnUpdateHeroActions;
	}

	private void OnDestroy()
	{
		InputHandler.OnUpdateHeroActions -= OnInputHandlerOnOnUpdateHeroActions;
	}

	private void OnInputHandlerOnOnUpdateHeroActions(HeroActions actions)
	{
		if (inputHandler != null && inputModule != null)
		{
			inputModule.MoveAction = inputHandler.inputActions.MoveVector;
			inputModule.SubmitAction = inputHandler.inputActions.MenuSubmit;
			inputModule.CancelAction = inputHandler.inputActions.MenuCancel;
			inputModule.JumpAction = inputHandler.inputActions.Jump;
			inputModule.AttackAction = inputHandler.inputActions.Attack;
			inputModule.CastAction = inputHandler.inputActions.Cast;
			inputModule.MoveAction = inputHandler.inputActions.MoveVector;
		}
		else
		{
			Debug.LogError("Unable to bind player action set to Input Module.");
		}
	}
}
