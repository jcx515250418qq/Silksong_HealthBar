using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuStyleSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
	{
		private MenuStyles styles;

		private readonly List<int> indexList = new List<int>();

		private new void OnEnable()
		{
			styles = MenuStyles.Instance;
			if (!styles || styles.Styles.Length == 0)
			{
				return;
			}
			indexList.Clear();
			List<string> list = new List<string>();
			for (int i = 0; i < styles.Styles.Length; i++)
			{
				MenuStyles.MenuStyle menuStyle = styles.Styles[i];
				if (menuStyle.IsAvailable)
				{
					list.Add(menuStyle.DisplayName);
					indexList.Add(i);
				}
			}
			optionList = list.ToArray();
			selectedOptionIndex = indexList.IndexOf(Mathf.Min(styles.CurrentStyle));
			if (selectedOptionIndex < 0)
			{
				selectedOptionIndex = 0;
			}
			UpdateText();
		}

		public new void OnMove(AxisEventData move)
		{
			if (base.interactable)
			{
				if (MoveOption(move.moveDir))
				{
					UpdateStyle();
				}
				else
				{
					base.OnMove(move);
				}
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				PointerClickCheckArrows(eventData);
				UpdateStyle();
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			MoveOption(MoveDirection.Right);
			UpdateStyle();
		}

		private void UpdateStyle()
		{
			if ((bool)styles && indexList.Count > 0)
			{
				selectedOptionIndex = Mathf.Clamp(selectedOptionIndex, 0, indexList.Count - 1);
				styles.SetStyle(indexList[selectedOptionIndex], fade: true);
			}
		}
	}
}
