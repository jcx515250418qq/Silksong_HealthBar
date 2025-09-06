using System;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	[Serializable]
	public class LocalizedValue
	{
		public string locale;

		[TextArea(1, 3)]
		public string text;
	}
}
