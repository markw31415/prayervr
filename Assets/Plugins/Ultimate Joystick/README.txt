Thank you for purchasing the Ultimate Joystick UnityPackage!

/* ------- < IMPORTANT INFORMATION > ------- */
Within Unity, please go to Window / Ultimate UI / Ultimate Joystick to access important information on how to get started using the Ultimate Joystick. There is
a ton of information available to help you get the Ultimate Joystick into your project as fast as possible. However, if you can't view the in-engine documentation
window, please see the information below.
/* ----- < END IMPORTANT INFORMATION > ----- */


// --- IF YOU CAN'T VIEW THE ULTIMATE JOYSTICK WINDOW, READ THIS SECTION --- //

	// --- HOW TO CREATE --- //
To create a Ultimate Joystick in your scene, simply go up to GameObject / UI / Ultimate UI / Ultimate Joystick. What this does is locates the Ultimate Joystick
prefab that is located within the Editor Default Resources folder, and creates an Ultimate Joystick within the scene. This method of adding an Ultimate Joystick
to your scene ensures that the joystick will have a Canvas and an EventSystem so that it can work correctly.

	// --- HOW TO REFERENCE --- //
One of the great things about the Ultimate Joystick is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is to
name the joystick in the Script Reference section. After this is complete, you will be able to reference that particular joystick by it's name from a static function
within the Ultimate Joystick script. After the joystick has been given a name in the Script Reference section, we can get that joystick's position by creating a
Vector2 variable at run time and storing the result from the static function: 'GetPosition'. This Vector2 will be the joystick's position, and will contain float
values between -1, and 1, with 0 being at the center. Keep in mind that the joystick's left and right ( horizontal ) movement is translated into this Vector2's X
value, while the up and down ( vertical ) is the Vector2's Y value. This will be important when applying the Ultimate Joystick's position to your scripts.

Let's assume that we want to use a joystick for a characters movement. The first thing to do is to assign the name "Movement" in the Joystick Name variable located
in the Script Reference section of the Ultimate Joystick. After that, we need to create a Vector2 variable to store the result of the joystick's position returned
by the 'GetPosition' function. In order to get the "Movement" joystick's position, we need to pass in the name "Movement" as the argument.

	// C# EXAMPLE //
		Vector2 joystickPosition = UltimateJoystick.GetPosition( "Movement" );

	// JAVASCRIPT EXAMPLE //
		var joystickPosition : Vector2 = UltimateJoystick.GetPosition( "Movement" );

The joystickPosition variable now contains the values of the position that the Movement joystick was in at the movement it was referenced. Now we can use this
information in any way that is desired. For example, if you are wanting to put the joystick's position into a character movement script, you would create a Vector3
variable for movement direction, and put in the appropriate values of the Ultimate Joystick's position.

	// C# EXAMPLE //
		Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );

	// JAVASCRIPT EXAMPLE //
		var movementDirection : Vector3 = new Vector3( joystickPosition.x, 0, joystickPosition.y );

In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z directions. This is because you generally don't
want your character to move in the Y direction unless the user jumps. That is why we put the joystickPosition.y value into the Z value of the movementDirection
variable. Understanding how to use the values from any input is important when creating character controllers, so experiment with the values and try to understand
how the mobile input can be used in different ways.



/* ------------------< CHANGE >------------------ */
/* --------------------< LOG >------------------- */
	// Ultimate Joystick //
		v1.6.3 - Adding better update positioning options line - 506...also created new script UltimateJoystickUpdater.
		v1.6.4 - Fixed throwable option snag on line 205.
		v1.6.5 - Simple script updates.
		v1.6.6 - Fixed Draggable option to accept axis constraints.
		v1.6.7 - Condensed the tap count functionality down to one countdown function.
		v1.6.8 - Added new static reference functions to enhance use, as well as added functions for updating highlight and tension colors.
		v1.6.9 - Added exposed values for game making plugin support.
		v1.7.0 - Added advanced fade functionality.
		v1.7.1 - Final cleanup of the script for the 2.0 update.

	// Ultimate Joystick Editor //
		v1.2.6 - Fix to flickering images when editor script was running in play mode.
		v1.2.7 - Added better functionality to undo/redo and simplified scripts.
		v1.2.8 - Updated editor to not show help box for axis constraints not working with the draggable option. - Option is fixed now and supports axis constraints.
		v1.2.9 - Updated editor to show new variables in the UltimateJoystick.cs script.
		v1.3.0 - Preping editor for the removing of support for all CanvasScaler options.
		v1.3.1 - Adding in the functionality from the CreateUltimateJoystickEditor.cs script to reduce the amount of script that the package comes with.
		v1.3.2 - Added in options to the Script Reference section for reference of the new Static functions within the Ultimate Joystick.
		v1.3.3 - Added in new options to the Script Reference section for the exposed values of the Ultimate Joystick.
		v1.3.4 - Changed options within the Touch Actions section to accept the changes made to the fade functionality of UJ.
		v1.3.5 - Final cleanup of script to prep for 2.0 release.
		
	// Ultimate Joystick Creator //
		v1.0.0 - Initial creation of new class within the UltimateJoystickEditor.cs script. Retain all functionality from the previous script.
		v1.0.1 - Removed the adding of a CanvasScaler onto the Canvas to avoid confusion to the user.
		v1.0.2 - Change a few of the checks for the needed canvas before creating a new one.
		v1.0.3 - Added new public static function for requesting a canvas so that other functions can get a new canvas using this script.