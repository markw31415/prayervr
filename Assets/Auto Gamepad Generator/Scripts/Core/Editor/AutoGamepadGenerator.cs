#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0
#define UNITY_LESS_5_1
#endif


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AutoGamepad.Core;

namespace AutoGamepad.Core
{
    public class AutoGamepadGenerator : EditorWindow
    {

        Texture2D titleTexture;
        GUIStyle titleStyle;
        Vector2 scrollPos;
        SerializedObject serializedObject;
        SerializedProperty axesProperty;
        float timeElapsed = 0f;
        bool isDirty;


        //for editor windows enums state don't get saved unless static
        public static SortedList<string, UnityInputAxes> inputCollection; //input collection
        public static Dictionary<string, bool> isFoldedOutCollection; //bools for editor foldouts
        public static Dictionary<string, AutoGamepadConstants.DEFAULT_CONTROLLER_MAPPING> controllerMappingCollection; //enums for what controller mapping
        public static Dictionary<string, AutoGamepadConstants.UNITY_PLAYER_NUMBER> playerNumberCollection; //enums for what player
        public static Dictionary<string, int> virtualNameCount;
        public static Dictionary<string, UnityInputAxes> overrideCollection;
        public static bool isProcessing = false;
        public static bool isSuccessful = false;
        public static AutoGamepadConstants.PLATFORM platformEnum = AutoGamepadConstants.PLATFORM.Windows;

        [MenuItem("Window/AutoGamepadGenerator")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<AutoGamepadGenerator>();

            #if UNITY_LESS_5_1
                window.title = "Auto Gamepad";
            #endif
            
            #if !UNITY_LESS_5_1
                GUIContent content = new GUIContent("Auto Gamepad");
                window.titleContent = content;
            #endif

            window.minSize = new Vector2(850f, 600f);

            // Prevent the window from being destroyed when a new
            // scene is loaded into the editor.
            Object.DontDestroyOnLoad(window);

        }
        void Awake()
        {
            //title
            titleTexture = (Texture2D)Resources.Load("AGG_Title", typeof(Texture2D));

            titleStyle = new GUIStyle();
            titleStyle.fixedHeight = 50f;
            titleStyle.fixedWidth = 780f;

            inputCollection = new SortedList<string, UnityInputAxes>();
            isFoldedOutCollection = new Dictionary<string, bool>();


        }

        private void OnGUI()
        {
            //EditorGUI,EditorGUILayout,GUI, and GUI Layout seem to have overlap
            //use the layouts when possible as they auto format

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            //Title logo for our custom window
            GUILayout.Label(titleTexture, titleStyle);            

            if (inputCollection == null || inputCollection.Count == 0 || isFoldedOutCollection == null || isFoldedOutCollection.Count == 0 || isDirty)
            {
                //read the inputmanager asset file
                inputCollection = AutoGamepadUtilities.ReadAxisAsset();

                //initialize the foldoutlist
                AutoGamepadUtilities.InitializeFoldoutList();

                isDirty = false;
            }

            //Display the input Manager
            AutoGamepadUtilities.DisplayCurrentInputManager();

            EditorGUILayout.EndScrollView();

            

        }

        void Update()
        {
            
            if (isProcessing || isSuccessful)
            {
                timeElapsed += Time.deltaTime;
            }

            if (timeElapsed > .0025f)
            {
                //timer has expired
                if (isProcessing)
                {
                    timeElapsed = 0f;
                    isProcessing = false;
                    isSuccessful = true;
                    isDirty = true;
                    Repaint();
                }
                else
                {
                    timeElapsed = 0f;
                    isSuccessful = false;
                    isDirty = true;
                    Repaint();
                }

            }
        }


    }
}
