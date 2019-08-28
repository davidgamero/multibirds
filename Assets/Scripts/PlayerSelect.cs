using UnityEngine;

public class PlayerSelect : MonoBehaviour,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener
{
	private string[] choices = {"1","2","3","4"};
	private bool[] playersOn = {true,false,false,false};
	private bool[] playersReady = {true,false,false,false};
	private int selectedChoice = 0;

	//controller share
	bool controllerShareOn = false;
	bool controllerShareLastState;
	bool controllerShareJustOn = false;
	public Texture OUYA_Controller;
	public Texture OUYA_U;
	public Texture OUYA_LB;
	public Texture OUYA_LT;
	public Texture OUYA_RB;
	public Texture OUYA_RT;
	public Texture OUYA_O;
	private float triggerImageOffset = 100f;
	
	private float L_STICK_DEADZONE = 0.2f;
	
	public GUIStyle styleNormal;
	public GUIStyle styleHighlighted;
	public GUIStyle stylePressO;
	public GUIStyle styleX;
	
	//last button presses for navigation
	private bool rightPressed = false; //true for one frame after new DPAD_UP press
	private bool leftPressed = false; //true for one frame after new DPAD_DOWN press
	private bool lastRightPressed = false;
	private bool lastLeftPressed = false;
	
	//select button boolean
	private bool selectPressed = false;
	
	//last joystick tilts for navigation
	private bool lJoyRight = false; //true for one frame after left joystick become positive outside deadzone
	private bool lJoyLeft = false;	//true for one frame after left joystick become negative outside deadzone
	private bool lastLJoyRight = false;
	private bool lastLJoyLeft = false;

	//bird sprites
	private GameObject bird1;
	private GameObject bird2;
	private GameObject bird3;
	private GameObject bird4;

	private int numberOfPlayers;
	
	public OuyaSDK.OuyaPlayer Index;
	public OuyaSDK.OuyaPlayer Index2;
	public OuyaSDK.OuyaPlayer Index3;
	public OuyaSDK.OuyaPlayer Index4;

	void Start(){
		PlayerPrefs.SetInt ("numberOfPlayers", 1);
		numberOfPlayers = 1;
		bird1 = GameObject.Find("Blue Bird");
		bird2 = GameObject.Find("Red Bird");
		bird3 = GameObject.Find("Yellow Bird");
		bird4 = GameObject.Find("Green Bird");

		//detect previous controller share state
		if(PlayerPrefs.GetInt("controllerShare",0) == 1){
			//yes controllerShare
			controllerShareOn = true;
		}else{
			//no controllerShare
			controllerShareOn = false;
		}

		if (controllerShareOn) {
			selectedChoice = 1;
		}
		controllerShareLastState = controllerShareOn;  //make last state the starting state
	}

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
		//show player numbers options
		for (int i = 0; i < choices.Length; i++) {
			if( i == selectedChoice){
				GUI.Label(new Rect( Screen.width * ((i + 2f)/ 7f) - 50f, (2f *Screen.height/7f) , 100, 40), choices[i], styleHighlighted);
			}else{
				GUI.Label(new Rect( Screen.width * ((i + 2f)/ 7f) - 50f, (2f *Screen.height/7f) , 100, 40), choices[i], styleNormal);
			}
		}
		//player 2 x
		if (!playersReady [1]) {
			GUI.Label(new Rect( Screen.width * ((3f)/ 7f) - 50f, (3f *Screen.height/7f) , 100, 40), "X", styleX);
		}
		if (!playersReady [2]) {
			GUI.Label(new Rect( Screen.width * ((4f)/ 7f) - 50f, (3f *Screen.height/7f) , 100, 40), "X", styleX);
		}
		if (!playersReady [3]) {
			GUI.Label(new Rect( Screen.width * ((5f)/ 7f) - 50f, (3f *Screen.height/7f) , 100, 40), "X", styleX);
		}

		//players label
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, (Screen.height/7f) , 100, 40), "Players", styleNormal);

//		//press o to join
//		GUI.Label(new Rect( (Screen.width / 2f) - 50f, (5f * Screen.height/7f) , 100, 40), "Press     to join", stylePressO);
	

		//controllerShare GUI

		//u button
		GUI.DrawTexture(new Rect( (Screen.width * (3f / 4f) - 40f - 400f), ((Screen.height * (6.0f / 7.0f)) - 60.0f - 10f), 105, 120), OUYA_U);

		if(controllerShareOn) {
			//show controllerShare guide
			GUI.Label(new Rect( (Screen.width * (3f / 4f) - 50f), (6f * Screen.height/7f) - 20f , 100, 40), "ControllerShare - On", stylePressO);

			//p1 RB and RT
			GUI.DrawTexture(new Rect( Screen.width * (2f / 7f) - 50f, (4f * Screen.height/7f) - 45f, 100, 90), OUYA_RB);
			GUI.DrawTexture(new Rect( Screen.width * (2f / 7f) - 50f, (4f * Screen.height/7f) - 45f + triggerImageOffset, 100, 90), OUYA_RT);
			//p2 LB and LT
			GUI.DrawTexture(new Rect( Screen.width * (3f / 7f) - 50f, (4f * Screen.height/7f) - 45f, 100, 90), OUYA_LB);
			GUI.DrawTexture(new Rect( Screen.width * (3f / 7f) - 50f, (4f * Screen.height/7f) - 45f + triggerImageOffset, 100, 90), OUYA_LT);
			//first shared controller
			GUI.DrawTexture(new Rect( Screen.width * (2.5f / 7f) - 60f, (4f * Screen.height/7f) - 45f + (triggerImageOffset / 2f), 120, 90), OUYA_Controller);


			//p3 RB and RT
			GUI.DrawTexture(new Rect( Screen.width * (4f / 7f) - 50f, (4f * Screen.height/7f) - 45f, 100, 90), OUYA_RB);
			GUI.DrawTexture(new Rect( Screen.width * (4f / 7f) - 50f, (4f * Screen.height/7f) - 45f + triggerImageOffset, 100, 90), OUYA_RT);
			//p4 LB and LT
			GUI.DrawTexture(new Rect( Screen.width * (5f / 7f) - 50f, (4f * Screen.height/7f) - 45f, 100, 90), OUYA_LB);
			GUI.DrawTexture(new Rect( Screen.width * (5f / 7f) - 50f, (4f * Screen.height/7f) - 45f + triggerImageOffset, 100, 90), OUYA_LT);
			//second shared controller
			GUI.DrawTexture(new Rect( Screen.width * (4.5f / 7f) - 60f, (4f * Screen.height/7f) - 45f + (triggerImageOffset / 2f), 120, 90), OUYA_Controller);




		}else{
			//show normal guide
			GUI.Label(new Rect( (Screen.width * (3f / 4f) - 50f), (6f * Screen.height/7f) - 20f, 100, 40), " ControllerShare - Off", stylePressO);

			//press o to join
			GUI.Label(new Rect( (Screen.width / 2f) - 50f - 10f, (4f * Screen.height/7f) , 100, 40), "Press     to join", stylePressO);
			GUI.DrawTexture(new Rect( (Screen.width / 2f) - 50f - 35f, (4f * Screen.height/7f) - 50f, 105, 120), OUYA_O);

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
		//rightPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);
		rightPressed = (Input.GetKey(KeyCode.LeftArrow) ||
		                OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, Index));
		
		if (!lastRightPressed && rightPressed) {
			//if it wasnt pressed last time but it is now
			lastRightPressed = true; //log that it is pressed now for the next cycle
			rightPressed = true; //signal new button press
		}else{
			lastRightPressed = rightPressed; //log the current state for next cycle
			rightPressed = false; //signal no new button press
		}
		
		//leftPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index);
		leftPressed = (Input.GetKey(KeyCode.RightArrow) ||
		               OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, Index));
		
		if (!lastLeftPressed && leftPressed) {
			//if it wasnt pressed last time but it is now
			lastLeftPressed = true; //log that it is pressed now for the next cycle
			leftPressed = true; //signal new button press
		}else{
			lastLeftPressed = leftPressed; //log the current state for next cycle
			leftPressed = false; //signal no new button press
		}
		
		//new joystick tilt detection
		//get the raw tilts to bools
		if (OuyaExampleCommon.GetAxisRaw (OuyaSDK.KeyEnum.AXIS_LSTICK_X, Index) < -L_STICK_DEADZONE) {
			lJoyRight = true;
		} else {
			lJoyRight = false;	
		}
		if (OuyaExampleCommon.GetAxisRaw (OuyaSDK.KeyEnum.AXIS_LSTICK_X, Index) > L_STICK_DEADZONE) {
			lJoyLeft = true;
		} else {
			lJoyLeft = false;	
		}
		
		//evaluate new tilts
		if (!lastLJoyRight && lJoyRight) {
			//if it wasnt up last time but it is now
			lastLJoyRight = true; //log that it is up now for the next cycle
			lJoyRight = true; //signal new up tilt
		}else{
			lastLJoyRight = lJoyRight; //log the current state for next cycle
			lJoyRight = false; //signal no new joystick tilt
		}
		
		if (!lastLJoyLeft && lJoyLeft) {
			//if it wasnt up last time but it is now
			lastLJoyLeft = true; //log that it is up now for the next cycle
			lJoyLeft = true; //signal new joystick tilt
		}else{
			lastLJoyLeft = lJoyLeft; //log the current state for next cycle
			lJoyLeft = false; //signal no new joystick tilt
		}
		
		//select button
		//selectPressed = OuyaExampleCommon.GetButtonLeft (OuyaSDK.KeyEnum.BUTTON_O, Index);
		
		//controllerShare button
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_U, Index)) {
			controllerShareOn = !controllerShareOn;
		}

		//exit shortcut
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index)
		    || Input.GetKeyDown(KeyCode.Backspace)) {
			Application.LoadLevel("Main Menu");
		}
		
		
		//what to do if a choice is selected
		switch(selectedChoice){
		case 3:

			break;
		case 2:

			break;
		case 1:

			break;
		default:

			break;
		}

		//bird renderers
		if (playersReady [1]) {
			bird2.renderer.enabled = true;
		}else{
			bird2.renderer.enabled = false;
		}
		if (playersReady [2]) {
			bird3.renderer.enabled = true;
		}else{
			bird3.renderer.enabled = false;
		}
		if (playersReady [3]) {
			bird4.renderer.enabled = true;
		}else{
			bird4.renderer.enabled = false;
		}

		numberOfPlayers = selectedChoice + 1;
		setPlayers (numberOfPlayers);
		PlayerPrefs.SetInt ("numberOfPlayers", numberOfPlayers);
	
		
		//handling choice movement
		if ((rightPressed || lJoyRight) && (selectedChoice > 0)) {
			selectedChoice -= 1;
		}
		
		if((leftPressed || lJoyLeft) && (selectedChoice < (choices.Length - 1))){
			selectedChoice += 1;
		}

		//p1 O button to start game
		if (Input.GetKeyDown("space") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index)
			|| OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_RT, Index)
		    || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_RB, Index)){
			print ("got it");
			//everybody in
			if(compare4Bools()){
				//save controllerShare options
				if(controllerShareOn){
					PlayerPrefs.SetInt ("controllerShare",1);
				}else{
					PlayerPrefs.SetInt("controllerShare",0);
				}

				PlayerPrefs.SetInt ("numberOfPlayers", numberOfPlayers);
				PlayerPrefs.Save();
				Application.LoadLevel("Game Level");

			}
		}

		//ready up buttons
		if(controllerShareOn){
		//yes controller share ready up

			//p2 ready up
			if (Input.GetKeyDown("a") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_LB, Index)
			    || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_LT, Index)) {
				if(playersOn[1]){
					playersReady[1] = true;
				}
			}
			
			//p3 ready up
			if (Input.GetKeyDown("'") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index2)
			    || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_RT, Index2)
			    || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_RB, Index2)) {
				if(playersOn[2]){
					playersReady[2] = true;
				}
			}
			
			//p4 ready up
			if (Input.GetKeyDown("5") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_LB, Index2)
			    || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_LT, Index2)) {
				if(playersOn[3]){
					playersReady[3] = true;
				}
			}

		}else{
		//no controller share ready up

			//p2 ready up
			if (Input.GetKeyDown("a") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index2)) {
				if(playersOn[1]){
					playersReady[1] = true;
				}
			}

			//p3 ready up
			if (Input.GetKeyDown("'") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index3)) {
				if(playersOn[2]){
					playersReady[2] = true;
				}
			}

			//p4 ready up
			if (Input.GetKeyDown("5") || OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index4)) {
				if(playersOn[3]){
					playersReady[3] = true;
				}
			}
		}

		//controllerShareState catcher
		if ((controllerShareOn != controllerShareLastState) && controllerShareLastState) {
			//if state changed and it used to be on
			controllerShareJustOn = true;

			//controllerShare was just turned off so set players to one to avoid dummy players signed in
			selectedChoice = 0;
		}else{
			controllerShareJustOn = false;
		}

		//auto move to 2 player when turning on controllershare
		if ((controllerShareOn != controllerShareLastState) && !controllerShareLastState) {
			//controllerShare was just turned on so set the game to two player
			selectedChoice = 1;
		}

		controllerShareLastState = controllerShareOn;  //log current state as last


	}//end void Update()

	/** activate players up to numm and deactivate after that
	 * */
	void setPlayers (int numm){
		//set on true up to numm
		for(int i = 0; i < numm; i++){
			playersOn[i] = true;
		}
		//set on false beyond numm
		for (int i = numm; i < playersOn.Length; i++) {
			playersOn[i] = false;
		}
		//set ready false beyond numm
		for (int i = numm; i < playersOn.Length; i++) {
			playersReady[i] = false;
		}
	}

	bool compare4Bools(){
		for (int i = 0; i < playersOn.Length; i++) {
			if(playersOn[i] != playersReady[i]){
				return false;
			}
		}
		return true;

	}

	/**returns dash and boolean as string ON or OFF
	 **/
	private string dashState(bool state){
		if(state){
			return "- On";
		}else{
			return "- Off";
		}
	}
}