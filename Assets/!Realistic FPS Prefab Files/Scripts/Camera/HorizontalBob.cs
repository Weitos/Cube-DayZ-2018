//HorizontalBob.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;

public class HorizontalBob : MonoBehaviour {
	[HideInInspector]
	public GameObject playerObj;
	//variables for horizontal sine bob of camera and weapons
	private float timer = 0.0f;
	[HideInInspector]
	public float bobbingSpeed = 0.0f;
	[HideInInspector]
	public float bobbingAmount = 0.0f;
	[HideInInspector]
	public float translateChange = 0.0f;
	[HideInInspector]
	public float waveslice = 0.0f;
	private float dampVelocity = 0.0f;
	private float totalAxes;
	[HideInInspector]
	public float dampOrg = 0.0f;//Smoothed view postion to be passed to CameraKick script
	private float dampTo = 0.0f;
	private float horizontal;
	private float vertical;
	private float inputSpeed;

	private bool _isInitialized;
	private FPSRigidBodyWalker _fpsWalker;
	private FPSPlayer _fpsPlayer;
	private CameraKick _cameraKick;

	private void Awake()
	{
		//set up external script references
		_fpsWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		_fpsPlayer = playerObj.GetComponent<FPSPlayer>();
		_cameraKick = Camera.main.GetComponent<CameraKick>();
		_isInitialized = true;
	}

	void Update (){
		if(_isInitialized && Time.timeScale > 0 && Time.deltaTime > 0){//allow pausing by setting timescale to 0
			
			waveslice = 0.0f;
			if(!_fpsPlayer.useAxisInput){
				horizontal = _fpsWalker.inputX;//get input from player movement script
				vertical   = _fpsWalker.inputY;
			}else{
				horizontal = _fpsWalker.inputXSmoothed;//get input from player movement script
				vertical   = _fpsWalker.inputYSmoothed;	
			}
		
			if (Mathf.Abs(horizontal) != 0 || Mathf.Abs(vertical) != 0 && _fpsWalker.grounded){//Perform bob only when moving and grounded
				waveslice = Mathf.Sin(timer);
				if(Mathf.Abs(_fpsWalker.inputY) > 0.1f){
					inputSpeed = Mathf.Abs(_fpsWalker.inputY);
				}else{
					inputSpeed = Mathf.Abs(_fpsWalker.inputX);
				}
				timer = timer + bobbingSpeed * inputSpeed * Time.deltaTime;
				if (timer > Mathf.PI * 2.0f) {
				  timer = timer - (Mathf.PI * 2.0f);
				}
			}else{
				timer = 0.0f;//reset timer when stationary to start bob cycle from neutral position
			}
		
			if (waveslice != 0){
			   translateChange = waveslice * bobbingAmount;
			   totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			   totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			   translateChange = totalAxes * translateChange;
				//set position for smoothing function
				dampTo = translateChange / Time.deltaTime * 0.01f;//divide position by deltaTime for framerate independence
			}else{
				//reset variables to prevent view twitching when falling
				dampTo = 0.0f;
				totalAxes = 0.0f;
				translateChange = 0.0f;
			}
			//use SmoothDamp to smooth position and remove any small glitches in bob amount 
			dampOrg = Mathf.SmoothDamp(dampOrg, dampTo, ref dampVelocity, 0.1f, Mathf.Infinity, Time.deltaTime);
			//pass bobbing amount to the camera kick script in the camera object after smoothing
			_cameraKick.dampOriginX = dampOrg;
		}
	}
}