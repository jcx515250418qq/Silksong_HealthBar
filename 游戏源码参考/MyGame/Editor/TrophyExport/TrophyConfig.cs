using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MyGame.Editor.TrophyExport
{
	[Serializable]
	[XmlRoot("trophyconf")]
	public class TrophyConfig
	{
		[XmlAttribute("version")]
		public string Version = "1.1";

		[XmlAttribute("platform")]
		public string Platform = "ps4";

		[XmlAttribute("policy")]
		public string Policy = "large";

		[XmlElement("title-name")]
		public string TitleName = "";

		[XmlElement("title-detail")]
		public string TitleDetail = "";

		[XmlElement("trophy")]
		public List<TrophyEntry> Trophies = new List<TrophyEntry>();
	}
}
