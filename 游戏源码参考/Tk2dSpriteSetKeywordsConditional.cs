using System.Collections.Generic;
using UnityEngine;

public sealed class Tk2dSpriteSetKeywordsConditional : MonoBehaviour
{
	[SerializeField]
	private List<PlayerDataTest> tests = new List<PlayerDataTest>();

	[SerializeField]
	private tk2dSprite sprite;

	[SerializeField]
	private bool includeChildren;

	[SerializeField]
	private string[] keywords;

	private void Reset()
	{
		sprite = GetComponent<tk2dSprite>();
	}

	private void Awake()
	{
		foreach (PlayerDataTest test in tests)
		{
			if (!test.IsFulfilled)
			{
				return;
			}
		}
		bool flag = sprite != null;
		if (!flag)
		{
			sprite = GetComponent<tk2dSprite>();
			flag = sprite != null;
		}
		if (flag)
		{
			sprite.ForceBuild();
			string[] array = keywords;
			foreach (string keyword in array)
			{
				sprite.EnableKeyword(keyword);
			}
		}
		if (includeChildren)
		{
			tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>(includeInactive: true);
			foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
			{
				if (!flag || !(tk2dSprite2 == sprite))
				{
					string[] array = keywords;
					foreach (string keyword2 in array)
					{
						tk2dSprite2.ForceBuild();
						tk2dSprite2.EnableKeyword(keyword2);
					}
				}
			}
		}
		base.enabled = false;
	}
}
