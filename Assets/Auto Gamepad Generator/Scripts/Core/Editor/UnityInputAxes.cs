using UnityEngine;
using System.Collections;
using AutoGamepad.Core;

namespace AutoGamepad.Core
{
    public class UnityInputAxes
    {
        //artificial to keep original order
        public int inputAssetIndex;

        //real members
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;
        public float gravity;
        public float dead;
        public float sensitivity;
        public bool snap;
        public bool invert;
        public AutoGamepadConstants.UNITY_AXIS_TYPE type;
        public AutoGamepadConstants.UNITY_AXIS_NUMBER axis;
        public AutoGamepadConstants.UNITY_PLAYER_NUMBER joyNum;

    }
}
