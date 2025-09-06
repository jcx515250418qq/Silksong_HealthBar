using UnityEngine;

public class SetZPerHeroSide : MonoBehaviour
{
	[SerializeField]
	private Vector2 origin;

	[SerializeField]
	private bool checkScale;

	[Space]
	[SerializeField]
	private float heroLeftZ;

	[SerializeField]
	private float heroRightZ;

	private HeroController hc;

	private bool wasHeroRight;

	public float ExpectedZ { get; private set; }

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(origin, 0.2f);
	}

	private void Start()
	{
		hc = HeroController.instance;
		wasHeroRight = true;
		SetZ(isRight: true);
	}

	private void LateUpdate()
	{
		Vector2 vector = base.transform.TransformPoint(origin);
		bool flag = hc.transform.position.x > vector.x;
		if (checkScale && base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		if (flag != wasHeroRight)
		{
			wasHeroRight = flag;
			SetZ(flag);
		}
	}

	private void SetZ(bool isRight)
	{
		ExpectedZ = (isRight ? heroRightZ : heroLeftZ);
		base.transform.SetLocalPositionZ(ExpectedZ);
	}
}
