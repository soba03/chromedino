using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    [SerializeField] private Animator dinoAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;

    [Header("Audio")] [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip dieSFX;


    private bool _isGameStarted = false;
    private bool _isTouchingGround = true;
    private bool _isDead = false;

    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        bool isJumpButtonPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
        bool isCrouchButtonPressed = Input.GetKey(KeyCode.LeftControl)
                                    || Input.GetKey(KeyCode.RightControl)
                                    || Input.GetKey(KeyCode.DownArrow);

        if (!_isDead)
        {
            if (isJumpButtonPressed)
            {
                if (_isGameStarted == true && _isTouchingGround == true)
                {
                    //Jump
                    Debug.Log("Jumping...");
                    Jump();
                }

                else
                {
                    _isGameStarted = true;
                    GameManager.Instance.Beginning = true;
                }
            }
            else if (isCrouchButtonPressed && _isTouchingGround)
            {
                //Crouch
            }
        }


        dinoAnimator.SetBool("Beginning", _isGameStarted);
        dinoAnimator.SetBool("Folding", isCrouchButtonPressed && _isTouchingGround && !isJumpButtonPressed);
        dinoAnimator.SetBool("Ending", _isDead);

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            _isTouchingGround = true;
        } 
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            //Die
            _isDead = true;
           GameManager.Instance.Ending = true;

            GameManager.Instance.ShowGameEndScreen();

            audio.clip = dieSFX;
            audio.Play();
        }
    } 

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce);
        _isTouchingGround = false;

        audio.clip = jumpSFX;
        audio.Play();
    }
}
