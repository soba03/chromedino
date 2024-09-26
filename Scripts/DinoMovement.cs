using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    [SerializeField] private Animator dinoAnimator;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;

    private bool _isGameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isJumpButtonPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
        bool isCrouchButtonPressed = Input.GetKeyDown(KeyCode.LeftControl) 
                                    || Input.GetKeyDown(KeyCode.RightControl) 
                                    || Input.GetKeyDown(KeyCode.DownArrow);
        
        if (isJumpButtonPressed){
            if(_isGameStarted == true){
                //Jump
                Debug.Log("Jumping...");
                Jump();
            }else{
                _isGameStarted = true;
                //Start moving
            }
        } else if (isCrouchButtonPressed){
            //Crouch

        }
    }
    void Jump(){
        rb.AddForce(Vector2.up * jumpForce);
    }
}
