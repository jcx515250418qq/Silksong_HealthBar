using TMProOld;
using UnityEngine;
using UnityEngine.UI;

public class VibrationTesterButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private TextMeshProUGUI text;

	public CameraShakeProfile profile;

	public CameraManagerReference reference;

	public bool isPlaying;

	private void Start()
	{
		button.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		Toggle();
	}

	public void SetUp(CameraShakeProfile profile, CameraManagerReference reference)
	{
		isPlaying = false;
		this.profile = profile;
		this.reference = reference;
		UpdateText();
	}

	private void UpdateText()
	{
		string text = profile.name + " - " + (isPlaying ? "<color=\"green\">Is Playing</color=\"green\">" : "<color=\"red\">Not Playing</color=\"red\">");
		this.text.text = text;
	}

	public void Play()
	{
		isPlaying = true;
		reference.DoShake(profile, this);
		UpdateText();
	}

	public void Stop()
	{
		isPlaying = false;
		reference.CancelShake(profile);
		UpdateText();
	}

	public void Toggle()
	{
		if (!isPlaying)
		{
			Play();
		}
		else
		{
			Stop();
		}
	}
}
