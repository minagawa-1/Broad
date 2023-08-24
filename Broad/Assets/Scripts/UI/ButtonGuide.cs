using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/ButonGuide", 11)]
[ExecuteAlways]
public class ButtonGuide : Image
{
    [SerializeField] IconButton m_IconButton;

    GamepadType m_GamepadType;

    bool m_Highlight;

    Vector2 m_BasisSizeDelta;

    public IconButton iconButton
    {
        get { return m_IconButton; }
        set { m_IconButton = value; OnChangedIconButton(); }
    }

    public void OnChangedIconButton()
    {
        var tex = GamepadButtonIconLoader.Load(iconButton);
        sprite =  Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    }

    protected override void Awake()
    {
        m_GamepadType = GamepadType.None;
        m_Highlight = false;
        m_BasisSizeDelta = rectTransform.sizeDelta;
    }

    private void Update()
    {
        var gamepadType = GamepadButtonIconLoader.GetGamepadType();

        // ゲームパッド種別が異なる場合はアイコンを更新する
        if (m_GamepadType != gamepadType) {
            m_GamepadType = gamepadType;
            OnChangedIconButton();
        }

        // ボタンを検知して拡縮
        IconHIghlight();
    }

    void IconHIghlight()
    {
        bool press = IsPressedButton(m_IconButton);

        // ハイライト状態にする
        if (!m_Highlight && press)
        {
            m_Highlight = true;

            rectTransform.DOKill();
            rectTransform.DOSizeDelta(m_BasisSizeDelta * 1.2f, 0.1f).SetEase(Ease.OutCubic);
        }

        // 非ハイライト状態にする
        else if (m_Highlight && !press)
        {
            m_Highlight = false;

            rectTransform.DOKill();
            rectTransform.DOSizeDelta(m_BasisSizeDelta, 0.1f).SetEase(Ease.InCubic);
        }
    }

    bool IsPressedButton(IconButton iconButton)
    {
        var pad = Gamepad.current;

        if (pad == null) return false;

        switch (iconButton)
        {
            case IconButton.Dpad:            return pad.dpad.ReadValue() != Vector2.zero;
            case IconButton.DpadUp:          return pad.dpad.ReadValue().x > 0f;
            case IconButton.DpadDown:        return pad.dpad.ReadValue().x < 0f;
            case IconButton.DpadLeft:        return pad.dpad.ReadValue().y < 0f;
            case IconButton.DpadRight:       return pad.dpad.ReadValue().y > 0f;
            case IconButton.DpadVertical:    return pad.dpad.ReadValue().y != 0f;
            case IconButton.DpadHorizontal:  return pad.dpad.ReadValue().x != 0f;
            case IconButton.A:               return pad.buttonEast .isPressed;
            case IconButton.B:               return pad.buttonSouth.isPressed;
            case IconButton.X:               return pad.buttonNorth.isPressed;
            case IconButton.Y:               return pad.buttonWest .isPressed;
            case IconButton.LeftStick:       return pad.leftStick .ReadValue() != Vector2.zero;
            case IconButton.RightStick:      return pad.rightStick.ReadValue() != Vector2.zero;
            case IconButton.LeftStickPress:  return pad.leftStickButton .isPressed;
            case IconButton.RightStickPress: return pad.rightStickButton.isPressed;
            case IconButton.L:               return pad.leftTrigger  .isPressed;
            case IconButton.R:               return pad.rightTrigger .isPressed;
            case IconButton.ZL:              return pad.leftShoulder .isPressed;
            case IconButton.ZR:              return pad.rightShoulder.isPressed;
            case IconButton.Start:           return pad.startButton.IsPressed();
            case IconButton.Select:          return pad.selectButton.IsPressed(); ;
            case IconButton.Home:            return false;
            default: return false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonGuide), true)]
[CanEditMultipleObjects]
/// <summary>
///   Custom Editor for the Image Component.
///   Extend this class to write a custom editor for an Image-derived component.
/// </summary>
public class ImageEditor : GraphicEditor
{
    SerializedProperty m_IconButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_IconButton = serializedObject.FindProperty(nameof(m_IconButton));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_IconButton);
        EditorGUILayout.PropertyField(m_Color);
        EditorGUILayout.PropertyField(m_Material);
        EditorGUILayout.PropertyField(m_RaycastTarget);
        EditorGUILayout.PropertyField(m_RaycastPadding);
        EditorGUILayout.PropertyField(m_Maskable);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            ButtonGuide script = target as ButtonGuide;
            if (script != null)
            {
                script.OnChangedIconButton();
            }
        }
    }
}
#endif