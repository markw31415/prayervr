/* Written by Kaz Crowe */
/* UltimateJoystickEditor.cs ver. 1.3.5 */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateJoystick ) )]
public class UltimateJoystickEditor : Editor
{
	/* -----< ASSIGNED VARIABLES >----- */
	SerializedProperty joystick, joystickSizeFolder;
	SerializedProperty highlightBase, highlightJoystick;
	SerializedProperty tensionAccentUp, tensionAccentDown;
	SerializedProperty tensionAccentLeft, tensionAccentRight;
	SerializedProperty joystickAnimator, joystickBase;
	
	/* -----< SIZE AND PLACEMENT >----- */
	SerializedProperty scalingAxis, anchor, joystickTouchSize;
	SerializedProperty customTouchSize_X, customTouchSize_Y;
	SerializedProperty customTouchSizePos_X, customTouchSizePos_Y;
	SerializedProperty dynamicPositioning;
	SerializedProperty joystickSize, radiusModifier;
	SerializedProperty customSpacing_X, customSpacing_Y;
	
	/* -----< STYLES AND OPTIONS >----- */
	SerializedProperty touchPad, throwable, draggable;
	SerializedProperty throwDuration;
	SerializedProperty showHighlight, showTension;
	SerializedProperty highlightColor, tensionColorNone, tensionColorFull;
	SerializedProperty axis, boundary;
	SerializedProperty xDeadZone, yDeadZone, deadZoneOption;
	
	/* --------< TOUCH ACTION >-------- */
	SerializedProperty useAnimation, useFade;
	SerializedProperty tapCountOption, tapCountDuration;
	SerializedProperty tapCountEvent, targetTapCount;
	SerializedProperty fadeUntouched, fadeTouched;
	
	/* ------< SCRIPT REFERENCE >------ */
	SerializedProperty joystickName, exposeValues;
	SerializedProperty horizontalValue, verticalValue;
	
	/* // ----< ANIMATED SECTIONS >---- \\ */
	AnimBool AssignedVariables, SizeAndPlacement, StyleAndOptions;
	AnimBool TouchActions, ScriptReference;

	/* // ----< ANIMATED VARIABLE >---- \\ */
	AnimBool customTouchSizeOption, throwableOption;
	AnimBool highlightOption, tensionOption;
	AnimBool dzOneValueOption, dzTwoValueOption;
	AnimBool tcOption, tcTargetTapOption;
	AnimBool animationOption, fadeOption;

	AnimBool joystickNameUnassigned, joystickNameAssigned;
	AnimBool exposeValuesBool;

	public enum ScriptCast{ Vector2, distance, horizontalFloat, verticalFloat, getJoystickState }// Add reference for all of the static functions
	ScriptCast scriptCast;

	SerializedProperty fadeInDuration, fadeOutDuration;

	Canvas parentCanvas;
	
	
	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();
		
		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;
		
		parentCanvas = GetParentCanvas();
	}

	Canvas GetParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return null;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{ 
			// If there is a Canvas component, return the component.
			if( parent.transform.GetComponent<Canvas>() && parent.transform.GetComponent<Canvas>().enabled == true )
				return parent.transform.GetComponent<Canvas>();
			
			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && PrefabUtility.GetPrefabType( Selection.activeGameObject ) != PrefabType.Prefab )
			UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );

		return null;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	// Function called to display an interactive header.
	void DisplayHeader ( string headerName, string editorPref, AnimBool targetAnim )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
		{
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );
			targetAnim.target = EditorPrefs.GetBool( editorPref );
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	bool CanvasErrors ()
	{
		// If the selection is actually the prefab within the Project window, then return no errors.
		if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) == PrefabType.Prefab )
			return false;

		// If parentCanvas is unassigned, then get a new canvas and return no errors.
		if( parentCanvas == null )
		{
			parentCanvas = GetParentCanvas();
			return false;
		}

		// If the parentCanvas is not enabled, then return true for errors.
		if( parentCanvas.enabled == false )
			return true;

		// If the canvas' renderMode is not the needed one, then return true for errors.
		if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			return true;

		// If the canvas has a CanvasScaler component and it is not the correct option.
		if( parentCanvas.GetComponent<CanvasScaler>() && parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}
	
	/*
	For more information on the OnInspectorGUI and adding your own variables
	in the UltimateJoystick.cs script and displaying them in this script,
	see the EditorGUILayout section in the Unity Documentation to help out.
	*/
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();

		if( CanvasErrors() == true )
		{
			if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			{
				EditorGUILayout.LabelField( "Canvas", EditorStyles.boldLabel );
				EditorGUILayout.HelpBox( "The parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Ultimate Joystick to function correctly.", MessageType.Error );
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( 5 );
				if( GUILayout.Button( "Update Canvas" ) )
				{
					parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
					parentCanvas = GetParentCanvas();
				}
				GUILayout.Space( 5 );
				if( GUILayout.Button( "Update Joystick" ) )
				{
					UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );
					parentCanvas = GetParentCanvas();
				}
				GUILayout.Space( 5 );
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			if( parentCanvas.GetComponent<CanvasScaler>() )
			{
				if( parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
				{
					EditorGUILayout.LabelField( "Canvas Scaler", EditorStyles.boldLabel );
					EditorGUILayout.HelpBox( "The Canvas Scaler component located on the parent Canvas needs to be set to 'Constant Pixel Size' in order for the Ultimate Joystick to function correctly.", MessageType.Error );
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space( 5 );
					if( GUILayout.Button( "Update Canvas" ) )
					{
						parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						parentCanvas = GetParentCanvas();
						UltimateJoystick joystick = ( UltimateJoystick )target;
						joystick.UpdatePositioning();
					}
					GUILayout.Space( 5 );
					if( GUILayout.Button( "Update Joystick" ) )
					{
						UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					GUILayout.Space( 5 );
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}
			}
			return;
		}

		UltimateJoystick uj = ( UltimateJoystick ) target;
		
		#region ASSIGNED VARIABLES
		/* ----------------------------------------< ** ASSIGNED VARIABLES ** >---------------------------------------- */
		DisplayHeader( "Assigned Variables", "UUI_Variables", AssignedVariables );
		if( EditorGUILayout.BeginFadeGroup( AssignedVariables.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystick );
			EditorGUILayout.PropertyField( joystickSizeFolder, new GUIContent( "Size Folder" ) );
			EditorGUILayout.PropertyField( joystickBase );
			EditorGUI.indentLevel = 0;
			
			if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Highlight Variables", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( highlightBase );
				EditorGUILayout.PropertyField( highlightJoystick );
				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Tension Variables", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( tensionAccentUp, new GUIContent( "Tension Up" ) );
				EditorGUILayout.PropertyField( tensionAccentDown, new GUIContent( "Tension Down" ) );
				EditorGUILayout.PropertyField( tensionAccentLeft, new GUIContent( "Tension Left" ) );
				EditorGUILayout.PropertyField( tensionAccentRight, new GUIContent( "Tension Right" ) );
				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( animationOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Touch Action Variables", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( joystickAnimator );
				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END ASSIGNED VARIABLES ** >-------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		#region SIZE AND PLACEMENT
		/* ----------------------------------------< ** SIZE AND PLACEMENT ** >---------------------------------------- */
		DisplayHeader( "Size and Placement", "UUI_SizeAndPlacement", SizeAndPlacement );
		if( EditorGUILayout.BeginFadeGroup( SizeAndPlacement.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "The axis to scale the Ultimate Joystick from." ) );
			EditorGUILayout.PropertyField( anchor, new GUIContent( "Anchor", "The side of the screen that the\njoystick will be anchored to." ) );
			EditorGUILayout.PropertyField( joystickTouchSize, new GUIContent( "Touch Size", "The size of the area in which\nthe touch can be initiated." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( uj.joystickTouchSize == UltimateJoystick.JoystickTouchSize.Custom )
					customTouchSizeOption.target = true;
				else
					customTouchSizeOption.target = false;
			}
			if( EditorGUILayout.BeginFadeGroup( customTouchSizeOption.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Touch Size Customization" );
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.Slider( customTouchSize_X, 0.0f, 100.0f, new GUIContent( "Width", "The width of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSize_Y, 0.0f, 100.0f, new GUIContent( "Height", "The height of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_X, 0.0f, 100.0f, new GUIContent( "X Position", "The x position of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_Y, 0.0f, 100.0f, new GUIContent( "Y Position", "The y position of the Joystick Touch Area." ) );
				}
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( SizeAndPlacement.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( dynamicPositioning, new GUIContent( "Dynamic Positioning", "Moves the joystick to the position of the initial touch." ) );
			EditorGUILayout.Slider( joystickSize, 1.0f, 4.0f, new GUIContent( "Joystick Size", "The overall size of the joystick." ) );
			EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can\nmove visually from the center." ) );
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.LabelField( "Joystick Position" );
			EditorGUI.indentLevel = 1;
			EditorGUILayout.Slider( customSpacing_X, 0.0f, 50.0f, new GUIContent( "X Position:", "The horizontal position of the joystick." ) );
			EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:", "The vertical position of the joystick." ) );
			EditorGUI.indentLevel = 0;
			EditorGUILayout.EndVertical();
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END SIZE AND PLACEMENT ** >-------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		#region STYLE AND OPTIONS
		/* ----------------------------------------< ** STYLE AND OPTIONS ** >----------------------------------------- */
		DisplayHeader( "Style and Options", "UUI_StyleAndOptions", StyleAndOptions );
		if( EditorGUILayout.BeginFadeGroup( StyleAndOptions.faded ) )
		{
			EditorGUILayout.Space();
			
			// --------------------------< TOUCH PAD >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( touchPad, new GUIContent( "Touch Pad", "Disables the visuals of the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( uj.touchPad == true )
				{
					highlightOption.target = false;
					tensionOption.target = false;
				}

				SetTouchPad( uj );
				SetHighlight( uj );
				SetTensionAccent( uj );
			}
			
			if( uj.touchPad == true && uj.joystickBase == null )
				EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in the Assigned Variables section.", MessageType.Error );
			// ------------------------< END TOUCH PAD >------------------------ //
			
			// --------------------------< THROWABLE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( throwable, new GUIContent( "Throwable", "Smoothly transitions the joystick back to\ncenter when the input is released." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				throwableOption.target = uj.throwable;
			}
			
			if( EditorGUILayout.BeginFadeGroup( throwableOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( throwDuration, 0.05f, 1.0f, new GUIContent( "Throw Duration", "Time in seconds to return to center." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// ------------------------< END THROWABLE >------------------------ //
			
			// --------------------------< DRAGGABLE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( draggable, new GUIContent( "Draggable", "Drags the joystick to follow the touch if it is farther than the radius." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			
			if( uj.draggable == true && uj.boundary == UltimateJoystick.Boundary.Square )
				EditorGUILayout.HelpBox( "Draggable option will force the boundary to being circular. " +
				                        "Please use a circular boundary when using the draggable option.", MessageType.Warning );
			// ------------------------< END DRAGGABLE >------------------------ //
			
			EditorGUI.BeginDisabledGroup( uj.touchPad == true );// This is the start of the disabled fields if the user is using the touchPad option.

			// --------------------------< HIGHLIGHT >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showHighlight, new GUIContent( "Show Highlight", "Displays the highlight images with the Highlight Color variable." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetHighlight( uj );
				highlightOption.target = uj.showHighlight;
			}
			
			if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					uj.UpdateHighlightColor( uj.highlightColor );
				}
				
				if( uj.highlightBase == null && uj.highlightJoystick == null )
					EditorGUILayout.HelpBox( "No highlight images have been assigned. Please assign some highlight images before continuing.", MessageType.Error );
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// ------------------------< END HIGHLIGHT >------------------------ //
			
			// ---------------------------< TENSION >--------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showTension, new GUIContent( "Show Tension", "Displays the visual direction of the joystick using the tension color options." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetTensionAccent( uj );
				tensionOption.target = uj.showTension;
			}
			
			if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( tensionColorNone, new GUIContent( "Tension None", "The color displayed when the joystick\nis closest to center." ) );
				EditorGUILayout.PropertyField( tensionColorFull, new GUIContent( "Tension Full", "The color displayed when the joystick\nis at the furthest distance." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					TensionAccentReset( uj );
				}
				
				if( uj.tensionAccentUp == null || uj.tensionAccentDown == null || uj.tensionAccentLeft == null || uj.tensionAccentRight == null )
					EditorGUILayout.HelpBox( "Some tension accents are unassigned. Please assign all images before continuing.", MessageType.Error );
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// -------------------------< END TENSION >------------------------- //
			
			EditorGUI.EndDisabledGroup();// This is the end for the Touch Pad option.
			
			// -----------------------------< AXIS >---------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( axis, new GUIContent( "Axis", "Contrains the joystick to a certain axis." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ---------------------------< END AXIS >-------------------------- //
			
			// ---------------------------< BOUNDARY >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( boundary, new GUIContent( "Boundry", "Determines how the joystick's position is clamped." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// -------------------------< END BOUNDARY >------------------------ //
			
			// --------------------------< DEAD ZONE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( deadZoneOption, new GUIContent( "Dead Zone", "Forces the joystick position to being only values of -1, 0, and 1." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				dzOneValueOption.target = uj.deadZoneOption == UltimateJoystick.DeadZoneOption.OneValue ? true : false;
				dzTwoValueOption.target = uj.deadZoneOption == UltimateJoystick.DeadZoneOption.TwoValues ? true : false;
			}
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			if( EditorGUILayout.BeginFadeGroup( dzTwoValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "X Dead Zone", "X values within this range will be forced to 0." ) );
				EditorGUILayout.Slider( yDeadZone, 0.0f, 1.0f, new GUIContent( "Y Dead Zone", "Y values within this range will be forced to 0." ) );
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( dzOneValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "Values within this range will be forced to 0." ) );
				uj.yDeadZone = uj.xDeadZone;
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ------------------------< END DEAD ZONE >------------------------ //
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END STYLE AND OPTIONS ** >------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		#region TOUCH ACTIONS
		/* ------------------------------------------< ** TOUCH ACTIONS ** >----------------------------------------- */
		DisplayHeader( "Touch Actions", "UUI_TouchActions", TouchActions );
		if( EditorGUILayout.BeginFadeGroup( TouchActions.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( tapCountOption, new GUIContent( "Tap Count", "Allows the joystick to calculate double taps and a touch and release within a certain time window." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				tcOption.target = uj.tapCountOption != UltimateJoystick.TapCountOption.NoCount ? true : false;
				tcTargetTapOption.target = uj.tapCountOption == UltimateJoystick.TapCountOption.Accumulate ? true : false;
			}

			if( EditorGUILayout.BeginFadeGroup( tcOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( tapCountEvent );
				EditorGUILayout.Slider( tapCountDuration, 0.0f, 1.0f, new GUIContent( "Tap Time Window", "Time in seconds that the joystick can recieve taps." ) );
				if( EditorGUILayout.BeginFadeGroup( tcTargetTapOption.faded ) )
					EditorGUILayout.IntSlider( targetTapCount, 1, 5, new GUIContent( "Target Tap Count", "How many taps to activate the Tap Count Event?" ) );
				if( TouchActions.faded == 1 )
					EditorGUILayout.EndFadeGroup();

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( TouchActions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useAnimation, new GUIContent( "Use Animation", "Play animation in reaction to input." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetAnimation( uj );
				animationOption.target = uj.useAnimation;
			}
			if( uj.useAnimation == true )
			{
				EditorGUI.indentLevel = 1;
				if( uj.joystickAnimator == null )
					EditorGUILayout.HelpBox( "Joystick Animator needs to be assigned.", MessageType.Error );
				EditorGUI.indentLevel = 0;
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useFade, new GUIContent( "Use Fade", "Fade joystick visuals when touched,\nand released?" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( uj.useFade == true )
					uj.gameObject.GetComponent<CanvasGroup>().alpha = uj.fadeUntouched;
				else
					uj.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;

				fadeOption.target = uj.useFade;
			}
			if( EditorGUILayout.BeginFadeGroup( fadeOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( fadeUntouched, 0.0f, 1.0f, new GUIContent( "Fade Untouched", "The alpha of the joystick when it is NOT receiving input." ) );
				EditorGUILayout.Slider( fadeTouched, 0.0f, 1.0f, new GUIContent( "Fade Touched", "The alpha of the joystick when receiving input." ) );
				EditorGUILayout.PropertyField( fadeInDuration );
				EditorGUILayout.PropertyField( fadeOutDuration );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					uj.gameObject.GetComponent<CanvasGroup>().alpha = uj.fadeUntouched;
				}
				
				EditorGUI.indentLevel = 0;
			}
			if( TouchActions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* ------------------------------------------< ** END TOUCH ACTIONS ** >------------------------------------------ */
		#endregion
		
		EditorGUILayout.Space();
		
		#region SCRIPT REFERENCE
		/* ------------------------------------------< ** SCRIPT REFERENCE ** >------------------------------------------- */
		DisplayHeader( "Script Reference", "UUI_ScriptReference", ScriptReference );
		if( EditorGUILayout.BeginFadeGroup( ScriptReference.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystickName, new GUIContent( "Joystick Name", "The name of the targeted joystick used for static referencing." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( uj.joystickName == string.Empty )
				{
					joystickNameUnassigned.target = true;
					joystickNameAssigned.target = false;
				}
				else
				{
					joystickNameUnassigned.target = false;
					joystickNameAssigned.target = true;
				}
			}

			if( EditorGUILayout.BeginFadeGroup( joystickNameUnassigned.faded ) )
			{
				EditorGUILayout.HelpBox( "Please assign a Joystick Name in order to be able to get this joystick's position dynamically.", MessageType.Warning );
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUILayout.BeginFadeGroup( joystickNameAssigned.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Script Reference:", EditorStyles.boldLabel );
				scriptCast = ( ScriptCast )EditorGUILayout.EnumPopup( "Joystick Use: ", scriptCast );
				GUILayout.Space( 5 );
				if( scriptCast == ScriptCast.Vector2 )
					EditorGUILayout.TextField( "UltimateJoystick.GetPosition( \"" + uj.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.distance )
					EditorGUILayout.TextField( "UltimateJoystick.GetDistance( \"" + uj.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.horizontalFloat )
					EditorGUILayout.TextField( "UltimateJoystick.GetPosition( \"" + uj.joystickName + "\" ).x" );
				else if( scriptCast == ScriptCast.verticalFloat )
					EditorGUILayout.TextField( "UltimateJoystick.GetPosition( \"" + uj.joystickName + "\" ).y" );
				else
					EditorGUILayout.TextField( "UltimateJoystick.GetJoystickState( \"" + uj.joystickName + "\" )" );
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( exposeValues, new GUIContent( "Expose Values", "Should this script expose it's values for certain game making plugins? This option does not effect performance of other references." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				exposeValuesBool.target = uj.exposeValues;
			}

			if( EditorGUILayout.BeginFadeGroup( exposeValuesBool.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Current Position:", EditorStyles.boldLabel );
				EditorGUILayout.LabelField( "Horizontal Value: " + uj.horizontalValue.ToString( "F2" ) );
				EditorGUILayout.LabelField( "Vertical Value: " + uj.verticalValue.ToString( "F2" ) );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* -----------------------------------------< ** END SCRIPT REFERENCE ** >---------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		/* ----------------------------------------------< ** HELP TIPS ** >---------------------------------------------- */

		if( uj.joystick == null )
			EditorGUILayout.HelpBox( "Joystick needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( uj.joystickSizeFolder == null )
			EditorGUILayout.HelpBox( "Joystick Size Folder needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( uj.joystickBase == null )
			EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		/* --------------------------------------------< ** END HELP TIPS ** >-------------------------------------------- */

		Repaint();
	}
	
	// This function stores the references to the variables of the target.
	void StoreReferences ()
	{
		/* -----< ASSIGNED VARIABLES >----- */
		joystick = serializedObject.FindProperty( "joystick" );
		joystickSizeFolder = serializedObject.FindProperty( "joystickSizeFolder" );
		joystickBase = serializedObject.FindProperty( "joystickBase" );
		highlightBase = serializedObject.FindProperty( "highlightBase" );
		highlightJoystick = serializedObject.FindProperty( "highlightJoystick" );
		tensionAccentUp = serializedObject.FindProperty( "tensionAccentUp" );
		tensionAccentDown = serializedObject.FindProperty( "tensionAccentDown" );
		tensionAccentLeft = serializedObject.FindProperty( "tensionAccentLeft" );
		tensionAccentRight = serializedObject.FindProperty( "tensionAccentRight" );
		joystickAnimator = serializedObject.FindProperty( "joystickAnimator" );
		
		/* -----< SIZE AND PLACEMENT >----- */
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		joystickTouchSize = serializedObject.FindProperty( "joystickTouchSize" );
		customTouchSize_X = serializedObject.FindProperty( "customTouchSize_X" );
		customTouchSize_Y = serializedObject.FindProperty( "customTouchSize_Y" );
		customTouchSizePos_X = serializedObject.FindProperty( "customTouchSizePos_X" );
		customTouchSizePos_Y = serializedObject.FindProperty( "customTouchSizePos_Y" );
		dynamicPositioning = serializedObject.FindProperty( "dynamicPositioning" );
		joystickSize = serializedObject.FindProperty( "joystickSize" );
		radiusModifier = serializedObject.FindProperty( "radiusModifier" );
		customSpacing_X = serializedObject.FindProperty( "customSpacing_X" );
		customSpacing_Y = serializedObject.FindProperty( "customSpacing_Y" );
		
		/* -----< STYLES AND OPTIONS >----- */
		touchPad = serializedObject.FindProperty( "touchPad" );
		throwable = serializedObject.FindProperty( "throwable" );
		draggable = serializedObject.FindProperty( "draggable" );
		throwDuration = serializedObject.FindProperty( "throwDuration" );
		showHighlight = serializedObject.FindProperty( "showHighlight" );
		highlightColor = serializedObject.FindProperty( "highlightColor" );
		showTension = serializedObject.FindProperty( "showTension" );
		tensionColorNone = serializedObject.FindProperty( "tensionColorNone" );
		tensionColorFull = serializedObject.FindProperty( "tensionColorFull" );
		axis = serializedObject.FindProperty( "axis" );
		boundary = serializedObject.FindProperty( "boundary" );
		deadZoneOption = serializedObject.FindProperty( "deadZoneOption" );
		xDeadZone = serializedObject.FindProperty( "xDeadZone" );
		yDeadZone = serializedObject.FindProperty( "yDeadZone" );
		
		/* --------< TOUCH ACTION >-------- */
		useAnimation = serializedObject.FindProperty( "useAnimation" );
		useFade = serializedObject.FindProperty( "useFade" );
		tapCountOption = serializedObject.FindProperty( "tapCountOption" );
		tapCountDuration = serializedObject.FindProperty( "tapCountDuration" );
		targetTapCount = serializedObject.FindProperty( "targetTapCount" );
		tapCountEvent = serializedObject.FindProperty( "tapCountEvent" );
		fadeUntouched = serializedObject.FindProperty( "fadeUntouched" );
		fadeTouched = serializedObject.FindProperty( "fadeTouched" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );

		/* ------< SCRIPT REFERENCE >------ */
		joystickName = serializedObject.FindProperty( "joystickName" );
		exposeValues = serializedObject.FindProperty( "exposeValues" );
		
		/* // ----< ANIMATED SECTIONS >---- \\ */
		AssignedVariables = new AnimBool( EditorPrefs.GetBool( "UUI_Variables" ) );
		SizeAndPlacement = new AnimBool( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) );
		StyleAndOptions = new AnimBool( EditorPrefs.GetBool( "UUI_StyleAndOptions" ) );
		TouchActions = new AnimBool( EditorPrefs.GetBool( "UUI_TouchActions" ) );
		ScriptReference = new AnimBool( EditorPrefs.GetBool( "UUI_ScriptReference" ) );
		
		/* // ----< ANIMATED VARIABLES >---- \\ */
		UltimateJoystick ult = ( UltimateJoystick ) target;
		customTouchSizeOption = new AnimBool( ult.joystickTouchSize == UltimateJoystick.JoystickTouchSize.Custom ? true : false );
		throwableOption = new AnimBool( ult.throwable );
		highlightOption = new AnimBool( ult.showHighlight );
		tensionOption = new AnimBool( ult.showTension );
		dzOneValueOption = new AnimBool( ult.deadZoneOption == UltimateJoystick.DeadZoneOption.OneValue ? true : false );
		dzTwoValueOption = new AnimBool( ult.deadZoneOption == UltimateJoystick.DeadZoneOption.TwoValues ? true : false );
		tcOption = new AnimBool( ult.tapCountOption != UltimateJoystick.TapCountOption.NoCount ? true : false );
		tcTargetTapOption = new AnimBool( ult.tapCountOption == UltimateJoystick.TapCountOption.Accumulate ? true : false );
		animationOption = new AnimBool( ult.useAnimation );
		fadeOption = new AnimBool( ult.useFade );

		joystickNameUnassigned = new AnimBool( ult.joystickName != string.Empty ? false : true );
		joystickNameAssigned = new AnimBool( ult.joystickName != string.Empty ? true : false );
		exposeValuesBool = new AnimBool( ult.exposeValues == true ? true : false );

		SetTouchPad( ult );
		SetHighlight( ult );
		SetAnimation( ult );
		SetTensionAccent( ult );

		if( ult.useFade == true )
		{
			if( !ult.GetComponent<CanvasGroup>() )
				ult.gameObject.AddComponent<CanvasGroup>();

			ult.gameObject.GetComponent<CanvasGroup>().alpha = ult.fadeUntouched;
		}
		else
		{
			if( !ult.GetComponent<CanvasGroup>() )
				ult.gameObject.AddComponent<CanvasGroup>();

			ult.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
		}
	}

	#region Internal Set Functions
	void SetTouchPad ( UltimateJoystick ult )
	{
		if( ult.touchPad == true )
		{
			if( ult.showHighlight == true )
				ult.showHighlight = false;
			if( ult.showTension == true )
				ult.showTension = false;

			if( ult.dynamicPositioning == false )
				ult.dynamicPositioning = true;

			if( ult.joystickBase != null && ult.joystickBase.GetComponent<Image>().enabled == true )
				ult.joystickBase.GetComponent<Image>().enabled = false;
				
			if( ult.joystick.GetComponent<Image>().enabled == true )
				ult.joystick.GetComponent<Image>().enabled = false;
		}
		else
		{
			if( ult.joystickBase != null )
			{
				if( ult.joystickBase.GetComponent<Image>().enabled == false )
					ult.joystickBase.GetComponent<Image>().enabled = true;
			}
			if( ult.joystick.GetComponent<Image>().enabled == false )
				ult.joystick.GetComponent<Image>().enabled = true;
		}
	}

	void SetHighlight ( UltimateJoystick uj )
	{
		if( uj.showHighlight == true )
		{
			if( uj.highlightBase != null && uj.highlightBase.gameObject.activeInHierarchy == false )
				uj.highlightBase.gameObject.SetActive( true );
			if( uj.highlightJoystick != null && uj.highlightJoystick.gameObject.activeInHierarchy == false )
				uj.highlightJoystick.gameObject.SetActive( true );
			
			uj.UpdateHighlightColor( uj.highlightColor );
		}
		else
		{
			if( uj.highlightBase != null && uj.highlightBase.gameObject.activeInHierarchy == true )
				uj.highlightBase.gameObject.SetActive( false );
			if( uj.highlightJoystick != null && uj.highlightJoystick.gameObject.activeInHierarchy == true )
				uj.highlightJoystick.gameObject.SetActive( false );
		}
	}
	
	void SetTensionAccent ( UltimateJoystick uj )
	{
		if( uj.showTension == true )
		{
			if( uj.tensionAccentUp == null || uj.tensionAccentDown == null || uj.tensionAccentLeft == null || uj.tensionAccentRight == null )
				return;
			
			if( uj.tensionAccentUp != null && uj.tensionAccentUp.gameObject.activeInHierarchy == false )
				uj.tensionAccentUp.gameObject.SetActive( true );
			if( uj.tensionAccentDown != null && uj.tensionAccentDown.gameObject.activeInHierarchy == false )
				uj.tensionAccentDown.gameObject.SetActive( true );
			if( uj.tensionAccentLeft != null && uj.tensionAccentLeft.gameObject.activeInHierarchy == false )
				uj.tensionAccentLeft.gameObject.SetActive( true );
			if( uj.tensionAccentRight != null && uj.tensionAccentRight.gameObject.activeInHierarchy == false )
				uj.tensionAccentRight.gameObject.SetActive( true );
			
			TensionAccentReset( uj );
		}
		else
		{
			if( uj.tensionAccentUp != null && uj.tensionAccentUp.gameObject.activeInHierarchy == true )
				uj.tensionAccentUp.gameObject.SetActive( false );
			if( uj.tensionAccentDown != null && uj.tensionAccentDown.gameObject.activeInHierarchy == true )
				uj.tensionAccentDown.gameObject.SetActive( false );
			if( uj.tensionAccentLeft != null && uj.tensionAccentLeft.gameObject.activeInHierarchy == true )
				uj.tensionAccentLeft.gameObject.SetActive( false );
			if( uj.tensionAccentRight != null && uj.tensionAccentRight.gameObject.activeInHierarchy == true )
				uj.tensionAccentRight.gameObject.SetActive( false );
		}
	}
	
	void TensionAccentReset ( UltimateJoystick uj )
	{
		uj.tensionAccentUp.color = uj.tensionColorNone;
		uj.tensionAccentDown.color = uj.tensionColorNone;
		uj.tensionAccentLeft.color = uj.tensionColorNone;
		uj.tensionAccentRight.color = uj.tensionColorNone;
	}
	
	void SetAnimation ( UltimateJoystick uj )
	{
		if( uj.useAnimation == true )
		{
			if( uj.joystickAnimator != null )
				if( uj.joystickAnimator.enabled == false )
					uj.joystickAnimator.enabled = true;
		}
		else
		{
			if( uj.joystickAnimator != null )
				if( uj.joystickAnimator.enabled == true )
					uj.joystickAnimator.enabled = false;
		}
	}
	#endregion
}

/* Written by Kaz Crowe */
/* UltimateJoystickCreator.cs ver. 1.0.3 */
public class UltimateJoystickCreator
{
	[MenuItem( "GameObject/UI/Ultimate UI/Ultimate Joystick", false, 0 )]
	private static void CreateUltimateJoystick ()
	{
		GameObject joystickPrefab = EditorGUIUtility.Load( "Ultimate Joystick/UltimateJoystick.prefab" ) as GameObject;

		if( joystickPrefab == null )
		{
			Debug.LogError( "Could not find 'UltimateJoystick.prefab' in any Editor Default Resources folders." );
			return;
		}
		CreateNewUI( joystickPrefab );
	}
	
	[MenuItem( "GameObject/UI/Ultimate UI/Simple Joystick", false, 1 )]
	private static void CreateSimpleJoystick ()
	{
		GameObject joystickPrefab = EditorGUIUtility.Load( "Ultimate Joystick/SimpleJoystick.prefab" ) as GameObject;

		if( joystickPrefab == null )
		{
			Debug.LogError( "Could not find 'SimpleJoystick.prefab' in any Editor Default Resources folders." );
			return;
		}
		CreateNewUI( joystickPrefab );
	}
	
	private static void CreateNewUI ( Object objectPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( objectPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = objectPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}

	private static void CreateNewCanvas ( GameObject child )
	{
		GameObject root = new GameObject( "Ultimate UI Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );

		child.transform.SetParent( root.transform, false );
		
		CreateEventSystem();
	}

	private static void CreateEventSystem ()
	{
		Object esys = Object.FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();
			
			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	/* PUBLIC STATIC FUNCTIONS */
	public static void RequestCanvas ( GameObject child )
	{
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		for( int i = 0; i < allCanvas.Length; i++ )
		{
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && !allCanvas[ i ].GetComponent<CanvasScaler>() )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}
}