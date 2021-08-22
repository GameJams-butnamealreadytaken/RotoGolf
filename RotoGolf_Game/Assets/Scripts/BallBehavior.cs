using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public GameManager GameManager;

    public GameObject Arrow;
    public GameObject ShotArrow;

    private Rigidbody2D BallBody;

    public float MaxSpeed;

    private float fScaleSpeedFactor;

    private float fShotPower;
    private bool bTriggerShoot;
    public bool CanBeShot { get; private set; }

    private float fSpeedIncrementFactor;

    private float fStopCounter;

    public AudioClip BallShoot;
    public AudioClip HoleHit;

    private void Start()
    {
        BallBody = GetComponent<Rigidbody2D>();
        BallBody.gravityScale = 0.0f;

        fShotPower = 0.0f;
        bTriggerShoot = false;
        CanBeShot = false;
        fStopCounter = 0.0f;

        fSpeedIncrementFactor = 1.0f;

        // Store factor between max scale and max speed to set shot arrow scale accordingly
        float fMaxArrowScale = Arrow.transform.localScale.y;
        fScaleSpeedFactor = MaxSpeed / fMaxArrowScale;
    }

    public void IncrementShotSpeed()
    {
        fShotPower += 3.0f * fSpeedIncrementFactor * Time.deltaTime;

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
        if (!CanBeShot)
            return;

        bTriggerShoot = true;
        fStopCounter = 0.0f;
    }

    public void SetArrowRotation(Vector2 vDirection)
    {
        // From normalize dir vector (ball - cursor) then get angle in degree to set object rotation
        // + take camera rotation into account

        float fAngle = Mathf.Atan2(vDirection.y, vDirection.x) * Mathf.Rad2Deg;

        Vector3 rotation = Arrow.transform.eulerAngles;
        rotation.z = fAngle;

        rotation.z += Camera.main.transform.rotation.eulerAngles.z - 90.0f;

        Arrow.transform.eulerAngles = rotation;
        ShotArrow.transform.eulerAngles = rotation;
    }

    private void FixedUpdate()
    {
        if (!CanBeShot)
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
            BallBody.AddForce(Arrow.transform.up * fShotPower);

            OnShoot();
        }
    }

    private void OnShoot()
    {
        GameManager.PlayAudioClip(BallShoot);

        Arrow.SetActive(false);
        ShotArrow.SetActive(false);
        CanBeShot = false;

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
        // Check if ball already hit the hole, avoid enablin arrows then
        if (GameManager.bGameIsOver)
            return;

        CanBeShot = true;
        Arrow.SetActive(true);
        ShotArrow.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            //TODO play ground hit sound
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HoleBallDetector"))
        {
            GameManager.PlayAudioClip(HoleHit);
            GameManager.OnBallHitHole();
        }
    }
}
