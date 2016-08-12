using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AutoGamepad.Core
{
    public class AndroidAGGControllerMapping : BaseAGGControllerMapping
    {
        public override List<UnityInputAxes> fetchMappingObject(string name, AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING control, AutoGamepadConstants.UNITY_PLAYER_NUMBER player)
        {
            List<UnityInputAxes> inputAxisCollection = new List<UnityInputAxes>();
            UnityInputAxes inputAxis;

            switch (control)
            {
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.LeftStickHorizontal:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.LeftStickVertical:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.Y_Axis_2nd, true);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.RightStickHorizontal:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.ThirdAxis, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.RightStickVertical:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.FourthAxis, true);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.DpadHorizontal:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.FifthAxis, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.DpadVertical:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.SixthAxis, true);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.TriggersCombined_ControllerSpecific:
                    inputAxis = null;
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.LeftTrigger_ControllerSpecific:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.SeventhAxis, false);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.ThirteenthAxis, false);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.FourteenthAxis, true);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "button 6", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.RightTrigger_ControllerSpecific:
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.EighthAxis, false);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "", "", player, AutoGamepadConstants.ITEM_TYPE.Analog, AutoGamepadConstants.UNITY_AXIS_TYPE.JoystickAxis, AutoGamepadConstants.UNITY_AXIS_NUMBER.TwelvethAxis, false);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "button 7", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.A_Button:
                    inputAxis = createAxis(name, "button 0", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.B_Button:
                    inputAxis = createAxis(name, "button 1", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.X_Button:
                    inputAxis = createAxis(name, "button 2", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.Y_Button:
                    inputAxis = createAxis(name, "button 3", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.LeftBumper:
                    inputAxis = createAxis(name, "button 4", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.RightBumper:
                    inputAxis = createAxis(name, "button 5", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.Start:
                    inputAxis = createAxis(name, "button 10", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.Back_ControllerSpecific:
                    inputAxis = createAxis(name, "button 11", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    inputAxisCollection.Add(inputAxis);
                    inputAxis = createAxis(name, "escape", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.LeftStickPush_ControllerSpecific:
                    inputAxis = createAxis(name, "button 8", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.RightStickPush_ControllerSpecific:
                    inputAxis = createAxis(name, "button 9", "", player, AutoGamepadConstants.ITEM_TYPE.Digital, AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton, AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st, false);
                    break;
                case AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.None:
                    inputAxis = null;
                    break;
                default:
                    inputAxis = null;
                    break;

            }

            if (inputAxis != null)
                inputAxisCollection.Add(inputAxis);

            return inputAxisCollection;
        }
    }
}
