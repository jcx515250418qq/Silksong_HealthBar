using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Add multiple options to the options of the Dropdown UI Component")]
	public class UiDropDownAddOptions : ComponentAction<Dropdown>
	{
		[RequiredField]
		[CheckForComponent(typeof(Dropdown))]
		[Tooltip("The GameObject with the UI DropDown component.")]
		public FsmOwnerDefault gameObject;

		[CompoundArray("Options", "Text", "Image")]
		[Tooltip("The text to use for this option.")]
		public FsmString[] optionText;

		[ObjectType(typeof(Sprite))]
		[Tooltip("The image to use for this option.")]
		public FsmObject[] optionImage;

		private Dropdown dropDown;

		private List<Dropdown.OptionData> options;

		public override void Reset()
		{
			gameObject = null;
			optionText = new FsmString[1];
			optionImage = new FsmObject[1];
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				dropDown = cachedComponent;
			}
			DoAddOptions();
			Finish();
		}

		private void DoAddOptions()
		{
			if (!(dropDown == null))
			{
				options = new List<Dropdown.OptionData>();
				for (int i = 0; i < optionText.Length; i++)
				{
					FsmString fsmString = optionText[i];
					options.Add(new Dropdown.OptionData
					{
						text = fsmString.Value,
						image = (optionImage[i].RawValue as Sprite)
					});
				}
				dropDown.AddOptions(options);
			}
		}
	}
}
