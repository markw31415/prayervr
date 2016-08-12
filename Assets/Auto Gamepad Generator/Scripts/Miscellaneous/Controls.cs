using UnityEngine;
using System;
using System.Collections;

public class Controls : MonoBehaviour {

    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    public string playerNumber;


    Rigidbody myRigidbody;
    float horizontal, vertical;
    float sensitivity = 5f;
    Vector3 actionVectorPosition;
    Vector3 computerVector;
    string currentColor;
    bool cycleLock = false;

    void Start()
    {
        myRigidbody = this.GetComponent<Rigidbody>();

        if (currentColor == null)
        {
            currentColor = "Red";
        }
    }

    void Update()
    {

        steerBall();
        changeColor();
    }

    public void changeColor ()
    {

        if (Input.GetButtonDown(playerNumber + "_ZRed"))
        {
            this.GetComponent<MeshRenderer>().material = redMaterial;
            currentColor = "Red";
        }


        if (Input.GetButtonDown(playerNumber + "_ZBlue"))
        {
            this.GetComponent<MeshRenderer>().material = blueMaterial;
            currentColor = "Blue";
        }

        if (Input.GetButtonDown(playerNumber + "_ZGreen"))
        {
            this.GetComponent<MeshRenderer>().material = greenMaterial;
            currentColor = "Green";
        }

        if (Input.GetButtonDown(playerNumber + "_ZYellow"))
        {
            this.GetComponent<MeshRenderer>().material = yellowMaterial;
            currentColor = "Yellow";
        }

        if (Input.GetButtonDown(playerNumber + "_ZPrevious"))
        {
            cycleColor(false, currentColor);            
        }

        if (Input.GetButtonDown(playerNumber + "_ZNext"))
        {
            cycleColor(true, currentColor);           
        }

        if (Input.GetAxis(playerNumber + "_ZCycle") < 0f && !cycleLock )
        {
            cycleColor(false, currentColor);
            cycleLock = true;
            Invoke("unlock", 100f / (Input.GetAxis(playerNumber + "_ZCycle") * 2000f));
        }

        if (Input.GetAxis(playerNumber + "_ZCycle") > 0f && !cycleLock)
        {
            cycleColor(true, currentColor);
            cycleLock = true;
            Invoke("unlock", 100f / (Input.GetAxis(playerNumber + "_ZCycle") * 2000f));
        }
    }

    public void steerBall()
    {

        horizontal = Input.GetAxis(playerNumber + "_Horizontal");
        vertical = Input.GetAxis(playerNumber + "_Vertical");

        horizontal *= -sensitivity;
        vertical *= -sensitivity;

        actionVectorPosition.x = horizontal;
        actionVectorPosition.y = 0f;
        actionVectorPosition.z = vertical;

        myRigidbody.AddForce(actionVectorPosition);

    }

    public void cycleColor(bool forward,string current)
    {
        if (forward)
        {
            if (current.Equals("Red"))
            {
                this.GetComponent<MeshRenderer>().material = greenMaterial;
                currentColor = "Green";
            }
            else if (current.Equals("Green"))
            {
                this.GetComponent<MeshRenderer>().material = blueMaterial;
                currentColor = "Blue";
            }
            else if (current.Equals("Blue"))
            {
                this.GetComponent<MeshRenderer>().material = yellowMaterial;
                currentColor = "Yellow";
            }
            else
            {
                this.GetComponent<MeshRenderer>().material = redMaterial;
                currentColor = "Red";
            }
        }
        else
        {
            if (current.Equals("Red"))
            {
                this.GetComponent<MeshRenderer>().material = yellowMaterial;
                currentColor = "Yellow";
            }
            else if (current.Equals("Green"))
            {
                this.GetComponent<MeshRenderer>().material = redMaterial;
                currentColor = "Red";
            }
            else if (current.Equals("Blue"))
            {
                this.GetComponent<MeshRenderer>().material = greenMaterial;
                currentColor = "Green";
            }
            else
            {
                this.GetComponent<MeshRenderer>().material = blueMaterial;
                currentColor = "Blue";
            }
        }
    }

    public void unlock()
    {
        cycleLock = false;
    }





}
