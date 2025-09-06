using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Toolbar. NOTE: Arrays must be the same length as NumButtons or empty.")]
	public class GUILayoutToolbar : GUILayoutAction
	{
		[Tooltip("The number of buttons in the toolbar")]
		public FsmInt numButtons;

		[Tooltip("Store the index of the selected button in an Integer Variable")]
		[UIHint(UIHint.Variable)]
		public FsmInt selectedButton;

		[Tooltip("Event to send when each button is pressed.")]
		public FsmEvent[] buttonEventsArray;

		[Tooltip("Image to use on each button.")]
		public FsmTexture[] imagesArray;

		[Tooltip("Text to use on each button.")]
		public FsmString[] textsArray;

		[Tooltip("Tooltip to use for each button.")]
		public FsmString[] tooltipsArray;

		[Tooltip("A named GUIStyle to use for the toolbar buttons. Default is Button.")]
		public FsmString style;

		[Tooltip("Update the content of the buttons every frame. Useful if the buttons are using variables that change.")]
		public bool everyFrame;

		private GUIContent[] contents;

		public GUIContent[] Contents
		{
			get
			{
				if (contents == null)
				{
					SetButtonsContent();
				}
				return contents;
			}
		}

		private void SetButtonsContent()
		{
			if (contents == null)
			{
				contents = new GUIContent[numButtons.Value];
			}
			for (int i = 0; i < numButtons.Value; i++)
			{
				contents[i] = new GUIContent();
			}
			for (int j = 0; j < imagesArray.Length; j++)
			{
				contents[j].image = imagesArray[j].Value;
			}
			for (int k = 0; k < textsArray.Length; k++)
			{
				contents[k].text = textsArray[k].Value;
			}
			for (int l = 0; l < tooltipsArray.Length; l++)
			{
				contents[l].tooltip = tooltipsArray[l].Value;
			}
		}

		public override void Reset()
		{
			base.Reset();
			numButtons = 0;
			selectedButton = null;
			buttonEventsArray = new FsmEvent[0];
			imagesArray = new FsmTexture[0];
			tooltipsArray = new FsmString[0];
			style = "Button";
			everyFrame = false;
		}

		public override void OnEnter()
		{
			string text = ErrorCheck();
			if (!string.IsNullOrEmpty(text))
			{
				LogError(text);
				Finish();
			}
		}

		public override void OnGUI()
		{
			if (everyFrame)
			{
				SetButtonsContent();
			}
			bool changed = GUI.changed;
			GUI.changed = false;
			selectedButton.Value = GUILayout.Toolbar(selectedButton.Value, Contents, style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				if (selectedButton.Value < buttonEventsArray.Length)
				{
					base.Fsm.Event(buttonEventsArray[selectedButton.Value]);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				GUI.changed = changed;
			}
		}

		public override string ErrorCheck()
		{
			string text = "";
			if (imagesArray.Length != 0 && imagesArray.Length != numButtons.Value)
			{
				text += "Images array doesn't match NumButtons.\n";
			}
			if (textsArray.Length != 0 && textsArray.Length != numButtons.Value)
			{
				text += "Texts array doesn't match NumButtons.\n";
			}
			if (tooltipsArray.Length != 0 && tooltipsArray.Length != numButtons.Value)
			{
				text += "Tooltips array doesn't match NumButtons.\n";
			}
			return text;
		}
	}
}
