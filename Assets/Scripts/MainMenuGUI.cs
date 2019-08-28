using UnityEngine;

public class MainMenuGUI : MonoBehaviour,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener
{
	private string[] choices = {"Play","Options","Credits","Donate","Exit"};
	private int selectedChoice = 0;

	private float L_STICK_DEADZONE = 0.2f;

	public GUIStyle styleNormal;
	public GUIStyle styleHighlighted;

	//last button presses for navigation
	private bool upPressed = false; //true for one frame after new DPAD_UP press
	private bool downPressed = false; //true for one frame after new DPAD_DOWN press
	private bool lastUpPressed = false;
	private bool lastDownPressed = false;

	//select button boolean
	private bool selectPressed = false;

	//last joystick tilts for navigation
	private bool lJoyUp = false; //true for one frame after left joystick become positive outside deadzone
	private bool lJoyDown = false;	//true for one frame after left joystick become negative outside deadzone
	private bool lastLJoyUp = false;
	private bool lastLJoyDown = false;

	public OuyaSDK.OuyaPlayer Index;
	
	void Awake()
	{
		OuyaSDK.registerMenuButtonUpListener(this);
		OuyaSDK.registerMenuAppearingListener(this);
		OuyaSDK.registerPauseListener(this);
		OuyaSDK.registerResumeListener(this);
		Input.ResetInputAxes();
	}
	void OnDestroy()
	{
		OuyaSDK.unregisterMenuButtonUpListener(this);
		OuyaSDK.unregisterMenuAppearingListener(this);
		OuyaSDK.unregisterPauseListener(this);
		OuyaSDK.unregisterResumeListener(this);
		Input.ResetInputAxes();
	}

	void OnGUI(){
		//run this once per index in array choices
		for (int i = 0; i < choices.Length; i++) {
			if( i == selectedChoice){
				GUI.Label(new Rect( (Screen.width / 2f) - 50f, ((Screen.height/3f) + (i*(Screen.height/9f)) ), 100, 40), choices[i], styleHighlighted);
			}else{
				GUI.Label(new Rect( (Screen.width / 2f) - 50f, ((Screen.height/3f) + (i*(Screen.height/9f)) ), 100, 40), choices[i], styleNormal);
			}
		}
	}

	public void OuyaMenuButtonUp()
	{
	}
	
	public void OuyaMenuAppearing()
	{
	}
	
	public void OuyaOnPause()
	{
	}
	
	public void OuyaOnResume()
	{
	}
	
	void Update()
	{
		//new button press detection
		//upPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);
		upPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);

		if (!lastUpPressed && upPressed) {
			//if it wasnt pressed last time but it is now
			lastUpPressed = true; //log that it is pressed now for the next cycle
			upPressed = true; //signal new button press
		}else{
			lastUpPressed = upPressed; //log the current state for next cycle
			upPressed = false; //signal no new button press
		}

		//downPressed = Input.GetKeyDown("down");
		downPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index);
	
		if (!lastDownPressed && downPressed) {
			//if it wasnt pressed last time but it is now
			lastDownPressed = true; //log that it is pressed now for the next cycle
			downPressed = true; //signal new button press
		}else{
			lastDownPressed = downPressed; //log the current state for next cycle
			downPressed = false; //signal no new button press
		}

		//new joystick tilt detection
		//get the raw tilts to bools
		if (OuyaExampleCommon.GetAxisRaw (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, Index) < -L_STICK_DEADZONE) {
			lJoyUp = true;
		} else {
			lJoyUp = false;	
		}
		if (OuyaExampleCommon.GetAxisRaw (OuyaSDK.KeyEnum.AXIS_LSTICK_Y, Index) > L_STICK_DEADZONE) {
			lJoyDown = true;
		} else {
			lJoyDown = false;	
		}

		//evaluate new tilts
		if (!lastLJoyUp && lJoyUp) {
			//if it wasnt up last time but it is now
			lastLJoyUp = true; //log that it is up now for the next cycle
			lJoyUp = true; //signal new up tilt
		}else{
			lastLJoyUp = lJoyUp; //log the current state for next cycle
			lJoyUp = false; //signal no new joystick tilt
		}

		if (!lastLJoyDown && lJoyDown) {
			//if it wasnt up last time but it is now
			lastLJoyDown = true; //log that it is up now for the next cycle
			lJoyDown = true; //signal new joystick tilt
		}else{
			lastLJoyDown = lJoyDown; //log the current state for next cycle
			lJoyDown = false; //signal no new joystick tilt
		}

		//select button
		//selectPressed = Input.GetKeyDown("space");
		selectPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index);

		//exit shortcut
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index)) {
			Application.Quit();
		}


		//what to do if a choice is selected
		if (selectPressed) {
			switch(selectedChoice){
			case 0:
				Application.LoadLevel("Player Select");
				break;
			case 1:
				Application.LoadLevel("Options");
				break;
			case 2:
				Application.LoadLevel("Credits");
				break;
			case 3:
				Application.LoadLevel("Donate");
				break;
			case 4:
				Application.Quit();
				break;
			default:
				break;
			}
		}

		//handling choice movement
		if ((upPressed || lJoyUp) && (selectedChoice > 0)) {
			selectedChoice -= 1;
		}

		if((downPressed || lJoyDown) && (selectedChoice < (choices.Length - 1))){
			selectedChoice += 1;
		}


	}
}