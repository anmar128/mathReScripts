using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// GameObject and Vector3 variables to be used
	// for gameview initialization
	public Vector3 robotValues = new Vector3 (0f, 2.5f, 0f);
	public Vector3 startValues = new Vector3 (0f, 1.5f, 0f);
	public Vector3 finishValues = new Vector3 (0f, 1.9f, 0f);
	public GameObject robot;
	public GameObject startPoint;
	public GameObject finishPoint;
	public GameObject breakPoint;
	public GameObject youWin;
	public GameObject tada;

	// GameObject and Vector3 variables to be used
	// for actionList initialization and updates
	public Vector3 listStartValues = new Vector3 (-7.2f, 5.2f, 0f);
	public Vector3 pressPlayValues = new Vector3 (-6.25f, -3.5f, 0f);
	public Vector3 pressStopValues = new Vector3 (-7.25f, -3.5f, 0f);
	public Vector3 pressRestartValues = new Vector3 (7.5f, 5.5f, 0f);
	public Vector3 pressWinstartValues = new Vector3 (3.9f, 5.5f, 0f);

	public GameObject listBk10;
	public GameObject listBk05;
	public GameObject listBk01;
	public GameObject listFw01;
	public GameObject listFw05;
	public GameObject listFw10;
	public GameObject listUp10;
	public GameObject listUp05;
	public GameObject listUp01;
	public GameObject listDn01;
	public GameObject listDn05;
	public GameObject listDn10;
	public GameObject pressPlay;
	public GameObject pressStop;
	public GameObject pressRestart;
	public GameObject pressWinstart;
	private int clickdRestart = 0;
	private int winRestart = 0;

	// GameObject and Vector3 variables to be used
	// for numbers-in-the-gameview initialization
	public Vector3 numStartValues = new Vector3 (0f, 1f, 0f);
	public Vector3 numFinishValues = new Vector3 (0f, 1f, 0f);
	public GameObject numOne;
	public GameObject numTwo;
	public GameObject numThree;
	public GameObject numFour;
	public GameObject numFive;
	public GameObject numSix;
	public GameObject numSeven;
	public GameObject numEight;
	public GameObject numNine;
	public GameObject numZero;
	public GameObject numComma;
	public GameObject numMinus;

	// GameObject and Rigidbody variables to be used for the robot gameplay
	private GameObject robotCl;

	// Int variables containing the start, finish and current points of the robot
	private int randStartX;
	private int randStartY;
	private int randFinishX;
	private int randFinishY;
	private int currPointX;
	private int currPointY;
	private int prevPointX;
	private int prevPointY;
	private float midPointX;
	private float midPointY;
	private int numPerm;	// Mid-num -- not perm ~ 0, start/finish-num -- perm ~ 1
	// Variables to be used for real-time checking
	private int playMode;	// Not playing ~ 0, playing ~ 1
	private int rolling;	// Stop ~ 0, keep on rollin' ~ 1
	private int directionX;	// Left ~ -1, stopped ~ 0, right ~ 1
	private int directionY;	// Down ~ -1, stopped ~ 0, up ~ 1
	private int nextDir;	// Horizontal ~ 0, vertical ~ 1
	private float dex;		// Distance from prevX to currX point
	private float dey;		// Distance from prevY to currY point

	// Variables containing the actual x-values of line start, end and speed
	public float xMin = -6.5f;
	public float xMax = 6.5f;
	public float yMin = -1.5f;
	public float yMax = 6.5f;
	private float speed = 0.5f;
	private AudioSource audioSource;

	// Initialization
	// For the maximum startValueX/Y and finishValueX/Y shall be
	// |startValueX| = |finishValueX| = 6.5, startValueY = 6.5 & finishValueY = -1.5
	// So we will display numbers in x ~ [-32,32] and y ~ [-20,20]
	// startPointX ~ [-10,10], finishPointX ~ [-32,-15] || [15,32]
	// startPointY ~ [-7,7], finishPointY ~ [-20,-10] || [10,20]
	void Start () {

		audioSource = GetComponent<AudioSource> ();
		randStartX = Random.Range(-10, 10);
		randFinishX = Random.Range(14, 30);
		if (Random.Range(1,4) <= 2) {
			randFinishX = -randFinishX;
		}
		randStartY = Random.Range(-7, 7);
		randFinishY = Random.Range(9, 17);
		if (Random.Range(1,4) <= 2) {
			randFinishY = -randFinishY;
		}
		print (randStartX);
		print (randStartY);
		print (randFinishX);
		print (randFinishY);

		InitializeRobot (randStartX, randStartY, currPointX, currPointY, randFinishX, randFinishY);

	}
	
	// Update is called once per frame
	// movs contains the sequence of moves entered
	// q,w,e ~ left -- a,s,d ~ right -- r, t, y ~ up -- f, g, h ~ down
	private string movs = "";
	private float nextClick = 0.0f;
	public float clickRate = 0.25f;

	void Update () {
		
		string pressTag = "errPress";

		// Get mouse input
		if (Input.GetMouseButton (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				string hitTag = hit.transform.tag;
				// Handle clicks to restart the game
				if (((hitTag == "butRestart") || (hitTag == "pressdRestart") || (winRestart > 0)) && (Time.time > nextClick)) {
					clickdRestart = clickdRestart + 1;
					nextClick = Time.time + clickRate;
					print (clickdRestart);
					if (clickdRestart > 1) {
						GameObject[] prevList;
						prevList = GameObject.FindGameObjectsWithTag ("pressdRestart");
						for (int i = 0; i < prevList.Length; i++) {
							Destroy (prevList [i]);
						}
						clickdRestart = 0;
						Application.LoadLevel ("Scene01");
					}
					else {
						Vector3 butPosition = pressRestartValues;
						Quaternion butRotation = Quaternion.identity;
						GameObject pressButton = Instantiate (pressRestart, butPosition, butRotation) as GameObject;
						pressButton.gameObject.tag = "pressdRestart";
					}
				}

				// Clicks won't work when in play-mode
				if (playMode == 0) {
					// Handle click on the control panel icons -- to add actions on the actionList
					if ((hitTag == "left10") && (Time.time > nextClick)) {
						movs = movs + "q";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "left05") && (Time.time > nextClick)) {
						movs = movs + "w";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "left01") && (Time.time > nextClick)) {
						movs = movs + "e";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "right01") && (Time.time > nextClick)) {
						movs = movs + "a";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "right05") && (Time.time > nextClick)) {
						movs = movs + "s";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "right10") && (Time.time > nextClick)) {
						movs = movs + "d";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "up10") && (Time.time > nextClick)) {
						movs = movs + "r";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "up05") && (Time.time > nextClick)) {
						movs = movs + "t";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "up01") && (Time.time > nextClick)) {
						movs = movs + "y";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "down01") && (Time.time > nextClick)) {
						movs = movs + "f";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "down05") && (Time.time > nextClick)) {
						movs = movs + "g";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					if ((hitTag == "down10") && (Time.time > nextClick)) {
						movs = movs + "h";
						nextClick = Time.time + clickRate;
						EnqueueToActionList (movs);
					}
					// Handle click on the actionList -- to remove actions from the actionList
					if ((hitTag == "action01") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						// Calculate new actionList (movs) -- update actionList
						movs = DeleteFromActionList (movs, 1);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action02") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 2);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action03") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 3);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action04") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 4);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action05") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 5);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action06") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 6);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action07") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 7);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action08") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 8);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action09") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 9);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action10") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 10);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action11") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 11);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action12") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 12);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action13") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 13);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action14") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 14);
						EnqueueToActionList (movs);
					}
					if ((hitTag == "action15") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						movs = DeleteFromActionList (movs, 15);
						EnqueueToActionList (movs);
					}

					// Enter play mode -- click on Play button
					if ((hitTag == "butPlay") && (Time.time > nextClick)) {
						nextClick = Time.time + clickRate;
						currPointX = randStartX;
						currPointY = randStartY;

						// Update Play-button
						pressTag = "pressdPlay";
						Vector3 pressPosition = new Vector3 (pressPlayValues.x, pressPlayValues.y, pressPlayValues.z);
						Quaternion pressRotation = Quaternion.identity;
						GameObject pressButton = Instantiate (pressPlay, pressPosition, pressRotation) as GameObject;
						pressButton.gameObject.tag = pressTag;
						// Update Stop-button -- in theory there should be only one item
						// tagged pressdStop, but search for multiple entries just in case
						GameObject[] prevList;
						prevList = GameObject.FindGameObjectsWithTag ("pressdStop");
						for (int i = 0; i < prevList.Length; i++) {
							Destroy (prevList [i]);
						}
						// Delete x-value lines from previous plays
						prevList = GameObject.FindGameObjectsWithTag ("line");
						for (int i = 0; i < prevList.Length; i++) {
							Destroy (prevList [i]);
						}
						prevList = GameObject.FindGameObjectsWithTag ("breaknum");
						for (int l = 0; l < prevList.Length; l++) {
							Destroy (prevList[l]);
						}

						prevList = GameObject.FindGameObjectsWithTag ("pressdPlay");
						if (prevList.Length > 0) {
							InitializeRobot (randStartX, randStartY, randStartX, randStartY, randFinishX, randFinishY);
						}
						// Actually enter play mode
						playMode = 1;
						print (movs);
						StartCoroutine (MoveRobot (movs));
					}
				}
				// Stop play mode -- click on Stop button
				// Stop button should work even when in play-mode
				if ((hitTag == "butStop") && (Time.time > nextClick)) {
					nextClick = Time.time + clickRate;
					// Update Stop-button
					pressTag = "pressdStop";
					Vector3 pressPosition = new Vector3 (pressStopValues.x, pressStopValues.y, pressStopValues.z);
					Quaternion pressRotation = Quaternion.identity;
					GameObject pressButton = Instantiate (pressStop, pressPosition, pressRotation) as GameObject;
					pressButton.gameObject.tag = pressTag;
					// Update Play-button -- in theory there should be only one item
					// tagged pressdPlay, but search for multiple entries just in case
					GameObject[] prevList;
					prevList = GameObject.FindGameObjectsWithTag ("pressdPlay");
					for (int i = 0; i < prevList.Length; i++){
						Destroy (prevList[i]);
					}
					// Re-initialize robot and start-finish lines
					playMode = 0;
					InitializeRobot (randStartX, randStartY, randStartX, randStartY, randFinishX, randFinishY);
				}
			}
		}

		// ADDSTUFF -- Real-time gameplay
		if (playMode == 1) {
			if (rolling == 1) {
				WalkRobot (prevPointX, currPointX, prevPointY, currPointY, directionX, directionY, nextDir, dex, dey, speed);
				if (nextDir == 0) {
					//print ("Move horizontally, dammit!");
					if (directionX > 0) {
						if (nearlyEqual (robotCl.transform.position.x, ValueX (currPointX), 0.05f) || (robotCl.transform.position.x > ValueX (currPointX))) {
							rolling = 0;
						}
					} 
					else {
						if (nearlyEqual (robotCl.transform.position.x, ValueX (currPointX), 0.05f) || (robotCl.transform.position.x < ValueX (currPointX))) {
							rolling = 0;
						}
					}
				} 
				else {
					//print ("Move vertically, dammit!");
					if (directionY > 0) {
						if (nearlyEqual (robotCl.transform.position.y, robotValues.y + ValueY (currPointY), 0.05f) || (robotCl.transform.position.y > robotValues.y + ValueY (currPointY))) {
							rolling = 0;
						}
					} 
					else {
						if (nearlyEqual (robotCl.transform.position.y, robotValues.y + ValueY (currPointY), 0.05f) || (robotCl.transform.position.y < robotValues.y + ValueY (currPointY))) {
							rolling = 0;
						}
					}
				}
			}
		}

	}

	// Initialize robot and start-finish lines
	void InitializeRobot (int randStartX, int randStartY, int currPointX, int currPointY, int randFinishX, int randFinishY) {
		string robotTag = "Player";

		currPointX = randStartX;
		prevPointX = randStartX;
		currPointY = randStartY;
		prevPointY = randStartY;
		playMode = 0;
		rolling = 0;
		directionX = 0;
		directionY = 0;
		numPerm = 1;
		clickdRestart = 0;
		winRestart = 0;

		// Delete robot copies from previous plays
		// Must delete only if there is at least one pressdPlay-tagged object
		GameObject[] prevList;
		prevList = GameObject.FindGameObjectsWithTag ("pressdPlay");
		if (prevList.Length > 0) {
			GameObject[] prevList2;
			prevList2 = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < prevList2.Length; i++) {
				Destroy (prevList2[i]);
			}
		}
		prevList = GameObject.FindGameObjectsWithTag ("youwin");
		if (prevList.Length > 0) {
			GameObject[] prevList2;
			prevList2 = GameObject.FindGameObjectsWithTag ("youwin");
			for (int i = 0; i < prevList2.Length; i++) {
				Destroy (prevList2[i]);
			}
		}
		prevList = GameObject.FindGameObjectsWithTag ("permnum");
		for (int l = 0; l < prevList.Length; l++) {
			Destroy (prevList[l]);
		}

		// Initialize poisition-rotation for the lines and the robot
		Vector3 startPosition = new Vector3 (ValueX(randStartX), startValues.y + ValueY(randStartY), startValues.z);
		Vector3 robotStart = new Vector3 (startPosition.x, robotValues.y + ValueY(randStartY), robotValues.z);
		Vector3 finishPosition = new Vector3 (ValueX(randFinishX), finishValues.y + ValueY(randFinishY), finishValues.z);
		Quaternion startRotation = Quaternion.identity;
		Quaternion robotRotation = Quaternion.identity;
		Quaternion finishRotation = Quaternion.identity;

		// Instantiate poisition-rotation for the start-finish lines and the robot
		//Instantiate (robot, robotStart, robotRotation);
		Instantiate (startPoint, startPosition, startRotation);
		Instantiate (finishPoint, finishPosition, finishRotation);
		robotCl = Instantiate (robot, robotStart, robotRotation) as GameObject;
		robotCl.gameObject.tag = robotTag;

		// Intantiate poisition-rotation for the numbers at the start -- Convert randStart to string
		// Instantiate numbers at the start
		string strStartX = randStartX.ToString();
		string strStartY = randStartY.ToString();
		Vector3 numStartPosition = new Vector3 (numStartValues.x + ValueX(randStartX), numStartValues.y + ValueY(randStartY), numStartValues.z);
		Quaternion numStartRotation = Quaternion.identity;
		InitializeNumbers (strStartX, strStartY, numStartPosition, numStartRotation, numPerm);
		// Instantiate poisition-rotation for the numbers at the finish -- Convert randFinish to string
		// Instantiate numbers at the finish
		string strFinishX = randFinishX.ToString();
		string strFinishY = randFinishY.ToString();
		Vector3 numFinishPosition = new Vector3 (numFinishValues.x + ValueX(randFinishX), numFinishValues.y + ValueY(randFinishY), numFinishValues.z);
		Quaternion numFinishRotation = Quaternion.identity;
		InitializeNumbers (strFinishX, strFinishY, numFinishPosition, numFinishRotation, numPerm);
	}

	// Initialize numbers in the gameview
	void InitializeNumbers (string strNumX, string strNumY, Vector3 numPosition, Quaternion numRotation, int numPerm) {
		// Check the X-digits one-by-one and instantiate the corresponding number
		GameObject currNum;
		string numTag;
		if (numPerm == 0) {
			numTag = "breaknum";
		} 
		else {
			numTag = "permnum";
		}
		for (int i = 0; i < strNumX.Length; i++) 
		{
			char strCurr = strNumX [i];
			switch (strCurr)
			{
			case '1':
				currNum = Instantiate(numOne, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '2':
				currNum = Instantiate(numTwo, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '3':
				currNum = Instantiate(numThree, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '4':
				currNum = Instantiate(numFour, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '5':
				currNum = Instantiate(numFive, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '6':
				currNum = Instantiate(numSix, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '7':
				currNum = Instantiate(numSeven, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '8':
				currNum = Instantiate(numEight, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '9':
				currNum = Instantiate(numNine, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '0':
				currNum = Instantiate(numZero, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '-':
				currNum = Instantiate(numMinus, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			}
			numPosition.x = numPosition.x + 0.2f;
		}
		// Insert comma
		Vector3 commaPosition = new Vector3 (numPosition.x, numPosition.y - 0.25f, numPosition.z);
		currNum = Instantiate(numComma, commaPosition, numRotation) as GameObject;
		currNum.gameObject.tag = numTag;
		numPosition.x = numPosition.x + 0.2f;
		// Check the Y-digits one-by-one and instantiate the corresponding number
		for (int i = 0; i < strNumY.Length; i++) 
		{
			char strCurr = strNumY [i];
			switch (strCurr)
			{
			case '1':
				currNum = Instantiate(numOne, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '2':
				currNum = Instantiate(numTwo, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '3':
				currNum = Instantiate(numThree, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '4':
				currNum = Instantiate(numFour, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '5':
				currNum = Instantiate(numFive, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '6':
				currNum = Instantiate(numSix, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '7':
				currNum = Instantiate(numSeven, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '8':
				currNum = Instantiate(numEight, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '9':
				currNum = Instantiate(numNine, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '0':
				currNum = Instantiate(numZero, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			case '-':
				currNum = Instantiate(numMinus, numPosition, numRotation) as GameObject;
				currNum.gameObject.tag = numTag;
				break;
			}
			numPosition.x = numPosition.x + 0.2f;
		}
	}

	// Calculation of the robot's new position according to movs
	// Defined as a coroutine so Wait can be used
	IEnumerator MoveRobot (string movs) {
		char nextMov;
		float timish;
		GameObject tadaCl;
		Vector3 tadaPosition;
		Quaternion tadaRotation;
		// Variables holding current point position-rotation to be used for instantiation -- Convert currPoint to string
		//string currStr = currPoint.ToString();
		Vector3 currPosition = new Vector3 (ValueX(currPointX), robotValues.y + ValueY(currPointY), robotValues.z);
		Quaternion currRotation = Quaternion.identity;
		
		numPerm = 0;

		for (int i = 0; i < movs.Length; i++) {
			// End if stop-button is pushed
			if (playMode == 0) {
				yield return false;
			}
			prevPointX = currPointX;
			prevPointY = currPointY;
			nextMov = movs [i];
			switch (nextMov) 
			{
			case 'q':
				currPointX = currPointX - 10;
				nextDir = 0;
				break;
			case 'w':
				currPointX = currPointX - 5;
				nextDir = 0;
				break;
			case 'e':
				currPointX = currPointX - 1;
				nextDir = 0;
				break;
			case 'a':
				currPointX = currPointX + 1;
				nextDir = 0;
				break;
			case 's':
				currPointX = currPointX + 5;
				nextDir = 0;
				break;
			case 'd':
				currPointX = currPointX + 10;
				nextDir = 0;
				break;
			case 'r':
				currPointY = currPointY + 10;
				nextDir = 1;
				break;
			case 't':
				currPointY = currPointY + 5;
				nextDir = 1;
				break;
			case 'y':
				currPointY = currPointY + 1;
				nextDir = 1;
				break;
			case 'f':
				currPointY = currPointY - 1;
				nextDir = 1;
				break;
			case 'g':
				currPointY = currPointY - 5;
				nextDir = 1;
				break;
			case 'h':
				currPointY = currPointY - 10;
				nextDir = 1;
				break;
			}
			print (currPointX);
			print (currPointY);
			// Calculate direction and time to yield
			// With speed ~ 0.5 the default time to add is 0.5
			if (prevPointX < currPointX) {
				directionX = 1;
				timish = 0.5f + (currPointX - prevPointX)/10;
			} else {
				directionX = -1;
				timish = 0.5f + (prevPointX - currPointX)/10;
			}
			if (prevPointY < currPointY) {
				directionY = 1;
				timish = 0.5f + (currPointY - prevPointY)/10;
			} else {
				directionY = -1;
				timish = 0.5f + (prevPointY - currPointY)/10;
			}
			rolling = 1;
			dex = Mathf.Abs (currPointX - prevPointX);
			dey = Mathf.Abs (currPointY - prevPointY);
			timish = Mathf.Min (timish, 1.5f);
			// For some reason when moving along the y-axis and dey = 10 waiting takes more than expected
			WalkRobot (prevPointX, currPointX, prevPointY, currPointY, directionX, directionY, nextDir, dex, dey, speed);
			yield return new WaitForSeconds (timish);
			currPosition = new Vector3 (ValueX(currPointX), startValues.y + ValueY(currPointY), startValues.z);
			GameObject breakLine = Instantiate (breakPoint, currPosition, currRotation) as GameObject;
			breakLine.gameObject.tag = "line";
			// ADDSTUFF
			GameObject[] prevList;
			prevList = GameObject.FindGameObjectsWithTag ("breaknum");
			for (int l = 0; l < prevList.Length; l++) {
				Destroy (prevList[l]);
			}
			// Intantiate poisition-rotation for the numbers -- Convert currPoint to string
			// Instantiate numbers
			string strCurrX = currPointX.ToString();
			string strCurrY = currPointY.ToString();
			Vector3 numCurrPosition = new Vector3 (numStartValues.x + ValueX(currPointX), numStartValues.y + ValueY(currPointY), numStartValues.z);
			Quaternion numCurrRotation = Quaternion.identity;
			InitializeNumbers (strCurrX, strCurrY, numCurrPosition, numCurrRotation, numPerm);
		}
		// Check the result
		if ((currPointX == randFinishX) && (currPointY == randFinishY)){
			//Vector3 bravoPosition = new Vector3 (0, 0, 0);
			//Quaternion bravoRotation = Quaternion.identity;
			//GameObject bravo = Instantiate (youWin, bravoPosition, bravoRotation) as GameObject;
			//bravo.gameObject.tag = "youwin";
			GameObject[] prevList;
			prevList = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < prevList.Length; i++) {
				Destroy (prevList[i]);
			}
			tadaPosition = robotCl.transform.position;
			tadaRotation = robotCl.transform.rotation;
			tadaCl = Instantiate (tada, tadaPosition, tadaRotation) as GameObject;
			tadaCl.gameObject.tag = "youwin";
			winRestart = 1;
			clickdRestart = 1;
			// Manually display restart button
			Vector3 butPosition = pressWinstartValues;
			Quaternion butRotation = Quaternion.identity;
			GameObject pressButton = Instantiate (pressWinstart, butPosition, butRotation) as GameObject;
			pressButton.gameObject.tag = "pressdRestart";
		}
		playMode = 0;
	}

	// Calculate the x-value of the robot or the start-finish lines
	// given the actual theoritical value
	// |startValueX| = |finishValueX| = 6.5, display numbers in [-32,32]
	float ValueX (float cufPos) {
		float actPos;
		actPos = cufPos / 5;
		return (actPos);
	}

	// Calculate the x-value of the robot or the start-finish lines
	// given the actual theoritical value
	// startValueY = 6.5 & finishValueY = -1.5, display numbers in [-20,20]
	float ValueY (float cufPos) {
		float actPos;
		actPos = cufPos / 5;
		return (actPos);
	}

	// Check for equality in floats
	bool nearlyEqual(float a, float b, float epsilon) {
		float absA = Mathf.Abs(a);
		float absB = Mathf.Abs(b);
		float absD = Mathf.Abs(absA - absB);

		if ( a * b < 0f) {
			return false;
		} else if (a == b) {
			return true;
		} else {
			return absD < epsilon;
		}
	}

	// Display the movement from one point to another
	// For default numbering system -- displaying numbers in [-32,32]
	// dex = dx' = {1, 5, 10}, dx = {0.2, 1, 2}, same for dey = dy', dy
	void WalkRobot (int prevPointX, int currPointX, int prevPointY, int currPointY, int directionX, int directionY, int nextDir, float dex, float dey, float speed) {
		Vector3 vic = Vector3.zero;
		audioSource.Play ();
		if (nextDir == 0) {
			switch (directionX) {
			case 1:
				//print ("On your right!");
				// Turn robot's face right
				robotCl.transform.localScale = new Vector3(2, 2, 0);
				vic = new Vector3 (dex, 0f, 0f);
				break;
			case -1:
				//print ("On your left!");
				// Turn robot's face left
				robotCl.transform.localScale = new Vector3(-2, 2, 0);
				vic = new Vector3 (-dex, 0f, 0f);
				break;
			}
		}
		else {
			switch (directionY) {
			case 1:
				//print ("Look up!");
				// Turn robot's face right
				robotCl.transform.localScale = new Vector3(2, 2, 0);
				vic = new Vector3 (0f, dey, 0f);
				break;
			case -1:
				//print ("Look down!");
				// Turn robot's face left
				robotCl.transform.localScale = new Vector3(-2, 2, 0);
				vic = new Vector3 (0f, -dey, 0f);
				break;
			}
		}
		robotCl.transform.Translate (vic*speed*Time.deltaTime);

	}

	// Enqueue actions to the actionList
	void EnqueueToActionList(string movs){
		char nextMov;
		string nextTag = "errList";
		// Initialize poisition-rotation for the action list
		Vector3 listPosition = new Vector3 (listStartValues.x, listStartValues.y, listStartValues.z);
		Quaternion listRotation = Quaternion.identity;

		// List containing the actions -- To be used for tags
		GameObject[] actionList;
		actionList = new GameObject[15];

		// Delete items created from previous function calls
		GameObject[] prevList;
		if (movs.Length >= 0){
			// Delete copies of items tagged "action01"
			prevList = GameObject.FindGameObjectsWithTag ("action01");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action02"
			prevList = GameObject.FindGameObjectsWithTag ("action02");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action03"
			prevList = GameObject.FindGameObjectsWithTag ("action03");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action04"
			prevList = GameObject.FindGameObjectsWithTag ("action04");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action05"
			prevList = GameObject.FindGameObjectsWithTag ("action05");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action06"
			prevList = GameObject.FindGameObjectsWithTag ("action06");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action07"
			prevList = GameObject.FindGameObjectsWithTag ("action07");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action08"
			prevList = GameObject.FindGameObjectsWithTag ("action08");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action09"
			prevList = GameObject.FindGameObjectsWithTag ("action09");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action10"
			prevList = GameObject.FindGameObjectsWithTag ("action10");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action11"
			prevList = GameObject.FindGameObjectsWithTag ("action11");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action12"
			prevList = GameObject.FindGameObjectsWithTag ("action12");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action13"
			prevList = GameObject.FindGameObjectsWithTag ("action13");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action14"
			prevList = GameObject.FindGameObjectsWithTag ("action14");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
			// Delete copies of items tagged "action15"
			prevList = GameObject.FindGameObjectsWithTag ("action15");
			for (int i = 0; i < prevList.Length; i++){
				Destroy (prevList[i]);
			}
		}

		for (int i = 0; i < movs.Length; i++) {
			nextMov = movs [i];
			// Calculate current tag
			switch (i){
			case 0:
				nextTag = "action01";
				break;
			case 1:
				nextTag = "action02";
				break;
			case 2:
				nextTag = "action03";
				break;
			case 3:
				nextTag = "action04";
				break;
			case 4:
				nextTag = "action05";
				break;
			case 5:
				nextTag = "action06";
				break;
			case 6:
				nextTag = "action07";
				break;
			case 7:
				nextTag = "action08";
				break;
			case 8:
				nextTag = "action09";
				break;
			case 9:
				nextTag = "action10";
				break;
			case 10:
				nextTag = "action11";
				break;
			case 11:
				nextTag = "action12";
				break;
			case 12:
				nextTag = "action13";
				break;
			case 13:
				nextTag = "action14";
				break;
			case 14:
				nextTag = "action15";
				break;
				//default:
				//	print("Unexpected error!");
				//	nextTag = "errorList";
				//	break;
			}

			// Get the next move from movs string and add it to the action list
			switch (nextMov) {
			case 'q':
				actionList[i] = Instantiate(listBk10, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'w':
				actionList[i] = Instantiate(listBk05, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'e':
				actionList[i] = Instantiate(listBk01, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'a':
				actionList[i] = Instantiate(listFw01, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 's':
				actionList[i] = Instantiate(listFw05, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'd':
				actionList[i] = Instantiate(listFw10, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'r':
				actionList[i] = Instantiate(listUp10, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 't':
				actionList[i] = Instantiate(listUp05, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'y':
				actionList[i] = Instantiate(listUp01, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'f':
				actionList[i] = Instantiate(listDn01, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'g':
				actionList[i] = Instantiate(listDn05, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			case 'h':
				actionList[i] = Instantiate(listDn10, listPosition, listRotation) as GameObject;
				actionList[i].gameObject.tag = nextTag;
				actionList[i].gameObject.name = nextTag;
				break;
			}
			// Change the y-axis value of listPosition -- To be used for the next action
			// Due to the possibility of 10+ moves, a second column is used when reaching 7
			// CHANGE TO EIGHT SO THERE's ROOM FOR 15 MOVS
			if (i == 7) {
				listPosition.x = listPosition.x + 1.1f;
				listPosition.y = listStartValues.y;
			} 
			else {
				listPosition.y = listPosition.y - 1.1f;
			}
		}
	}

	// Delete the m-th move from the actionList
	// Given the way this function is called it is impossible to try and delete an invalid move
	string DeleteFromActionList (string movs, int m) {
		char nextMov;
		string newMov = "";
		int acts = movs.Length;

		if (m == 1) {
			for (int i = 1; i < movs.Length; i++) {
				nextMov = movs [i];
				newMov = newMov + nextMov;
			}
		} else if (m == acts) {
			for (int i = 0; i < acts - 1; i++) {
				nextMov = movs [i];
				newMov = newMov + nextMov;
			}
		} else {
			for (int i = 0; i < m - 1; i++) {
				nextMov = movs [i];
				newMov = newMov + nextMov;
			}
			for (int i = m; i < acts; i++) {
				nextMov = movs [i];
				newMov = newMov + nextMov;
			}
		}
		return (newMov);
	}

}
