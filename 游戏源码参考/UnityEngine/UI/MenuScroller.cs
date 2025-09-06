using System.Collections;
using PolyAndCode.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuScroller : UIBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
	{
		[SerializeField]
		private RecyclableScrollRect scrollRect;

		[SerializeField]
		private float scrollFirstDelay;

		[SerializeField]
		private float scrollLerpTime;

		[SerializeField]
		private AnimationCurve scrollLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private float scrollRepeatAmount;

		[SerializeField]
		private float scrollRepeatAmountFast;

		private int previousScrollDir;

		private bool isSelected;

		private double nextScrollTime;

		private Coroutine scrollRoutine;

		private HeroActions inputActions;

		protected override void Awake()
		{
			base.Awake();
			inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Deselect();
			previousScrollDir = 0;
		}

		private void Update()
		{
			if (isSelected)
			{
				int num = 0;
				if (inputActions.Up.IsPressed)
				{
					num--;
				}
				if (inputActions.Down.IsPressed)
				{
					num++;
				}
				bool isFast = false;
				if (inputActions.RsUp.IsPressed)
				{
					num--;
					isFast = true;
				}
				if (inputActions.RsDown.IsPressed)
				{
					num++;
					isFast = true;
				}
				DoInput(num, isFast);
				previousScrollDir = num;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			isSelected = true;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			Deselect();
		}

		private void Deselect()
		{
			isSelected = false;
		}

		private void DoInput(int scrollDir, bool isFast)
		{
			if (scrollDir == 0)
			{
				if (previousScrollDir != 0 && scrollRoutine == null)
				{
					scrollRoutine = StartCoroutine(ScrollLerped(previousScrollDir));
				}
				nextScrollTime = 0.0;
			}
			else if (!(Time.timeAsDouble < nextScrollTime))
			{
				if (scrollRoutine != null)
				{
					StopCoroutine(scrollRoutine);
					scrollRoutine = null;
				}
				if (!isFast && previousScrollDir == 0)
				{
					nextScrollTime = Time.timeAsDouble + (double)scrollFirstDelay;
					scrollRoutine = StartCoroutine(ScrollLerped(scrollDir));
					return;
				}
				float num = scrollRect.GetCellSize() / scrollRect.GetContentSize();
				float num2 = (isFast ? scrollRepeatAmountFast : scrollRepeatAmount);
				float scrollPosition = scrollRect.GetScrollPosition();
				scrollPosition += num * num2 * Mathf.Sign(scrollDir) * Time.deltaTime;
				scrollRect.SetScrollPosition(scrollPosition);
			}
		}

		private IEnumerator ScrollLerped(int direction)
		{
			float num = scrollRect.GetCellSize() / scrollRect.GetContentSize();
			float num2 = num * Mathf.Sign(direction);
			float startScrollPos = scrollRect.GetScrollPosition();
			float endScrollPos = startScrollPos + num2;
			endScrollPos = Mathf.Round(endScrollPos / num) * num;
			for (float elapsed = 0f; elapsed < scrollLerpTime; elapsed += Time.deltaTime)
			{
				float t = scrollLerpCurve.Evaluate(elapsed / scrollLerpTime);
				float scrollPosition = Mathf.Lerp(startScrollPos, endScrollPos, t);
				scrollRect.SetScrollPosition(scrollPosition);
				yield return null;
			}
			scrollRect.SetScrollPosition(endScrollPos);
		}
	}
}
