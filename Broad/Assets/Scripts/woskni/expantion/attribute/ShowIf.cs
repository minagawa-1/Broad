using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ShowIfAttribute : PropertyAttribute
{
    public string condition { get; }

    /// <summary>条件付き表示(条件がtrueのときのみフィールドを表示する)</summary>
    /// <param name="condition">条件</param>
    public ShowIfAttribute(string condition)
    {
        this.condition = condition;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ConditionalShowAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShowProperty(property)) EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShowProperty(property)) return 0;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private bool ShouldShowProperty(SerializedProperty property)
    {
        string condition = (attribute as ShowIfAttribute).condition;

        // 空白除去
        condition = condition.Replace(" ", "");
        condition = condition.Replace("　", "");

        // 演算子で分割
        // ( ) * / % + - == != >= > <= < ! && ||
        string pattern = @"(\(|\)|\*|/|%|\+|-|==|!=|>=|>|<=|<|!|&&|\|\|)";
        string[] tokens = System.Text.RegularExpressions.Regex.Split(condition, pattern);

        return Analyze.ParseTokens(property, tokens);
    }

    public class Analyze
    {
        public static bool ParseTokens(SerializedProperty property, string[] tokens)
        {
            Stack<object> operandStack = new Stack<object>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string token in tokens)
            {
                // 中身がない場合はcontinue
                if(string.IsNullOrEmpty(token) || token == " ") continue;

                // かっこ
                if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        string op = operatorStack.Pop();
                        ProcessOperator(property, op, operandStack);
                    }

                    operatorStack.Pop(); // "(" を取り除く
                }

                // 演算子
                else if (IsOperator(token))
                {
                    while (operatorStack.Count > 0 && IsOperator(operatorStack.Peek())
                        && GetPriority(token) <= GetPriority(operatorStack.Peek()))
                    {
                        string op = operatorStack.Pop();
                        ProcessOperator(property, op, operandStack);
                    }

                    operatorStack.Push(token);
                }

                // オペランド
                else
                {
                    if (float.TryParse(token, out float floatValue)) operandStack.Push(floatValue);
                    else if (int.TryParse(token, out int intValue))  operandStack.Push(intValue);
                    else
                    {
                        // フィールド名として解釈される場合
                        var p = property.serializedObject.FindProperty(token);

                        // フィールドが発見できない場合は、propertyのメンバアクセス指定子を探す
                        if (p == null && property.propertyPath.Contains("."))
                        {
                            string path = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.') + 1);

                            p = property.serializedObject.FindProperty(path + token);
                        }

                        // それでもフィールドが発見できない場合は、文字列として判定する
                        if (p == null) operandStack.Push(token);

                        else operandStack.Push(GetFieldValue(p));
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                ProcessOperator(property, op, operandStack);
            }

            return System.Convert.ToBoolean(operandStack.Pop());
        }

        static bool IsOperator(string token)
        {
            return token == "!"  ||
                   token == "+"  || token == "-"  || token == "*"  || token == "/"  || token == "%"  ||
                   token == "&&" || token == "||" ||
                   token == "==" || token == "!=" || token == ">"  || token == ">=" || token == "<"  || token == "<=";
        }

        /// <summary>演算子の優先順位 取得</summary>
        /// <param name="op">検索する演算子</param>
        static int GetPriority(string op)
        {
            switch (op)
            {
                case "!":   return 5;
                
                case "*":
                case "/":
                case "%":   return 4;

                case "+":
                case "-":   return 3;

                case "==":
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":  return 2;

                case "&&":  return 1;
                case "||":  return 0;

                default:    return -1;
            }
        }

        static void ProcessOperator(SerializedProperty property, string op, Stack<object> operandStack)
        {
        // 否定演算子
            if (op == "!")
            {
                bool operand = System.Convert.ToBoolean(operandStack.Pop());
                operandStack.Push(!operand);
            }

        // 加算・減算・乗算・除算・剰余演算子
            else if (op == "+")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                object result;

                // 文字列の場合は文字列結合処理・数値の場合は加算
                if (left is string || right is string) result = System.Convert.ToString(left) + System.Convert.ToString(right);
                else                                   result = System.Convert.ToDouble(left) + System.Convert.ToDouble(right);

                operandStack.Push(result);
            }
            else if (op == "-")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                operandStack.Push(System.Convert.ToDouble(left) - System.Convert.ToDouble(right));
            }
            else if (op == "*")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                operandStack.Push(System.Convert.ToDouble(left) * System.Convert.ToDouble(right));
            }
            else if (op == "/")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                operandStack.Push(System.Convert.ToDouble(left) / System.Convert.ToDouble(right));
            }
            else if (op == "%")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                operandStack.Push(System.Convert.ToDouble(left) % System.Convert.ToDouble(right));
            }

        // 比較演算子
            else if (op == "==")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();

                operandStack.Push(Compare(left, right) == 0);
            }
            else if (op == "!=")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();
                operandStack.Push(Compare(left, right) != 0);
            }
            else if (op == ">")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();
                operandStack.Push(Compare(left, right) > 0);
            }
            else if (op == ">=")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();
                operandStack.Push(Compare(left, right) >= 0);
            }
            else if (op == "<")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();
                operandStack.Push(Compare(left, right) < 0);
            }
            else if (op == "<=")
            {
                object right = operandStack.Pop();
                object left  = operandStack.Pop();
                operandStack.Push(Compare(left, right) <= 0);
            }

        // 論理積・論理和
            else if (op == "&&")
            {
                bool right = System.Convert.ToBoolean(operandStack.Pop());
                bool left  = System.Convert.ToBoolean(operandStack.Pop());
                operandStack.Push(left && right);
            }
            else if (op == "||")
            {
                bool right = System.Convert.ToBoolean(operandStack.Pop());
                bool left  = System.Convert.ToBoolean(operandStack.Pop());
                operandStack.Push(left || right);
            }

        // その他
            else
                throw new System.Exception("未定義の演算子: " + op);
        }

        static object GetFieldValue(SerializedProperty property)
        {
            // 定義されていないデータ型の場合は、メンバアクセスの親名と判断する
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:    return property.intValue;
                case SerializedPropertyType.Float:      return property.floatValue;
                case SerializedPropertyType.Boolean:    return property.boolValue;
                case SerializedPropertyType.String:     return property.stringValue;
                case SerializedPropertyType.Character:  return property.intValue;

                case SerializedPropertyType.Enum:       return property.enumValueIndex;
                case SerializedPropertyType.LayerMask:  return property.intValue;
                case SerializedPropertyType.ArraySize:  return property.arraySize;

                default:                                return property.propertyPath;
            }
        }

        static int Compare(object left, object right)
        {
            System.Type type1 = left.GetType();
            System.Type type2 = right.GetType();

            // string
            if (type1 == typeof(string) || type2 == typeof(string))
                return left.ToString() == right.ToString() ? 0 : -1;

            // char
            else if (type1 == typeof(char) || type2 == typeof(char))
                return left.ToString() == right.ToString() ? 0 : -1;

            // float
            else if (type1 == typeof(float) || type2 == typeof(float))
            {
                float value1 = System.Convert.ToSingle(left);
                float value2 = System.Convert.ToSingle(right);
                return value1.CompareTo(value2);
            }

            // int
            else if (type1 == typeof(int) || type2 == typeof(int))
            {
                int value1 = System.Convert.ToInt32(left);
                int value2 = System.Convert.ToInt32(right);
                return value1.CompareTo(value2);
            }

            // bool
            if (type1 == typeof(bool) || type2 == typeof(bool))
            return System.Convert.ToBoolean(left) == System.Convert.ToBoolean(right) ? 0 : -1;

            // その他のデータ型による比較
            else
                throw new System.InvalidOperationException($"対応していません: {type1}");
        }
    }
}
#endif