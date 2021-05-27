using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public Toggle toggle;
    [SerializeField] private AudioSource audioSource;
    public AudioClip UI1;
    public AudioClip UI2;
    private void Awake() => toggle.onValueChanged.AddListener(val => audioSource.PlayOneShot(UI2));
    public void OnPointerEnter(PointerEventData eventData) => audioSource.PlayOneShot(UI1);
}