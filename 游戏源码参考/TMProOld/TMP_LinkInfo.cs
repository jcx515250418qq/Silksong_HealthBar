namespace TMProOld
{
	public struct TMP_LinkInfo
	{
		public TMP_Text textComponent;

		public int hashCode;

		public int linkIdFirstCharacterIndex;

		public int linkIdLength;

		public int linkTextfirstCharacterIndex;

		public int linkTextLength;

		internal char[] linkID;

		internal void SetLinkID(char[] text, int startIndex, int length)
		{
			if (linkID == null || linkID.Length < length)
			{
				linkID = new char[length];
			}
			for (int i = 0; i < length; i++)
			{
				linkID[i] = text[startIndex + i];
			}
		}

		public string GetLinkText()
		{
			string text = string.Empty;
			TMP_TextInfo textInfo = textComponent.textInfo;
			for (int i = linkTextfirstCharacterIndex; i < linkTextfirstCharacterIndex + linkTextLength; i++)
			{
				text += textInfo.characterInfo[i].character;
			}
			return text;
		}

		public string GetLinkID()
		{
			if (textComponent == null)
			{
				return string.Empty;
			}
			return new string(linkID, 0, linkIdLength);
		}
	}
}
