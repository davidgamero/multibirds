using UnityEngine;
using System.Collections.Generic;

public class Donate : MonoBehaviour,
OuyaSDK.IPauseListener,
OuyaSDK.IResumeListener,
OuyaSDK.IFetchGamerInfoListener,
OuyaSDK.IGetProductsListener,
OuyaSDK.IPurchaseListener,
OuyaSDK.IGetReceiptsListener
{
	private string[] choices = {"$1.00","$2.00","$5.00","Go Back"};
	private int selectedChoice = 0;
	
	private float L_STICK_DEADZONE = 0.2f;
	
	public GUIStyle styleNormal;
	public GUIStyle styleHighlighted;
	
	/// The products to display for purchase
	public string[] Purchasables =
	{
		"1",
		"2",
		"5",
		"__DECLINED__THIS_PURCHASE",
	};

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

	private List<OuyaSDK.Purchasable> productIdentifierList;
	private List<OuyaSDK.Product> m_products;
	
	public OuyaSDK.OuyaPlayer Index;

	void Start(){

		productIdentifierList = new List<OuyaSDK.Purchasable>();
		m_products = new List<OuyaSDK.Product>();

		foreach (string productId in Purchasables)
		{
			OuyaSDK.Purchasable purchasable = new OuyaSDK.Purchasable();
			purchasable.productId = productId;
			productIdentifierList.Add(purchasable);
		}
		
		OuyaSDK.requestProductList(productIdentifierList);

	}

	void Awake()
	{
		OuyaSDK.registerPauseListener(this);
		OuyaSDK.registerResumeListener(this);
		OuyaSDK.registerFetchGamerInfoListener(this);
		OuyaSDK.registerGetProductsListener(this);
		OuyaSDK.registerPurchaseListener(this);
		OuyaSDK.registerGetReceiptsListener(this);

		Input.ResetInputAxes();
	}
	void OnDestroy()
	{
		OuyaSDK.unregisterPauseListener(this);
		OuyaSDK.unregisterResumeListener(this);
		OuyaSDK.unregisterFetchGamerInfoListener(this);
		OuyaSDK.unregisterGetProductsListener(this);
		OuyaSDK.unregisterPurchaseListener(this);
		OuyaSDK.unregisterGetReceiptsListener(this);

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

	public void OuyaOnPause()
	{
	}
	
	public void OuyaOnResume()
	{
	}

	public void OuyaFetchGamerInfoOnSuccess(string uuid, string username)
	{
	}
	public void OuyaFetchGamerInfoOnFailure(int errorCode, string errorMessage)
	{
	}
	public void OuyaFetchGamerInfoOnCancel()
	{
	}

	public void OuyaGetProductsOnSuccess(List<OuyaSDK.Product> products)
	{
		m_products.Clear();
		foreach (OuyaSDK.Product product in products)
		{
			m_products.Add(product);
		}
	}

	public void OuyaGetProductsOnFailure(int errorCode, string errorMessage)
	{
	}
	public void OuyaGetProductsOnCancel()
	{
	}

	public void OuyaPurchaseOnSuccess(OuyaSDK.Product product)
	{
	}
	public void OuyaPurchaseOnFailure(int errorCode, string errorMessage)
	{
	}
	public void OuyaPurchaseOnCancel()
	{
	}

	public void OuyaGetReceiptsOnSuccess(List<OuyaSDK.Receipt> receipts)
	{
		foreach (OuyaSDK.Receipt receipt in receipts)
		{
			if (receipt.identifier == "__MY_ID__")
			{
				//detected purchase
			}
		}
	}
	public void OuyaGetReceiptsOnFailure(int errorCode, string errorMessage)
	{
	}
	public void OuyaGetReceiptsOnCancel()
	{
	}


	void Update()
	{
		//new button press detection
		//upPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);
		upPressed = OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_DPAD_UP, Index);
		
		if (!lastUpPressed && upPressed) {
			//if it wasnt pressed last time but it is now
			lastUpPressed = true; //log that it is pressed now for the next cycle
			upPressed = true; //signal new button press
		}else{
			lastUpPressed = upPressed; //log the current state for next cycle
			upPressed = false; //signal no new button press
		}
		
		//downPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, Index);
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
		//selectPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index);
		selectPressed = OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_O, Index);
		
		//exit shortcut
		if (OuyaExampleCommon.GetButtonDown (OuyaSDK.KeyEnum.BUTTON_A, Index)) {
			Application.LoadLevel("Main Menu");
		}
		
		
		//what to do if a choice is selected
		if (selectPressed) {
			OuyaSDK.Purchasable toBeBought = new OuyaSDK.Purchasable();
			switch(selectedChoice){
			case 0:
				toBeBought.productId = "1";
				OuyaSDK.requestPurchase(toBeBought);

				break;
			case 1:
				toBeBought.productId = "2";
				OuyaSDK.requestPurchase(toBeBought);
				break;
			case 2:
				toBeBought.productId = "5";
				OuyaSDK.requestPurchase(toBeBought);
				break;
			case 3:
				Application.LoadLevel("Main Menu");
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