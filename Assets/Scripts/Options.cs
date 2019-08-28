using UnityEngine;

public class Options : MonoBehaviour,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener
{
	private string[] choices = {"Music","Hell Mode","Double Pipesets","Save and Go Back"};//array of choices
	private int[]    toggles = {   0   ,       0   ,       1         ,      9999        };//array of backup default toggle states for playerprefs without default values
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

		//import old player prefs
		toggles[0] = PlayerPrefs.GetInt ("music",1);
		toggles[1] = PlayerPrefs.GetInt ("hellmode",0);
		toggles [2] = PlayerPrefs.GetInt ("doublepipesets", 1);	//default double pipes to on

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
				GUI.Label(new Rect( (Screen.width / 2f) - 50f, (((2 * Screen.height)/9f) + (i*(Screen.height/9f)) ), 100, 40),
				          choices[i] + dashState(toggles[i]), styleHighlighted);
			}else{
				GUI.Label(new Rect( (Screen.width / 2f) - 50f, (((2 * Screen.height)/9f) + (i*(Screen.height/9f)) ), 100, 40),
				          choices[i] + dashState(toggles[i]), styleNormal);
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

	public void saveAndGoBack(){
		//save playerPrefs and go back
		PlayerPrefs.SetInt("music" , toggles[0]);
		PlayerPrefs.SetInt("hellmode" , toggles[1]);
		PlayerPrefs.SetInt ("doublepipesets", toggles [2]);

		PlayerPrefs.Save();
		Application.LoadLevel("Main Menu");
	}
	
	void Update()
	{
		//new button press detection
		//upPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);
		upPressed = (Input.GetKey ("up") ||
		             OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index));
		
		if (!lastUpPressed && upPressed) {
			//if it wasnt pressed last time but it is now
			lastUpPressed = true; //log that it is pressed now for the next cycle
			upPressed = true; //signal new button press
		}else{
			lastUpPressed = upPressed; //log the current state for next cycle
			upPressed = false; //signal no new button press
		}
		
		//downPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index);
		downPressed = (Input.GetKey ("down") ||
		               OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index));
		
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
		//selectPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index);
		selectPressed = (Input.GetKeyDown ("space") ||
		                 OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index));

		//back shortcut
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index)) {
			saveAndGoBack();
		}
		
		
		//what to do if a choice is selected
		if (selectPressed) {
			switch(selectedChoice){
			case 0:
				toggles[0] = flipInt(toggles[0]);
				break;
			case 1:
				toggles[1] = flipInt(toggles[1]);
				break;
			case 2:
				toggles[2] = flipInt(toggles[2]);
				break;
			case 3:
				saveAndGoBack();
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

	/**returns dash and boolean as string except blank for 9999
	 **/
	private string dashState(int state){
		if (state == 9999) {
			//blank state for go back option
			return "";
		}else if(state == 1){
			return "- On";
		}else{
			return "- Off";
		}
	}

	private string onOff(bool state){
		if(state){
			return "On";
		}else{
			return "Off";
		}
	}

	/**turns 1s to 0s and vice a versa but ignore all other values
	 **/
	private int flipInt(int toBeFlipped){
		//turns 1s to 0s and vice a versa but ignore all other values
		if (toBeFlipped == 0) {
			toBeFlipped = 1;
		}else if(toBeFlipped == 1){
			toBeFlipped = 0;
		}
		return toBeFlipped;
	}
}