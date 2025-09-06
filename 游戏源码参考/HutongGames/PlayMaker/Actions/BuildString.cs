using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Builds a String from other Strings.")]
	public class BuildString : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Array of Strings to combine.")]
		public FsmString[] stringParts;

		[Tooltip("Separator to insert between each String. E.g. space character.")]
		public FsmString separator;

		[Tooltip("Add Separator to end of built string.")]
		public FsmBool addToEnd;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the final String in a variable.")]
		public FsmString storeResult;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		private string result;

		private static StringBuilder stringBuilder = new StringBuilder();

		public override void Reset()
		{
			stringParts = new FsmString[3];
			separator = null;
			addToEnd = true;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoBuildString();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoBuildString();
		}

		private void DoBuildString()
		{
			if (storeResult == null)
			{
				return;
			}
			string value = separator.Value;
			bool flag = !string.IsNullOrEmpty(value);
			int num = 0;
			num += stringParts.Length;
			if (flag)
			{
				if (num >= 2)
				{
					num++;
				}
				if (addToEnd.Value)
				{
					num++;
				}
			}
			if (num >= 3)
			{
				stringBuilder.Clear();
				for (int i = 0; i < stringParts.Length - 1; i++)
				{
					stringBuilder.Append(stringParts[i]);
					if (flag)
					{
						stringBuilder.Append(value);
					}
				}
				stringBuilder.Append(stringParts[^1]);
				if (flag && addToEnd.Value)
				{
					stringBuilder.Append(value);
				}
				storeResult.Value = stringBuilder.ToString();
				stringBuilder.Clear();
				return;
			}
			result = string.Empty;
			for (int j = 0; j < stringParts.Length - 1; j++)
			{
				result += stringParts[j];
				if (flag)
				{
					result += value;
				}
			}
			result += stringParts[^1];
			if (flag && addToEnd.Value)
			{
				result += value;
			}
			storeResult.Value = result;
		}
	}
}
