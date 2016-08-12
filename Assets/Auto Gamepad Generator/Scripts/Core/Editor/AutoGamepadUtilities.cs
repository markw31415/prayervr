using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AutoGamepad.Core;

namespace AutoGamepad.Core
{
    public static class AutoGamepadUtilities
    {

        public static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {

            //copy so we don't iterate original
            SerializedProperty copiedProperty = parent.Copy();

            bool moreChildren = true;

            //step one level into child
            copiedProperty.Next(true);
            
            //iterate on all properties one level deep
            while (moreChildren)
            {
                //found the child we were looking for
                if (copiedProperty.name.Equals(name))
                    return copiedProperty;

                //move to the next property
                moreChildren = copiedProperty.Next(false);
            }            
            
            //if we get here we didn't find it
            return null;
        }


        //this function is responsible for reading and populating a dictionary collection that corresponds to
        //the current InputManager file
        public static SortedList<string,UnityInputAxes> ReadAxisAsset()
        {
            SortedList<string, UnityInputAxes> fileCollection = new SortedList<string, UnityInputAxes>();
            UnityInputAxes currentObject;

            //read the file
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            int count = axesProperty.arraySize;

            for (int i = 0; i < count; i++)
            {
                currentObject = new UnityInputAxes();
                SerializedProperty childElement = axesProperty.GetArrayElementAtIndex(i);

                currentObject.inputAssetIndex = i;
                currentObject.name = GetChildProperty(childElement, "m_Name").stringValue;
                currentObject.descriptiveName = GetChildProperty(childElement, "descriptiveName").stringValue;
                currentObject.descriptiveNegativeName = GetChildProperty(childElement, "descriptiveNegativeName").stringValue;
                currentObject.negativeButton = GetChildProperty(childElement, "negativeButton").stringValue;
                currentObject.positiveButton = GetChildProperty(childElement, "positiveButton").stringValue;
                currentObject.altNegativeButton = GetChildProperty(childElement, "altNegativeButton").stringValue;
                currentObject.altPositiveButton = GetChildProperty(childElement, "altPositiveButton").stringValue;
                currentObject.gravity = GetChildProperty(childElement, "gravity").floatValue;
                currentObject.dead = GetChildProperty(childElement, "dead").floatValue;
                currentObject.sensitivity = GetChildProperty(childElement, "sensitivity").floatValue;
                currentObject.snap = GetChildProperty(childElement, "snap").boolValue;
                currentObject.invert = GetChildProperty(childElement, "invert").boolValue;
                currentObject.type = (AutoGamepadConstants.UNITY_AXIS_TYPE)GetChildProperty(childElement, "type").intValue;
                currentObject.axis = (AutoGamepadConstants.UNITY_AXIS_NUMBER)GetChildProperty(childElement, "axis").intValue;
                currentObject.joyNum = (AutoGamepadConstants.UNITY_PLAYER_NUMBER)GetChildProperty(childElement, "joyNum").intValue;

                fileCollection.Add(currentObject.name + "__" + currentObject.inputAssetIndex.ToString(), currentObject);
            }

            return fileCollection;

        }

        //this function is responsible for reading and populating a dictionary collection that corresponds to
        //the current InputManager file
        public static void UpdateAxisAsset(List<UnityInputAxes> newAxisCollection)
        {
            //read the file
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            int oldObjectCount = axesProperty.arraySize;
            int i = oldObjectCount;

            axesProperty.arraySize += newAxisCollection.Count;
            serializedObject.ApplyModifiedProperties();

            foreach (UnityInputAxes axis in newAxisCollection)
            {               
                //get the element at i
                SerializedProperty childElement = axesProperty.GetArrayElementAtIndex(i);

                //set the values
                GetChildProperty(childElement, "m_Name").stringValue = axis.name;
                GetChildProperty(childElement, "descriptiveName").stringValue = axis.descriptiveName;
                GetChildProperty(childElement, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
                GetChildProperty(childElement, "negativeButton").stringValue = axis.negativeButton;
                GetChildProperty(childElement, "positiveButton").stringValue = axis.positiveButton;
                GetChildProperty(childElement, "altNegativeButton").stringValue = axis.altNegativeButton;
                GetChildProperty(childElement, "altPositiveButton").stringValue = axis.altPositiveButton;
                GetChildProperty(childElement, "gravity").floatValue = axis.gravity;
                GetChildProperty(childElement, "dead").floatValue = axis.dead;
                GetChildProperty(childElement, "sensitivity").floatValue = axis.sensitivity;
                GetChildProperty(childElement, "snap").boolValue = axis.snap;
                GetChildProperty(childElement, "invert").boolValue = axis.invert;
                GetChildProperty(childElement, "type").intValue = (int)axis.type;
                GetChildProperty(childElement, "axis").intValue = (int)axis.axis;
                GetChildProperty(childElement, "joyNum").intValue = (int)axis.joyNum;

                i++;
            }

            serializedObject.ApplyModifiedProperties();

        }

        //this function is displaying the current Input Manager file
        public static void DisplayCurrentInputManager()
        {
            string[] split;
            string[] splitter = new string[] {"__"};

            string currentRealName = "";
            int currentIndex = 0;

            GUIStyle titleHeading = new GUIStyle(GUI.skin.label);
            titleHeading.fontStyle = FontStyle.Bold;
            titleHeading.fontSize = 20;

            GUIStyle labelHeadings = new GUIStyle(GUI.skin.label);
            labelHeadings.fontStyle = FontStyle.Bold;

            GUILayout.Space(10f);

            EditorGUILayout.LabelField("Mapping Settings:", titleHeading, GUILayout.Height(30f));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Platform:", GUILayout.Width(140f));
            AutoGamepadGenerator.platformEnum = (AutoGamepadConstants.PLATFORM)EditorGUILayout.EnumPopup(AutoGamepadGenerator.platformEnum, GUILayout.Width(140f));
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);


            EditorGUILayout.LabelField("Current Input Manager:",titleHeading, GUILayout.Height(30f));

            foreach (string currentKey in AutoGamepadGenerator.inputCollection.Keys)
            {
                //remember this is a sorted list (alphabetical
                split = AutoGamepadGenerator.inputCollection[currentKey].name.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

                if (!split[0].Equals(currentRealName))
                {
                    //new unique name
                    currentRealName = split[0];

                    GUILayout.BeginHorizontal();

                    //create foldout
                    AutoGamepadGenerator.isFoldedOutCollection[currentRealName] = EditorGUILayout.Foldout(AutoGamepadGenerator.isFoldedOutCollection[currentRealName], currentRealName + "(" + AutoGamepadGenerator.virtualNameCount[currentRealName] + ")");

                    GUILayout.Label("Controller Mapping: ", labelHeadings, GUILayout.Width(130f));
                    AutoGamepadGenerator.controllerMappingCollection[currentRealName] = (AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING)EditorGUILayout.EnumPopup(AutoGamepadGenerator.controllerMappingCollection[currentRealName], GUILayout.Width(140f));
                    //types = (AutoGamepadConstants.CONTROLLER_MAPPING) EditorGUILayout.EnumPopup(types,GUILayout.Width(140f));

                    GUILayout.Label("        Player: ", labelHeadings, GUILayout.Width(80f));
                    AutoGamepadGenerator.playerNumberCollection[currentRealName] = (AutoGamepadConstants.UNITY_PLAYER_NUMBER)EditorGUILayout.EnumPopup(AutoGamepadGenerator.playerNumberCollection[currentRealName]);
                    //ptypes = (AutoGamepadConstants.UNITY_PLAYER_NUMBER)EditorGUILayout.EnumPopup(ptypes);

                    GUILayout.EndHorizontal();

                    if (AutoGamepadGenerator.isFoldedOutCollection[currentRealName])
                    {
                        //if the foldout children is visible show all that share current real name
                        DisplayInputAxis(AutoGamepadGenerator.inputCollection[currentKey], AutoGamepadGenerator.overrideCollection[currentRealName]);
                        currentIndex = AutoGamepadGenerator.inputCollection.IndexOfKey(currentKey) + 1;

                        //we need to check if anything else shares the name
                        for (int i = currentIndex; i < AutoGamepadGenerator.inputCollection.Count; i++)
                        {
                            split = AutoGamepadGenerator.inputCollection.Values[i].name.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

                            if (split[0].Equals(currentRealName))
                            {
                                //matches name
                                DisplayInputAxis(AutoGamepadGenerator.inputCollection.Values[i], null);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //next item that shares name (we already took care of this)
                }

            }

            GUILayout.Space(20f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Controller Support"))
            {
                generateSpecifiedAxes();
            }
            GUILayout.Space(50f);
            if (GUILayout.Button("Delete All Generated Axes"))
            {
                deleteGeneratedAxes();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            if (AutoGamepadGenerator.isProcessing)
            {
                GUILayout.Label("Processing...",labelHeadings);
            }

            if (AutoGamepadGenerator.isSuccessful)
            {
                GUILayout.Label("Success!",labelHeadings);
            }
        }

        //this function is responsible for creating the initial foldout list
        public static void InitializeFoldoutList()
        {
            string[] split;
            string[] splitter = new string[] { "__" };

            string currentRealName = "";

            //clear the foldout list
            AutoGamepadGenerator.isFoldedOutCollection = new Dictionary<string, bool>();

            //also clear the controller mapping and player since they correspond to the foldout list
            AutoGamepadGenerator.controllerMappingCollection = new Dictionary<string, AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING>();
            AutoGamepadGenerator.playerNumberCollection = new Dictionary<string, AutoGamepadConstants.UNITY_PLAYER_NUMBER>();
            AutoGamepadGenerator.virtualNameCount = new Dictionary<string, int>();
            AutoGamepadGenerator.overrideCollection = new Dictionary<string,UnityInputAxes>();

            //looks for unique names and creates the list
            foreach (string currentKey in AutoGamepadGenerator.inputCollection.Keys)
            {
                //for the foldout list we don't care about the index
                split = AutoGamepadGenerator.inputCollection[currentKey].name.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

                if (!split[0].Equals(currentRealName))
                {
                    currentRealName = split[0];
                    AutoGamepadGenerator.isFoldedOutCollection.Add(currentRealName, false);
                    AutoGamepadGenerator.controllerMappingCollection.Add(currentRealName, AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING.None);
                    AutoGamepadGenerator.playerNumberCollection.Add(currentRealName, AutoGamepadConstants.UNITY_PLAYER_NUMBER.GetMotionFromAllJoysticks);
                    AutoGamepadGenerator.virtualNameCount.Add(currentRealName, 1);
                    AutoGamepadGenerator.overrideCollection.Add(currentRealName, new UnityInputAxes());
                }
                else
                {
                    AutoGamepadGenerator.virtualNameCount[currentRealName]++;
                }
                
            }

        }

        //this function is responsible for displaying a single UnityInputAxis object
        public static void DisplayInputAxis(UnityInputAxes inputAxis, UnityInputAxes overideAxis)
        {
            GUIStyle oneIndent = new GUIStyle(GUI.skin.label);
            oneIndent.margin = new RectOffset(20, 0, 0, 0);
            oneIndent.fontStyle = FontStyle.Bold;

            GUIStyle twoIndent = new GUIStyle(GUI.skin.label);
            twoIndent.margin = new RectOffset(40, 0, 0, 0);
            

            GUILayout.Label("InputManager Index: " + inputAxis.inputAssetIndex.ToString(), oneIndent);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Positive Descriptive Name: " + inputAxis.descriptiveName, twoIndent,GUILayout.Width(400f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Negative Descriptive Name: " + inputAxis.descriptiveNegativeName, twoIndent, GUILayout.Width(400f));
            GUILayout.EndHorizontal();

            GUILayout.Label("Positive Button: " + inputAxis.positiveButton,twoIndent);
            GUILayout.Label("Negative Button: " + inputAxis.negativeButton,twoIndent);
            GUILayout.Label("Alternative Positive Button: " + inputAxis.altPositiveButton, twoIndent);
            GUILayout.Label("Alternative Negative Button: " + inputAxis.altNegativeButton, twoIndent);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gravity: " + inputAxis.gravity.ToString(), twoIndent, GUILayout.Width(400f));
            if (overideAxis != null)
            {
                GUILayout.Label("Gravity Overide: ", GUILayout.Width(180f));
                overideAxis.gravity = EditorGUILayout.FloatField(overideAxis.gravity);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Dead Zone: " + inputAxis.dead.ToString(), twoIndent, GUILayout.Width(400f));
            if (overideAxis != null)
            {
                GUILayout.Label("Dead Zone Overide: ", GUILayout.Width(180f));
                overideAxis.dead = EditorGUILayout.FloatField(overideAxis.dead);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sensitivity: " + inputAxis.sensitivity.ToString(), twoIndent,GUILayout.Width(400f));
            if (overideAxis != null)
            {
                GUILayout.Label("Sensitivity Overide: ", GUILayout.Width(180f));
                overideAxis.sensitivity = EditorGUILayout.FloatField(overideAxis.sensitivity);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Snap: " + inputAxis.snap.ToString(), twoIndent,GUILayout.Width(400f));
            if (overideAxis != null)
            {
                GUILayout.Label("Snap Overide: ", GUILayout.Width(180f));
                overideAxis.snap = EditorGUILayout.Toggle(overideAxis.snap);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Invert: " + inputAxis.invert.ToString(), twoIndent, GUILayout.Width(400f));
            if (overideAxis != null)
            {
                GUILayout.Label("Invert Overide: ", GUILayout.Width(180f));
                overideAxis.invert = EditorGUILayout.Toggle(overideAxis.invert);
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Type: " + inputAxis.type.ToString(), twoIndent);
            GUILayout.Label("Axis: " + inputAxis.axis.ToString(), twoIndent);
            GUILayout.Label("Joystick Number: " + inputAxis.joyNum.ToString(), twoIndent);
        }

        //This function is responsible for reading all the settings from the AGG window and then generating the appropriate addition axes
        public static void generateSpecifiedAxes()
        {
            BaseAGGControllerMapping controllerMapping;
            List<UnityInputAxes> newAxis;
            List<UnityInputAxes> newAxisCollection = new List<UnityInputAxes>();
            string[] split;
            string[] splitter = new string[] { "__" };
            string currentRealName = "";

            AutoGamepadGenerator.isProcessing = true;

            //current platform determines which class to instantiate
            switch (AutoGamepadGenerator.platformEnum)
            {
                case AutoGamepadConstants.PLATFORM.Windows:
                    controllerMapping = new WinXinputAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.Mac:
                    controllerMapping = new MacAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.Linux:
                    controllerMapping = new LinuxAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.IOS:
                case AutoGamepadConstants.PLATFORM.AppleTV:
                    controllerMapping = new IosAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.Android:
                case AutoGamepadConstants.PLATFORM.AndroidTV:
                case AutoGamepadConstants.PLATFORM.FireTV:
                    controllerMapping = new AndroidAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.Xbox:
                    controllerMapping = new XboxAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.Playstation:
                    controllerMapping = new PlaystationAGGControllerMapping();
                    break;
                case AutoGamepadConstants.PLATFORM.WebGL:
                    controllerMapping = new WebglAGGControllerMapping();
                    break;
                default:
                    //shouldn't happen
                    controllerMapping = null;
                    break;
            }

            
            //loop through each axis and generate based on settings
            foreach (string currentKey in AutoGamepadGenerator.inputCollection.Keys)
            {
                //for the foldout list we don't care about the index
                split = AutoGamepadGenerator.inputCollection[currentKey].name.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);

                if (!split[0].Equals(currentRealName))
                {
                    currentRealName = split[0];
                    
                    
                    if (controllerMapping != null)
                    {
                        //generate the new axis (or collection for instance android triggers have two mappings
                        newAxis = controllerMapping.fetchMappingObject(currentRealName, AutoGamepadGenerator.controllerMappingCollection[currentRealName], AutoGamepadGenerator.playerNumberCollection[currentRealName]);
                                                
                        //add it to the list for now if not null (list will be added to file at the end)
                        if (newAxis != null)
                        {
                            foreach (UnityInputAxes axis in newAxis)
                            {
                                newAxisCollection.Add(axis);
                            }
                        }
                    }
                }
                else
                {
                    //duplicate in original collection so this one is already generated above
                }

            }

            //now we have the Collection of all the axis to be added so add it to the file
            UpdateAxisAsset(newAxisCollection);
            
        }

        //This function is responsible for clearing out the current set of axes that were generated by AGG (only the generated ones)
        //this is commonly used when you are switching platforms and want to generate a new set that corresponds to the new platform
        public static void deleteGeneratedAxes()
        {
            AutoGamepadGenerator.isProcessing = true;

            //read the file
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedObject newSerializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);

            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
            SerializedProperty newAxesProperty = newSerializedObject.FindProperty("m_Axes");

            //clear the new one and populate it with only the real axes not the generated ones
            newAxesProperty.arraySize = 0;
            newSerializedObject.ApplyModifiedProperties();

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                //get the element at i
                SerializedProperty childElement = axesProperty.GetArrayElementAtIndex(i);

                if (GetChildProperty(childElement, "descriptiveName").stringValue.Equals(AutoGamepadConstants.GENERATED))
                {
                    //this is generated do not copy over
                }
                else
                {
                    //this is a real axes so add this to new object
                    newAxesProperty.arraySize++;
                    newSerializedObject.ApplyModifiedProperties();

                    //get the element
                    SerializedProperty newChildElement = newAxesProperty.GetArrayElementAtIndex(newAxesProperty.arraySize - 1);

                    GetChildProperty(newChildElement, "m_Name").stringValue = GetChildProperty(childElement, "m_Name").stringValue;
                    GetChildProperty(newChildElement, "descriptiveName").stringValue = GetChildProperty(childElement, "descriptiveName").stringValue;
                    GetChildProperty(newChildElement, "descriptiveNegativeName").stringValue = GetChildProperty(childElement, "descriptiveNegativeName").stringValue;
                    GetChildProperty(newChildElement, "negativeButton").stringValue = GetChildProperty(childElement, "negativeButton").stringValue;
                    GetChildProperty(newChildElement, "positiveButton").stringValue = GetChildProperty(childElement, "positiveButton").stringValue;
                    GetChildProperty(newChildElement, "altNegativeButton").stringValue = GetChildProperty(childElement, "altNegativeButton").stringValue;
                    GetChildProperty(newChildElement, "altPositiveButton").stringValue = GetChildProperty(childElement, "altPositiveButton").stringValue;
                    GetChildProperty(newChildElement, "gravity").floatValue = GetChildProperty(childElement, "gravity").floatValue;
                    GetChildProperty(newChildElement, "dead").floatValue = GetChildProperty(childElement, "dead").floatValue;
                    GetChildProperty(newChildElement, "sensitivity").floatValue = GetChildProperty(childElement, "sensitivity").floatValue;
                    GetChildProperty(newChildElement, "snap").boolValue = GetChildProperty(childElement, "snap").boolValue;
                    GetChildProperty(newChildElement, "invert").boolValue = GetChildProperty(childElement, "invert").boolValue;
                    GetChildProperty(newChildElement, "type").intValue = GetChildProperty(childElement, "type").intValue;
                    GetChildProperty(newChildElement, "axis").intValue = GetChildProperty(childElement, "axis").intValue;
                    GetChildProperty(newChildElement, "joyNum").intValue = GetChildProperty(childElement, "joyNum").intValue;

                }


            }
            //apply the changes
            newSerializedObject.ApplyModifiedProperties();

            //copy it over and apply
            serializedObject = newSerializedObject;
            serializedObject.ApplyModifiedProperties();


        }


    }

}
