using System.Collections;
using UnityEngine;

public class SpriteTweenColorNeutral : MonoBehaviour
{
	private Color Color = new Color(1f, 1f, 1f, 1f);

	private float Duration = 0.25f;

	private void ColorReturnNeutral()
	{
		tk2dSprite component = GetComponent<tk2dSprite>();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("from", component.color);
		hashtable.Add("to", Color);
		hashtable.Add("time", Duration);
		hashtable.Add("OnUpdate", "updateSpriteColor");
		hashtable.Add("looptype", iTween.LoopType.none);
		hashtable.Add("easetype", iTween.EaseType.linear);
		iTween.ValueTo(base.gameObject, hashtable);
	}

	private void updateSpriteColor(Color color)
	{
		GetComponent<tk2dSprite>().color = color;
	}

	private void onEnable()
	{
	}
}
