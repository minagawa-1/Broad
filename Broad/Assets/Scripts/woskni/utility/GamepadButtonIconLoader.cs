using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.LowLevel;

public class GamepadButtonIconLoader
{
    enum GamepadType
    {
        None,
        Joycon,
        DualShock,
        Xbox
    }

    static Dictionary<GamepadButton, string> m_JoyconImages = new Dictionary<GamepadButton, string>()
    {
        { GamepadButton.East        , "button_a" },
        { GamepadButton.South       , "button_b" },
        { GamepadButton.North       , "button_x" },
        { GamepadButton.West        , "button_y" },

        { GamepadButton.A           , "button_a" },
        { GamepadButton.B           , "button_b" },
        { GamepadButton.X           , "button_x" },
        { GamepadButton.Y           , "button_y" },

        { GamepadButton.DpadUp      , "dpad3_up"    },
        { GamepadButton.DpadDown    , "dpad3_down"  },
        { GamepadButton.DpadLeft    , "dpad3_left"  },
        { GamepadButton.DpadRight   , "dpad3_right" },

        { GamepadButton.Circle      , "dpad3_horizontal" },
        { GamepadButton.Cross       , "dpad3_vertical"   }
    };

    static Dictionary<GamepadButton, string> m_DualShockImages = new Dictionary<GamepadButton, string>()
    {
        { GamepadButton.East        , "button_circle"   },
        { GamepadButton.South       , "button_cross"    },
        { GamepadButton.North       , "button_triangle" },
        { GamepadButton.West        , "button_square"   },

        { GamepadButton.Circle      , "button_circle"   },
        { GamepadButton.Cross       , "button_cross"    },
        { GamepadButton.Triangle    , "button_triangle" },
        { GamepadButton.Square      , "button_square"   },

        { GamepadButton.DpadUp      , "dpad2_up"    },
        { GamepadButton.DpadDown    , "dpad2_down"  },
        { GamepadButton.DpadLeft    , "dpad2_left"  },
        { GamepadButton.DpadRight   , "dpad2_right" },

        { GamepadButton.A           , "dpad2_horizontal" },
        { GamepadButton.B           , "dpad2_vertical"   }
    };

    static Dictionary<GamepadButton, string> m_XboxImages = new Dictionary<GamepadButton, string>()
    {
        { GamepadButton.East        , "button_b" },
        { GamepadButton.South       , "button_a" },
        { GamepadButton.North       , "button_y" },
        { GamepadButton.West        , "button_x" },

        { GamepadButton.B           , "button_b" },
        { GamepadButton.A           , "button_a" },
        { GamepadButton.Y           , "button_y" },
        { GamepadButton.X           , "button_x" },

        { GamepadButton.DpadUp      , "dpad1_up"    },
        { GamepadButton.DpadDown    , "dpad1_down"  },
        { GamepadButton.DpadLeft    , "dpad1_left"  },
        { GamepadButton.DpadRight   , "dpad1_right" },

        { GamepadButton.Circle      , "dpad1_horizontal" },
        { GamepadButton.Cross       , "dpad1_vertical"   }
    };

    public static Texture2D Load(GamepadButton button)
    {
        string path = $"{Application.dataPath}/Textures/ControllerIcon/{GetButtonImageName(button)}.png";

        // 画像データを取得
        byte[] textureData = System.IO.File.ReadAllBytes(path);

        // データから画像を読み込む
        var texture = new Texture2D(0, 0);
        texture.LoadImage(textureData);

        // 読み込んだ画像を返す
        return texture;
    }

    /// <summary>ゲームパッドのボタンから画像名を取得</summary>
    /// <param name="button">ボタン</param>
    static string GetButtonImageName(GamepadButton button)
    {
        switch (GetGamepadType())
        {
            case GamepadType.None:      return m_JoyconImages[button];
            case GamepadType.Joycon:    return m_JoyconImages[button];
            case GamepadType.DualShock: return m_DualShockImages[button];
            case GamepadType.Xbox:      return m_XboxImages[button];
            default:                    return "";
        }
    }

    /// <summary>現在使用しているゲームパッドの種類を取得</summary>
    static GamepadType GetGamepadType()
    {
        Gamepad current = Gamepad.current;

        if      (current == null)                   return GamepadType.None;
        else if (current is SwitchProControllerHID) return GamepadType.Joycon;
        else if (current is DualShockGamepad)       return GamepadType.DualShock;
        else if (current is XInputController)       return GamepadType.Xbox;

        return GamepadType.None;
    }
}
