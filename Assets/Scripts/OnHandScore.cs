using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnHandScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;
    public void Inc(int amount)
    {
        score += amount;
        scoreText.text = $"{score}";
    }

    public void Reset()
    {
        score = 0;
        scoreText.text = $"{score}";
    }
}
