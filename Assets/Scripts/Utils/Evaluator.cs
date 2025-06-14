using UnityEngine;
using System;
using System.Collections.Generic;

public class Evaluator
{
    public Dictionary<string, float> variables;

    public Evaluator(Dictionary<string, float> preset = null)
    {
        variables = preset ?? new Dictionary<string, float>();
    }

    public float Evaluate(string expression)
    {
        try
        {
            return EvaluateRpn(expression);
        }
        catch (Exception e)
        {
            try {
                return EvaluateInfix(expression);
            } catch {
                Debug.LogError($"Bad expression: {expression}");
                throw e;
            }
        }
    }

    public int EvaluateInt(string expression)
    {
        return (int)Evaluate(expression);
    }

    public float EvaluateRpn(string rpn)
    {
        Stack<float> stack = new();

        foreach (string token in rpn.Split(' '))
        {
            if (variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                if (stack.Count < 2)
                    throw new InvalidOperationException($"Not enough values on the stack for operator '{token}'");

                float b = stack.Pop();
                float a = stack.Pop();

                stack.Push(token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    "%" => a % b,
                    _ => throw new InvalidOperationException($"Unknown token '{token}'")
                });
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException($"RPN expression ended with {stack.Count} values on the stack");

        return stack.Pop();
    }

    private float EvaluateInfix(string infix)
    {
        // Convert infix to RPN using the Shunting Yard algorithm
        string[] output = ConvertToRpn(infix);
        // Evaluate the resulting RPN
        return EvaluateRpn(string.Join(' ', output));
    }

    private string[] ConvertToRpn(string infix)
    {
        List<string> output = new();
        Stack<string> ops = new();
        string[] tokens = infix.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, float> precedence = new()
        {
            ["+"] = 1,
            ["-"] = 1,
            ["*"] = 2,
            ["/"] = 2,
            ["%"] = 2
        };

        foreach (string token in tokens)
        {
            if (precedence.ContainsKey(token))
            {
                while (ops.Count > 0 &&
                       precedence.TryGetValue(ops.Peek(), out float p2) &&
                       p2 >= precedence[token])
                {
                    output.Add(ops.Pop());
                }
                ops.Push(token);
            }
            else if (token == "(")
            {
                ops.Push(token);
            }
            else if (token == ")")
            {
                while (ops.Count > 0 && ops.Peek() != "(")
                    output.Add(ops.Pop());

                if (ops.Count == 0 || ops.Pop() != "(")
                    throw new InvalidOperationException("Mismatched parentheses");
            }
            else
            {
                output.Add(token);
            }
        }

        while (ops.Count > 0)
        {
            string op = ops.Pop();
            if (op == "(" || op == ")")
                throw new InvalidOperationException("Mismatched parentheses");

            output.Add(op);
        }

        return output.ToArray();
    }
}
