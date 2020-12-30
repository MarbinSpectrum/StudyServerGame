using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    private void Awake() => instance = this;

    public InputField sendInput;
    public RectTransform chatContent;
    public Text chatText;
    private RectTransform chatTextRect;
    public ScrollRect chatScrollRect;

    public void ShowMessage(string data)
    {
        if (!chatText.text.Equals(string.Empty))
            chatText.text += "\n";
        chatText.text += data;

        if (chatTextRect == null)
            chatTextRect = chatText.GetComponent<RectTransform>();
        Fit(chatTextRect);
        Fit(chatContent);
        Invoke("ScrollDelay", 0.03f);

    }

    void Fit(RectTransform rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

    void ScrollDelay() => chatScrollRect.verticalScrollbar.value = 0;


}
