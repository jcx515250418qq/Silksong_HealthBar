using UnityEngine;

public class AbyssWaterTendrils : MonoBehaviour
{
	[SerializeField]
	private float appearRadius;

	[SerializeField]
	private float flowerDisappearRadius;

	[SerializeField]
	private Animator dropAnimator;

	private HeroController hc;

	private Vector3 worldPos;

	private bool wasAppeared;

	private static Vector3 _heroPos;

	private static bool _hasWhiteFlower;

	private static int _heroPosFrame;

	private static readonly int _shouldAppearId = Animator.StringToHash("ShouldAppear");

	private float appearRadiusSqr;

	private float flowerDisappearRadiusSqr;

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Gizmos.color = Color.grey;
		Gizmos.DrawWireSphere(position, appearRadius);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(position, flowerDisappearRadius);
	}

	private void Start()
	{
		hc = HeroController.instance;
		worldPos = base.transform.position;
		appearRadiusSqr = appearRadius * appearRadius;
		flowerDisappearRadiusSqr = flowerDisappearRadius * flowerDisappearRadius;
	}

	private void OnValidate()
	{
		appearRadiusSqr = appearRadius * appearRadius;
		flowerDisappearRadiusSqr = flowerDisappearRadius * flowerDisappearRadius;
	}

	private void Update()
	{
		if (Time.frameCount != _heroPosFrame)
		{
			_heroPos = hc.transform.position;
			_hasWhiteFlower = hc.playerData.HasWhiteFlower;
			_heroPosFrame = Time.frameCount;
		}
		float num = Vector2.SqrMagnitude(worldPos - _heroPos);
		bool flag = !(num > appearRadiusSqr) && ((!_hasWhiteFlower || !(num < flowerDisappearRadiusSqr)) ? true : false);
		dropAnimator.SetBool(_shouldAppearId, flag);
		if (flag && !wasAppeared && dropAnimator.cullingMode == AnimatorCullingMode.CullCompletely)
		{
			dropAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			if (Random.Range(0, 2) == 0)
			{
				base.transform.FlipLocalScale(x: true);
			}
		}
		wasAppeared = flag;
	}
}
