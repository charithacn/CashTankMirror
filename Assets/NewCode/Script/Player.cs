using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BoomTanks.Multiplayer
{
    public class Player : NetworkBehaviour
    {

        private Rigidbody2D rigidbody;
        public float moveSpeed = 5f;
        public float groundCheckRadius = 1f;
        public LayerMask groundLayer;
        public bool isGrounded;
        public Transform targetObject;

        [SerializeField] private Shoot shoot;
        public Vector2 gravityDirection;

        private bool vehicleEnterToRealRotatioPosition;

        public TyreController frontTyreControl;
        public TyreController backTyreControl;

        private Transform currentPosition;

        bool frontTyreActive;
        bool backTyreActive;

        bool rotateFrontTyreActive;
        bool rotateBackTyreActive;

        public Transform startPos; // Starting position
        public Transform endPos; // Ending position
        private Vector2 targetPosition;

        private Vector2 previousPosition;
        public bool moveGroundStartRotation;

        private bool shouldReturnToPreviousPosition = false;

        private bool isShootToFlooer;

        //game over objects
        bool isLost;
        public LayerMask lostLayer;
        public float rotationspeed = 15f;
        private void Awake()
        {
            //transform.position = GameObject.Find("Position").transform.position;
        }
        public override void OnStartClient()
        {
            print("OnNetworkSpawn");
            transform.position = GameObject.Find("Position").transform.position;
            isShootToFlooer = false;
        }
        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            //.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = 0;
            vehicleEnterToRealRotatioPosition = false;
            previousPosition = rigidbody.position;

            moveGroundStartRotation = false;

            //GameObject.Find("StartNetwork").SendMessage("StartTheGame");


        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            frontTyreActive = frontTyreControl.GetComponent<TyreController>().tyre_ground;
            backTyreActive = backTyreControl.GetComponent<TyreController>().tyre_ground;

            rotateFrontTyreActive = frontTyreControl.GetComponent<TyreController>().rotate_tyre_ground;
            rotateBackTyreActive = backTyreControl.GetComponent<TyreController>().rotate_tyre_ground;


        }
        void Update()
        {
            isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
            isLost = Physics2D.OverlapCircle(transform.position, groundCheckRadius, lostLayer);
            if (targetObject != null & !isShootToFlooer)
            {
                rigidbody.gravityScale = 0f;
                // Calculate the direction vector from the player to the target object
                gravityDirection = (targetObject.position - transform.position).normalized;

                // Apply gravity force in the calculated direction
                rigidbody.AddForce((gravityDirection) * 40f);
            }
            else
            {
                //rigidbody.gravityScale = 1f;
            }


            if (isShootToFlooer && isGrounded)
            {
                print("Entered to the Grround");
                rigidbody.velocity = Vector2.zero;
                rigidbody.angularVelocity = 0f;

                //isShootToFlooer = false;
                //rigidbody.bodyType = RigidbodyType2D.Dynamic;

            }
            if (isGrounded)
            {
                rigidbody.gravityScale = 0f;
            }
            else
            {
                rigidbody.gravityScale = 1f;
            }
            if (!rotateBackTyreActive || !rotateFrontTyreActive)
            {
                if (isGrounded & moveGroundStartRotation)
                {
                    float rotationDirection = Vector2.SignedAngle(-gravityDirection, Vector2.up);

                    // Calculate the rotation angle based on rotation speed and time
                    float rotationAngle = rotationDirection * rotationspeed * Time.deltaTime;

                    // Rotate the car around the Z axis
                    transform.Rotate(0f, 0f, rotationAngle);
                    Debug.Log("Start the rotation");
                }

            }
            if (rotateBackTyreActive && rotateFrontTyreActive && !(frontTyreActive && backTyreActive))
            {
                gravityDirection = (targetObject.position - transform.position).normalized;

                // Apply gravity force in the calculated direction
                rigidbody.AddForce(gravityDirection * 30f);
                // rigidbody.velocity = gravityDirection * 30f;
                Debug.Log("Gravity added to rotate");

            }

        }

        private void OnDrawGizmosSelected()
        {
            // Draw ground check circle in editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
        }
        public void Move(Vector2 position)
        {

            if (!isShootToFlooer)
            {
                if (frontTyreActive && backTyreActive)
                {
                    // Move the Rigidbody2D


                    rigidbody.velocity = position.normalized * moveSpeed;
                    print("Rigidbody working " + position);

                    // Update previous position
                    previousPosition = rigidbody.position;

                    // Set shouldReturnToPreviousPosition to false
                    shouldReturnToPreviousPosition = false;
                }
                else
                {
                    // Stop the Rigidbody2D
                    rigidbody.velocity = Vector2.zero;

                    // Set shouldReturnToPreviousPosition to true
                    shouldReturnToPreviousPosition = true;

                    rigidbody.angularVelocity = 0f;
                }

                if (shouldReturnToPreviousPosition)
                {
                    // Move Rigidbody2D to previous position using interpolation
                    rigidbody.position = Vector2.Lerp(rigidbody.position, previousPosition, Time.deltaTime * moveSpeed);

                }
            }
            else
            {
                //rigidbody.gravityScale = 1f;
            }
            if (position != Vector2.zero && (frontTyreActive || backTyreActive))
            {

                print("Move ground start =true /////////////////////////");

                isShootToFlooer = false;
            }
            if (isLost)
            {
                GameManager.instance.SendMessage("Lost");
            }

        }

        public void ShootRotation(Vector2 value)
        {
            shoot.SendMessage("ShooterRotate", value);
        }
        public void AddForce(Vector2 pos)
        {
            isShootToFlooer = true;
            isGrounded = false;
            //targetObject = null;
            Debug.Log("Add force");
            rigidbody.AddForce(pos);
            moveGroundStartRotation = true;
            StartCoroutine(WaitForFloor());
        }

        IEnumerator WaitForFloor()
        {
            yield return new WaitForSeconds(0.5f);
            //isShootToFlooer = false;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            print("Detect the collision");
            if (collision.gameObject.tag.Equals("Dead"))
            {

            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            vehicleEnterToRealRotatioPosition = true;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            vehicleEnterToRealRotatioPosition = false;

        }
    }
}