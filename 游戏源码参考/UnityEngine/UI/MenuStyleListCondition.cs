namespace UnityEngine.UI
{
	public class MenuStyleListCondition : MenuButtonListCondition
	{
		public override bool IsFulfilled()
		{
			MenuStyles instance = MenuStyles.Instance;
			if (!instance)
			{
				return false;
			}
			int num = 0;
			MenuStyles.MenuStyle[] styles = instance.Styles;
			for (int i = 0; i < styles.Length; i++)
			{
				if (styles[i].IsAvailable)
				{
					num++;
				}
				if (num > 1)
				{
					return true;
				}
			}
			return false;
		}
	}
}
