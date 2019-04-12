using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VimanaVoid
{    

    public enum State
    {
        Iddle,
        Walking,
        WalkingBackward,
        Running,
        RunningBackward,
        Sprinting,
        Crouched,
        CrouchedIddle
    }

    [RequireComponent(typeof(InputController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]float walkSpeed = 5f;
        [SerializeField]float walkBackwardSpeed = 5f;
        [SerializeField]float runSpeed = 10f;
        [SerializeField]float runBackwardSpeed = 10f;
        [SerializeField]float sprintSpeed = 15f;
        [SerializeField]float crouchSpeed = 3f;
        [SerializeField]float dampingRotation = 1f;
        [SerializeField]float JumpHeight = 2f;
        public State currentState;

        // private float speed;        
        private float moveSpeed;
        private CharacterController player;
        private Camera mainCamera;
        private InputController _playerInput;
        private Vector3 moveDirection;

        public bool canSprint;
        public Text debugData;

        void Start()
        {
            player = GetComponent<CharacterController>();
            _playerInput = GetComponent<InputController>();
            mainCamera = Camera.main;
        }

        void Update()
        {
            DebugData();
            _playerInput.UpdateInput();
            MoveSpeed();
        }
    
        void FixedUpdate()
        {
            Move(); 
        }
        

        // public void Move()
        // {
        //     if(_playerInput.HasMoveInput)
        //     {
        //         var desiredRotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
        //         transform.rotation =  Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * dampingRotation);
        //     }

        //     moveDirection = (transform.forward * _playerInput.MoveInput.y) + (transform.right * _playerInput.MoveInput.x);
        //     moveDirection = moveDirection.normalized; 

        //     player.Move(moveDirection * moveSpeed * Time.deltaTime);
        // }

        private Vector3 playerInput;
        private Vector3 movePlayer;
        private Vector3 camRight;
        private Vector3 camForward;
        private float gravity = 9.8f;
        private float fallSpeed;
        public bool moveRelativeToInput = true;
        public void Move()
        {
            playerInput = new Vector3(_playerInput.MoveInput.x, 0, _playerInput.MoveInput.y);
            
            playerInput = playerInput.normalized;
            CamDirection();

            movePlayer = playerInput.x * camRight + playerInput.z * camForward;
            movePlayer = movePlayer * MoveSpeed();

            if(moveRelativeToInput)
            {
                // Forward is relative to input
                player.transform.LookAt(player.transform.position + movePlayer);
            }
            else
            {
                // Forward is relative to camera
                if(_playerInput.HasMoveInput)
                {
                    var desiredRotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
                    transform.rotation =  Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * dampingRotation);
                }
            }

            SetGravity();
            player.Move(movePlayer * Time.deltaTime);

        }

        void CamDirection()
        {
            camForward = Camera.main.transform.forward;
            camRight = Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;
            
            camForward = camForward.normalized;
            camRight = camRight.normalized;                       
        }

        void SetGravity()
        {
            if(player.isGrounded)
            {
                fallSpeed = -gravity * Time.deltaTime;
            }
            else
            {
                fallSpeed -= gravity * Time.deltaTime;
            }
            movePlayer.y = fallSpeed;
        }

        private float MoveSpeed()
        {
            // Implementar WalkBackwards
            // ---------------------------------------
            float deadZone = 0.1f;
            moveSpeed = 0;
            // print("Input in move(): " + input);
            float horizontal = _playerInput.MoveInput.x;
            float vertical = _playerInput.MoveInput.y;
            // print("Player horizontal: " + horizontal);
            // print("Player vertical: " + vertical);
            // print("WalkInput: " + _playerInput.WalkInput);

            if(vertical > 0.80f)
                canSprint = true;
            else
                canSprint = false;

            // if(horizontal < Mathf.Abs(1f))
            //     canSprint = false;

            // print("Move: " + _playerInput.MoveInput.x);

            // Walk Backwards
          
            if(_playerInput.WalkInput && vertical < deadZone)
            {
                moveSpeed = walkBackwardSpeed;
                currentState = State.Walking;
            }
            // Walk
            else if(_playerInput.WalkInput)
            {
                moveSpeed = walkSpeed;
                currentState = State.Walking;
            }
            // Sprint
            else if(_playerInput.SprintInput && canSprint)
            {
                // print("Sprinting");
                moveSpeed = sprintSpeed;
                currentState = State.Sprinting;
            }
            // Run Backwards
            else if(vertical < -deadZone)
            {
                // print("Running Back (Vertical: " + vertical + ")");
                moveSpeed = runBackwardSpeed;
                currentState = State.Running;
            }
            // Run
            else if(Mathf.Abs(horizontal) > deadZone || vertical > deadZone)
            {
                // print("Running");
                moveSpeed = runSpeed;
                currentState = State.Running;
            }
            else 
            {
                // print("No movement");
                currentState = State.Iddle;
                moveSpeed = 0;
            }
            
            
            if(_playerInput.MoveInput.y == 0 && _playerInput.MoveInput.x == 0)
            {
                // print("No movement - Iddle");
                currentState = State.Iddle;
                moveSpeed = 0;
            }
            return moveSpeed;
        }


        void DebugData()
        {
            var text = "Horizontal Input: " + _playerInput.MoveInput.x.ToString() + "\n" + 
                        "Vertical Input: " + _playerInput.MoveInput.y.ToString() + "\n" + 
                        "Speed: " + moveSpeed + "\n" + 
                        "Real Speed: " + player.velocity.magnitude.ToString() + "\n" + 
                        "Current State: " + currentState.ToString();
            debugData.text = text; 
        }

    }

}