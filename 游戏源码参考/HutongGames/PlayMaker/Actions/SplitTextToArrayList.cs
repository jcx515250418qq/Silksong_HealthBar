using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Split a text asset or string into an arrayList")]
	public class SplitTextToArrayList : ArrayListActions
	{
		public enum ArrayMakerParseStringAs
		{
			String = 0,
			Int = 1,
			Float = 2
		}

		public enum SplitSpecialChars
		{
			NewLine = 0,
			Tab = 1,
			Space = 2
		}

		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("From where to start parsing, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		[Tooltip("the range of parsing")]
		public FsmInt parseRange;

		[ActionSection("Source")]
		[Tooltip("Text asset source")]
		public TextAsset textAsset;

		[Tooltip("Text Asset is ignored if this is set.")]
		public FsmString OrThisString;

		[ActionSection("Split")]
		[Tooltip("Split")]
		public SplitSpecialChars split;

		[Tooltip("Split is ignored if this value is not empty. Each chars taken in account for split")]
		public FsmString OrThisChar;

		[ActionSection("Value")]
		[Tooltip("Parse the line as a specific type")]
		public ArrayMakerParseStringAs parseAsType;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			startIndex = null;
			parseRange = null;
			textAsset = null;
			split = SplitSpecialChars.NewLine;
			parseAsType = ArrayMakerParseStringAs.String;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				splitText();
			}
			Finish();
		}

		public void splitText()
		{
			if (!isProxyValid())
			{
				return;
			}
			string text;
			if (OrThisString.Value.Length == 0)
			{
				if (textAsset == null)
				{
					return;
				}
				text = textAsset.text;
			}
			else
			{
				text = OrThisString.Value;
			}
			proxy.arrayList.Clear();
			string[] array;
			if (OrThisChar.Value.Length == 0)
			{
				char separator = '\n';
				switch (split)
				{
				case SplitSpecialChars.Tab:
					separator = '\t';
					break;
				case SplitSpecialChars.Space:
					separator = ' ';
					break;
				}
				array = text.Split(separator);
			}
			else
			{
				array = text.Split(OrThisChar.Value.ToCharArray());
			}
			int value = startIndex.Value;
			int num = array.Length;
			if (parseRange.Value > 0)
			{
				num = Mathf.Min(num - value, parseRange.Value);
			}
			string[] array2 = new string[num];
			int num2 = 0;
			for (int i = value; i < value + num; i++)
			{
				array2[num2] = array[i];
				num2++;
			}
			if (parseAsType == ArrayMakerParseStringAs.String)
			{
				proxy.arrayList.InsertRange(0, array2);
			}
			else if (parseAsType == ArrayMakerParseStringAs.Int)
			{
				int[] array3 = new int[array2.Length];
				int num3 = 0;
				string[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					int.TryParse(array4[j], out array3[num3]);
					num3++;
				}
				proxy.arrayList.InsertRange(0, array3);
			}
			else if (parseAsType == ArrayMakerParseStringAs.Float)
			{
				float[] array5 = new float[array2.Length];
				int num4 = 0;
				string[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					float.TryParse(array4[j], out array5[num4]);
					num4++;
				}
				proxy.arrayList.InsertRange(0, array5);
			}
		}
	}
}
