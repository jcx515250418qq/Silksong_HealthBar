using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionCountdownTimer : MonoBehaviour
{
	public int timerDuration;

	public Text timerText;

	private int currentTime;

	private bool running;

	private InputHandler ih;

	private void Start()
	{
		ih = GameManager.instance.inputHandler;
	}

	public void StartTimer()
	{
		currentTime = timerDuration;
		UpdateTimerText();
		running = true;
		StartCoroutine("CountDown");
	}

	public void CancelTimer()
	{
		running = false;
		StopCoroutine("CountDown");
	}

	private void TickDown()
	{
		if (currentTime == 0)
		{
			timerText.text = "";
			running = false;
			CancelTimer();
			StartCoroutine(RollbackRes());
		}
		else
		{
			UpdateTimerText();
			currentTime--;
		}
	}

	private void UpdateTimerText()
	{
		timerText.text = currentTime.ToString();
	}

	private IEnumerator CountDown()
	{
		for (int i = 0; i < 20; i++)
		{
			if (running)
			{
				TickDown();
				yield return GameManager.instance.timeTool.TimeScaleIndependentWaitForSeconds(1f);
			}
		}
	}

	private IEnumerator RollbackRes()
	{
		ih.StopUIInput();
		yield return null;
		UIManager.instance.UIGoToVideoMenu(rollbackRes: true);
	}
}
