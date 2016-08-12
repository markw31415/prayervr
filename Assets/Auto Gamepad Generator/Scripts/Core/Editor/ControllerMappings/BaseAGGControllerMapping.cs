using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AutoGamepad.Core
{
    public abstract class BaseAGGControllerMapping
    {


        public UnityInputAxes createAxis(string name, string positiveControlName, string negativeControlName, AutoGamepadConstants.UNITY_PLAYER_NUMBER player, AutoGamepadConstants.ITEM_TYPE type, AutoGamepadConstants.UNITY_AXIS_TYPE unityType, AutoGamepadConstants.UNITY_AXIS_NUMBER axisNumber,bool invert)
        {
            UnityInputAxes inputAxis = new UnityInputAxes();

            inputAxis.name = name;


            if (!positiveControlName.Equals(""))
            {
                if (positiveControlName.Equals("escape"))
                {
                    inputAxis.positiveButton = "escape";
                }
                else
                {
                    inputAxis.positiveButton = fetchControllerPrefix(player) + positiveControlName;
                }
            }
            if (!negativeControlName.Equals(""))
            {
                if (negativeControlName.Equals("escape"))
                {
                    inputAxis.negativeButton = "escape";
                }
                else
                {
                    inputAxis.negativeButton = fetchControllerPrefix(player) + negativeControlName;
                }
                
            }

            if (type == AutoGamepadConstants.ITEM_TYPE.Digital)
            {
                inputAxis.sensitivity = (AutoGamepadGenerator.overrideCollection[name].sensitivity == 0f) ? 1000f : AutoGamepadGenerator.overrideCollection[name].sensitivity;
                inputAxis.gravity = (AutoGamepadGenerator.overrideCollection[name].gravity == 0f) ? 1000f : AutoGamepadGenerator.overrideCollection[name].gravity;
                inputAxis.dead = (AutoGamepadGenerator.overrideCollection[name].dead == 0f) ? .001f : AutoGamepadGenerator.overrideCollection[name].dead;
            }
            else
            {
                inputAxis.sensitivity = (AutoGamepadGenerator.overrideCollection[name].sensitivity == 0f) ? 1f : AutoGamepadGenerator.overrideCollection[name].sensitivity;
                inputAxis.gravity = (AutoGamepadGenerator.overrideCollection[name].gravity == 0f) ? 0f : AutoGamepadGenerator.overrideCollection[name].gravity;
                inputAxis.dead = (AutoGamepadGenerator.overrideCollection[name].dead == 0f) ? .19f : AutoGamepadGenerator.overrideCollection[name].dead;
            }

            inputAxis.snap = (AutoGamepadGenerator.overrideCollection[name].snap == false) ? false : true;
            inputAxis.invert = (AutoGamepadGenerator.overrideCollection[name].invert == false) ? invert : !invert;
            inputAxis.type = unityType;
            inputAxis.axis = axisNumber;
            inputAxis.joyNum = player;
            inputAxis.descriptiveName = AutoGamepadConstants.GENERATED;

            

            return inputAxis;
        }

        public string fetchControllerPrefix(AutoGamepadConstants.UNITY_PLAYER_NUMBER player)
        {
            string returnString = "";
            switch (player)
            {
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks:
                    returnString = "joystick ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick1:
                    returnString = "joystick 1 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick2:
                    returnString = "joystick 2 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick3:
                    returnString = "joystick 3 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick4:
                    returnString = "joystick 4 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick5:
                    returnString = "joystick 5 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick6:
                    returnString = "joystick 6 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick7:
                    returnString = "joystick 7 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick8:
                    returnString = "joystick 8 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick9:
                    returnString = "joystick 9 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick10:
                    returnString = "joystick 10 ";
                    break;
                case AutoGamepadConstants.UNITY_PLAYER_NUMBER.Joystick11:
                    returnString = "joystick 11 ";
                    break;
                default:
                    returnString = "joystick ";
                    break;
            }

            return returnString;
        }

        public abstract List<UnityInputAxes> fetchMappingObject(string name, AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING control,AutoGamepadConstants.UNITY_PLAYER_NUMBER player);
        
    }
}