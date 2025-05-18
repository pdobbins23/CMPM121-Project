using UnityEngine;
using System.Collections.Generic;
using System;

public class RPN {
    public static int EvalInt(string exp, Dictionary<string, float> vars)
    {
        return Mathf.RoundToInt(eval(exp, vars));
    }

    public static float eval(string exp, Dictionary<string, float> vars)
    {
        Stack<float> nums = new Stack<float>();
        string[] tokens = exp.Split(' ');
        foreach (var token in tokens)
        {
            if (float.TryParse(token, out float num))
            {
                nums.Push(num);
            }
            else if (vars.ContainsKey(token))
            {
                nums.Push(vars[token]);
            }
            else
            {
                float b = nums.Pop();
                float a = nums.Pop();
                switch (token)
                {
                    case "+": nums.Push(a + b); break;
                    case "-": nums.Push(a - b); break;
                    case "*": nums.Push(a * b); break;
                    case "/": nums.Push(a / b); break;
                    case "%": nums.Push(a % b); break;
                    default:
                        Debug.Log("RPN Parse Error: " + token);
                        return 0;
                }
            }
        }

        return nums.Pop();
    }
}
