using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Singleton to manage the active LiveChat feed.
/// </summary>
public class LiveChatFeed : MonoBehaviour
{
    [SerializeField]
    GameObject chatMessagePrefab;

    [SerializeField]
    LiveChatMessageList chatMessageTemplates;

    [SerializeField]
    [Range(0, 1)]
    float objectMessageChance = 0.2f;

    //A quick reference to the current trending objects on-screen.
    List<string> activeTrendingObjects = new();

    //Holds the full set of ChatMembers that can appear
    List<string> chatMembers = new();

    //Holds the full set of created ChatMessage objects
    Queue<GameObject> chatMessageObjectQueue = new();

    //Holds the full set of ChatMessage objects on display
    Queue<GameObject> activeChatMessages = new();

    //Holds the set of ChatMembers on hold from chatting
    Queue<string> pendingChatMemberReturns = new();

    //Represents how well chat enjoys the Player's stream
    //Held between [-1, 1] and influences the nature of the messages / emotes
    //float chatPlayerAffinity = 0;

    //The maximum number of messages that can be displayed at once
    int maxDisplayedMessages = 15;

    //The minimum text size of ChatMessages for readability
    readonly int minMessageTextSize = 20;

    //The current interval between chat messages spawning
    [SerializeField]
    [Range(0.25f, 2)]
    float currentMessageCooldown = 0.5f;

    float messageCooldownTimer = 0;

    //The fixed height to apply to all chat messages
    float messageRectHeight;
    
    //Is the ChatFeed active?
    bool isActive = true;

    //Has the ChatFeed been stopped?
    bool stopRequested = false;

    float chatAffinityScore = 0;

    void Awake()
    {
        RectTransform panelRect = GetComponent<RectTransform>();
        VerticalLayoutGroup panelVLG = GetComponent<VerticalLayoutGroup>();

        float panelHeight = panelRect.rect.height;
        float minMessageHeight = minMessageTextSize + chatMessagePrefab.GetComponent<TextMeshProUGUI>().margin.y * 2;

        if (chatMessageTemplates == null)
        {
            Debug.LogError("LiveChatFeed is missing 'ChatMessageList' reference.");
        }

        if (chatMessagePrefab == null)
        {
            Debug.LogError("LiveChatFeed is missing 'ChatMessage' prefab reference.");
        }

        //Determine the maximum number of messages to display
        maxDisplayedMessages = 
            Mathf.Min(maxDisplayedMessages, Mathf.FloorToInt(panelHeight / minMessageHeight));

        //Determine the size in pixels of each chat message
        messageRectHeight = Mathf.FloorToInt(panelHeight / maxDisplayedMessages);

        panelVLG.padding.top = Mathf.FloorToInt(panelHeight % maxDisplayedMessages);

        Debug.LogFormat("{0} - Max: {1} Height: {2} Padding: {3}", panelHeight, maxDisplayedMessages, messageRectHeight, panelVLG.padding.top);
        GenerateChatMembers();
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (messageCooldownTimer <= 0)
        {
            GenerateChatMessage();
            messageCooldownTimer = Random.Range(currentMessageCooldown - 0.2f, currentMessageCooldown + 0.2f);

            while (pendingChatMemberReturns.TryDequeue(out var member))
            {
                chatMembers.Add(member);
            }
        }

        if (stopRequested)
        {
            ClearChatMessages();
            isActive = false;
            stopRequested = false;
        }

        if (messageCooldownTimer > 0)
            messageCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Clears all active chat messages.
    /// </summary>
    void ClearChatMessages()
    {
        while (activeChatMessages.Count > 0)
        {
            HideChatMessageObject(activeChatMessages.Dequeue());
        }

        messageCooldownTimer = 2f;
        pendingChatMemberReturns.Clear();
    }

    /// <summary>
    /// Create a new ChatMessage GameObject and parent it to the MessagePanel.
    /// </summary>
    /// <returns></returns>
    GameObject CreateChatMessageObject()
    {
        GameObject newChatMessage = Instantiate(chatMessagePrefab);
        RectTransform chatMessageRect = newChatMessage.GetComponent<RectTransform>();
        newChatMessage.transform.SetParent(gameObject.transform);
        chatMessageRect.localScale = Vector3.one;
        chatMessageRect.transform.localPosition = Vector3.zero;
        chatMessageRect.sizeDelta = new Vector2(chatMessageRect.sizeDelta.x, messageRectHeight);

        return newChatMessage;
    }

    /// <summary>
    /// Generate a series of ChatMembers to consistently appear in the LiveChat feed.
    /// </summary>
    void GenerateChatMembers()
    {
        chatMembers = new() {
            "Ajnin",
            "DTheCJ0hns0n",
            "SoftieFan4Ever",
            "LumbagoTTV",
            "CyberChroma",
            "Hman5",
            "AnlbrewMat3",
            "Meowkaplier",
            "NotAnAlien_",
            "GnortsMrAlien",
            "helordada",
            "_wsm_",
            "gAlaxyK1ng",
            "prism0",
            "bob"
        };
    }

    /// <summary>
    /// Generate a new Chat Message and add it to the LiveChat Feed.
    /// </summary>
    void GenerateChatMessage()
    {
        //Generate the random ChatMember to deliver the message
        int chatMemberIdx = Random.Range(0, chatMembers.Count);
        string chatMemberName = chatMembers[chatMemberIdx];

        int messageSentiment;

        if (chatAffinityScore > 0)
            messageSentiment = Random.value > Mathf.Min(chatAffinityScore, 0.5f) ? 0 : 1;

        else
            messageSentiment = -Random.value < Mathf.Max(chatAffinityScore, -0.5f) ? 0 : -1;

        string chatMessageText = Random.value < objectMessageChance && activeTrendingObjects.Count > 0 ?
            chatMessageTemplates.GetRandomObjectChatMessage(
                activeTrendingObjects[Random.Range(0, activeTrendingObjects.Count)]) :
            chatMessageTemplates.GetRandomChatMessage(messageSentiment);

        //Get the ChatMessage GameObject to display the message
        if (!chatMessageObjectQueue.TryDequeue(out GameObject newMessageObject))
        {
            newMessageObject = CreateChatMessageObject();
        }

        //Hide the oldest chat message
        if (activeChatMessages.Count >= maxDisplayedMessages)
            HideChatMessageObject(activeChatMessages.Dequeue());

        //Set the ChatMessage GameObject information
        newMessageObject.transform.SetAsLastSibling();
        newMessageObject.GetComponent<TextMeshProUGUI>().text = 
            string.Format("{0}: {1}", chatMemberName, chatMessageText);

        newMessageObject.GetComponent<TextMeshProUGUI>().enabled = true;
        newMessageObject.GetComponent<TextMeshProUGUI>().color = Random.ColorHSV(70f/255f, 120f/255f, 0, 1, 0, 1);
        newMessageObject.SetActive(true);

        activeChatMessages.Enqueue(newMessageObject);
    }

    /// <summary>
    /// Remove a ChatMessage from the LiveChatFeed, hiding it altogether.
    /// </summary>
    /// <param name="chatMessageObject"></param>
    void HideChatMessageObject(GameObject chatMessageObject)
    {
        chatMessageObject.SetActive(false);
        chatMessageObjectQueue.Enqueue(chatMessageObject);
    }

    /// <summary>
    /// Adds an Object to the list of Trending Objects.
    /// </summary>
    /// <param name="objectName"></param>
    public void OnObjectStartTrending(string objectName)
    {
        activeTrendingObjects.Add(objectName);
    }

    /// <summary>
    /// Removes an Object from the list of Trending Objects.
    /// </summary>
    /// <param name="objectName"></param>
    public void OnObjectStopTrending(string objectName)
    {
        activeTrendingObjects.Remove(objectName);
    }

    /// <summary>
    /// Update the Chat Affinity Score according to score performance.
    /// </summary>
    /// <param name="scoreAdded"></param>
    public void OnUpdatePlayerScore(int scoreAdded)
    {
        if (scoreAdded > 0)
            chatAffinityScore = Mathf.Min(chatAffinityScore + 0.1f, 1);
        else
            chatAffinityScore = Mathf.Max(chatAffinityScore - 0.1f, -1);
    }

    /// <summary>
    /// Slows down the LiveChat feed.
    /// </summary>
    public void SlowDownFeed()
    {
        currentMessageCooldown = Mathf.Min(currentMessageCooldown + 0.25f, 2f);
    }

    /// <summary>
    /// Speeds up the LiveChat feed.
    /// </summary>
    public void SpeedUpFeed()
    {
        currentMessageCooldown = Mathf.Max(currentMessageCooldown - 0.25f, 0.25f);
    }

    /// <summary>
    /// Starts the LiveChat feed.
    /// </summary>
    public void StartChat()
    {
        isActive = true;
    }

    /// <summary>
    /// Completely halts the LiveChat feed and clears the ChatMessages.
    /// </summary>
    public void StopChat()
    {
        //Reset chatPlayerAffinity
        stopRequested = true;
    }
}
