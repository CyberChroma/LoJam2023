using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This ScriptableObject contains the full list of possible chat messages for the LiveChat Feed.
/// </summary>
[CreateAssetMenu]
public class LiveChatMessageList : ScriptableObject
{

    [SerializeField]
    List<string> positiveMessages;

    [SerializeField]
    List<string> negativeMessages;

    [SerializeField]
    List<string> neutralMessages;

    [SerializeField]
    List<string> objectMessages;

    private void OnEnable()
    {
        if (positiveMessages == null)
            positiveMessages = new();

        if (negativeMessages == null)
            negativeMessages = new();

        if (neutralMessages == null)
            neutralMessages = new();

        if (objectMessages == null)
            objectMessages = new();
    }

    /// <summary>
    /// Get and return a random ChatMessage according to the desired sentiment.
    /// </summary>
    /// <param name="sentiment"></param>
    /// <returns></returns>
    public string GetRandomChatMessage(int sentiment = 0)
    {
        if (sentiment == 1)
            return positiveMessages[Random.Range(0, positiveMessages.Count)];

        else if (sentiment == -1)
            return negativeMessages[Random.Range(0, negativeMessages.Count)];

        else
            return neutralMessages[Random.Range(0, neutralMessages.Count)];
    }

    /// <summary>
    /// Format and return a random object chat message using the object name passed.
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public string GetRandomObjectChatMessage(string objectName)
    {
        int messageIdx = Random.Range(0, objectMessages.Count);

        return string.Format(objectMessages[messageIdx], objectName);
    }
}
