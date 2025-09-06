using System;
using UnityEngine;
using UnityEngine.Audio;

public class CompletionStyles : MonoBehaviour
{
	[Serializable]
	public class CompletionStyle
	{
		[Serializable]
		public class CameraCurves
		{
			[Range(0f, 5f)]
			public float Saturation = 1f;

			public AnimationCurve RedChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

			public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

			public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		}

		public string ReadMenuStyleKey;

		public GameObject StyleObject;

		public CameraCurves CameraColorCorrection;

		public Color AmbientColor;

		public float AmbientIntensity;

		public float BlurPlaneVibranceOffset;

		public AudioMixerSnapshot MusicSnapshot;
	}

	public CompletionStyle[] Styles;

	private void OnEnable()
	{
		UpdateStyle();
	}

	private void Start()
	{
		UpdateStyle();
	}

	private void UpdateStyle()
	{
		CompletionStyle[] styles = Styles;
		foreach (CompletionStyle completionStyle in styles)
		{
			if ((bool)completionStyle.StyleObject)
			{
				completionStyle.StyleObject.SetActive(value: false);
			}
		}
		string @string = Platform.Current.RoamingSharedData.GetString("unlockedMenuStyle", null);
		for (int j = 0; j < Styles.Length; j++)
		{
			CompletionStyle completionStyle2 = Styles[j];
			if (string.IsNullOrEmpty(completionStyle2.ReadMenuStyleKey) || !(@string != completionStyle2.ReadMenuStyleKey))
			{
				SetStyle(j);
				break;
			}
		}
	}

	public void SetStyle(int index)
	{
		if (index < 0 || index >= Styles.Length)
		{
			Debug.LogError("Menu Style \"" + index + "\" is out of bounds.");
			return;
		}
		GameManager instance = GameManager.instance;
		CompletionStyle completionStyle = Styles[index];
		if ((bool)completionStyle.MusicSnapshot)
		{
			instance.AudioManager.ApplyMusicSnapshot(completionStyle.MusicSnapshot, 0f, 0f);
		}
		for (int i = 0; i < Styles.Length; i++)
		{
			CompletionStyle completionStyle2 = Styles[i];
			if ((bool)completionStyle2.StyleObject)
			{
				bool active = index == i;
				completionStyle2.StyleObject.SetActive(active);
			}
		}
		instance.sm.IsGradeOverridden = true;
		GameCameras instance2 = GameCameras.instance;
		if ((bool)instance2 && (bool)instance2.colorCorrectionCurves)
		{
			CompletionStyle.CameraCurves cameraColorCorrection = completionStyle.CameraColorCorrection;
			instance2.colorCorrectionCurves.saturation = cameraColorCorrection.Saturation;
			instance2.colorCorrectionCurves.redChannel = cameraColorCorrection.RedChannel;
			instance2.colorCorrectionCurves.greenChannel = cameraColorCorrection.GreenChannel;
			instance2.colorCorrectionCurves.blueChannel = cameraColorCorrection.BlueChannel;
		}
		CustomSceneManager.SetLighting(completionStyle.AmbientColor, completionStyle.AmbientIntensity);
		BlurPlane.SetVibranceOffset(completionStyle.BlurPlaneVibranceOffset);
	}
}
