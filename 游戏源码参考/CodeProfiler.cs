using System.Collections.Generic;
using UnityEngine;

public class CodeProfiler : MonoBehaviour
{
	private float startTime;

	private float nextOutputTime = 5f;

	private int numFrames;

	private static Dictionary<string, ProfilerRecording> recordings = new Dictionary<string, ProfilerRecording>();

	private string displayText;

	private Rect displayRect = new Rect(10f, 10f, 460f, 300f);

	private void Awake()
	{
		startTime = Time.time;
		displayText = "\n\nTaking initial readings...";
	}

	private void OnGUI()
	{
		GUI.Box(displayRect, "Code Profiler");
		GUI.Label(displayRect, displayText);
	}

	public static void Begin(string id)
	{
		if (!recordings.ContainsKey(id))
		{
			recordings[id] = new ProfilerRecording(id);
		}
		recordings[id].Start();
	}

	public static void End(string id)
	{
		recordings[id].Stop();
	}

	private void Update()
	{
		numFrames++;
		if (!(Time.time > nextOutputTime))
		{
			return;
		}
		int totalWidth = 10;
		displayText = "\n\n";
		float num = (Time.time - startTime) * 1000f;
		float num2 = num / (float)numFrames;
		float num3 = 1000f / (num / (float)numFrames);
		displayText += "Avg frame time: ";
		displayText = displayText + num2.ToString("0.#") + "ms, ";
		displayText = displayText + num3.ToString("0.#") + " fps \n";
		displayText += "Total".PadRight(totalWidth);
		displayText += "MS/frame".PadRight(totalWidth);
		displayText += "Calls/fra".PadRight(totalWidth);
		displayText += "MS/call".PadRight(totalWidth);
		displayText += "Label";
		displayText += "\n";
		foreach (KeyValuePair<string, ProfilerRecording> recording in recordings)
		{
			ProfilerRecording value = recording.Value;
			float num4 = value.Seconds * 1000f;
			float num5 = num4 * 100f / num;
			float num6 = num4 / (float)numFrames;
			float num7 = num4 / (float)value.Count;
			float num8 = (float)value.Count / (float)numFrames;
			displayText += (num5.ToString("0.000") + "%").PadRight(totalWidth);
			displayText += (num6.ToString("0.000") + "ms").PadRight(totalWidth);
			displayText += num8.ToString("0.000").PadRight(totalWidth);
			displayText += (num7.ToString("0.0000") + "ms").PadRight(totalWidth);
			displayText += value.id;
			displayText += "\n";
			value.Reset();
		}
		Debug.Log(displayText);
		numFrames = 0;
		startTime = Time.time;
		nextOutputTime = Time.time + 5f;
	}
}
