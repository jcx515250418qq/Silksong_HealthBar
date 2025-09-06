using UnityEngine;

public class HeroNailImbuementEffect : MonoBehaviour
{
	[SerializeField]
	private NailElements nailElement;

	[SerializeField]
	private GameObject[] activateObjects;

	private bool hasStarted;

	private HeroNailImbuement imbuementControl;

	private void Awake()
	{
		HeroController instance = HeroController.instance;
		imbuementControl = instance.GetComponent<HeroNailImbuement>();
	}

	private void OnEnable()
	{
		if (!hasStarted)
		{
			return;
		}
		bool active = imbuementControl.CurrentElement == nailElement;
		GameObject[] array = activateObjects;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(active);
			}
		}
	}

	private void Start()
	{
		hasStarted = true;
		OnEnable();
	}
}
