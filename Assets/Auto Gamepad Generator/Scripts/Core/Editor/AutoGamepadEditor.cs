using UnityEngine;
using UnityEditor;
using System.Collections;
using AutoGamepad.Core;

[InitializeOnLoad]
public static class AutoGamepadEditor
{

    static AutoGamepadEditor()
    {
        EditorApplication.hierarchyWindowChanged += OnHierarchyChange;
    }
    static void OnHierarchyChange()
    {

        if (EditorApplication.currentScene.Contains("AGG_exampleScene")) 
        {
            //move the scene view camera to look at the texture
            if (SceneView.lastActiveSceneView != null)
            { 
                SceneView.lastActiveSceneView.LookAt(new Vector3(0f,6f,6f),Quaternion.Euler(new Vector3(30f,180f,0f)),3f);
                SceneView.lastActiveSceneView.Repaint();
            }

            
            //add our example axes (if not already present when we read the file below)
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
            SerializedProperty childElement;

            bool isAGGExampleAxesPresent = false;

            int count = axesProperty.arraySize;

            for (int i = 0; i < count; i++)
            {
                childElement = axesProperty.GetArrayElementAtIndex(i);

                if (AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue.Equals("AGG_P1_Horizontal"))
                {
                    isAGGExampleAxesPresent = true;
                    break;

                }
            }

            //the examples were not present add them and save the file (there are 18 example axes)
            //unfortunately have to add manually
            if (!isAGGExampleAxesPresent)
            {
                axesProperty.arraySize += 18;
                serializedObject.ApplyModifiedProperties();


                //AGG_P1_Horizontal
                childElement = axesProperty.GetArrayElementAtIndex(count);

                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_Horizontal";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "left";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "right";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_Horizontal
                childElement = axesProperty.GetArrayElementAtIndex(count);

                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_Horizontal";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "a";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "d";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_Vertical
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_Vertical";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "down";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "up";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_Vertical
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_Vertical";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "s";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "w";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZRed
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZRed";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[0]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZRed
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZRed";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "f";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZBlue
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZBlue";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[.]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZBlue
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZBlue";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "g";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZGreen
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZGreen";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[1]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZGreen
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZGreen";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "v";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZYellow
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZYellow";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[2]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZYellow
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZYellow";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "b";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZNext
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZNext";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[9]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZNext
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZNext";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "e";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZPrevious
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZPrevious";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[7]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZPrevious
                childElement = axesProperty.GetArrayElementAtIndex(count);
                
                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZPrevious";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "q";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P1_ZCycle
                childElement = axesProperty.GetArrayElementAtIndex(count);

                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P1_ZCycle";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "[4]";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "[6]";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;
                count++;

                //AGG_P2_ZCycle
                childElement = axesProperty.GetArrayElementAtIndex(count);

                AutoGamepadUtilities.GetChildProperty(childElement, "m_Name").stringValue = "AGG_P2_ZCycle";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "descriptiveNegativeName").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "negativeButton").stringValue = "z";
                AutoGamepadUtilities.GetChildProperty(childElement, "positiveButton").stringValue = "x";
                AutoGamepadUtilities.GetChildProperty(childElement, "altNegativeButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "altPositiveButton").stringValue = "";
                AutoGamepadUtilities.GetChildProperty(childElement, "gravity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "dead").floatValue = .001f;
                AutoGamepadUtilities.GetChildProperty(childElement, "sensitivity").floatValue = 1000f;
                AutoGamepadUtilities.GetChildProperty(childElement, "snap").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "invert").boolValue = false;
                AutoGamepadUtilities.GetChildProperty(childElement, "type").intValue = (int)AutoGamepadConstants.UNITY_AXIS_TYPE.KeyOrMouseButton;
                AutoGamepadUtilities.GetChildProperty(childElement, "axis").intValue = (int)AutoGamepadConstants.UNITY_AXIS_NUMBER.X_Axis_1st;
                AutoGamepadUtilities.GetChildProperty(childElement, "joyNum").intValue = (int)AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks;

                serializedObject.ApplyModifiedProperties();
            }


            //we only need to execute once on our scenes            
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChange;
        }

    }
}