/* Written by Kaz Crowe */
/* UltimateJoystick.cs ver. 1.7.2 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/*
 * First off, the script is using [ExecuteInEditMode] to be able to show changes in real time.
 * This will not affect anything within a build or play mode. This simply makes the script
 * able to be run while in the Editor in Edit Mode.
*/
[ExecuteInEditMode]
public class UltimateJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	/* ----- > ASSIGNED VARIABLES < ----- */
	public RectTransform joystick, joystickSizeFolder, joystickBase;
	RectTransform baseTrans;
	Vector2 textureCenter = Vector2.zero, defaultPos = Vector2.zero;
	Vector3 joystickCenter = Vector3.zero;
	public Image highlightBase, highlightJoystick;
	public Image tensionAccentUp, tensionAccentDown;
	public Image tensionAccentLeft, tensionAccentRight;

	/* ----- > SIZE AND PLACEMENT < ----- */
	public enum ScalingAxis{ Width, Height }
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor{ Left, Right }
	public Anchor anchor = Anchor.Left;
	public enum JoystickTouchSize{ Default, Medium, Large, Custom }
	public JoystickTouchSize joystickTouchSize = JoystickTouchSize.Default;
	public float joystickSize = 1.75f, radiusModifier = 4.5f;
	float radius = 1.0f;
	public bool dynamicPositioning = false;
	public float customTouchSize_X = 50.0f, customTouchSize_Y = 75.0f;
	public float customTouchSizePos_X = 0.0f, customTouchSizePos_Y = 0.0f;
	public float customSpacing_X = 5.0f, customSpacing_Y = 20.0f;

	/* ----- > STYLE AND OPTIONS < ----- */
	public bool touchPad = false, throwable = false, draggable = false;
	public float throwDuration = 0.5f;
	bool isThrowing = false;
	public bool showHighlight = false;
	public Color highlightColor = new Color( 1, 1, 1, 1 );
	public bool showTension = false;
	public Color tensionColorNone = new Color( 1, 1, 1, 1 ), tensionColorFull = new Color( 1, 1, 1, 1 );
	public enum Axis{ Both, X, Y }
	public Axis axis = Axis.Both;
	public enum Boundary{ Circular, Square }
	public Boundary boundary = Boundary.Circular;
	public enum DeadZoneOption{ DoNotUse, OneValue, TwoValues }
	public DeadZoneOption deadZoneOption = DeadZoneOption.DoNotUse;
	public float xDeadZone = 0.1f, yDeadZone = 0.1f;

	/* ----- > TOUCH ACTIONS < ----- */
	public enum TapCountOption{ NoCount, Accumulate, TouchRelease }
	public TapCountOption tapCountOption = TapCountOption.NoCount;
	public float tapCountDuration = 0.5f;
	public UnityEvent tapCountEvent;
	public int targetTapCount = 2;
	float currentTapTime = 0.0f;
	int tapCount = 0;
	public Animator joystickAnimator;
	public bool useAnimation = false, useFade = false;
	int animationID = 0;
	CanvasGroup joystickGroup;
	public float fadeUntouched = 1.0f, fadeTouched = 0.5f;
	public float fadeInDuration = 1.0f, fadeOutDuration = 1.0f;
	float fadeInSpeed = 1.0f, fadeOutSpeed = 1.0f;

	/* ----- > SCRIPT REFERENCE < ----- */
	static Dictionary<string,UltimateJoystick> UltimateJoysticks = new Dictionary<string, UltimateJoystick>();
	public string joystickName;
	public bool exposeValues = false;
	public float horizontalValue = 0.0f;
	public float verticalValue = 0.0f;
	bool joystickState = false;


	void Awake ()
	{
		// If the game is not being run and the joystick name has been assigned, then register the joystick.
		if( Application.isPlaying == true && joystickName != string.Empty )
			RegisterJoystick( joystickName );
	}

	void Start ()
	{
		if( Application.isPlaying == false )
			return;

		// First off, UpdatePositioning of the joystick.
		UpdatePositioning();

		// Set the highlight is the user is wanting to show highlight.
		if( showHighlight == true )
			UpdateHighlightColor( highlightColor );

		// Reset the tension accents if the user is wanting to show tension.
		if( showTension == true )
			TensionAccentReset();

		if( useFade == true )
		{
			// Configure the fade speeds.
			fadeInSpeed = 1.0f / fadeInDuration;
			fadeOutSpeed = 1.0f / fadeOutDuration;
		}

		// Get the hash ID of the targeted animation if the user is wanting to show animation.
		if( useAnimation == true )
			animationID = Animator.StringToHash( "Touch" );

		if( !GetParentCanvas().GetComponent<UltimateJoystickUpdater>() )
			GetParentCanvas().gameObject.AddComponent( typeof( UltimateJoystickUpdater ) );
	}

	// This function is called on the initial touch down.
	public void OnPointerDown ( PointerEventData touchInfo )
	{
		// Set the joystick state since the joystick is being interacted with.
		joystickState = true;

		// If the throwable option is selected and isThrowing, then stop the current movement.
		if( throwable == true && isThrowing == true )
			StopCoroutine( "ThrowableMovement" );

		// If dynamicPositioning or touchpad are enabled...
		if( dynamicPositioning == true || touchPad == true )
		{
			// Then move the joystickSizeFolder to the position of the touch.
			joystickSizeFolder.position = touchInfo.position - textureCenter;

			// Set the joystickCenter so that the position can be calculated correctly.
			joystickCenter = touchInfo.position;
		}

		// If the user wants animation to be shown, do that here.
		if( useAnimation == true )
			joystickAnimator.SetBool( animationID, true );

		// If the user wants the joystick to fade, do that here.
		if( useFade == true && joystickGroup != null )
			StartCoroutine( "FadeLogic" );

		// If the user is wanting to use any tap count...
		if( tapCountOption != TapCountOption.NoCount )
		{
			// If the user is accumulating taps...
			if( tapCountOption == TapCountOption.Accumulate )
			{
				// If the TapCountdown is not counting down...
				if( currentTapTime <= 0 )
				{
					// Set tapCount to 1 since this is the initial touch and start the TapCountdown.
					tapCount = 1;
					StartCoroutine( "TapCountdown" );
				}
				// Else the TapCountdown is currently counting down, so increase the current tapCount.
				else
					++tapCount;
			}
			// Else the user wants to touch and release, so start the TapCountdown timer.
			else
				StartCoroutine( "TapCountdown" );
		}

		// Call UpdateJoystick with the info from the current PointerEventData.
		UpdateJoystick( touchInfo );
	}

	// This function is called when the user is dragging the joystick.
	public void OnDrag ( PointerEventData touchInfo )
	{
		// Call UpdateJoystick with the info from the current PointerEventData.
		UpdateJoystick( touchInfo );
	}

	// This function is called when the user has released the touch.
	public void OnPointerUp ( PointerEventData touchInfo )
	{
		// Set the joystickState bool to false since the touch has lifted.
		joystickState = false;

		// If dynamicPositioning, touchpad, or draggable are enabled...
		if( dynamicPositioning == true || touchPad == true || draggable == true )
		{
			// The joystickSizeFolder needs to be reset back to the default position.
			joystickSizeFolder.position = defaultPos;

			// Reset the joystickCenter since the touch has been released.
			joystickCenter = joystickBase.position;
		}

		// If the user has the throwable option enable, begin ThrowableMovement().
		if( throwable == true )
			StartCoroutine( "ThrowableMovement" );
		else
		{
			// Reset the joystick's position back to center.
			joystick.position = joystickCenter;

			// If the user has showHighlight enabled, and the highlightJoystick variable is assigned, reset it too.
			if( showHighlight == true && highlightJoystick != null )
				highlightJoystick.transform.position = joystickCenter;
		}

		// If the user has showTension enabled, then reset the tension if throwable is disabled.
		if( showTension == true && throwable == false )
			TensionAccentReset();

		// If the user has useAnimation enabled, set that here.
		if( useAnimation == true )
			joystickAnimator.SetBool( animationID, false );
		
		// If the user is wanting to use the TouchAndRelease tapcount...
		if( tapCountOption == TapCountOption.TouchRelease )
		{
			// If the tapTime is still above zero, then invoke the tapCountEvent.
			if( currentTapTime > 0 )
				tapCountEvent.Invoke();

			// Reset the current tap time to zero.
			currentTapTime = 0;
		}

		// Set the reference to the x and y position so they can be referenced from PlayMaker or whatever else..
		if( exposeValues == true && throwable == false )
		{
			horizontalValue = 0.0f;
			verticalValue = 0.0f;
		}
	}

	// This function updates the Ultimate Joystick according to the current touch.
	void UpdateJoystick ( PointerEventData touchInfo )
	{
		// Create a new Vector2 to equal the vector from the curret touch to the center of joystick.
		Vector2 tempVector = touchInfo.position - ( Vector2 )joystickCenter;

		// If the user wants only one axis, then zero out the opposite value.
		if( axis == Axis.X )
			tempVector.y = 0;
		else if( axis == Axis.Y )
			tempVector.x = 0;

		// If the user wants a circular boundary for the joystick, then clamp the magnitude by the radius.
		if( boundary == Boundary.Circular )
			tempVector = Vector2.ClampMagnitude( tempVector, radius );
		// Else the user wants a square boundry, so clamp X and Y individually.
		else if( boundary == Boundary.Square )
		{
			tempVector.x = Mathf.Clamp( tempVector.x,  -radius,  radius );
			tempVector.y = Mathf.Clamp( tempVector.y,  -radius,  radius );
		}

		// Apply the tempVector to the joystick's position.
		joystick.transform.position = ( Vector2 )joystickCenter + tempVector;

		// If the user is showing highlight and the highlightJoystick is assigned, then move the highlight to match the joystick's position.
		if( showHighlight == true && highlightJoystick != null )
			highlightJoystick.transform.position = joystick.transform.position;

		// If the user has showTension enabled, then display the Tension.
		if( showTension == true )
			TensionAccentDisplay();

		// If the user wants to drag the joystick along with the touch...
		if( draggable == true )
		{
			// Store the position of the current touch.
			Vector3 currentTouchPosition = touchInfo.position;
			// If the user is using any axis option, then align the current touch position.
			if( axis != Axis.Both )
			{
				if( axis == Axis.X )
					currentTouchPosition.y = joystickCenter.y;
				else
					currentTouchPosition.x = joystickCenter.x;
			}
			// Then find the distance that the touch is from the center of the joystick.
			float touchDistance = Vector3.Distance( joystickCenter, currentTouchPosition );

			// If the touchDistance is greater than the set radius...
			if( touchDistance >= radius )
			{
				// Figure out the current position of the joystick.
				Vector2 joystickPosition = ( joystick.position - joystickCenter ) / radius;

				// Move the joystickSizeFolder in the direction that the joystick is, multiplied by the difference in distance of the max radius.
				joystickSizeFolder.position += new Vector3( joystickPosition.x, joystickPosition.y, 0 ) * ( touchDistance - radius );

				// Reconfigure the joystickCenter since the joystick has now moved it position.
				joystickCenter = joystickBase.position;
			}
		}

		if( exposeValues == true )
		{
			horizontalValue = JoystickPosition.x;
			verticalValue = JoystickPosition.y;
		}
	}

	// This function will configure the position of an image based on the size and custom spacing selected.
	Vector2 ConfigureImagePosition ( Vector2 textureSize, Vector2 customSpacing )
	{
		// First, fix the customSpacing to be a value between 0.0f and 1.0f.
		Vector2 fixedCustomSpacing = customSpacing / 100;

		// Then configure position spacers according to the screen's dimensions, the fixed spacing and texture size.
		float positionSpacerX = Screen.width * fixedCustomSpacing.x - ( textureSize.x * fixedCustomSpacing.x );
		float positionSpacerY = Screen.height * fixedCustomSpacing.y - ( textureSize.y * fixedCustomSpacing.y );

		// Create a temporary Vector2 to modify and return.
		Vector2 tempVector;

		// If it's left, simply apply the positionxSpacerX, else calculate out from the right side and apply the positionSpaceX.
		tempVector.x = anchor == Anchor.Left ? positionSpacerX : ( Screen.width - textureSize.x ) - positionSpacerX;

		// Apply the positionSpacerY variable.
		tempVector.y = positionSpacerY;

		// Return the updated temporary Vector.
		return tempVector;
	}

	// This function is called only when showTension is true, and only when the joystick is moving.
	void TensionAccentDisplay ()
	{
		// Create a temporary Vector2 for the joystick current position.
		Vector2 tension = ( joystick.position - joystickCenter ) / radius;

		// If the joystick is to the right...
		if( tension.x > 0 )
		{
			// Then lerp the color according to tension's X position.
			tensionAccentRight.color = Color.Lerp( tensionColorNone, tensionColorFull, tension.x );
			
			// If the opposite tension is not tensionColorNone, the make it so.
			if( tensionAccentLeft.color != tensionColorNone )
				tensionAccentLeft.color = tensionColorNone;
		}
		// Else the joystick is to the left...
		else
		{
			// Mathf.Abs gives a positive number to lerp with.
			tension.x = Mathf.Abs( tension.x );

			// Repeat above steps...
			tensionAccentLeft.color = Color.Lerp( tensionColorNone, tensionColorFull, tension.x );
			if( tensionAccentRight.color != tensionColorNone )
				tensionAccentRight.color = tensionColorNone;
		}

		// If the joystick is up...
		if( tension.y > 0 )
		{
			// Then lerp the color according to tension's Y position.
			tensionAccentUp.color = Color.Lerp( tensionColorNone, tensionColorFull, tension.y );

			// If the opposite tension is not tensionColorNone, the make it so.
			if( tensionAccentDown.color != tensionColorNone )
				tensionAccentDown.color = tensionColorNone;
		}
		// Else the joystick is down...
		else
		{
			// Mathf.Abs gives a positive number to lerp with.
			tension.y = Mathf.Abs( tension.y );
			tensionAccentDown.color = Color.Lerp( tensionColorNone, tensionColorFull, tension.y );

			// Repeat above steps...
			if( tensionAccentUp.color != tensionColorNone )
				tensionAccentUp.color = tensionColorNone;
		}
	}

	// This function resets the tension image's colors back to default.
	void TensionAccentReset ()
	{
		tensionAccentUp.color = tensionColorNone;
		tensionAccentDown.color = tensionColorNone;
		tensionAccentLeft.color = tensionColorNone;
		tensionAccentRight.color = tensionColorNone;
	}

	IEnumerator TapCountdown ()
	{
		// Set the current tap time to the max.
		currentTapTime = tapCountDuration;
		while( currentTapTime > 0 )
		{
			// Reduce the current time.
			currentTapTime -= Time.deltaTime;

			// If the tapCountOption is supposed to accumulate...
			if( tapCountOption == TapCountOption.Accumulate )
			{
				// If the current time is above 0, and the tap count has been acheived...
				if( currentTapTime > 0 && tapCount >= targetTapCount )
				{
					// Set the current time to 0 to interupt the coroutine.
					currentTapTime = 0;

					// Invoke the assigned tapCountEvent.
					tapCountEvent.Invoke();
				}
			}
			yield return null;
		}
	}

	// This function is for returning the joystick back to center of a set amount of time.
	IEnumerator ThrowableMovement ()
	{
		// Start throwing...
		isThrowing = true;

		// Fix the throwDuration into a speed.
		float throwSpeed = 1.0f / throwDuration;

		// Store the position of where the joystick is currently.
		Vector3 startJoyPos = joystick.position;

		for( float i = 0.0f; i < 1.0f; i += Time.deltaTime * throwSpeed )
		{
			// Lerp the joystick's position from where this coroutine started to the center.
			joystick.position = Vector3.Lerp( startJoyPos, joystickCenter, i );

			// If the user is using highlight and the highlightJoystick is assigned, move that as well.
			if( showHighlight == true && highlightJoystick != null )
				highlightJoystick.transform.position = joystick.position;

			// If the user has showTension enabled, then display the tension as the joystick moves.
			if( showTension == true )
				TensionAccentDisplay();

			if( exposeValues == true )
			{
				horizontalValue = JoystickPosition.x;
				verticalValue = JoystickPosition.y;
			}

			yield return null;
		}

		isThrowing = false;

		// Finalize the joystick's position.
		joystick.position = joystickCenter;

		// If the user is using highlight and the highlightJoystick is assigned, finalize that as well.
		if( showHighlight == true && highlightJoystick != null )
			highlightJoystick.transform.position = joystick.position;

		// Here at the end, reset the tension.
		if( showTension == true )
			TensionAccentReset();

		if( exposeValues == true )
		{
			horizontalValue = 0.0f;
			verticalValue = 0.0f;
		}
	}

	// This function is used only to find the canvas parent if its not the root object.
	Canvas GetParentCanvas ()
	{
		Transform parent = transform.parent;
		while( parent != null )
		{ 
			if( parent.transform.GetComponent<Canvas>() )
				return parent.transform.GetComponent<Canvas>();

			parent = parent.transform.parent;
		}
		return null;
	}

	CanvasGroup GetCanvasGroup ()
	{
		if( GetComponent<CanvasGroup>() )
			return GetComponent<CanvasGroup>();
		else
		{
			gameObject.AddComponent<CanvasGroup>();
			return GetComponent<CanvasGroup>();
		}
	}

	Vector2 JoystickPositionDeadZone
	{
		get
		{
			Vector2 tempVec = ( joystick.position - joystickCenter ) / radius;
			
			// If the X value is to the LEFT, then update the deadZone vector2.x to -1 if it is.
			if( tempVec.x < -xDeadZone )
				tempVec.x = -1;
			// Else check if it is to the RIGHT, then update the deadZone vector2.x to 1 if it is.
			else if( tempVec.x > xDeadZone )
				tempVec.x = 1;
			// Else it is not past the deadZone values, so set it to zero.
			else
				tempVec.x = 0;
			
			// If the Y value is DOWN and then update the deadZone vector2.y to -1 if it is.
			if( tempVec.y < -yDeadZone )
				tempVec.y = -1;
			// Else check if it is UP, then update the deadZone vector2.y to 1 if it is.
			else if( tempVec.y > yDeadZone )
				tempVec.y = 1;
			// Else it is not past the deadZone values, so set it to zero.
			else
				tempVec.y = 0;
			
			return tempVec;
		}
	}

	void RegisterJoystick ( string joystickName )
	{
		if( UltimateJoysticks.ContainsKey( joystickName ) )
			UltimateJoysticks.Remove( joystickName );
		
		UltimateJoysticks.Add( joystickName, GetComponent<UltimateJoystick>() );
	}

	IEnumerator FadeLogic ()
	{
		// Store the current value for the alpha of the joystickGroup.
		float currentFade = joystickGroup.alpha;

		// If the fadeInSpeed is NaN, then just set the alpha to the desired fade.
		if( float.IsInfinity( fadeInSpeed ) )
			joystickGroup.alpha = fadeTouched;
		// Else run the loop to fade to the desired alpha over time.
		else
		{
			for( float fadeIn = 0.0f; fadeIn < 1.0f && joystickState == true; fadeIn += Time.deltaTime * fadeInSpeed )
			{
				joystickGroup.alpha = Mathf.Lerp( currentFade, fadeTouched, fadeIn );
				yield return null;
			}
		}

		if( joystickState == true )
			joystickGroup.alpha = fadeTouched;

		// while loop for while joystickState is true
		while( joystickState == true )
			yield return null;

		// Set the current fade value.
		currentFade = joystickGroup.alpha;

		// If the fadeOutSpeed value is NaN, then apply the desired alpha.
		if( float.IsInfinity( fadeOutSpeed ) )
			joystickGroup.alpha = fadeUntouched;
		// Else run the loop for fading out.
		else
		{
			for( float fadeOut = 0.0f; fadeOut < 1.0f && joystickState == false; fadeOut += Time.deltaTime * fadeOutSpeed )
			{
				joystickGroup.alpha = Mathf.Lerp( currentFade, fadeUntouched, fadeOut );
				yield return null;
			}
		}

		if( joystickState == false )
			joystickGroup.alpha = fadeUntouched;
	}

	#if UNITY_EDITOR
	void Update ()
	{
		// Keep the joystick updated while the game is not being played.
		if( Application.isPlaying == false )
			UpdatePositioning();
	}
	#endif

	#region Public Functions
	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Updates the size and placement of the Ultimate Joystick. Useful for screen rotations or changing of screen size.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If any of the needed components are left unassigned, then inform the user and return.
		if( joystickSizeFolder == null || joystickBase == null || joystick == null )
		{
			Debug.LogError( "There are some needed components that are not currently assigned. Please check the Assigned Variables section and be sure to assign all of the components." );
			return;
		}
		
		// Set the current reference size for scaling.
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;
		
		// Configure the target size for the joystick graphic.
		float textureSize = referenceSize * ( joystickSize / 10 );
		
		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();
		
		// Get a position for the joystick based on the position variables.
		Vector2 imagePosition = ConfigureImagePosition( new Vector2( textureSize, textureSize ), new Vector2( customSpacing_X, customSpacing_Y ) );
		
		// If the user wants a custom touch size...
		if( joystickTouchSize == JoystickTouchSize.Custom )
		{
			// Fix the custom size variables.
			float fixedFBPX = customTouchSize_X / 100;
			float fixedFBPY = customTouchSize_Y / 100;
			
			// Depending on the joystickTouchSize options, configure the size.
			baseTrans.sizeDelta = new Vector2( Screen.width* fixedFBPX, Screen.height * fixedFBPY );
			
			// Send the size and custom positioning to the ConfigureImagePosition function to get the exact position.
			Vector2 imagePos = ConfigureImagePosition( baseTrans.sizeDelta, new Vector2( customTouchSizePos_X, customTouchSizePos_Y ) );

			// Apply the new position.
			baseTrans.position = imagePos;
		}
		else
		{
			// Temporary float to store a modifier for the touch area size.
			float fixedTouchSize = joystickTouchSize == JoystickTouchSize.Large ? 2.0f : joystickTouchSize == JoystickTouchSize.Medium ? 1.51f : 1.01f;
			
			// Temporary Vector2 to store the default size of the joystick.
			Vector2 tempVector = new Vector2( textureSize, textureSize );
			
			// Apply the joystick size multiplied by the fixedTouchSize.
			baseTrans.sizeDelta = tempVector * fixedTouchSize;
			
			// Apply the imagePosition modified with the difference of the sizeDelta divided by 2, multiplied by the scale of the parent canvas.
			baseTrans.position = imagePosition - ( ( baseTrans.sizeDelta - tempVector ) / 2 );
		}

		// If the options dictate that the default position needs to be stored...
		if( dynamicPositioning == true || touchPad == true || draggable == true )
		{
			// Set the texture center so that the joystick can move to the touch position correctly.
			textureCenter = new Vector2( textureSize / 2, textureSize / 2 );
			
			// Also need to store the default position so that it can return after the touch has been lifted.
			defaultPos = imagePosition;
		}
		
		// Apply the size and position to the joystickSizeFolder.
		joystickSizeFolder.sizeDelta = new Vector2( textureSize, textureSize );
		joystickSizeFolder.position = imagePosition;
		
		// Configure the size of the Ultimate Joystick's radius.
		radius = joystickSizeFolder.sizeDelta.x * ( radiusModifier / 10 );
		
		// Store the joystick's center so that JoystickPosition can be configured correctly.
		joystickCenter = joystick.position;

		// If the user wants to fade, and the joystickGroup is unassigned, find the CanvasGroup.
		if( useFade == true && joystickGroup == null )
			joystickGroup = GetCanvasGroup();
	}

	/// <summary>
	/// Returns the position of the Ultimate Joystick in a Vector2 Variable. X = Horizontal, Y = Vertical.
	/// </summary>
	/// <value>The Ultimate Joystick's Position.</value>
	public Vector2 JoystickPosition
	{
		get
		{
			if( deadZoneOption != DeadZoneOption.DoNotUse )
				return JoystickPositionDeadZone;

			return ( joystick.position - joystickCenter ) / radius;
		}
	}

	/// <summary>
	/// Returns the distance of the Ultimate Joystick from center.
	/// </summary>
	/// <value>The Ultimate Joystick's Distance.</value>
	public float JoystickDistance
	{
		get{ return Vector3.Distance( joystick.position, joystickCenter ) / radius; }
	}

	/// <summary>
	/// Returns the state of the Ultimate Joystick. This function will return true when the joystick is being interacted with, and false when not.
	/// </summary>
	public bool JoystickState
	{
		get{ return joystickState; }
	}

	/// <summary>
	/// Resets the Ultimate Joystick if it becomes stuck or needs to be released for some other reason.
	/// </summary>
	public void ResetJoystick ()
	{
		OnPointerUp( null );
	}

	/// <summary>
	/// If showHighlight is true, this function will update the color of the highlights attached to the Ultimate Joystick with the targeted color.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the highlight images.</param>
	public void UpdateHighlightColor ( Color targetColor )
	{
		if( showHighlight == false )
			return;

		highlightColor = targetColor;
		
		// Check if each variable is assigned so there is not a null reference exception when applying color.
		if( highlightBase != null )
			highlightBase.color = highlightColor;
		if( highlightJoystick != null )
			highlightJoystick.color = highlightColor;
	}

	/// <summary>
	/// If showTension is true, this function will update the colors of the tension accents attached to the Ultimate Joystick with the targeted colors.
	/// </summary>
	/// <param name="targetTensionNone">The target color to use for idle tension.</param>
	/// <param name="targetTensionFull">The target color to use for full tension.</param>
	public void UpdateTensionColors ( Color targetTensionNone, Color targetTensionFull )
	{
		if( showTension == false )
			return;

		tensionColorNone = targetTensionNone;
		tensionColorFull = targetTensionFull;
	}
	/* ------------------------------------------- *** END PUBLIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
	#endregion
	
	#region Static Functions
	/* --------------------------------------------- *** STATIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Updates the size and placement of the Ultimate Joystick. Useful for screen rotations or changing of joystick options.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	public static void UpdatePositioning ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].UpdatePositioning();
	}


	/// <summary>
	/// Gets the joystick position.
	/// </summary>
	/// <returns>The joystick position.</returns>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	public static Vector2 GetPosition ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return Vector2.zero;
		
		return UltimateJoysticks[ joystickName ].JoystickPosition;
	}

	/// <summary>
	/// Returns the distance of the Ultimate Joystick from center.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	/// <returns>A value between 0 and 1 representing the distance of the joystick from it's center.</returns>
	public static float GetDistance ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;
		return UltimateJoysticks[ joystickName ].JoystickDistance;
	}

	/// <summary>
	/// Resets the Ultimate Joystick if it becomes stuck or needs to be released for some other reason.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	public static void ResetJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;
		UltimateJoysticks[ joystickName ].ResetJoystick();
	}

	/// <summary>
	/// Returns the state of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	/// <returns>Returns true when the joystick is being interacted with, and false when not.</returns>
	public static bool GetJoystickState ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return UltimateJoysticks[ joystickName ].joystickState;
	}

	/// <summary>
	/// If showHighlight is true, this function will update the color of the highlights attached to the Ultimate Joystick with the targeted color.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	/// <param name="targetColor">The target color to apply to the highlight images.</param>
	public static void UpdateHighlightColor ( string joystickName, Color targetColor )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].UpdateHighlightColor( targetColor );
	}

	/// <summary>
	/// If showTension is true, this function will update the colors of the tension accents attached to the Ultimate Joystick with the targeted colors.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	/// <param name="targetTensionNone">The target color to use for idle tension.</param>
	/// <param name="targetTensionFull">The target color to use for full tension.</param>
	public static void UpdateTensionColors ( string joystickName, Color targetTensionNone, Color targetTensionFull )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].UpdateTensionColors( targetTensionNone, targetTensionFull );
	}

	static bool JoystickConfirmed ( string joystickName )
	{
		if( !UltimateJoysticks.ContainsKey( joystickName ) )
		{
			Debug.LogWarning( "No Ultimate Joystick has been registered with the name: " + joystickName + "." );
			return false;
		}
		return true;
	}
	/* ------------------------------------------- *** END STATIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
	#endregion
}

/* Written by Kaz Crowe */
/* UltimateJoystickUpdater.cs ver 1.0.4 */
public class UltimateJoystickUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		StartCoroutine( "YieldPositioning" );
	}

	IEnumerator YieldPositioning ()
	{
		yield return new WaitForEndOfFrame();

		UltimateJoystick[] allJoysticks = FindObjectsOfType( typeof( UltimateJoystick ) ) as UltimateJoystick[];

		for( int i = 0; i < allJoysticks.Length; i++ )
			allJoysticks[ i ].UpdatePositioning();
	}
}