using System;
using System.Collections;


namespace CoreLib
{
    class Expression
    {
        private static bool isOperator(string op)
        {
            switch (op)
            {
                case "&&":       
                case "||":
                case "|":
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "==":
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                case "!":
                    return true;
            }
            return false;
        }

        private static float getPrecedence(string operators)
        {
            switch (operators)
            {
                case "(":
                case ")":
                    return 1;
                case "&&":
                    return 1.4f;
                case "||":
                    return 1.2f;
                case "|":
                    return 1.6f;
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "==":
                    return 1.8f;
                case "+":
                    return 2;
                case "-":
                    return 2;
                case "*":
                case "/":
                    return 4;
                case "^":
                    return 5;
                case "!":
                    return 6;
            }
            return 0;
        }

        private static bool isHigherOrEqual(string op1, string op2)
        {
            return getPrecedence(op1) >= getPrecedence(op2);
        }

        public static float calcuateExpression(string formulas)
        {
            // chia chuoi thanh cac 
            string[] elements = formulas.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("do dai " + elements.Length);
            foreach (string e in elements)
                Console.WriteLine(e);

            ArrayList opstack = new ArrayList();
            ArrayList postfixEx = new ArrayList();
            for (int i = 0; i < elements.Length; i++)
            {
                string e = elements[i];
                switch (e)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        while (opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]) && isHigherOrEqual((string)opstack[opstack.Count - 1], e))
                        {
                            postfixEx.Add(opstack[opstack.Count - 1]);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        opstack.Add(e);
                        break;
                    case "(":
                        opstack.Add(e);
                        break;
                    case ")":// lay ra tinh het phep tinh ra cho den khi gap (
                        while ((string)opstack[opstack.Count - 1] != "(" && opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]))
                        {
                            string op = (string)opstack[opstack.Count - 1];
                            postfixEx.Add(op);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        if (opstack.Count > 0)
                            opstack.RemoveAt(opstack.Count - 1);
                        break;
                    default:
                        float operand = Convert.ToSingle(e);
                        postfixEx.Add(operand);
                        break;
                }
            }

            while (opstack.Count > 0)
            {
                postfixEx.Add(opstack[opstack.Count - 1]);
                opstack.RemoveAt(opstack.Count - 1);
            }

            foreach (object e in postfixEx)
                Console.WriteLine(e);

            // tinh
            for (int i = 0; i < postfixEx.Count; i++)
            {
                if (postfixEx[i] is float)
                {
                    opstack.Add(postfixEx[i]);
                }
                else
                {

                    float y = (float)opstack[opstack.Count - 1];
                    opstack.RemoveAt(opstack.Count - 1);
                    float x = (float)opstack[opstack.Count - 1];
                    opstack.RemoveAt(opstack.Count - 1);

                    switch ((string)postfixEx[i])
                    {
                        case "+":
                            opstack.Add(x + y);
                            break;
                        case "-":
                            opstack.Add(x - y);
                            break;
                        case "*":
                            opstack.Add(x * y);
                            break;
                        case "/":
                            opstack.Add(x / y);
                            break;
                        default:
                            Console.WriteLine("toan tu khong biet " + postfixEx[i]);
                            break;
                    }

                }
            }

            return opstack.Count == 0 ? 0 : (float)opstack[0];
        }


        public static bool calcuateLogicExpression(string formulas)
        {
            // chia chuoi thanh cac 
            string[] elements = formulas.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("do dai " + elements.Length);
            foreach (string e in elements)
                Console.Write(e+" ");
            Console.WriteLine();
            ArrayList opstack = new ArrayList();
            ArrayList postfixEx = new ArrayList();
            for (int i = 0; i < elements.Length; i++)
            {
                string e = elements[i];
                switch (e)
                {
                    case "&&":
                    case "||":
                    case "|":
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                    case "==":
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "^":
                    case "!":
                        while (opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]) && isHigherOrEqual((string)opstack[opstack.Count - 1], e))
                        {
                            postfixEx.Add(opstack[opstack.Count - 1]);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        opstack.Add(e);
                        break;
                    case "(":
                        opstack.Add(e);
                        break;
                    case ")":// lay ra tinh het phep tinh ra cho den khi gap (
                        while ((string)opstack[opstack.Count - 1] != "(" && opstack.Count > 0 && isOperator((string)opstack[opstack.Count - 1]))
                        {
                            string op = (string)opstack[opstack.Count - 1];
                            postfixEx.Add(op);
                            opstack.RemoveAt(opstack.Count - 1);
                        }
                        if (opstack.Count > 0)
                            opstack.RemoveAt(opstack.Count - 1);
                        break;
                    default:
                        float operand = Convert.ToSingle(e);
                        postfixEx.Add(operand);
                        break;
                }
            }

            while (opstack.Count > 0)
            {
                postfixEx.Add(opstack[opstack.Count - 1]);
                opstack.RemoveAt(opstack.Count - 1);
            }

            foreach (object e in postfixEx)
                Console.Write(e+" ");
            Console.WriteLine();

            // tinh
            for (int i = 0; i < postfixEx.Count; i++)
            {
                if (postfixEx[i] is float || postfixEx[i] is bool)
                {
                    opstack.Add(postfixEx[i]);
                }
                else
                {
                    switch ((string)postfixEx[i])
                    {
                        case "&&":
                            bool yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            bool xB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(xB && yB);
                            break;
                        case "||":
                            yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            xB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(xB || yB);
                            break;
                        case "|":
                            yB = (bool)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(!yB);
                            break;
                        case "!":
                            float y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            float x  = 0;
                            //opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(factorial((int) y));
                            break;
                        case ">":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x > y);
                            break;
                        case "<":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x < y);
                            break;
                        case ">=":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x >= y);
                            break;
                        case "<=":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x <= y);
                            break;
                        case "==":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x == y);
                            break;

                        case "^":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add((float)Math.Pow(x, y));
                            break;

                        case "+":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x + y);
                            break;
                        case "-":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x - y);
                            break;
                        case "*":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x * y);
                            break;
                        case "/":
                            y = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            x = (float)opstack[opstack.Count - 1];
                            opstack.RemoveAt(opstack.Count - 1);
                            opstack.Add(x / y);
                            break;
                        default:
                            Console.WriteLine("toan tu khong biet " + postfixEx[i]);
                            break;
                    }

                    
                }
            }

            return opstack.Count == 0 ? false : (bool)opstack[0];
        }

        public static float factorial(int n) {
            float result = 1;
            for (int i = 1; i < n + 1; i++)
            {
                result *= i;
            }
            return result;
        }

    }
}
