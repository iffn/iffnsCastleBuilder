using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyHumanPlayerController : MonoBehaviour
{
    [SerializeField] GameObject Head;
    [SerializeField] float WalkSpeed = 2f;
    [SerializeField] float RunSpeed = 4f;
    [SerializeField] float RotationSpeed = 600f;
    [SerializeField] float JumpStrength = 50f;
    [SerializeField] NavigationTools LinkedNavigationTools;
    [SerializeField] float colliderDiameter = 0.4f;
    [SerializeField] float colliderHeight = 1.65f;
    [SerializeField] float eyeHeight = 1.65f;

    Rigidbody currentRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        currentRigidbody = transform.GetComponent<Rigidbody>();

        if(LinkedNavigationTools == null) MouseUnlockAndVisibilityType = false;
        
        CapsuleCollider collider = transform.GetComponent<CapsuleCollider>();
        collider.radius = colliderDiameter * 0.5f;
        collider.height = colliderHeight;
        collider.center = Vector3.up * colliderHeight * 0.5f;
        
        /*
        CapsuleCollider collider = transform.GetComponent<CapsuleCollider>();
        collider.radius = 0.5f;
        collider.height = 2f;
        collider.center = Vector3.zero;
        */


        Head.transform.localPosition = Vector3.up * eyeHeight;
    }

    void Move(Vector3 speed)
    {
        //transform.Translate(speed * Time.deltaTime);
        currentRigidbody.velocity += transform.rotation * speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime);

        Head.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime);

        currentRigidbody.velocity = new Vector3(0, currentRigidbody.velocity.y, 0);

        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Move(Vector3.forward * RunSpeed);
            }
            else
            {
                Move(Vector3.forward * WalkSpeed);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            Move(Vector3.back * WalkSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector3.left * WalkSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Move(Vector3.right * WalkSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LinkedNavigationTools.ReturnFromPlayerToRTSControls();
        }
    }

    public bool MouseUnlockAndVisibilityType
    {
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentRigidbody.AddForce(Vector3.up * JumpStrength);
        }
    }
}
