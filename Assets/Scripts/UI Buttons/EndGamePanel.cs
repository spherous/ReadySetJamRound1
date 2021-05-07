using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private void Awake() {
        BankedScore banked = GameObject.FindObjectOfType<BankedScore>();
        scoreText.text = $"{banked.score} Carts Returned";
    }
}