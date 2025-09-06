using System;
using System.Reflection;
using TeamCherry.SharedUtils;
using UnityEngine;

[Serializable]
public class PlayerDataTest
{
	public enum TestType
	{
		Bool = 0,
		Int = 1,
		Float = 2,
		Enum = 3,
		String = 4
	}

	public enum NumTestType
	{
		Equal = 0,
		NotEqual = 1,
		LessThan = 2,
		MoreThan = 3
	}

	public enum StringTestType
	{
		Equal = 0,
		NotEqual = 1,
		Contains = 2,
		NotContains = 3
	}

	[Serializable]
	public struct Test
	{
		public TestType Type;

		public string FieldName;

		public bool BoolValue;

		public NumTestType NumType;

		public int IntValue;

		public float FloatValue;

		public string StringValue;

		public StringTestType StringType;

		public bool IsFulfilled(PlayerDataBase playerData)
		{
			float? num = null;
			float num2 = 0f;
			switch (Type)
			{
			case TestType.Bool:
				return playerData.GetVariable<bool>(FieldName) == BoolValue;
			case TestType.Int:
				num = playerData.GetVariable<int>(FieldName);
				num2 = IntValue;
				break;
			case TestType.Float:
				num = playerData.GetVariable<float>(FieldName);
				num2 = FloatValue;
				break;
			case TestType.Enum:
			{
				FieldInfo field = playerData.GetType().GetField(FieldName);
				if (field.FieldType.IsEnum)
				{
					num = (int)field.GetValue(playerData);
				}
				else
				{
					Debug.LogErrorFormat("Could not test field {0} is it was not an enum type", FieldName);
				}
				num2 = IntValue;
				break;
			}
			case TestType.String:
			{
				string text = playerData.GetVariable<string>(FieldName) ?? string.Empty;
				switch (StringType)
				{
				case StringTestType.Equal:
					return text.Equals(StringValue);
				case StringTestType.NotEqual:
					return !text.Equals(StringValue);
				case StringTestType.Contains:
					return text.Contains(StringValue);
				case StringTestType.NotContains:
					return !text.Contains(StringValue);
				}
				break;
			}
			}
			if (!num.HasValue)
			{
				return false;
			}
			return NumType switch
			{
				NumTestType.Equal => num == num2, 
				NumTestType.NotEqual => num != num2, 
				NumTestType.LessThan => num < num2, 
				NumTestType.MoreThan => num > num2, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	[Serializable]
	public struct TestGroup
	{
		[Tooltip("All tests in this group must be passed for the test group to pass (only 1 test group needs to pass for entire test to pass, however).")]
		public Test[] Tests;

		public bool IsFulfilled(PlayerDataBase playerData)
		{
			Test[] tests = Tests;
			foreach (Test test in tests)
			{
				if (!test.IsFulfilled(playerData))
				{
					return false;
				}
			}
			return true;
		}
	}

	private readonly PlayerDataBase playerDataOverride;

	[Tooltip("If any single test group succeeds, test will be passed.")]
	public TestGroup[] TestGroups;

	public bool IsFulfilled
	{
		get
		{
			if (TestGroups.Length == 0)
			{
				return true;
			}
			TestGroup[] testGroups = TestGroups;
			foreach (TestGroup testGroup in testGroups)
			{
				if (testGroup.IsFulfilled(playerDataOverride ?? GameManager.instance.playerData))
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsDefined
	{
		get
		{
			TestGroup[] testGroups = TestGroups;
			for (int i = 0; i < testGroups.Length; i++)
			{
				if (testGroups[i].Tests.Length != 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public PlayerDataTest()
	{
		playerDataOverride = null;
		TestGroups = Array.Empty<TestGroup>();
	}

	public PlayerDataTest(PlayerDataBase playerDataOverride)
	{
		this.playerDataOverride = playerDataOverride;
		TestGroups = Array.Empty<TestGroup>();
	}

	public PlayerDataTest(string fieldName, bool value)
	{
		playerDataOverride = null;
		TestGroups = new TestGroup[1]
		{
			new TestGroup
			{
				Tests = new Test[1]
				{
					new Test
					{
						FieldName = fieldName,
						Type = TestType.Bool,
						BoolValue = value
					}
				}
			}
		};
	}
}
