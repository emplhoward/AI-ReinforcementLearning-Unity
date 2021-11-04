using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    float leftRightAngle = 0;
    float upDownAngle = 0;
    float currentSensitiveX = 4f;
    float currentSensitiveY = 2f;
    GameObject camBase;

    [SerializeField] GameObject player;
    float smooth = 10f;

    bool leftPos;
    Vector3 leftOffset = new Vector3(0, 1.5f, -4);
    Vector3 rightOffset = new Vector3(-0, 1.5f, -4);
    float PerspectiveRot = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetInitialCam();

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) SwitchCam();
    }

    private void LateUpdate()
    {
        MoveCamPivot();
        //RotateCamPivot();
    }

    void MoveCamPivot()
    {
        //camPivot.transform.position = Vector3.Slerp(camPivot.transform.position, playerBase.transform.position, Time.deltaTime * smooth);
        camBase.transform.position = player.transform.position;
    }

    //Set the camera to a rig, then make the camera a child of the rig.
    void RotateCamPivot()
    {
        Quaternion camPivotDirection;
        
        leftRightAngle += Input.GetAxis("Mouse X") * currentSensitiveX; //Rotation inputs for player to turn left and right:     
        upDownAngle += Input.GetAxis("Mouse Y") * currentSensitiveY;  //Rotation inputs for camera to turn up and down:
        upDownAngle = Mathf.Clamp(upDownAngle, -80, 80); //Process then clamp input before rotating:
        camPivotDirection = Quaternion.Euler(-upDownAngle, leftRightAngle, 0); //Convert the angles into a quaternion for the camera to rotate toward:

        camBase.transform.rotation = Quaternion.Slerp(camBase.transform.rotation, camPivotDirection, Time.deltaTime * smooth); //Rotate with smooth factor
        //camPivot.transform.rotation = camPivotDirection;
    }

    void SetInitialCam()
    {
        if (camBase == null)
        {
            camBase = new GameObject(); //Add a camera base as the parent; the camera will pivot around the base.
            transform.parent = camBase.transform;
        }

        if(player == null) Debug.Log("Set the player in the camera script.");

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = Vector3.zero;
        transform.position = transform.position + leftOffset;
        transform.localRotation = Quaternion.Euler(PerspectiveRot, 0, 0);
        leftPos = true;
    }

    void SwitchCam()
    {
        if(leftPos)
        {
            leftPos = false;
            transform.localPosition = Vector3.zero;
            transform.localPosition = transform.localPosition + rightOffset;
        }
        else
        {
            leftPos = true;
            transform.localPosition = Vector3.zero;
            transform.localPosition = transform.localPosition + leftOffset;
        }
    }
}
