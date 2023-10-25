using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MessageSentiment
{
    Positive = 0,
    Negative = 1,
    Neutral = 2,
}

/// <summary>
/// This ScriptableObject contains the full list of possible chat messages for the LiveChat Feed.
/// </summary>
[CreateAssetMenu]
public class LiveChatMessageList : ScriptableObject
{
    [System.Serializable]
    public class ChatMessageTemplate
    {
        public string messageId;
        public string messageBody;
        public bool useObject;
    }

    [SerializeField]
    [Tooltip("The list of possible chat messages that can appear in the live feed.")]
    List<ChatMessageTemplate> templateMessages;

    public List<ChatMessageTemplate> TemplateMessages { get { return templateMessages; } }

    private void OnValidate()
    {
        if (templateMessages == null)
            templateMessages = new();
    }
}
