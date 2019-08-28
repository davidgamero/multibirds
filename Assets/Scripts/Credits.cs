using UnityEngine;

public class Credits : MonoBehaviour,
OuyaSDK.IMenuButtonUpListener,
OuyaSDK.IMenuAppearingListener,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener
{
	private float L_STICK_DEADZONE = 0.2f;
	
	public GUIStyle styleNormal;
	public GUIStyle styleHighlighted;

	private bool selectPressed;
	
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
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (1.0f /7.0f), 100, 40),
		          "Developed by David Gamero", styleNormal);
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (2.0f /7.0f), 100, 40),
				   "Music is \"Surfin'Bird 8 bit\" by 8BitUzz", styleHighlighted);
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (3.0f /7.0f), 100, 40),
		          "Inpsired by work of Dong Nguyen", styleNormal);
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (4.0f /7.0f), 100, 40),
		          "Minor scripting help from Steven Burnley III", styleHighlighted);
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (5.0f /7.0f), 100, 40),
		          "Audio control by James Fernandes", styleNormal);
		GUI.Label(new Rect( (Screen.width / 2f) - 50f, Screen.height * (6.0f /7.0f), 100, 40),
		          "Comedy during development from Shubh Pitroda", styleHighlighted);
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
		PlayerPrefs.Save();
		Application.LoadLevel("Main Menu");
	}
	
	void Update()
	{
		//selectPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index);
		selectPressed = (Input.GetKeyDown ("space") ||
		                 OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index));
		
		//back shortcut
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index) || Input.GetKeyDown(KeyCode.Backspace)) {
			saveAndGoBack();
		}
		
		
		//what to do if a choice is selected
		if (selectPressed) {
			saveAndGoBack();
		}

	}
}