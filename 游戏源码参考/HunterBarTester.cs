using System.Collections.Generic;
using UnityEngine;

public sealed class HunterBarTester : MonoBehaviour
{
	[SerializeField]
	private List<UiProgressBar> progressBars = new List<UiProgressBar>();

	private float progress = 1f;

	private float angle;

	private float edgeFade = 3f;

	private bool isActive;

	public float Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = value;
			foreach (UiProgressBar progressBar in progressBars)
			{
				progressBar.Value = progress;
			}
		}
	}

	public float Angle
	{
		get
		{
			return angle;
		}
		set
		{
			angle = value;
			foreach (UiProgressBar progressBar in progressBars)
			{
				progressBar.SetAngle(value);
			}
		}
	}

	public float EdgeFade
	{
		get
		{
			return edgeFade;
		}
		set
		{
			edgeFade = value;
			foreach (UiProgressBar progressBar in progressBars)
			{
				progressBar.SetEdgeFade(value);
			}
		}
	}

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			foreach (UiProgressBar progressBar in progressBars)
			{
				progressBar.gameObject.SetActive(value);
			}
		}
	}

	private void Awake()
	{
		progressBars.Clear();
		progressBars.AddRange(GetComponentsInChildren<UiProgressBar>());
		Progress = 1f;
		IsActive = false;
	}

	public void UpdateBar(float deltaTime)
	{
		foreach (UiProgressBar progressBar in progressBars)
		{
			progressBar.UpdateBar(deltaTime);
		}
	}
}
