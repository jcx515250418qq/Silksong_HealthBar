using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Get the selected value (zero based index), sprite and text from a UI Dropdown Component")]
	public class UiDropDownGetSelectedData : ComponentAction<Dropdown>
	{
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The selected index of the dropdown (zero based index).")]
		[UIHint(UIHint.Variable)]
		public FsmInt index;

		[Tooltip("The selected text.")]
		[UIHint(UIHint.Variable)]
		public FsmString getText;

		[ObjectType(typeof(Sprite))]
		[Tooltip("The selected text.")]
		[UIHint(UIHint.Variable)]
		public FsmObject getImage;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private Dropdown dropDown;

		public override void Reset()
		{
			gameObject = null;
			index = null;
			getText = null;
			getImage = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				dropDown = cachedComponent;
			}
			GetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetValue();
		}

		private void GetValue()
		{
			if (!(dropDown == null))
			{
				if (!index.IsNone)
				{
					index.Value = dropDown.value;
				}
				if (!getText.IsNone)
				{
					getText.Value = dropDown.options[dropDown.value].text;
				}
				if (!getImage.IsNone)
				{
					getImage.Value = dropDown.options[dropDown.value].image;
				}
			}
		}
	}
}
