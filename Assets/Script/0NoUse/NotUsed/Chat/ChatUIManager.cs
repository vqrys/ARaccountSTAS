using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public Transform content;
    public GameObject chatItemPrefab;
    public Sprite flatBubbleSprite;

    ChatItemUI lastChat;

    public ChatItemUI GetLastChat()
    {
        return lastChat;
    }

    public void AddChat(string message, System.Action onFinish = null)
    {
        // 🔹 Ratakan bubble chat sebelumnya
        if (lastChat != null)
        {
            lastChat.SetFlatBackground(flatBubbleSprite);
        }

        GameObject chat = Instantiate(chatItemPrefab, content);
        ChatItemUI chatUI = chat.GetComponent<ChatItemUI>();

        // 🔹 Simpan chat terbaru sebagai lastChat
        lastChat = chatUI;

        chatUI.PlayChat(message, () =>
        {
            OnChatFinished();
            onFinish?.Invoke();
        });
    }

    // 🔥 Dipakai ulang di mana pun
    void OnChatFinished()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
