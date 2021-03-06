using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UniversalBallController : MonoBehaviour {

	
	public MapContstraints 	mapConstraints;
	public float 			startSpeed;
	public float 			startDelay;
	public GameObject       collisionEffect;


	const float             velocityConstraint  = 0.5f;
	bool                    ballInPlay          = false;
	int                     volleyCount         = 0;
	GameObject              collisionEffectInst;
	ParticleSystem          collisionParticles;
	Rigidbody               rb;
	float                   speed;
	ObjectRangeOfMotion     motionField;
	AxisOfPlay              axisOfPlay;
	Vector3                 english;
	RigidbodyConstraints    rbConstraints;

	// Use this for initialization
	void Start () {
		rb                      = GetComponent<Rigidbody> ();
		rb.maxAngularVelocity   = 50;
		speed                   = startSpeed;
		motionField             = mapConstraints.objectMotionField;
		axisOfPlay              = mapConstraints.axisOfPlay;
		english                 = Vector3.zero;
		collisionEffectInst     = Instantiate(collisionEffect);
		collisionParticles      = collisionEffectInst.GetComponent<ParticleSystem>();

		if(motionField == ObjectRangeOfMotion.PLANAR) {
			rbConstraints = RigidbodyConstraints.FreezePositionY;
		}

		rb.constraints = RigidbodyConstraints.FreezeAll;

		ScoreManager.playerScore    = 0; //Emily
		ScoreManager.enemyScore     = 0; //Emily

	}

	void FixedUpdate(){
		rb.AddForce (english);
	}

	// Update is called once per frame
	void Update () {
		if (!ballInPlay) {
			ballInPlay = true;
			StartCoroutine(startBall());
		}

		else {
			if( volleyCount > 2 ){
				volleyCount = 0;
				speed *= 1.1f;
			}
		}
	}

	void LateUpdate(){
		ResetVelocity();
	}

	// Initializes the ball movement and rotation after the specified startDelay ( in seconds )
	IEnumerator startBall(){
		yield return new WaitForSeconds(startDelay);
		rb.constraints = rbConstraints;
		rb.velocity = getRandomVelocity();
		rb.angularVelocity = getRandomDir() * 5;
	}

	// Returns a velocity vector in a random direction
	// based on the getRandomDir() function. Velocity
	// magnitude will be the current ball speed.
	Vector3 getRandomVelocity(){
		Vector3 dir = getRandomDir();
		Vector3 vel = dir * speed;
		return vel;
	}

	// Returns a normalized vector pointing in a random direction
	// Will favor direction along the axis of play, as well as
	// return a vector in the expected plane of motion (for 2D gameplay)
	Vector3 getRandomDir(){
		float x = Random.Range ( -10.0f, 10.0f );
		float y = Random.Range ( -3.0f, 3.0f );
		float z = Random.Range ( -10.0f, 10.0f );
		Vector3 dir = Vector3.zero;

		if(axisOfPlay == AxisOfPlay.Z) {
			x = Random.Range( -3.0f, 3.0f );
		}

		switch (motionField) {
		case ObjectRangeOfMotion.PLANAR:
			dir = new Vector3(x, 0, z);
			break;
		case ObjectRangeOfMotion.FULL_3D:
			dir = new Vector3(x, y, z);
			break;
		}

		return dir.normalized;

	}

	public void ResetVelocity(){

		float angle = Vector3.Angle ( new Vector3(0, 0, 1), rb.velocity );
		float adjustToAngle = 0.0f;

		if( motionField == ObjectRangeOfMotion.PLANAR && angle > 60 ){
			float x = rb.velocity.x;
			float z = rb.velocity.z;
			if( z > 0 ){
				if(x > 0){
					adjustToAngle = 60;
				}else{
					adjustToAngle = 310;
				}
			} else if ( z < 0 ){
				if(x > 0){
					adjustToAngle = 120;
				}else{
					adjustToAngle = 240;
				}
			}
		}


		if(adjustToAngle > 0 ){
			float x = Mathf.Sin(adjustToAngle * Mathf.Deg2Rad);
			float z = Mathf.Cos(adjustToAngle * Mathf.Deg2Rad);
			rb.velocity = new Vector3(x, 0, z) * speed;
		} else {
			rb.velocity = rb.velocity.normalized * speed;
		}

	}

	// Resets the state of the ball to begin a new volley.
	public void resetBall(){
		ballInPlay = false;
//		eulerAngleVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
		english = Vector3.zero;
		transform.position = Vector3.zero;
		speed = startSpeed;
		volleyCount = 0;
		rb.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Returns true if the ball's velocity vector points toward the given position
	public bool isMovingToward(Vector3 pos){
		Vector3 ballToPos = pos - transform.position;
		return Vector3.Dot(rb.velocity, ballToPos) > 0;
	}

	void OnCollisionEnter( Collision coll ){

		Vector3 point = coll.contacts[0].point;
		Vector3 norm = coll.contacts[0].normal;

		collisionEffectInst.transform.position = point;
		collisionEffectInst.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -norm);
		collisionParticles.Play();

		// Increment the volley count if the ball collides with a paddle.
		if (coll.gameObject.CompareTag ("Enemy") || coll.gameObject.CompareTag ("Player")) {
			PaddleEnglish pe = coll.gameObject.GetComponent<PaddleEnglish>();
			volleyCount++;
			english.x = -pe.XVel * 50;
			english.y = -pe.YVel * 50;
			rb.velocity += new Vector3(pe.XVel * 10, pe.YVel * 10, 0);
			rb.angularVelocity += new Vector3(pe.YVel * 100, pe.XVel * 100, 0);

		} else {
			english = Vector3.zero;
		}

	}
	

//	void OnCollisionExit( Collision c ){
//
//	}
}
