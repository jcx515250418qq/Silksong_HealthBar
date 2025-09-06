using System.Collections;
using UnityEngine;

public class CameraFade : MonoBehaviour, IOnGUI
{
	public enum FadeTypes
	{
		NONE = 0,
		BLACK_TO_CLEAR = 1,
		CLEAR_TO_BLACK = 2
	}

	private GUIStyle backgroundStyle = new GUIStyle();

	private Texture2D fadeTexture;

	private Color currentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	private Color targetScreenOverlayColor = new Color(0f, 0f, 0f, 1f);

	private Color desiredOverlayColor;

	private Color deltaColor = new Color(0f, 0f, 0f, 0f);

	private int fadeGUIDepth = -1000;

	[Header("Fade On Scene Start")]
	[Space(6f)]
	[Tooltip("Type of fade to do on Start.")]
	public FadeTypes fadeOnStart;

	[Tooltip("The time in seconds to wait after Start before performing the delay.")]
	public float startDelay;

	[Tooltip("Time to perform fade in seconds on Start.")]
	public float fadeTime;

	private bool drawGUI;

	public int GUIDepth => 0;

	private void Awake()
	{
		fadeTexture = new Texture2D(1, 1);
		fadeTexture.name = $"{this} Fade Texture";
		backgroundStyle.normal.background = fadeTexture;
	}

	private IEnumerator Start()
	{
		if (fadeOnStart == FadeTypes.BLACK_TO_CLEAR)
		{
			SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
		}
		else if (fadeOnStart == FadeTypes.CLEAR_TO_BLACK)
		{
			SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
		}
		if (startDelay > 0f)
		{
			yield return new WaitForSeconds(startDelay);
		}
		else
		{
			yield return new WaitForEndOfFrame();
		}
		if (fadeOnStart == FadeTypes.BLACK_TO_CLEAR)
		{
			FadeToTransparent(fadeTime);
		}
		else if (fadeOnStart == FadeTypes.CLEAR_TO_BLACK)
		{
			FadeToBlack(fadeTime);
		}
	}

	private void OnDisable()
	{
		ToggleDrawGUI(draw: false);
	}

	private void OnDestroy()
	{
		if (fadeTexture != null)
		{
			Object.Destroy(fadeTexture);
			fadeTexture = null;
		}
	}

	private void Update()
	{
		if (currentScreenOverlayColor != targetScreenOverlayColor)
		{
			if (Mathf.Abs(currentScreenOverlayColor.a - targetScreenOverlayColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
			{
				desiredOverlayColor = targetScreenOverlayColor;
				SetScreenOverlayColor(desiredOverlayColor);
				deltaColor = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				desiredOverlayColor = currentScreenOverlayColor + deltaColor * Time.deltaTime;
			}
		}
		if (desiredOverlayColor.a > 0f)
		{
			ToggleDrawGUI(draw: true);
		}
		else
		{
			ToggleDrawGUI(draw: false);
		}
	}

	private void ToggleDrawGUI(bool draw)
	{
		if (drawGUI != draw)
		{
			if (draw)
			{
				GUIDrawer.AddDrawer(this);
			}
			else
			{
				GUIDrawer.RemoveDrawer(this);
			}
		}
	}

	public void DrawGUI()
	{
		SetScreenOverlayColor(desiredOverlayColor);
		if (currentScreenOverlayColor.a > 0f)
		{
			GUI.depth = fadeGUIDepth;
			GUI.Label(new Rect(-10f, -10f, Screen.width + 10, Screen.height + 10), fadeTexture, backgroundStyle);
		}
	}

	public void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		if (currentScreenOverlayColor != newScreenOverlayColor)
		{
			currentScreenOverlayColor = newScreenOverlayColor;
			fadeTexture.SetPixel(0, 0, currentScreenOverlayColor);
			fadeTexture.Apply();
		}
	}

	public void StartFade(Color newScreenOverlayColor, float fadeDuration)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		targetScreenOverlayColor = newScreenOverlayColor;
		deltaColor = (targetScreenOverlayColor - currentScreenOverlayColor) / (fadeDuration * 2f);
	}

	public void FadeToBlack(float duration)
	{
		SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
		StartFade(new Color(0f, 0f, 0f, 1f), duration);
	}

	public void FadeToTransparent(float duration)
	{
		SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
		StartFade(new Color(0f, 0f, 0f, 0f), duration);
	}
}
