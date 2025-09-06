using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mathos.Parser
{
	public class MathParser
	{
		private readonly List<string> roughExpr = new List<string>();

		private readonly List<double> args = new List<double>();

		public List<string> OperatorList { get; set; }

		public Dictionary<string, Func<double, double, double>> OperatorAction { get; set; }

		public Dictionary<string, Func<double[], double>> LocalFunctions { get; set; }

		public Dictionary<string, double> LocalVariables { get; set; }

		public CultureInfo CultureInfo { get; set; }

		public MathParser(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true, bool loadPreDefinedVariables = true, CultureInfo cultureInfo = null)
		{
			if (loadPreDefinedOperators)
			{
				OperatorList = new List<string>(10) { "^", "%", ":", "/", "*", "-", "+", ">", "<", "=" };
				OperatorAction = new Dictionary<string, Func<double, double, double>>(10);
				OperatorAction["^"] = Math.Pow;
				OperatorAction["%"] = (double a, double b) => a % b;
				OperatorAction[":"] = (double a, double b) => a / b;
				OperatorAction["/"] = (double a, double b) => a / b;
				OperatorAction["*"] = (double a, double b) => a * b;
				OperatorAction["-"] = (double a, double b) => a - b;
				OperatorAction["+"] = (double a, double b) => a + b;
				OperatorAction[">"] = (double a, double b) => (a > b) ? 1 : 0;
				OperatorAction["<"] = (double a, double b) => (a < b) ? 1 : 0;
				OperatorAction["="] = (double a, double b) => (Math.Abs(a - b) < 1E-08) ? 1 : 0;
			}
			else
			{
				OperatorList = new List<string>();
				OperatorAction = new Dictionary<string, Func<double, double, double>>();
			}
			if (loadPreDefinedFunctions)
			{
				LocalFunctions = new Dictionary<string, Func<double[], double>>(26);
				LocalFunctions["abs"] = (double[] inputs) => Math.Abs(inputs[0]);
				LocalFunctions["cos"] = (double[] inputs) => Math.Cos(inputs[0]);
				LocalFunctions["cosh"] = (double[] inputs) => Math.Cosh(inputs[0]);
				LocalFunctions["acos"] = (double[] inputs) => Math.Acos(inputs[0]);
				LocalFunctions["arccos"] = (double[] inputs) => Math.Acos(inputs[0]);
				LocalFunctions["sin"] = (double[] inputs) => Math.Sin(inputs[0]);
				LocalFunctions["sinh"] = (double[] inputs) => Math.Sinh(inputs[0]);
				LocalFunctions["asin"] = (double[] inputs) => Math.Asin(inputs[0]);
				LocalFunctions["arcsin"] = (double[] inputs) => Math.Asin(inputs[0]);
				LocalFunctions["tan"] = (double[] inputs) => Math.Tan(inputs[0]);
				LocalFunctions["tanh"] = (double[] inputs) => Math.Tanh(inputs[0]);
				LocalFunctions["atan"] = (double[] inputs) => Math.Atan(inputs[0]);
				LocalFunctions["arctan"] = (double[] inputs) => Math.Atan(inputs[0]);
				LocalFunctions["sqrt"] = (double[] inputs) => Math.Sqrt(inputs[0]);
				LocalFunctions["pow"] = (double[] inputs) => Math.Pow(inputs[0], inputs[1]);
				LocalFunctions["root"] = (double[] inputs) => Math.Pow(inputs[0], 1.0 / inputs[1]);
				LocalFunctions["rem"] = (double[] inputs) => Math.IEEERemainder(inputs[0], inputs[1]);
				LocalFunctions["sign"] = (double[] inputs) => Math.Sign(inputs[0]);
				LocalFunctions["exp"] = (double[] inputs) => Math.Exp(inputs[0]);
				LocalFunctions["floor"] = (double[] inputs) => Math.Floor(inputs[0]);
				LocalFunctions["ceil"] = (double[] inputs) => Math.Ceiling(inputs[0]);
				LocalFunctions["ceiling"] = (double[] inputs) => Math.Ceiling(inputs[0]);
				LocalFunctions["round"] = (double[] inputs) => Math.Round(inputs[0]);
				LocalFunctions["truncate"] = (double[] inputs) => (!(inputs[0] < 0.0)) ? Math.Floor(inputs[0]) : (0.0 - Math.Floor(0.0 - inputs[0]));
				LocalFunctions["log"] = (double[] inputs) => inputs.Length switch
				{
					1 => Math.Log10(inputs[0]), 
					2 => Math.Log(inputs[0], inputs[1]), 
					_ => 0.0, 
				};
				LocalFunctions["ln"] = (double[] inputs) => Math.Log(inputs[0]);
			}
			else
			{
				LocalFunctions = new Dictionary<string, Func<double[], double>>();
			}
			if (loadPreDefinedVariables)
			{
				LocalVariables = new Dictionary<string, double>(8);
				LocalVariables["pi"] = 3.14159265358979;
				LocalVariables["tao"] = 6.28318530717959;
				LocalVariables["e"] = 2.71828182845905;
				LocalVariables["phi"] = 1.61803398874989;
				LocalVariables["major"] = 0.61803398874989;
				LocalVariables["minor"] = 0.38196601125011;
				LocalVariables["pitograd"] = 57.2957795130823;
				LocalVariables["piofgrad"] = 0.01745329251994;
			}
			else
			{
				LocalVariables = new Dictionary<string, double>();
			}
			CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
		}

		public double Parse(string mathExpression)
		{
			return MathParserLogic(Lexer(mathExpression));
		}

		public double Parse(ReadOnlyCollection<string> mathExpression)
		{
			return MathParserLogic(new List<string>(mathExpression));
		}

		public double ProgrammaticallyParse(string mathExpression, bool correctExpression = true, bool identifyComments = true)
		{
			if (identifyComments)
			{
				mathExpression = Regex.Replace(mathExpression, "#\\{.*?\\}#", "");
				mathExpression = Regex.Replace(mathExpression, "#.*$", "");
			}
			if (correctExpression)
			{
				mathExpression = Correction(mathExpression);
			}
			double num;
			string text;
			if (mathExpression.Contains("let"))
			{
				if (mathExpression.Contains("be"))
				{
					text = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3, mathExpression.IndexOf("be", StringComparison.Ordinal) - mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
					mathExpression = mathExpression.Replace(text + "be", "");
				}
				else
				{
					text = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3, mathExpression.IndexOf("=", StringComparison.Ordinal) - mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
					mathExpression = mathExpression.Replace(text + "=", "");
				}
				text = text.Replace(" ", "");
				mathExpression = mathExpression.Replace("let", "");
				num = Parse(mathExpression);
				if (LocalVariables.ContainsKey(text))
				{
					LocalVariables[text] = num;
				}
				else
				{
					LocalVariables.Add(text, num);
				}
				return num;
			}
			if (!mathExpression.Contains(":="))
			{
				return Parse(mathExpression);
			}
			text = mathExpression.Substring(0, mathExpression.IndexOf(":=", StringComparison.Ordinal));
			mathExpression = mathExpression.Replace(text + ":=", "");
			num = Parse(mathExpression);
			text = text.Replace(" ", "");
			if (LocalVariables.ContainsKey(text))
			{
				LocalVariables[text] = num;
			}
			else
			{
				LocalVariables.Add(text, num);
			}
			return num;
		}

		public ReadOnlyCollection<string> GetTokens(string mathExpression)
		{
			return Lexer(mathExpression).AsReadOnly();
		}

		private string Correction(string input)
		{
			input = Regex.Replace(input, "\\b(sqr|sqrt)\\b", "sqrt", RegexOptions.IgnoreCase);
			input = Regex.Replace(input, "\\b(atan2|arctan2)\\b", "arctan2", RegexOptions.IgnoreCase);
			return input;
		}

		private List<string> Lexer(string expr)
		{
			string text = "";
			List<string> list = new List<string>();
			expr = expr.Replace("+-", "-");
			expr = expr.Replace("-+", "-");
			expr = expr.Replace("--", "+");
			for (int i = 0; i < expr.Length; i++)
			{
				char c = expr[i];
				if (char.IsWhiteSpace(c))
				{
					continue;
				}
				if (char.IsLetter(c))
				{
					if (i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
					{
						list.Add("*");
					}
					text += c;
					while (i + 1 < expr.Length && (char.IsLetterOrDigit(expr[i + 1]) || expr[i + 1] == '.'))
					{
						text += expr[++i];
					}
					list.Add(text);
					text = "";
				}
				else if (char.IsDigit(c))
				{
					text += c;
					while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
					{
						text += expr[++i];
					}
					list.Add(text);
					text = "";
				}
				else if (i + 1 < expr.Length && (c == '-' || c == '+') && char.IsDigit(expr[i + 1]) && (i == 0 || OperatorList.IndexOf(expr[i - 1].ToString(CultureInfo)) != -1 || (i - 1 > 0 && expr[i - 1] == '(')))
				{
					text += c;
					while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
					{
						text += expr[++i];
					}
					list.Add(text);
					text = "";
				}
				else if (c == '(')
				{
					if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
					{
						list.Add("*");
						list.Add("(");
					}
					else
					{
						list.Add("(");
					}
				}
				else
				{
					list.Add(c.ToString());
				}
			}
			return list;
		}

		private double MathParserLogic(List<string> tokens)
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				if (LocalVariables.Keys.Contains(tokens[i]))
				{
					tokens[i] = LocalVariables[tokens[i]].ToString(CultureInfo);
				}
			}
			while (tokens.IndexOf("(") != -1)
			{
				int num = tokens.LastIndexOf("(");
				int num2 = tokens.IndexOf(")", num);
				if (num >= num2)
				{
					throw new ArithmeticException("No closing bracket/parenthesis. Token: " + num.ToString(CultureInfo));
				}
				roughExpr.Clear();
				for (int j = num + 1; j < num2; j++)
				{
					roughExpr.Add(tokens[j]);
				}
				args.Clear();
				string text = tokens[(num != 0) ? (num - 1) : 0];
				double num4;
				if (LocalFunctions.Keys.Contains(text))
				{
					if (roughExpr.Contains(","))
					{
						for (int k = 0; k < roughExpr.Count; k++)
						{
							List<string> list = new List<string>();
							int num3 = ((roughExpr.IndexOf(",", k) != -1) ? roughExpr.IndexOf(",", k) : roughExpr.Count);
							while (k < num3)
							{
								list.Add(roughExpr[k++]);
							}
							args.Add((list.Count == 0) ? 0.0 : BasicArithmeticalExpression(list));
						}
						num4 = double.Parse(LocalFunctions[text](args.ToArray()).ToString(CultureInfo), CultureInfo);
					}
					else
					{
						num4 = double.Parse(LocalFunctions[text](new double[1] { BasicArithmeticalExpression(roughExpr) }).ToString(CultureInfo), CultureInfo);
					}
				}
				else
				{
					num4 = BasicArithmeticalExpression(roughExpr);
				}
				tokens[num] = num4.ToString(CultureInfo);
				tokens.RemoveRange(num + 1, num2 - num);
				if (LocalFunctions.Keys.Contains(text))
				{
					tokens.RemoveAt(num - 1);
				}
			}
			return BasicArithmeticalExpression(tokens);
		}

		private double BasicArithmeticalExpression(List<string> tokens)
		{
			switch (tokens.Count)
			{
			case 1:
				return double.Parse(tokens[0], CultureInfo);
			case 2:
			{
				string text = tokens[0];
				if (text == "-" || text == "+")
				{
					return double.Parse(((text == "+") ? "" : ((tokens[1].Substring(0, 1) == "-") ? "" : "-")) + tokens[1], CultureInfo);
				}
				return OperatorAction[text](0.0, double.Parse(tokens[1], CultureInfo));
			}
			case 0:
				return 0.0;
			default:
				foreach (string @operator in OperatorList)
				{
					while (tokens.IndexOf(@operator) != -1)
					{
						int num = tokens.IndexOf(@operator);
						double arg = double.Parse(tokens[num - 1], CultureInfo);
						double arg2 = double.Parse(tokens[num + 1], CultureInfo);
						tokens[num - 1] = OperatorAction[@operator](arg, arg2).ToString(CultureInfo);
						tokens.RemoveRange(num, 2);
					}
				}
				return double.Parse(tokens[0], CultureInfo);
			}
		}
	}
}
