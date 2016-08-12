using UnityEngine;
using System.Collections;

namespace AutoGamepad.Core
{
    public static class AutoGamepadConstants
    {
        //Unityenums
        public enum UNITY_AXIS_TYPE { KeyOrMouseButton = 0, MouseMovement = 1, JoystickAxis = 2 };

        public enum UNITY_AXIS_NUMBER { X_Axis_1st = 0, Y_Axis_2nd = 1, ThirdAxis = 2, FourthAxis = 3, FifthAxis = 4, SixthAxis = 5, SeventhAxis = 6
                                        , EighthAxis = 7 , NinthAxis = 8, TenthAxis = 9, EleventhAxis = 10, TwelvethAxis = 11, ThirteenthAxis = 12
                                        , FourteenthAxis = 13, FifteenthAxis = 14, SixteenthAxis = 15, SeventeenthAxis = 16, EighteenthAxis = 17
                                        , NinteenthAxis = 18, TwentiethAxis = 19 };

        public enum UNITY_PLAYER_NUMBER { GetMotionFromAllJoysticks = 0, Joystick1 = 1, Joystick2 = 2, Joystick3 = 3, Joystick4 = 4, Joystick5 = 5
                                        , Joystick6 = 6, Joystick7 = 7, Joystick8 = 8, Joystick9 = 9 , Joystick10 = 10, Joystick11 = 11};

        //AGG enums
        public enum DEFAULT_CONTROLLER_MAPPING { LeftStickHorizontal = 0, LeftStickVertical = 1, RightStickHorizontal = 2, RightStickVertical = 3
                                        , DpadHorizontal = 4, DpadVertical = 5, A_Button = 6, B_Button = 7, X_Button = 8, Y_Button = 9
                                        , LeftBumper = 10, RightBumper = 11, Start = 12, None = 13, Back_ControllerSpecific = 14
                                        , LeftStickPush_ControllerSpecific = 15, RightStickPush_ControllerSpecific = 16
                                        , TriggersCombined_ControllerSpecific = 17, LeftTrigger_ControllerSpecific = 18
                                        , RightTrigger_ControllerSpecific = 19 };

        public enum PLATFORM { Windows = 0, Mac = 1, Linux = 2, IOS = 3, Android = 4, AndroidTV = 5, FireTV = 6, Xbox = 7, Playstation = 8, WebGL = 9, AppleTV = 10 };

        public enum ITEM_TYPE { Digital = 0, Analog = 1 };

        public const string GENERATED = "AGG_Generated";

    }

}
