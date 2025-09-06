using System.Globalization;
using System.Text;
using TeamCherry.BuildBot;
using UnityEngine;
using UnityEngine.UI;

public class SetVersionNumber : MonoBehaviour
{
	private Text textUi;

	private void Awake()
	{
		textUi = GetComponent<Text>();
	}

	private void Start()
	{
		if (!(textUi != null))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder("1.0.28324");
		if (DemoHelper.IsDemoMode)
		{
			stringBuilder.Append(" (Demo)");
		}
		if (CheatManager.IsCheatsEnabled)
		{
			stringBuilder.Append("\n(CHEATS ENABLED)");
			BuildMetadata embedded = BuildMetadata.Embedded;
			if (embedded != null)
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-AU");
				stringBuilder.Append("\nLast Commit Time: " + embedded.CommitTime.ToString(cultureInfo));
				stringBuilder.Append("\nBuild Time: " + embedded.BuildTime.ToString(cultureInfo));
			}
		}
		textUi.text = stringBuilder.ToString();
	}
}
