using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public GameObject Ball;
    private BallBehavior BallBehavior;

    public float CameraRotationSpeed;

    public float GravityFactor;

    private bool bFirePressed;
    private bool bRotateCameraLeftPressed;
    private bool bRotateCameraRightPressed;

    private void Start()
    {
        BallBehavior = Ball.GetComponent<BallBehavior>();

        bFirePressed = false;
        bRotateCameraLeftPressed = false;
        bRotateCameraRightPressed = false;
    }

    public void OnLook(InputValue value)
    {
        //TODO handle aim with gamepad
    }

    public void OnFire()
    {
        if (bFirePressed)
        {
            // Set world gravity only on shot since we don't want te ball to move before
            Vector3 newGavrity = Camera.main.transform.up * -GravityFactor;
            Physics2D.gravity = newGavrity;

            BallBehavior.Shoot();
        }

        bFirePressed = !bFirePressed;
    }

    public void OnRotateLeft()
    {
        bRotateCameraLeftPressed = !bRotateCameraLeftPressed;
    }

    public void OnRotateRight(InputValue value)
    {
        bRotateCameraRightPressed = !bRotateCameraRightPressed;
    }

    private void Update()
    {
        // Handle camera rotation
        if (BallBehavior.bCanBeShot
            && (bRotateCameraLeftPressed || bRotateCameraRightPressed))
        {
            float fRotationFactor = bRotateCameraLeftPressed ? -1.0f : 1.0f;
            Camera.main.transform.Rotate(new Vector3(0.0f, 0.0f, CameraRotationSpeed * fRotationFactor));
        }

        // Compute shot arrow length
        if (bFirePressed)
        {
            BallBehavior.IncrementShotSpeed();
        }

        // Compute arrows rotation
        Vector2 cursor = Mouse.current.position.ReadValue();
        Vector2 ball = Camera.main.WorldToScreenPoint(Ball.transform.position);
        ball -= cursor;

        BallBehavior.SetArrowRotation(ball);
    }
}
