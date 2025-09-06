namespace UnityEngine.UI
{
	public class ControllerProfileButtons : MonoBehaviour
	{
		public Image profileHighlight1;

		public Image profileHighlight2;

		public Image profileHighlight3;

		public Image profileHighlight4;

		public void SelectItem(int num)
		{
			switch (num)
			{
			case 1:
				profileHighlight1.gameObject.SetActive(value: true);
				profileHighlight2.gameObject.SetActive(value: false);
				profileHighlight3.gameObject.SetActive(value: false);
				profileHighlight4.gameObject.SetActive(value: false);
				break;
			case 2:
				profileHighlight1.gameObject.SetActive(value: false);
				profileHighlight2.gameObject.SetActive(value: true);
				profileHighlight3.gameObject.SetActive(value: false);
				profileHighlight4.gameObject.SetActive(value: false);
				break;
			case 3:
				profileHighlight1.gameObject.SetActive(value: false);
				profileHighlight2.gameObject.SetActive(value: false);
				profileHighlight3.gameObject.SetActive(value: true);
				profileHighlight4.gameObject.SetActive(value: false);
				break;
			case 4:
				profileHighlight1.gameObject.SetActive(value: false);
				profileHighlight2.gameObject.SetActive(value: false);
				profileHighlight3.gameObject.SetActive(value: false);
				profileHighlight4.gameObject.SetActive(value: true);
				break;
			default:
				Debug.LogError("Invalid profile button ID");
				break;
			}
		}
	}
}
