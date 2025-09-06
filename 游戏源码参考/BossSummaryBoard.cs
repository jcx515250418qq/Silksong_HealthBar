using System.Collections.Generic;
using UnityEngine;

public class BossSummaryBoard : MonoBehaviour
{
	public List<BossStatue> bossStatues = new List<BossStatue>();

	[Space]
	public GameObject bossSummaryUI;

	private BossSummaryUI ui;

	private void Start()
	{
		if ((bool)bossSummaryUI)
		{
			GameObject gameObject = Object.Instantiate(bossSummaryUI);
			ui = gameObject.GetComponent<BossSummaryUI>();
			if ((bool)ui)
			{
				ui.SetupUI(bossStatues);
			}
			gameObject.SetActive(value: false);
		}
	}

	public void Show()
	{
		if ((bool)ui)
		{
			ui.Show();
		}
	}

	public void Hide()
	{
		if ((bool)ui)
		{
			ui.Hide();
		}
	}
}
