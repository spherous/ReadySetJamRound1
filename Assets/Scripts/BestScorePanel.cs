using TMPro;
using UnityEngine;

public class BestScorePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestText;

    private void Awake() => bestText.text = $"{PlayerPrefs.GetInt("Highscore", 0)}";
}