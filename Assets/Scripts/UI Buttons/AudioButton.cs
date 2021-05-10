using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public Button button;
    [SerializeField] private AudioSource audioSource;
    public AudioClip UI1;
    public AudioClip UI2;
    private void Awake() {
        button.onClick.AddListener(() => audioSource.PlayOneShot(UI2));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(UI1);
    }
}