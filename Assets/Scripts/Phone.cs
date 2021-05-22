using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Phone : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private Sprite phoneSprite2;
    [SerializeField] private Button sendButton;
    [SerializeField] private TextMeshProUGUI respondText;
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private GameManager gameManager;
    public Vector3 downPos;
    public float waitDuration;
    private float? waitUntilTime = null;
    public float slideDownDuration;
    private float slideEllapsedDuration = 0;
    bool slidingDown = false;

    bool sent = false;

    private void Awake() {
        sendButton.onClick.AddListener(() => {
            image.sprite = phoneSprite2;
            respondText.gameObject.SetActive(false);
            responseText.gameObject.SetActive(true);
            sendButton.gameObject.SetActive(false);
            waitUntilTime = Time.timeSinceLevelLoad + waitDuration;
            sent = true;
        });
    }

    private void Update() {
        if(slidingDown)
        {
            slideEllapsedDuration += Time.unscaledDeltaTime;
            rectTransform.localPosition = Vector3.Lerp(Vector3.zero, downPos, Mathf.Clamp01(slideEllapsedDuration/slideDownDuration));
            if(rectTransform.localPosition == downPos)
            {
                slidingDown = false;
                gameManager.StartGame();
            }
        }
        else if(waitUntilTime.HasValue && Time.timeSinceLevelLoad >= waitUntilTime.Value)
        {
            waitUntilTime = null;
            slidingDown = true;
        }
    }

    public void Enter(CallbackContext context)
    {
        if(!context.performed || sent)
            return;
        
        sendButton.onClick.Invoke();
    }
}