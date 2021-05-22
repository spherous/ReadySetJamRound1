using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private Button selectedButton = null;
    private void Awake() {
        BankedScore banked = GameObject.FindObjectOfType<BankedScore>();
        scoreText.text = $"{banked.score} Carts Returned";
        retryButton.Select();
        selectedButton = retryButton;
    }

    public void MakeSelection(CallbackContext context)
    {
        if(!context.performed)
            return;
        
        if(selectedButton == retryButton)
        {
            selectedButton = quitButton;
            quitButton.Select();
        }
        else if(selectedButton == quitButton || selectedButton == null)
        {
            selectedButton = retryButton;
            retryButton.Select();
        }
    }

    public void Enter(CallbackContext context)
    {
        if(selectedButton != null)
            selectedButton.onClick.Invoke();
    }
}