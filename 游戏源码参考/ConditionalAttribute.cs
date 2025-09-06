using System;

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalAttribute : PropertyModifierAttribute
{
	private bool wasEnabled;

	public string TargetName { get; private set; }

	public bool IsMethod { get; private set; }

	public bool ExpectedResult { get; private set; }

	public bool HideCompletely { get; private set; }

	public ConditionalAttribute(string targetName, bool expectedResult, bool isMethod = true, bool hideCompletely = true)
	{
		TargetName = targetName;
		ExpectedResult = expectedResult;
		IsMethod = isMethod;
		HideCompletely = hideCompletely;
	}
}
