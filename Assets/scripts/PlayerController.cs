using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
 	[SerializeField] float speedNormal = 5f;
    [SerializeField] float speedSprint = 7f;
	float speedActual;

    [SerializeField] float sensitivity = 2f; 

 	[SerializeField] float jumpForce = 5f;

	[SerializeField] float staminaMax = 5f;
    [SerializeField] TMP_Text staminaPercent;
    float stamina = 0;
	bool isSprinting;


	[SerializeField] TMP_Text fps_text;
    

    [SerializeField] int wallJumpMax = 2;
	int wallJumpActual = 0;

	Rigidbody rb;
	Camera playerCamera;

	bool isGrounded = true;
	bool isWalled = false; //TODO change name

	int number_of_frames;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		fps_text.enabled = false;

		rb.freezeRotation = true; 
		playerCamera = Camera.main;
		Cursor.lockState = CursorLockMode.Locked; 
		Cursor.visible = false;
		speedActual = speedNormal;

		Invoke("GetFramesPerSecond",1);
		RegenerateStanine();
	}

	void Update()
	{
		MovePlayer();
		RotateCamera();

		if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f && isGrounded)
		{
            if (stamina > 0)
            {
                stamina -= Time.deltaTime;
                speedActual = speedSprint;
				isSprinting = true;
            }
            else
            {
                speedActual = speedNormal;
				isSprinting = false;
            }
        }
        else
		{
			speedActual = speedNormal;
            isSprinting = false;
        }

		if (((isGrounded || (isWalled && wallJumpActual > 0)) && Input.GetButtonDown("Jump") && stamina > 1f))
		{
			Jump();
			stamina = Mathf.Round((stamina - 1f) * 10) / 10;


            if (isWalled)
			{
				wallJumpActual--;
			}
			
		}
		if (stamina < 0)
		{
			stamina = 0;
		}

		
		if (Input.GetKeyDown(KeyCode.F3))
		{
			fps_text.enabled = !fps_text.enabled;
		}
		staminaPercent.SetText((Mathf.Round(stamina/staminaMax*100f)).ToString()+"%");

		number_of_frames++;
	}

	//Control player 
	private void MovePlayer()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(horizontal, 0f, vertical) * speedActual * Time.deltaTime;
		movement = transform.TransformDirection(movement); 
		rb.MovePosition(rb.position + movement);
	}

	private void RotateCamera()
	{
		float mouseX = Input.GetAxis("Mouse X") * sensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

		rb.rotation *= Quaternion.Euler(Vector3.up * mouseX);

		
		Vector3 currentRotation = playerCamera.transform.eulerAngles;
		float newRotationX = currentRotation.x - mouseY;
		newRotationX = Mathf.Clamp(newRotationX, 0f, 180f); 
		playerCamera.transform.eulerAngles = new Vector3(newRotationX, currentRotation.y, 0f);
	}

	private void Jump()
	{
		rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

	}

    void RegenerateStanine()
    {
        if ((stamina < staminaMax) && isGrounded && !isSprinting)
        {
            stamina = Mathf.Round((stamina  +0.1f)*10)/10;
        }
        Invoke("RegenerateStanine", 0.1f);
    }

    //colisions

    void OnCollisionEnter(Collision collision)
	{

		if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Ladder") || collision.collider.CompareTag("Wall")) 
		{
			isGrounded = true;
			wallJumpActual = wallJumpMax;
		}   
		if (collision.collider.CompareTag("Ladder"))
		{

			rb.useGravity = false;
		}

	}

    void OnCollisionExit(Collision collision)
	{
		if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Ladder"))
		{
			isGrounded = false;
		}
		if (collision.collider.CompareTag("Ladder"))
		{
			rb.useGravity = true;
		}
		if (collision.collider.CompareTag("Wall"))
		{
			isWalled = false;
		}
	}

	//FPS
	void GetFramesPerSecond()
	{ 
		fps_text.text = "FPS: " + number_of_frames.ToString();
		number_of_frames = 0;
		
		Invoke("GetFramesPerSecond",1);

	}
}