﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StringCalculatorNS
{

    public class StringCalculator
    {
        readonly char[] Separators = new char[] { '+', '-', '/', '*' };
        readonly Dictionary<char, int> Prior = new Dictionary<char, int>()
        {
            {'+',6 },
            {'-',6 },
            {'/',5 },
            {'*',5 },
        };

        NumberFormatInfo nFI = new NumberFormatInfo()
        {
            NumberDecimalSeparator = ","
        };

        public double Calculate(string str)
        {
            return Calc(SplitByOp(str.Replace(" ", "").Replace(".", ",")));
        }

        private double Calc(List<string> res)
        {
            double num1, num2;
            if (res.Count == 1)
            {
                if (double.TryParse(res[0].ToString(), NumberStyles.Float | NumberStyles.AllowThousands, nFI, out num1))
                {
                    return num1;
                }
                else
                {
                    if (res[0][0] == '-' && res[0][1] == '(')
                    {
                        return -Calc(SplitByOp(res[0].ToString().Substring(1, res[0].ToString().Length - 1)));
                    }
                    if (res[0][0] == '(')
                    {
                        return Calc(SplitByOp(res[0].ToString().Substring(1, res[0].ToString().Length - 2)));
                    }
                    else
                    {
                        Console.WriteLine("InvalidInput");

                    }
                }
            }
            if (res.Count == 3)
            {
                if (!(double.TryParse(res[0].ToString(), NumberStyles.Float | NumberStyles.AllowThousands, nFI, out num1)))
                {
                    num1 = Calc(SplitByOp(res[0].ToString()));
                }
                if (!(double.TryParse(res[2].ToString(), NumberStyles.Float | NumberStyles.AllowThousands, nFI, out num2)))
                {
                    num2 = Calc(SplitByOp(res[2].ToString()));
                }
                return DoOp(num1, num2, res[1][0]);
            }
            int indMaxPrior = 1, MaxPrior = 0;
            for (int i = 1; i < res.Count; i += 2)
            {
                if (Prior[res[i][0]] >= MaxPrior)
                {
                    indMaxPrior = i;
                    MaxPrior = Prior[res[i][0]];
                }
            }
            num1 = Calc(res.GetRange(0, indMaxPrior));
            num2 = Calc(res.GetRange(indMaxPrior + 1, res.Count() - indMaxPrior - 1));
            return DoOp(num1, num2, res[indMaxPrior][0]);
        }

        private double DoOp(double num1, double num2, char op)
        {
            switch (op)
            {
                case '+': return num1 + num2;
                case '-': return num1 - num2;
                case '*': return num1 * num2;
                case '/':
                    if (num2 != 0)
                    {
                        return num1 / num2;
                    }
                    Console.WriteLine("DivByZero");
                    return 1;
                default:

                    Console.WriteLine("InvalidOp");
                    return 1;
            }
        }

        private List<string> SplitByOp(string str)
        {
            if (str.Length == 0)
            {
                Console.WriteLine("EmptyStr");
            }
            if (Separators.Contains(str[0]) && !(str[0] == '-'))
            {
                Console.WriteLine("NoFirstOp");
            }
            if (Separators.Contains(str[str.Length - 1]))
            {
                Console.WriteLine("NoSecondOp");
            }

            List<string> res = new List<string> { };
            int j = 0, brCounter = str[0] == '(' ? 1 : 0;

            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] == '(') brCounter++;

                if (brCounter == 0 && Separators.Contains(str[i]) && !(i == j && str[i] == '-' && str[i - 1] != ')'))
                {
                    if (i == j)
                    {
                        Console.WriteLine("DoubleOp");
                    }
                    res.Add(str.Substring(j, i - j));
                    res.Add(str[i].ToString());
                    j = i + 1;
                }

                if (str[i] == ')') brCounter--;
            }
            res.Add(str.Substring(j, str.Length - j).ToString());
            if (brCounter != 0)
            {
                Console.WriteLine("BracketCountErr");
            }
            return res;
        }
    }
}