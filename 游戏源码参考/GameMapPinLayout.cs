using UnityEngine;

public class GameMapPinLayout : MonoBehaviour
{
	public interface IEvaluateHook
	{
		void ForceEvaluate();
	}

	public interface ILayoutHook
	{
		void LayoutFinished();
	}

	[SerializeField]
	private Vector2 itemOffset;

	private bool isDirty;

	private void OnValidate()
	{
		DoLayout();
	}

	private void OnEnable()
	{
		if (isDirty)
		{
			DoLayout();
		}
	}

	public void DoLayout()
	{
		isDirty = false;
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeSelf)
			{
				num++;
			}
		}
		Vector2 vector = itemOffset * (num - 1) / 2f;
		int num2 = 0;
		foreach (Transform item2 in base.transform)
		{
			if (item2.gameObject.activeSelf)
			{
				Vector2 position = itemOffset * num2 - vector;
				item2.SetLocalPosition2D(position);
				item2.GetComponent<ILayoutHook>()?.LayoutFinished();
				num2++;
			}
		}
	}

	public void Evaluate()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: true);
			item.GetComponent<IEvaluateHook>()?.ForceEvaluate();
		}
		DoLayout();
	}

	public void SetLayoutDirty()
	{
		isDirty = true;
		if (base.gameObject.activeInHierarchy)
		{
			DoLayout();
		}
	}
}
