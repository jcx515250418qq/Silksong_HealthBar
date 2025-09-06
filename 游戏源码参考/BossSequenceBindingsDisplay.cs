using System.Reflection;
using UnityEngine;

public class BossSequenceBindingsDisplay : MonoBehaviour
{
	public GameObject[] bindingIcons;

	private void Start()
	{
		int num = CountCompletedBindings();
		for (int i = 0; i < bindingIcons.Length; i++)
		{
			bindingIcons[i].SetActive(i < num);
		}
	}

	public static void CountBindings(out int total, out int completed)
	{
		total = 0;
		completed = 0;
		FieldInfo[] fields = typeof(PlayerData).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType == typeof(BossSequenceDoor.Completion)))
			{
				continue;
			}
			BossSequenceDoor.Completion completion = (BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData);
			if (completion.completed)
			{
				if (completion.boundNail)
				{
					completed++;
				}
				if (completion.boundShell)
				{
					completed++;
				}
				if (completion.boundCharms)
				{
					completed++;
				}
				if (completion.boundSoul)
				{
					completed++;
				}
			}
			total += 4;
		}
	}

	public static int CountCompletedBindings()
	{
		int total = 0;
		int completed = 0;
		CountBindings(out total, out completed);
		return completed;
	}

	public static int CountTotalBindings()
	{
		int total = 0;
		int completed = 0;
		CountBindings(out total, out completed);
		return total;
	}
}
