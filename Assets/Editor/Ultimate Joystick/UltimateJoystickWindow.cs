/* Written by Kaz Crowe */
/* UltimateJoystickWindow.cs ver 2.0.0 */
// 2.0.0 - Adding completely new editor window to contain full documentation
using UnityEngine;
using UnityEditor;

public class UltimateJoystickWindow : EditorWindow
{
	GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.Height( 35 ) }; 

	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 300 ), GUILayout.Height( 330 ) };

	GUISkin style;

	enum CurrentMenu
	{
		MainMenu,
		HowTo,
		Overview,
		Documentation,
		Extras,
		OtherProducts,
		Feedback,
		ThankYou
	}
	static CurrentMenu currentMenu;
	static string menuTitle = "Main Menu";

	Texture2D scriptReference;
	Texture2D positionVisual;

	Texture2D ubPromo, usbPromo, fstpPromo;

	Vector2 scroll_HowTo = Vector2.zero, scroll_Overview = Vector2.zero, scroll_Docs = Vector2.zero, scroll_Extras = Vector2.zero;
	Vector2 scroll_OtherProd = Vector2.zero, scroll_Feedback = Vector2.zero, scroll_Thanks = Vector2.zero;

	[MenuItem( "Window/Ultimate UI/Ultimate Joystick", false, 0 )]
	static void Init ()
	{
		InitializeWindow();
	}

	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<UltimateJoystickWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	void OnEnable ()
	{
		style = ( GUISkin )EditorGUIUtility.Load( "Ultimate Joystick/UltimateJoystickEditorSkin.guiskin" );

		scriptReference = ( Texture2D )EditorGUIUtility.Load( "Ultimate Joystick/UJ_ScriptRef.jpg" );
		positionVisual = ( Texture2D )EditorGUIUtility.Load( "Ultimate Joystick/UJ_PosVisual.png" );
		ubPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/UB_Promo.png" );
		usbPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/USB_Promo.png" );
		fstpPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/FSTP_Promo.png" );
	}
	
	void OnGUI ()
	{
		if( style == null )
		{
			GUILayout.BeginVertical( "Box" );
			GUILayout.FlexibleSpace();

			ErrorScreen();

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			return;
		}

		GUI.skin = style;

		EditorGUILayout.Space();

		GUILayout.BeginVertical( "Box" );
		
		EditorGUILayout.LabelField( "Ultimate Joystick", GUI.skin.GetStyle( "WindowTitle" ) );

		GUILayout.Space( 3 );

		EditorGUILayout.LabelField( " Version 2.0.2", EditorStyles.whiteMiniLabel );//< ---- ALWAYS UPDATE

		GUILayout.Space( 12 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 5 );
		if( currentMenu != CurrentMenu.MainMenu && currentMenu != CurrentMenu.ThankYou )
		{
			EditorGUILayout.BeginVertical();
			GUILayout.Space( 5 );
			if( GUILayout.Button( "", GUI.skin.GetStyle( "BackButton" ), GUILayout.Width( 80 ), GUILayout.Height( 40 ) ) )
				BackToMainMenu();
			EditorGUILayout.EndVertical();
		}
		else
			GUILayout.Space( 80 );

		GUILayout.Space( 15 );
		EditorGUILayout.BeginVertical();
		GUILayout.Space( 14 );
		EditorGUILayout.LabelField( menuTitle, GUI.skin.GetStyle( "HeaderText" ) );
		EditorGUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 80 );
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		switch( currentMenu )
		{
			case CurrentMenu.MainMenu:
			{
				MainMenu();
			}break;
			case CurrentMenu.HowTo:
			{
				HowTo();
			}break;
			case CurrentMenu.Overview:
			{
				Overview();
			}break;
			case CurrentMenu.Documentation:
			{
				Documentation();
			}break;
			case CurrentMenu.Extras:
			{
				Extras();
			}break;
			case CurrentMenu.OtherProducts:
			{
				OtherProducts();
			}break;
			case CurrentMenu.Feedback:
			{
				Feedback();
			}break;
			case CurrentMenu.ThankYou:
			{
				ThankYou();
			}break;
			default:
			{
				MainMenu();
			}break;
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 20 );
		EditorGUILayout.EndVertical();
	}

	void ErrorScreen ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "ERROR", EditorStyles.boldLabel );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor Default Resources folder. Please ensure that the correct GUISkin, UltimateJoystickEditorSkin, is in the right folder( Editor Default Resources/Ultimate Joystick ) before trying to access the Ultimate Joystick Window.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 50 );
		EditorGUILayout.EndHorizontal();
	}

	void BackToMainMenu ()
	{
		currentMenu = CurrentMenu.MainMenu;
		menuTitle = "Main Menu";
	}
	
	#region MainMenu
	void MainMenu ()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.Space( 25 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "How To", buttonSize ) )
		{
			currentMenu = CurrentMenu.HowTo;
			menuTitle = "How To";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Overview", buttonSize ) )
		{
			currentMenu = CurrentMenu.Overview;
			menuTitle = "Overview";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Documentation", buttonSize ) )
		{
			currentMenu = CurrentMenu.Documentation;
			menuTitle = "Documentation";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Extras", buttonSize ) )
		{
			currentMenu = CurrentMenu.Extras;
			menuTitle = "Extras";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Other Products", buttonSize ) )
		{
			currentMenu = CurrentMenu.OtherProducts;
			menuTitle = "Other Products";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Feedback", buttonSize ) )
		{
			currentMenu = CurrentMenu.Feedback;
			menuTitle = "Feedback";
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndVertical();
	}
	#endregion

	#region HowTo
	void HowTo ()
	{
		scroll_HowTo = EditorGUILayout.BeginScrollView( scroll_HowTo, false, false, docSize );

		EditorGUILayout.LabelField( "How To Create", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   To create a Ultimate Joystick in your scene, simply go up to GameObject / UI / Ultimate UI / Ultimate Joystick. What this does is locates the Ultimate Joystick prefab that is located within the Editor Default Resources folder, and creates an Ultimate Joystick within the scene.\n\nThis method of adding an Ultimate Joystick to your scene ensures that the joystick will have a Canvas and an EventSystem so that it can work correctly.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 20 );

		EditorGUILayout.LabelField( "How To Reference", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   One of the great things about the Ultimate Joystick is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is to name the joystick in the Script Reference section. After this is complete, you will be able to reference that particular joystick by it's name from a static function within the Ultimate Joystick script.\n\nAfter the joystick has been given a name in the Script Reference section, we can get that joystick's position by creating a Vector2 variable at run time and storing the result from the static function: 'GetPosition'. This Vector2 will be the joystick's position, and will contain float values between -1, and 1, with 0 being at the center.\n\nKeep in mind that the joystick's left and right ( horizontal ) movement is translated into this Vector2's X value, while the up and down ( vertical ) is the Vector2's Y value. This will be important when applying the Ultimate Joystick's position to your scripts.", EditorStyles.wordWrappedLabel );
			
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 10 );
		GUILayout.Label(positionVisual, GUILayout.Width( 200 ), GUILayout.Height( 200 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 20 );

		EditorGUILayout.LabelField( "Example", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   Let's assume that we want to use a joystick for a characters movement. The first thing to do is to assign the name \"Movement\" in the Joystick Name variable located in the Script Reference section of the Ultimate Joystick.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "After that, we need to create a Vector2 variable to store the result of the joystick's position returned by the 'GetPosition' function. In order to get the \"Movement\" joystick's position, we need to pass in the name \"Movement\" as the argument.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "C# Example:", EditorStyles.boldLabel );
		EditorGUILayout.TextArea( "Vector2 joystickPosition = UltimateJoystick.GetPosition( \"Movement\" );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Javascript Example:", EditorStyles.boldLabel );
		EditorGUILayout.TextArea( "var joystickPosition : Vector2 = UltimateJoystick.GetPosition( \"Movement\" );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "The joystickPosition variable now contains the values of the position that the Movement joystick was in at the movement it was referenced. Now we can use this information in any way that is desired. For example, if you are wanting to put the joystick's position into a character movement script, you would create a Vector3 variable for movement direction, and put in the appropriate values of the Ultimate Joystick's position.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "C# Example:", EditorStyles.boldLabel );
		EditorGUILayout.TextArea( "Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Javascript Example:", EditorStyles.boldLabel );
		EditorGUILayout.TextArea( "var movementDirection : Vector3 = new Vector3( joystickPosition.x, 0, joystickPosition.y );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z directions. This is because you generally don't want your character to move in the Y direction unless the user jumps. That is why we put the joystickPosition.y value into the Z value of the movementDirection variable.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Understanding how to use the values from any input is important when creating character controllers, so experiment with the values and try to understand how the mobile input can be used in different ways.", EditorStyles.wordWrappedLabel );

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Overview
	void Overview ()
	{
		scroll_Overview = EditorGUILayout.BeginScrollView( scroll_Overview, false, false, docSize );

		EditorGUILayout.LabelField( "Assigned Variables", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   In the Assigned Variables section, there are a few components that should already be assigned if you are using one of the Prefabs that has been provided. If not, you will see error messages on the Ultimate Joystick inspector that will help you to see if any of these variables are left unassigned. Please note that these need to be assigned in order for the Ultimate Joystick to work properly.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 20 );
		
		/* //// --------------------------- < SIZE AND PLACEMENT > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Size And Placement", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   The Size and Placement section allows you to customize the joystick's size and placement on the screen, as well as determine where the user's touch can be processed for the selected joystick.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// Scaling Axis
		EditorGUILayout.LabelField( "Scaling Axis", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Determines which axis the joystick will be scaled from. If Height is chosen, then the joystick will scale itself proportionately to the Height of the screen.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// Anchor
		EditorGUILayout.LabelField( "Anchor", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Determines which side of the screen that the joystick will be anchored to.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Touch Size
		EditorGUILayout.LabelField( "Touch Size", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Touch Size configures the size of the area where the user can touch. You have the options of either 'Default','Medium', 'Large' or 'Custom'. When the option 'Custom' is selected, an additional box will be displayed that allows for a more adjustable touch area.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// Touch Size Customization
		EditorGUILayout.LabelField( "Touch Size Customization", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "If the 'Custom' option of the Touch Size is selected, then you will be presented with the Touch Size Customization box. Inside this box are settings for the Width and Height of the touch area, as well as the X and Y position of the touch area on the screen.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// Dynamic Positioning
		EditorGUILayout.LabelField( "Dynamic Positioning", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Dynamic Positioning will make the joystick snap to where the user touches, instead of the user having to touch a direct position to get the joystick. The Touch Size option will directly affect the area where the joystick can snap to.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// Joystick Size
		EditorGUILayout.LabelField( "Joystick Size", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Joystick Size will change the scale of the joystick. Since everything is calculated out according to screen size, your joystick Touch Size and other properties will scale proportionately with the joystick's size along your specified Scaling Axis.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// Radius
		EditorGUILayout.LabelField( "Radius", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Radius determines how far away the joystick will move from center when it is being used, and will scale proportionately with the joystick.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// Joystick Position
		EditorGUILayout.LabelField( "Joystick Position", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Joystick Position will present you with two sliders. The X value will determine how far the Joystick is away from the Left and Right sides of the screen, and the Y value from the Top and Bottom. This will encompass 50% of your screen, relevant to your Anchor selection.", EditorStyles.wordWrappedLabel );
		/* \\\\ -------------------------- < END SIZE AND PLACEMENT > --------------------------- //// */

		GUILayout.Space( 20 );

		/* //// ----------------------------- < STYLE AND OPTIONS > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Style And Options", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   The Style and Options section contains options that affect how the joystick handles and is visually presented to the user.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Touch Pad
		EditorGUILayout.LabelField( "Touch Pad", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Touch Pad presents you with the option to disable the visuals of the joystick, whilst keeping all functionality. When paired with Dynamic Positioning and Throwable, this option can give you a very smooth camera control.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Throwable
		EditorGUILayout.LabelField( "Throwable", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "The Throwable option gives you the option to have the joystick smoothly transition back to center after being let go of. This can be used to give your input a better feel.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// <------------------------------ FIX

		// Draggable
		EditorGUILayout.LabelField( "Draggable", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "The Draggable option will allow the joystick to move from it's default position when the joystick's position exceeds the set radius.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// <------------------------------ FIX

		// Show Highlight
		EditorGUILayout.LabelField( "Show Highlight", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Show Highlight will allow you to customize the set highlight images with a custom color. With this option, you will also be able to customize and set these images at runtime using the UpdateHighlightColor function. See the Documentation section for more details.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// <------------------------------ FIX

		// Show Tension
		EditorGUILayout.LabelField( "Show Tension", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "With Show Tension enabled, the joystick will display it's position visually using custom colors and images to display the direction and intensity of the position. With this option enabled, you will be able to update the tension colors at runtime using the UpdateTensionColors function. See the Documentation section for more information.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// Axis
		EditorGUILayout.LabelField( "Axis", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Axis determines which axis the joystick will snap to. By default it is set to Both, which means the joystick will use both the X and Y axis for movement. If either the X or Y option is selected, then the joystick will snap to the corresponding axis.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Boundary
		EditorGUILayout.LabelField( "Boundary", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Boundary will allow you to decide if you want to have a square or circular edge to your joystick.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Dead Zone
		EditorGUILayout.LabelField( "Dead Zone", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Dead Zone gives the option of setting one or two values that the joystick is constrained by. When selected, the joystick will be forced to a maximum value when it has past the set dead zone.", EditorStyles.wordWrappedLabel );
		/* //// --------------------------- < END STYLE AND OPTIONS > --------------------------- \\\\ */

		GUILayout.Space( 20 );

		/* //// ------------------------------- < TOUCH ACTIONS > ------------------------------- \\\\ */
		EditorGUILayout.LabelField( "Touch Actions", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   The Touch Actions section contains options for when the joystick is being interacted with, such as displaying when is the touch is intiated or released.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Tap Count
		EditorGUILayout.LabelField( "Tap Count", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "The Tap Count option allows you to decide if you want to store the amount of taps that the joystick recieves. The options provided with the Tap Count will allow you to customize the target amount of taps, the tap time window, and the event to be called when the tap count has been acheived.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Use Animation
		EditorGUILayout.LabelField( "Use Animation", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "If you would like the joystick to play an animation when being interacted with, then you will want to enable the Use Animation option.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// Use Fade
		EditorGUILayout.LabelField( "Use Fade", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "The Use Fade option will present you with settings for the targeted alpha for the touched and untouched states, as well as the duration for the fade between the targeted alpha settings.", EditorStyles.wordWrappedLabel );
		/* //// ----------------------------- < END TOUCH ACTIONS > ----------------------------- \\\\ */
		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Documentation
	void Documentation ()
	{
		scroll_Docs = EditorGUILayout.BeginScrollView( scroll_Docs, false, false, docSize );
		
		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", GUI.skin.GetStyle( "SectionHeader" ) );

		GUILayout.Space( 5 );

		// Vector2 JoystickPosition
		EditorGUILayout.LabelField( "Vector2 JoystickPosition", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the Ultimate Joystick's position in a Vector2 variable. The X value that is returned represents the Left and Right( Horizontal ) movement of the joystick, whereas the Y value represents the Up and Down( Vertical ) movement of the joystick. The values returned will always be between -1 and 1, with 0 being the center.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// float JoystickDistance
		EditorGUILayout.LabelField( "float JoystickDistance", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. The value returned will always be a value between 0 and 1.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// UpdatePositioning()
		EditorGUILayout.LabelField( "UpdatePositioning()", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the size and positioning of the Ultimate Joystick. This function can be used to update any options that may have been changed prior to Start().", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// ResetJoystick()
		EditorGUILayout.LabelField( "ResetJoystick()", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Resets the joystick back to it's neutral state.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// bool joystickState
		EditorGUILayout.LabelField( "bool JoystickState", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the state that the joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// UpdateHighlightColor()
		EditorGUILayout.LabelField( "UpdateHighlightColor( Color targetColor )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the colors of the assigned highlight images with the targeted color if the showHighlight variable is set to true. The targetColor variable will overwrite the current color setting for highlightColor and apply immidiately to the highlight images.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// UpdateTensionColors()
		EditorGUILayout.LabelField( "UpdateTensionColors( Color targetTensionNone, Color targetTensionFull )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the tension accent image colors with the targeted colors if the showTension variable is true. The tension colors will be set to the targeted colors, and will be applied when the joystick is next used.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 20 );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", GUI.skin.GetStyle( "SectionHeader" ) );

		GUILayout.Space( 5 );

		// UltimateJoystick.GetPosition
		EditorGUILayout.LabelField( "Vector2 UltimateJoystick.GetPosition( string joystickName )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the Ultimate Joystick's position in a Vector2 variable. This static function will return the same exact value as the JoystickPosition function. See JoystickPosition for more information.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 5 );

		// UltimateJoystick.GetDistance
		EditorGUILayout.LabelField( "float UltimateJoystick.GetDistance( string joystickName )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. This static function will return the same value as the JoystickDistance function. See JoystickDistance for more information", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// UltimateJoystick.UpdatePositioning
		EditorGUILayout.LabelField( "UltimateJoystick.UpdatePositioning( string joystickName )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the size and positioning of the Ultimate Joystick. This static function will call the public UpdatePositioning function of the referenced joystick. See UpdatePositioning() for more information.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// UltimateJoystick.ResetJoystick
		EditorGUILayout.LabelField( "UltimateJoystick.ResetJoystick( string joystickName )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Resets the joystick back to it's neutral state.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// UltimateJoystick.GetJoystickState
		EditorGUILayout.LabelField( "bool UltimateJoystick.GetJoystickState( string joystickName )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Returns the state that the joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// UltimateJoystick.UpdateHighlightColor()
		EditorGUILayout.LabelField( "UltimateJoystick.UpdateHighlightColor( string joystickName, Color targetColor )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the colors of the assigned highlight images with the targeted color. This static function calls the public UpdateHighlightColor function of the referenced joystick. See UpdateHighlightColor() above for more information.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// UltimateJoystick.UpdateTensionColors()
		EditorGUILayout.LabelField( "UltimateJoystick.UpdateTensionColors( string joystickName, Color targetTensionNone, Color targetTensionFull )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Updates the tension accent image colors with the targeted colors. This static function calls the public UpdateTensionColors function of the referenced joystick. See UpdateTensionColors() above for more information.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 20 );

		/* //// --------------------------- < VARIABLES > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Variables", GUI.skin.GetStyle( "SectionHeader" ) );

		GUILayout.Space( 5 );
		
		// joystick
		EditorGUILayout.LabelField( "joystick ( typeof( RectTransform ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The Joystick graphic that will move.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// joystickSizeFolder
		EditorGUILayout.LabelField( "joystickSizeFolder ( typeof( RectTransform ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The parent RectTransform that is used to size all of the joystick components within it.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// joystickBase
		EditorGUILayout.LabelField( "joystickBase ( typeof( RectTransform ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The Joystick Base RectTransform. This variable is used to determine the reset position of the joystick when not in use, as well as the default position if the Dynamic Positioning option is selected.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// highlightBase
		EditorGUILayout.LabelField( "highlightBase ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The highlight image to be used for the base of the joystick.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// highlightJoystick
		EditorGUILayout.LabelField( "highlightJoystick ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The highlight image to be used for the joystick.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// tensionAccentUp
		EditorGUILayout.LabelField( "tensionAccentUp ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The image to be used to display the tension in the up direction.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// tensionAccentDown
		EditorGUILayout.LabelField( "tensionAccentDown ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The image to be used to display the tension in the down direction.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// tensionAccentLeft
		EditorGUILayout.LabelField( "tensionAccentLeft ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The image to be used to display the tension in the left direction.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// tensionAccentRight
		EditorGUILayout.LabelField( "tensionAccentRight ( typeof( Image ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The image to be used to display the tension in the right direction.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// scalingAxis
		EditorGUILayout.LabelField( "scalingAxis ( typeof( UltimateJoystick.ScalingAxis ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option will determine which axis the joystick will stick to. If Height is selected, then the joystick will maintain a size that is relevant to the height of the screen. This option is useful to maintain the look of the joystick among different screen resolutions.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		// anchor
		EditorGUILayout.LabelField( "anchor ( typeof( UltimateJoystick.Anchor ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The anchor option will anchor the joystick to either the Left or Right side of the screen.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// joystickTouchSize
		EditorGUILayout.LabelField( "joystickTouchSize ( typeof( UltimateJoystick.JoystickTouchSize ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The options provided with the joystickTouchSize variable will determine how much of an area the user can hit to initiate the touch on the joystick.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( 5 );

		// joystickSize
		EditorGUILayout.LabelField( "joystickSize ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option determines the overall size of the joystick.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// radiusModifier
		EditorGUILayout.LabelField( "radiusModifier ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The radiusModifier variable will be used to calculate how far the joystick graphic can travel from the center of the touch area.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// dynamicPositioning
		EditorGUILayout.LabelField( "dynamicPositioning ( typeof( boolean ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Dynamic Positioning will make the joystick centered on the position of the user's initial touch on the joystick.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// customTouchSize_X
		EditorGUILayout.LabelField( "customTouchSize_X ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option is available when the joystickTouchSize option is set to 'Custom'. This option will determine how wide the custom touch area is.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( 5 );

		// customTouchSize_Y
		EditorGUILayout.LabelField( "customTouchSize_Y ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option is available when the joystickTouchSize option is set to 'Custom'. This option will determine the height of the custom touch area.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// customTouchSizePos_X
		EditorGUILayout.LabelField( "customTouchSizePos_X ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option is available when the joystickTouchSize option is set to 'Custom'. This option will determine the horizontal position of the joystick's touch area.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// customTouchSizePos_Y
		EditorGUILayout.LabelField( "customTouchSizePos_Y ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option is available when the joystickTouchSize option is set to 'Custom'. This option will determine the vertical position of the joystick's touch area.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// customSpacing_X
		EditorGUILayout.LabelField( "customSpacing_X ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option determines the horizontal position of the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// customSpacing_Y
		EditorGUILayout.LabelField( "customSpacing_Y ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option determines the vertical position of the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// touchPad
		EditorGUILayout.LabelField( "touchPad ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The Touch Pad option chooses if the joystick should be visible or not.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// throwable
		EditorGUILayout.LabelField( "throwable ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The throwable option determines if the joystick should return to center gradually or not. If enabled, the joystick will smoothly transition back to center when released.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// throwDuration
		EditorGUILayout.LabelField( "throwDuration ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Throw Duration determines how long in seconds the joystick will take to return back to a neutral position if the throwable option is enabled.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// draggable
		EditorGUILayout.LabelField( "draggable ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option will allow the joystick to move away from it's default position when the joystick's position exceeds the set radius.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// showHighlight
		EditorGUILayout.LabelField( "showHighlight ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "If the showHighlight option is enabled, the joystick will be able to display a custom highlight color on the set highlight images.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// highlightColor
		EditorGUILayout.LabelField( "highlightColor ( typeof( Color ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The custom color for the highlight images on the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// showTension
		EditorGUILayout.LabelField( "showTension ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "If the showTension option is enabled, the joystick will display tension when it is being used.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// tensionColorNone
		EditorGUILayout.LabelField( "tensionColorNone ( typeof( Color ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The custom color to be displayed when no tension is on the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// tensionColorFull
		EditorGUILayout.LabelField( "tensionColorFull ( typeof( Color ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The custom color to be displayed when there is full tension of the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// axis
		EditorGUILayout.LabelField( "axis ( typeof( UltimateJoystick.Axis ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The axis variable will allow the joystick to be clamped along the X, Y, or both axis.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// boundary
		EditorGUILayout.LabelField( "boundary ( typeof( UltimateJoystick.Boundary ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Boundary determines if the joystick's radius should be clamped to either a square or circular boundary.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// deadZoneOption
		EditorGUILayout.LabelField( "deadZoneOption ( typeof( UltimateJoystick.DeadZoneOption ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The deadZoneOption will send a modified joystick's position that is determined by the Dead Zone position variables below. If the deadZoneOption is used, then the joystick's position will be either -1, 0, or 1 on each axis used.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// xDeadZone
		EditorGUILayout.LabelField( "xDeadZone ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The horizontal distance before the joystick's position will be changed from 0 to either -1 or 1.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// yDeadZone
		EditorGUILayout.LabelField( "yDeadZone ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The vertical distance before the joystick's position will be changed from 0 to either -1 or 1.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );
		
		// tapCountOption
		EditorGUILayout.LabelField( "tapCountOption ( typeof( UltimateJoystick.TapCountOption ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "This option determines if the joystick should count a certain number of taps, and call a custom defined function when the target number of taps is acheived.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// tapCountDuration
		EditorGUILayout.LabelField( "tapCountDuration ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The time is seconds that the user can attempt the tap count.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// tapCountEvent
		EditorGUILayout.LabelField( "tapCountEvent ( typeof( UnityEvent ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The targeted function to be called when the tap count is achieved.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// targetTapCount
		EditorGUILayout.LabelField( "targetTapCount ( typeof( int ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The amount of taps required in order to acheive the tap count.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// useAnimation
		EditorGUILayout.LabelField( "useAnimation ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Determines if the joystick should display animations when being interacted with.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// joystickAnimator
		EditorGUILayout.LabelField( "joystickAnimator ( typeof( Animator ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The animator associated with the joystick.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// useFade
		EditorGUILayout.LabelField( "useFade ( typeof( bool ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Determines if the joystick should fade it's alpha channel when recieving input.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// fadeUntouched
		EditorGUILayout.LabelField( "fadeUntouched ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The targeted alpha when not being interacted with.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// fadeTouched
		EditorGUILayout.LabelField( "fadeTouched ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The targted alpha when the joystick is being interacted with.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// fadeInDuration
		EditorGUILayout.LabelField( "fadeInDuration ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The time is seconds to fade in to the targeted alpha.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );
		
		// fadeOutDuration
		EditorGUILayout.LabelField( "fadeOutDuration ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The time is seconds to fade out to the targeted alpha.", EditorStyles.wordWrappedLabel );
						
		GUILayout.Space( 5 );

		// exposeValues
		EditorGUILayout.LabelField( "exposeValues ( typeof( boolean ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "Determines if the Ultimate Joystick should expose the position values.", EditorStyles.wordWrappedLabel );
								
		GUILayout.Space( 5 );

		// horizontalValue
		EditorGUILayout.LabelField( "horizontalValue ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The current horizontal position of the joystick exposed in the editor for certain game making plugins.", EditorStyles.wordWrappedLabel );
										
		GUILayout.Space( 5 );

		// verticalValue
		EditorGUILayout.LabelField( "verticalValue ( typeof( float ) )", GUI.skin.GetStyle( "ItemHeader" ) );
		EditorGUILayout.LabelField( "The current vertical position of the joystick exposed in the editor for certain game making plugins.", EditorStyles.wordWrappedLabel );

		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region Extras
	void Extras ()
	{
		scroll_Extras = EditorGUILayout.BeginScrollView( scroll_Extras, false, false, docSize );
		EditorGUILayout.LabelField( "Videos", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   The links below are to the collection of videos that we have made in connection with the Ultimate Joystick. The Tutorial Videos are designed to get the Ultimate Joystick implemented into your project as fast as possible, and give you a good understanding of what you can achieve using it in your projects, whereas the demonstrations are videos showing how we, and others in the Unity community, have used assets created by Tank & Healer Studio in our projects.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Tutorials", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TmWdbR_bklluPeElJ_xUdO9" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Demonstrations", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TlkjepDAY_GnpA1CX-rFltz" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Example Scripts", GUI.skin.GetStyle( "SectionHeader" ) );
		EditorGUILayout.LabelField( "   Below is a link to a list of free example scripts that we have made available on our support website. Please feel free to use these as an example of how to get started on your own scripts. The scripts provided are fully commented to help you to grasp the concept behind the code. These scripts are by no means a complete solution, and are not intended to be used in finished projects.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Example Scripts", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/uj-example-scripts.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	#region OtherProducts
	void OtherProducts ()
	{
		scroll_OtherProd = EditorGUILayout.BeginScrollView( scroll_OtherProd, false, false, docSize );

		/* -------------- < ULTIMATE BUTTON > -------------- */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( ubPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );

		EditorGUILayout.LabelField( "Ultimate Button", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   Buttons are a core element of UI, and as such they should be easy to customize and implement. The Ultimate Button is the embodiment of that very idea. This code package takes the best of Unity's Input and UnityEvent methods and pairs it with exceptional customization to give you the most versatile button for your mobile project. Are you in need of a button for attacking, jumping, shooting, or all of the above? With Ultimate Button's easy size and placement options, style options, and touch actions, you'll have everything you need to create your custom buttons, whether they are simple or complex.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-button.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE BUTTON > ------------ */

		GUILayout.Space( 25 );

		/* ------------ < ULTIMATE STATUS BAR > ------------ */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( usbPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );

		EditorGUILayout.LabelField( "Ultimate Status Bar", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   The Ultimate Status Bar is a complete solution for displaying health, mana, energy, stamina, experience, or virtually any condition that you'd like like your player to be aware of. It can also be used to show a selected target's health, the progress of loading or casting, and even interacting with objects. Whatever type of progress display that you need, the Ultimate Status Bar can make it visually happen. Additionally, you have the option of using the many \"Ultimate\" textures provided, or you can easily use your own! If you are looking for a way to neatly display any type of status for your game, then consider the Ultimate Status Bar!", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-status-bar.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END STATUS BAR > --------------- */

		GUILayout.Space( 25 );

		/* -------------- < FROST STONE > -------------- */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( fstpPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );

		EditorGUILayout.LabelField( "Frost Stone: UI Texture Pack", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   This package is made to compliment Ultimate Joystick, Ultimate Button and Ultimate Status Bar. The Frost Stone: UI Texture Pack is an inspiring new look for your Ultimate Joystick, Ultimate Button and Ultimate Status Bar. These Frost Stone Textures will flawlessly blend with your current Ultimate UI code to give your game an incredible new look.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/frost-stone-texture-pack.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END FROST STONE > ------------ */

		EditorGUILayout.EndScrollView();
	}
	#endregion

	#region Feedback
	void Feedback ()
	{
		scroll_Feedback = EditorGUILayout.BeginScrollView( scroll_Feedback, false, false, docSize );

		EditorGUILayout.LabelField( "Having Problems?", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   If you experience any issues with the Ultimate Joystick, please email us right away. We will lend any assistance that we can to resolve any issues that you have.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		EditorGUILayout.LabelField( "Support Email:\n    tankandhealerstudio@outlook.com" , EditorStyles.boldLabel, GUILayout.Height( 30 ) );

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Good Experiences?", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   If you have appreciated how easy the Ultimate Joystick is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/en/#!/content/27695" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Show Us What You've Done!", GUI.skin.GetStyle( "SectionHeader" ) );

		EditorGUILayout.LabelField( "   If you have used any of the assets created by Tank & Healer Studio in your project, we would love to see what you have done. Contact us with any information on your game and we will be happy to support you in any way that we can!", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 5 );

		EditorGUILayout.LabelField( "Contact Us:\n    tankandhealerstudio@outlook.com" , EditorStyles.boldLabel, GUILayout.Height( 30 ) );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.EndScrollView();
	}
	#endregion

	#region ThankYou
	void ThankYou ()
	{
		scroll_Thanks = EditorGUILayout.BeginScrollView( scroll_Thanks, false, false, docSize );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "    The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Joystick asset package from the Unity Asset Store. If you have any questions about the Ultimate Joystick that are not covered in this Documentation Window, please don't hesitate to contact us at: ", EditorStyles.wordWrappedLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "       tankandhealerstudio@outlook.com" , EditorStyles.boldLabel );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "    We hope that the Ultimate Joystick will be a great help to you in the development of your game. After pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Ultimate Joystick into your project.\n", EditorStyles.wordWrappedLabel );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
		{
			EditorPrefs.SetBool( "UltimateJoystickStartup", true );
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( "Assets/Plugins/Ultimate Joystick/README.txt" );
			BackToMainMenu();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	#endregion
	
	[InitializeOnLoad]
	class UltimateJoystickInitialLoad
	{
		static UltimateJoystickInitialLoad ()
		{
			if( EditorPrefs.GetBool( "UltimateJoystickStartup" ) == false )
				EditorApplication.update += WaitForCompile;
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;
				
			currentMenu = CurrentMenu.ThankYou;
			menuTitle = "Thank You";

			InitializeWindow();

			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath( "Assets/Plugins/Ultimate Joystick/README.txt" );
		}
	}
}