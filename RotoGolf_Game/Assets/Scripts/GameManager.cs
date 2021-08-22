using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string NextLevelToLoad;

    public GameObject Ball;
    private BallBehavior BallBehavior;

    public GUIManager GUIManager;

    public float CameraRotationSpeed;

    public float GravityFactor;

    private AudioSource Audio;

    public AudioClip Applause;

    private bool bFirePressed;
    private bool bRotateCameraLeftPressed;
    private bool bRotateCameraRightPressed;

    public bool bGameIsOver { get; private set; }
    private bool bCanSkipToNextLevel;

    private Vector2 vGamepadStickRotation;

    private void Start()
    {
        BallBehavior = Ball.GetComponent<BallBehavior>();
        BallBehavior.GameManager = this;

        Audio = GetComponent<AudioSource>();

        bFirePressed = false;
        bRotateCameraLeftPressed = false;
        bRotateCameraRightPressed = false;

        bGameIsOver = false;
        bCanSkipToNextLevel = false;
    }

    public void PlayAudioClip(AudioClip clip)
    {
        Audio.PlayOneShot(clip);
    }

    public void OnBallHitHole()
    {
        Audio.PlayOneShot(Applause);

        GUIManager.DisplayScore();

        bGameIsOver = true;

        StartCoroutine("WaitEndAnimation");
    }

    IEnumerator WaitEndAnimation()
    {
        yield return new WaitForSeconds(3.0f);

        bCanSkipToNextLevel = true;
    }

    public void OnLook(InputValue value)
    {
        vGamepadStickRotation = value.Get<Vector2>();
    }

    public void OnReset()
    {
        LoadNewLevel(SceneManager.GetActiveScene().name);
    }

    private void LoadNewLevel(string LevelName)
    {
        Physics2D.gravity = new Vector2(0.0f, -GravityFactor);

        SceneManager.LoadScene(LevelName, LoadSceneMode.Single);
    }

    public void OnFire()
    {
        if (bGameIsOver)
        {
            if (bCanSkipToNextLevel)
            {
                LoadNewLevel(NextLevelToLoad);
            }

            return;
        }

        if (BallBehavior.CanBeShot && bFirePressed)
        {
            // Set world gravity only on shot since we don't want te ball to move before
            Vector3 newGavrity = Camera.main.transform.up * -GravityFactor;
            Physics2D.gravity = newGavrity;

            BallBehavior.Shoot();

            GUIManager.AddBallHit();
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
        if (bGameIsOver)
            return;

        // Handle camera rotation
        if (BallBehavior.CanBeShot
            && (bRotateCameraLeftPressed || bRotateCameraRightPressed))
        {
            float fRotationFactor = bRotateCameraLeftPressed ? -1.0f : 1.0f;
            Camera.main.transform.Rotate(new Vector3(0.0f, 0.0f, CameraRotationSpeed * fRotationFactor * Time.deltaTime));
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

        if (vGamepadStickRotation.magnitude >= 0.1f)
        {
            ball = -vGamepadStickRotation;
        }

        BallBehavior.SetArrowRotation(ball);
    }
}
