using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public GameObject Arrow;
    public GameObject ShotArrow;

    private Rigidbody2D BallBody;
    private Vector2 vArrowDirection;

    public float MaxSpeed;

    private float fScaleSpeedFactor;

    private float fShotPower;
    private bool bTriggerShoot;
    public bool bCanBeShot { get; private set; }

    private float fSpeedIncrementFactor;

    private float fStopCounter;

    private void Start()
    {
        BallBody = GetComponent<Rigidbody2D>();

        fShotPower = 0.0f;
        bTriggerShoot = false;
        bCanBeShot = false;
        fStopCounter = 0.0f;

        fSpeedIncrementFactor = 1.0f;

        // Store factor between max scale and max speed to set shot arrow scale accordingly
        float fMaxArrowScale = Arrow.transform.localScale.y;
        fScaleSpeedFactor = MaxSpeed / fMaxArrowScale;
    }

    public void IncrementShotSpeed()
    {
        fShotPower += 0.005f * fSpeedIncrementFactor;

        if (fShotPower > MaxSpeed || fShotPower <= 0.0f)
        {
            fSpeedIncrementFactor *= -1.0f;
        }

        Vector3 vScale = ShotArrow.transform.localScale;
        vScale.y = fShotPower / fScaleSpeedFactor;
        ShotArrow.transform.localScale = vScale;
    }

    public void Shoot()
    {
        if (!bCanBeShot)
            return;

        bTriggerShoot = true;
        fStopCounter = 0.0f;
    }

    public void SetArrowRotation(Vector2 vDirection)
    {
        // From normalize dir vector (ball - cursor) then get angle in degree to set object rotation
        // Substract 90 to have the right angle because..?

        vArrowDirection = vDirection.normalized;

        float fAngle = Mathf.Atan2(vDirection.y, vDirection.x) * Mathf.Rad2Deg;

        Vector3 rotation = Arrow.transform.eulerAngles;
        rotation.z = fAngle - 90.0f;

        Arrow.transform.eulerAngles = rotation;
        ShotArrow.transform.eulerAngles = rotation;
    }

    private void FixedUpdate()
    {
        if (!bCanBeShot)
        {
            float vSpeed = BallBody.velocity.magnitude;
            if (vSpeed < 0.2f)
            {
                fStopCounter += Time.fixedDeltaTime;

                // Check if ball has a slow speed for enough time, stop it then and allow a new shot
                if (fStopCounter >= 0.5f)
                {
                    BallBody.velocity = Vector2.zero;
                    BallBody.angularVelocity = 0.0f;
                    BallBody.gravityScale = 0.0f;

                    OnStop();
                }
            }
            else
            {
                fStopCounter = 0.0f;
            }
        }
        // Shot triggered by inputs, see Shoot() called by GameManager
        else if (bTriggerShoot)
        {
            BallBody.gravityScale = 1.0f;
            BallBody.AddForce(vArrowDirection * fShotPower);

            OnShoot();
        }
    }

    private void OnShoot()
    {
        Arrow.SetActive(false);
        ShotArrow.SetActive(false);
        bCanBeShot = false;

        bTriggerShoot = false;

        // Reset arrow shot scale for the next shot
        Vector3 vScale = ShotArrow.transform.localScale;
        vScale.y = 0.0f;
        ShotArrow.transform.localScale = vScale;

        fSpeedIncrementFactor = 1.0f;
        fShotPower = 0.0f;
    }

    private void OnStop()
    {
        bCanBeShot = true;
        Arrow.SetActive(true);
        ShotArrow.SetActive(true);
    }
}
