using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class PlayerMov : MonoBehaviour
{
    Rigidbody rigidBody;
    float currentSpeed;
    float maxSpeed; //Set in Start(), otherwise it does not save.
    float runSpeed;
    float walkSpeed;
    float acceleration;
    float rotSmooth = 5;
    [SerializeField] GameObject camBase;
    [SerializeField] GameObject terrainChecker;
    [SerializeField] Animator anim;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetInitialPosition();

        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidBody.mass = 100;
        rigidBody.drag = 0.1f;
        currentSpeed = 0;
        maxSpeed = 0.2f; //Full speed.
        runSpeed = 0.2f;
        walkSpeed = 0.1f;
        acceleration = 0.2f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleRunWalk();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveLegs();
    }

    void MoveLegs()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float rightInput = Input.GetAxisRaw("Horizontal");

        Vector3 forwardVector = new Vector3(forwardInput * camBase.transform.forward.x, 0, forwardInput * camBase.transform.forward.z); //Each vector has multiple components, the direction input must multiply by each component of the vector.
        Vector3 rightVector = new Vector3(rightInput * camBase.transform.right.x, 0, rightInput * camBase.transform.right.z); //Each vector has multiple components, the direction input must multiply by each component of the vector.
        Vector3 moveVector = forwardVector + rightVector;
        moveVector = moveVector.normalized;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //If there is an input
        {
            Quaternion moveRot = Quaternion.LookRotation(moveVector);  //Get quarternion of input.
            Quaternion newRot = Quaternion.Lerp(transform.rotation, moveRot, Time.fixedDeltaTime * rotSmooth); //Lerp the turn for a smooth turn.
            rigidBody.MoveRotation(newRot); //Rotate the rigid body to the quaternion variable.

            //Accelerate
            if (currentSpeed <= maxSpeed)
            {
                currentSpeed += Time.fixedDeltaTime * acceleration;
            }
        }
        else
        {
            currentSpeed = 0;

        }

       rigidBody.MovePosition(transform.position + transform.forward * currentSpeed);
       PreventFloorClipping();
       AnimateMovement();
    }

    void PreventFloorClipping()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(terrainChecker.transform.position, -transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        {
            //Debug.Log(hit.transform);          
        }
        else
        {
            if (Physics.Raycast(transform.position + new Vector3(0,1000,0), -transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
            {
                transform.position = hit.point;
            }
        }
    }

    void ToggleRunWalk()
    {
        currentSpeed = 0f;
        if (maxSpeed > walkSpeed) maxSpeed = walkSpeed;
        else if (maxSpeed < runSpeed) maxSpeed = runSpeed;
    }

    void AnimateMovement()
    {
        if(currentSpeed >= runSpeed) anim.SetInteger("moveState", 2);
        if(currentSpeed > 0 & currentSpeed < runSpeed) anim.SetInteger("moveState", 1);
        if(currentSpeed <= 0) anim.SetInteger("moveState", 0);
        Debug.Log(currentSpeed);
    }

    void SetInitialPosition()
    {        //Sphere cast down to get a good terrain position
        RaycastHit hitDown;
        if (Physics.SphereCast(transform.position, 1f, -transform.up, out hitDown, Mathf.Infinity)) // Does the ray intersect any objects:
        {
            if (hitDown.transform.gameObject.tag == "000") //If hit terrain with tag 000
            {
                transform.position = hitDown.point;
            }
            else
            {
                //Just float down
            }
        }
    }
}
