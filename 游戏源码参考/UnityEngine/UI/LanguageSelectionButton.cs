using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class LanguageSelectionButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		[Header("Language")]
		[SerializeField]
		private string language;

		[SerializeField]
		private LanguageSelector languageSelector;

		public string Language => language;

		protected override void Awake()
		{
			base.Awake();
			if (!languageSelector)
			{
				languageSelector = GetComponentInParent<LanguageSelector>();
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				base.OnSubmit(eventData);
				if ((bool)languageSelector)
				{
					languageSelector.SetLanguage(this);
				}
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}
	}
}
