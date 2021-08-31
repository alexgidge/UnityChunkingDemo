using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementController : MonoBehaviour
{
    public static MovementController Current;
    public Rigidbody2D playerBody;
    
    public int MoveSpeed; //10

    private void Start()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerBody.AddForce(new Vector2(0,MoveSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerBody.AddForce(new Vector2(-MoveSpeed * Time.deltaTime,0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerBody.AddForce(new Vector2(0,-MoveSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerBody.AddForce(new Vector2(MoveSpeed * Time.deltaTime, 0));
        }
    }
}
