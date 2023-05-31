using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ShowIfAttribute : PropertyAttribute
{
    public string condition { get; }

    /// <summary>�����t���\��(������true�̂Ƃ��̂݃t�B�[���h��\������)</summary>
    /// <param name="condition">����</param>
    public ShowIfAttribute(string condition)
    {
        this.condition = condition;
    }
}

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

        // �󔒏���
        condition = condition.Replace(" ", "");
        condition = condition.Replace("�@", "");

        // ���Z�q�ŕ���
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
                // ���g���Ȃ��ꍇ��continue
                if(string.IsNullOrEmpty(token) || token == " ") continue;

                // ������
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

                    operatorStack.Pop(); // "(" ����菜��
                }

                // ���Z�q
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

                // �I�y�����h
                else
                {
                    if (float.TryParse(token, out float floatValue)) operandStack.Push(floatValue);
                    else if (int.TryParse(token, out int intValue))  operandStack.Push(intValue);
                    else
                    {
                        // �t�B�[���h���Ƃ��ĉ��߂����ꍇ
                        var p = property.serializedObject.FindProperty(token);

                        // �t�B�[���h�������ł��Ȃ��ꍇ�́Aproperty�̃����o�A�N�Z�X�w��q��T��
                        if (p == null && property.propertyPath.Contains("."))
                        {
                            string path = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.') + 1);

                            p = property.serializedObject.FindProperty(path + token);
                        }

                        // ����ł��t�B�[���h�������ł��Ȃ��ꍇ�́A������Ƃ��Ĕ��肷��
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

        /// <summary>���Z�q�̗D�揇�� �擾</summary>
        /// <param name="op">�������鉉�Z�q</param>
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
        // �ے艉�Z�q
            if (op == "!")
            {
                bool operand = System.Convert.ToBoolean(operandStack.Pop());
                operandStack.Push(!operand);
            }

        // ���Z�E���Z�E��Z�E���Z�E��]���Z�q
            else if (op == "+")
            {
                object right = operandStack.Pop();
                object left = operandStack.Pop();
                object result;

                // ������̏ꍇ�͕����񌋍������E���l�̏ꍇ�͉��Z
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

        // ��r���Z�q
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

        // �_���ρE�_���a
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

        // ���̑�
            else
                throw new System.Exception("����`�̉��Z�q: " + op);
        }

        static object GetFieldValue(SerializedProperty property)
        {
            // ��`����Ă��Ȃ��f�[�^�^�̏ꍇ�́A�����o�A�N�Z�X�̐e���Ɣ��f����
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

            // ���̑��̃f�[�^�^�ɂ���r
            else
                throw new System.InvalidOperationException($"�Ή����Ă��܂���: {type1}");
        }
    }
}