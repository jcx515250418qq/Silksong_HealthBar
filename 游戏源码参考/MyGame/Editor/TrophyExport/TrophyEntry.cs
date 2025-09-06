using System;
using System.Xml.Serialization;

namespace MyGame.Editor.TrophyExport
{
	[Serializable]
	public class TrophyEntry
	{
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("name")]
		public string Name;

		[XmlElement("detail")]
		public string Detail;

		public TrophyEntry()
		{
		}

		public TrophyEntry(int id, string name, string detail)
		{
			Id = id.ToString("D3");
			Name = name;
			Detail = detail;
		}
	}
}
