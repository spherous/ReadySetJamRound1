using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BankedScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public int score {get; private set;} = 0;
    public void Inc(int amount)
    {
        score += amount;
        scoreText.text = $"{score}";
    }
}
