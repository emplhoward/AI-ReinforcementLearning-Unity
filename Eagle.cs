using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    Vector3 rightFaceRot = new Vector3(0f, 90f, 0f);
    Vector3 leftFaceRot = new Vector3(0f, -90f, 0f);
    int moveState = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moveState == 0)
        {
            if (transform.position.x < 3) transform.eulerAngles = rightFaceRot;
            else moveState = 1;
        }
        
        if(moveState == 1)
        {
            if (transform.position.x > -3) transform.eulerAngles = leftFaceRot;
            else moveState = 0;
        }

        transform.position = transform.position + transform.forward * Time.deltaTime;
    }
}
