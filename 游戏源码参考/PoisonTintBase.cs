using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public abstract class PoisonTintBase : MonoBehaviour
{
	[SerializeField]
	private bool doRecolour;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("doRecolour", false, false, false)]
	private bool doHueShift;

	[SerializeField]
	[Range(-1f, 1f)]
	[ModifiableProperty]
	[Conditional("doRecolour", false, false, false)]
	private float hueShift;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("WillReadFromTool", true, false, false)]
	private ToolItem readFromTool;

	public UnityEvent OnIsPoison;

	public UnityEvent OnIsNotPoison;

	private Color defaultColor;

	private bool isStateOverridden;

	private bool isPoisonOverride;

	public static readonly int HueShiftPropId = Shader.PropertyToID("_HueShift");

	protected abstract Color Colour { get; set; }

	private bool WillReadFromTool
	{
		get
		{
			if (!doRecolour)
			{
				return !doHueShift;
			}
			return false;
		}
	}

	public ToolItem ReadFromTool
	{
		get
		{
			return readFromTool;
		}
		set
		{
			readFromTool = value;
			UpdatePoison();
		}
	}

	protected virtual void Awake()
	{
		defaultColor = Colour;
	}

	private void OnEnable()
	{
		UpdatePoison();
	}

	private void UpdatePoison()
	{
		if (isStateOverridden)
		{
			UpdatePoison(isPoisonOverride);
			return;
		}
		bool isEquipped = Gameplay.PoisonPouchTool.IsEquipped;
		UpdatePoison(isEquipped);
	}

	private void UpdatePoison(bool isPoison)
	{
		if (isPoison)
		{
			if (doRecolour)
			{
				EnableKeyword("RECOLOUR");
				DisableKeyword("CAN_HUESHIFT");
				Colour = Gameplay.PoisonPouchTintColour;
			}
			else if (doHueShift)
			{
				DisableKeyword("RECOLOUR");
				EnableKeyword("CAN_HUESHIFT");
				SetFloat(HueShiftPropId, hueShift);
				Colour = defaultColor;
			}
			else if ((bool)readFromTool)
			{
				if (readFromTool.UsePoisonTintRecolour)
				{
					EnableKeyword("RECOLOUR");
					DisableKeyword("CAN_HUESHIFT");
					Colour = Gameplay.PoisonPouchTintColour;
				}
				else
				{
					DisableKeyword("RECOLOUR");
					EnableKeyword("CAN_HUESHIFT");
					SetFloat(HueShiftPropId, readFromTool.PoisonHueShift);
					Colour = defaultColor;
				}
			}
			OnIsPoison.Invoke();
		}
		else
		{
			DisableKeyword("RECOLOUR");
			DisableKeyword("CAN_HUESHIFT");
			Colour = defaultColor;
			OnIsNotPoison.Invoke();
		}
	}

	public void Clear()
	{
		isStateOverridden = false;
		isPoisonOverride = false;
		UpdatePoison(isPoison: false);
	}

	public void SetPoisoned(bool isPoison)
	{
		isStateOverridden = true;
		isPoisonOverride = isPoison;
		UpdatePoison(isPoison);
	}

	protected abstract void EnableKeyword(string keyword);

	protected abstract void DisableKeyword(string keyword);

	protected abstract void SetFloat(int propId, float value);
}
