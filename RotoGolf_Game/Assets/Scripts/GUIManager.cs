using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public Text HitText;
    public Text ScoreText;

    public int HitCounter { get; private set; }
    public int MaxHitNeeded;

    private void Start()
    {
        HitText.text = "0 / " + MaxHitNeeded;
    }

    public void AddBallHit()
    {
        HitCounter++;
        HitText.text = HitCounter + " / " + MaxHitNeeded;
    }

    public void DisplayScore()
    {
        int score = HitCounter - MaxHitNeeded;

        switch(score)
        {
            case -6: ScoreText.text = "Phoenix!"; break;
            case -5: ScoreText.text = "Ostrich!"; break;
            case -4: ScoreText.text = "Condor!"; break;
            case -3: ScoreText.text = "Albatros!"; break;
            case -2: ScoreText.text = "Eagle"; break;
            case -1: ScoreText.text = "Birdie"; break;
            case 0: ScoreText.text = "Par"; break;
            case 1: ScoreText.text = "Bogey"; break;
            case 2: ScoreText.text = "Double Bogey"; break;
            case 3: ScoreText.text = "Triple Bogey";  break;

            default: 
                if (score > 3)
                    ScoreText.text = "+" + score;
                else if (score < -6)
                    ScoreText.text = "Better than Phoenix, never seen that!";
                break;
        }

        ScoreText.enabled = true;
    }
}
