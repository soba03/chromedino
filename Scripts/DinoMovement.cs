using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    [SerializeField] private Animator dinoAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;

    [Header("Audio")] 
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip dieSFX;

    private bool _isGameStarted = false;
    private bool _isTouchingGround = true;
    private bool _isDead = false;
    private bool _hasShield = false; // Trạng thái có khiên

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
                // Crouch logic
            }
        }

        dinoAnimator.SetBool("Beginning", _isGameStarted);
        dinoAnimator.SetBool("Folding", isCrouchButtonPressed && _isTouchingGround && !isJumpButtonPressed);
        dinoAnimator.SetBool("Ending", _isDead);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            _hasShield = true; // Kích hoạt khiên khi khủng long nhặt được
            Destroy(other.gameObject); // Xóa khiên khỏi màn chơi
        }
        else if (other.CompareTag("Obstacle"))
        {
            if (_hasShield)
            {
                // Nếu có khiên, phá hủy chướng ngại vật và vô hiệu hóa khiên
                Destroy(other.gameObject);
                _hasShield = false; // Khiên bị mất sau khi va chạm
            }
            else
            {
                _isDead = true; // Khủng long chết nếu không có khiên
                GameManager.Instance.Ending = true; // Chỉ bật Ending khi khủng long thực sự chết
                GameManager.Instance.ShowGameEndScreen();
                audio.clip = dieSFX;
                audio.Play();
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            _isTouchingGround = true;
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
