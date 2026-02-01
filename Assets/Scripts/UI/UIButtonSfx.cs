using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSfx : MonoBehaviour, IPointerEnterHandler
{
    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(() => UIAudioPlayer.Instance?.PlayClick());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIAudioPlayer.Instance?.PlayHover();
    }
}
