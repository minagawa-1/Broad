using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

[System.Serializable]
public enum IconButton
{
    Dpad,
    DpadUp,
    DpadDown,
    DpadLeft,
    DpadRight,
    DpadVertical,
    DpadHorizontal,
    A,
    B,
    X,
    Y,
    LeftStick,
    LeftStickPress,
    RightStick,
    RightStickPress,
    L,
    R,
    ZL,
    ZR,
    Start,
    Select,
    Home,
    Console,
    Controller
}

public enum GamepadType
{
    None,
    Joycon,
    DualShock,
    Xbox
}

public class GamepadButtonIconLoader
{
    static Dictionary<IconButton, string> m_JoyconImages = new Dictionary<IconButton, string>()
    {
        { IconButton.Dpad           , "dpad3"            },
        { IconButton.DpadUp         , "dpad3_up"         },
        { IconButton.DpadDown       , "dpad3_down"       },
        { IconButton.DpadLeft       , "dpad3_left"       },
        { IconButton.DpadRight      , "dpad3_right"      },
        { IconButton.DpadVertical   , "dpad3_vertical"   },
        { IconButton.DpadHorizontal , "dpad3_horizontal" },

        { IconButton.A              , "button_a" },
        { IconButton.B              , "button_b" },
        { IconButton.X              , "button_x" },
        { IconButton.Y              , "button_y" },

        { IconButton.LeftStick      , "joystick3_left"        },
        { IconButton.LeftStickPress , "joystick3_left_press"  },
        { IconButton.RightStick     , "joystick3_right"       },
        { IconButton.RightStickPress, "joystick3_right_press" },

        { IconButton.L              , "bumper2_left"    },
        { IconButton.R              , "bumper2_right"   },
        { IconButton.ZL             , "trigger2_left"   },
        { IconButton.ZR             , "trigger2_right"  },

        { IconButton.Start          , "button_plus"     },
        { IconButton.Select         , "button_minus"    },
        { IconButton.Home           , "button_home"         },
        { IconButton.Console        , "console_switch"      },
        { IconButton.Controller     , "controller_switch"   }
    };

    static Dictionary<IconButton, string> m_DualShockImages = new Dictionary<IconButton, string>()
    {
        { IconButton.Dpad           , "dpad2"            },
        { IconButton.DpadUp         , "dpad2_up"         },
        { IconButton.DpadDown       , "dpad2_down"       },
        { IconButton.DpadLeft       , "dpad2_left"       },
        { IconButton.DpadRight      , "dpad2_right"      },
        { IconButton.DpadVertical   , "dpad2_vertical"   },
        { IconButton.DpadHorizontal , "dpad2_horizontal" },

        { IconButton.A              , "button_circle"   },
        { IconButton.B              , "button_cross"    },
        { IconButton.X              , "button_triangle" },
        { IconButton.Y              , "button_square"   },

        { IconButton.LeftStick      , "joystick2_left"        },
        { IconButton.LeftStickPress , "joystick2_left_press"  },
        { IconButton.RightStick     , "joystick2_right"       },
        { IconButton.RightStickPress, "joystick2_right_press" },

        { IconButton.L              , "bumper1_l1"      },
        { IconButton.R              , "bumper1_r1"      },
        { IconButton.ZL             , "trigger1_l2"     },
        { IconButton.ZR             , "trigger1_r2"     },

        { IconButton.Start          , "button_options"  },
        { IconButton.Select         , "button_share"    },
        { IconButton.Home           , "button_ps"       },
        { IconButton.Console        , "console_ps4"     },
        { IconButton.Controller     , "controller_ps4"  }
    };

    static Dictionary<IconButton, string> m_XboxImages = new Dictionary<IconButton, string>()
    {
        { IconButton.Dpad           , "dpad1"            },
        { IconButton.DpadUp         , "dpad1_up"         },
        { IconButton.DpadDown       , "dpad1_down"       },
        { IconButton.DpadLeft       , "dpad1_left"       },
        { IconButton.DpadRight      , "dpad1_right"      },
        { IconButton.DpadVertical   , "dpad1_vertical"   },
        { IconButton.DpadHorizontal , "dpad1_horizontal" },

        { IconButton.A              , "button_b" },
        { IconButton.B              , "button_a" },
        { IconButton.X              , "button_y" },
        { IconButton.Y              , "button_x" },

        { IconButton.LeftStick      , "joystick1_left"        },
        { IconButton.LeftStickPress , "joystick1_left_press"  },
        { IconButton.RightStick     , "joystick1_right"       },
        { IconButton.RightStickPress, "joystick1_right_press" },

        { IconButton.L              , "lb"  },
        { IconButton.R              , "rb"  },
        { IconButton.ZL             , "lt"  },
        { IconButton.ZR             , "rt"  },

        { IconButton.Start          , "button_menu"     },
        { IconButton.Select         , "button_view"     },
        { IconButton.Home           , "xbox"            },
        { IconButton.Console        , "console_xbox"    },
        { IconButton.Controller     , "controller_xbox" }
    };

    /// <summary>ボタンの種類から画像を読み込み</summary>
    /// <param name="button">ボタンの種類</param>
    public static Texture2D Load(IconButton button) => Resources.Load<Texture2D>($"ControllerIcon/{GetButtonImageName(button)}");

    /// <summary>ゲームパッドのボタンから画像名を取得</summary>
    /// <param name="button">ボタン</param>
    static string GetButtonImageName(IconButton button)
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
    public static GamepadType GetGamepadType()
    {
        Gamepad current = Gamepad.current;

        if      (current == null)                   return GamepadType.None;
        else if (current is SwitchProControllerHID) return GamepadType.Joycon;
        else if (current is DualShockGamepad)       return GamepadType.DualShock;
        else if (current is XInputController)       return GamepadType.Xbox;

        return GamepadType.None;
    }
}
