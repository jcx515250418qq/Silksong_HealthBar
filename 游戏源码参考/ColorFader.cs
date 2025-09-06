using System.Collections;
using TMProOld;
using UnityEngine;

public class ColorFader : MonoBehaviour
{
	public delegate void FadeEndEvent(bool up);

	public Color downColour = new Color(1f, 1f, 1f, 0f);

	public float downTime = 0.5f;

	public Color upColour = new Color(1f, 1f, 1f, 1f);

	public float upDelay;

	public float upTime = 0.4f;

	private Color initialColour;

	public bool useInitialColour = true;

	private SpriteRenderer spriteRenderer;

	private TextMeshPro textRenderer;

	private tk2dSprite tk2dSprite;

	private bool setup;

	private bool hasSpriteRenderer;

	private bool hasTextRenderer;

	private bool hasTk2dSprite;

	private Coroutine fadeRoutine;

	public event FadeEndEvent OnFadeEnd;

	private void Reset()
	{
		PlayMakerFSM[] components = GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if ((playMakerFSM.FsmTemplate ? playMakerFSM.FsmTemplate.name : playMakerFSM.FsmName) == "color_fader")
			{
				downColour = playMakerFSM.FsmVariables.GetFsmColor("Down Colour").Value;
				downTime = playMakerFSM.FsmVariables.GetFsmFloat("Down Time").Value;
				upColour = playMakerFSM.FsmVariables.GetFsmColor("Up Colour").Value;
				upDelay = playMakerFSM.FsmVariables.GetFsmFloat("Up Delay").Value;
				upTime = playMakerFSM.FsmVariables.GetFsmFloat("Up Time").Value;
				break;
			}
		}
	}

	private void Start()
	{
		Setup();
	}

	private void Setup()
	{
		if (setup)
		{
			return;
		}
		setup = true;
		if (!spriteRenderer)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		hasSpriteRenderer = spriteRenderer != null;
		if (hasSpriteRenderer)
		{
			initialColour = (useInitialColour ? spriteRenderer.color : Color.white);
			spriteRenderer.color = downColour * initialColour;
			return;
		}
		if (!textRenderer)
		{
			textRenderer = GetComponent<TextMeshPro>();
		}
		hasTextRenderer = textRenderer != null;
		if (hasTextRenderer)
		{
			initialColour = (useInitialColour ? textRenderer.color : Color.white);
			textRenderer.color = downColour * initialColour;
			return;
		}
		if (!tk2dSprite)
		{
			tk2dSprite = GetComponent<tk2dSprite>();
		}
		hasTk2dSprite = tk2dSprite != null;
		if (hasTk2dSprite)
		{
			initialColour = (useInitialColour ? tk2dSprite.color : Color.white);
			tk2dSprite.color = downColour * initialColour;
		}
	}

	public void Fade(bool up)
	{
		Setup();
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		if (up)
		{
			fadeRoutine = StartCoroutine(Fade(upColour, upTime, upDelay));
		}
		else
		{
			fadeRoutine = StartCoroutine(Fade(downColour, downTime, 0f));
		}
	}

	private IEnumerator Fade(Color to, float time, float delay)
	{
		Color from = (hasSpriteRenderer ? spriteRenderer.color : (hasTextRenderer ? textRenderer.color : (hasTk2dSprite ? tk2dSprite.color : Color.white)));
		if (delay > 0f)
		{
			yield return new WaitForSeconds(upDelay);
		}
		for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
		{
			Color color = Color.Lerp(from, to, elapsed / time) * initialColour;
			if (hasSpriteRenderer)
			{
				spriteRenderer.color = color;
			}
			else if (hasTextRenderer)
			{
				textRenderer.color = color;
			}
			else if (hasTk2dSprite)
			{
				tk2dSprite.color = color;
			}
			yield return null;
		}
		if (hasSpriteRenderer)
		{
			spriteRenderer.color = to * initialColour;
		}
		else if (hasTextRenderer)
		{
			textRenderer.color = to * initialColour;
		}
		else if (hasTk2dSprite)
		{
			tk2dSprite.color = to * initialColour;
		}
		if (this.OnFadeEnd != null)
		{
			this.OnFadeEnd(to == upColour);
		}
	}

	public void SetUpTime(float newUpTime)
	{
		upTime = newUpTime;
	}

	public void SetUpDelay(float newUpDelay)
	{
		upDelay = newUpDelay;
	}
}
