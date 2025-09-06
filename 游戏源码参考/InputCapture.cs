using System;
using System.Text;
using UnityEngine;

public sealed class InputCapture
{
	private bool isCapturing;

	private StringBuilder capturedInput = new StringBuilder();

	private TouchScreenKeyboard keyboard;

	public Action<string> OnKeyboardClosed;

	private string cachedString;

	private bool dirty = true;

	private bool justStarted;

	public void Update()
	{
		if (!isCapturing)
		{
			return;
		}
		if (keyboard == null)
		{
			string inputString = Input.inputString;
			foreach (char c in inputString)
			{
				switch (c)
				{
				case '\b':
					if (capturedInput.Length > 0)
					{
						capturedInput.Remove(capturedInput.Length - 1, 1);
						dirty = true;
					}
					break;
				case '\n':
				case '\r':
					if (!justStarted)
					{
						StopCapturing();
					}
					break;
				default:
					capturedInput.Append(c);
					dirty = true;
					break;
				}
			}
			justStarted = false;
		}
		else if (keyboard.status == TouchScreenKeyboard.Status.Done)
		{
			capturedInput.Clear();
			capturedInput.Append(keyboard.text);
			keyboard = null;
			dirty = true;
			StopCapturing();
		}
	}

	public void StartCapturing(string startingText)
	{
		isCapturing = true;
		dirty = true;
		capturedInput.Clear();
		justStarted = true;
		if (!string.IsNullOrEmpty(startingText))
		{
			capturedInput.Append(startingText);
		}
		if (TouchScreenKeyboard.isSupported)
		{
			OpenTouchScreenKeyboard();
		}
	}

	public void StopCapturing()
	{
		isCapturing = false;
		if (keyboard != null)
		{
			capturedInput.Clear();
			capturedInput.Append(keyboard.text);
			keyboard = null;
			dirty = true;
		}
		OnKeyboardClosed?.Invoke(GetCapturedInput());
	}

	public string GetCapturedInput()
	{
		if (dirty)
		{
			dirty = false;
			cachedString = capturedInput.ToString();
		}
		return cachedString;
	}

	public void ClearCapturedInput()
	{
		dirty = true;
		capturedInput.Clear();
	}

	private void OpenTouchScreenKeyboard()
	{
		keyboard = TouchScreenKeyboard.Open(GetCapturedInput(), TouchScreenKeyboardType.Default, autocorrection: false, multiline: false, secure: false, alert: false);
	}
}
