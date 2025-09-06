using System;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleHorizontalLayoutGroup : HorizontalLayoutGroup
{
	[SerializeField]
	private int targetRectChildren;

	private Action doCalculateLayoutInputHorizontal;

	private Action doCalculateLayoutInputVertical;

	private Action doSetLayoutHorizontal;

	private Action doSetLayoutVertical;

	private void DoLayoutMethod(Action method)
	{
		bool flag = m_ChildForceExpandWidth;
		m_ChildForceExpandWidth = base.rectChildren.Count > targetRectChildren;
		float num = m_Spacing;
		if (base.rectChildren.Count > targetRectChildren)
		{
			m_Spacing = 0f;
		}
		method();
		m_ChildForceExpandWidth = flag;
		m_Spacing = num;
	}

	public override void CalculateLayoutInputHorizontal()
	{
		if (doCalculateLayoutInputHorizontal == null)
		{
			doCalculateLayoutInputHorizontal = base.CalculateLayoutInputHorizontal;
		}
		DoLayoutMethod(doCalculateLayoutInputHorizontal);
	}

	public override void CalculateLayoutInputVertical()
	{
		if (doCalculateLayoutInputVertical == null)
		{
			doCalculateLayoutInputVertical = base.CalculateLayoutInputVertical;
		}
		DoLayoutMethod(doCalculateLayoutInputVertical);
	}

	public override void SetLayoutHorizontal()
	{
		if (doSetLayoutHorizontal == null)
		{
			doSetLayoutHorizontal = base.SetLayoutHorizontal;
		}
		DoLayoutMethod(doSetLayoutHorizontal);
	}

	public override void SetLayoutVertical()
	{
		if (doSetLayoutVertical == null)
		{
			doSetLayoutVertical = base.SetLayoutVertical;
		}
		DoLayoutMethod(doSetLayoutVertical);
	}
}
