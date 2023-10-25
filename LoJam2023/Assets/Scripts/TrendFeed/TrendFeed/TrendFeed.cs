using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Singleton Instance to manage the list of active TrendCards influencing Player Collectable score.
/// </summary>
public class TrendFeed : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ScriptableObject containing the full list of collectable objects.")]
    CollectableObjectList masterObjectListSO;

    [SerializeField]
    GameObject shortPanelPrefab = null;

    [SerializeField]
    GameObject longPanelPrefab = null;

    [SerializeField]
    [Tooltip("The maximum number of trends that can be on-screen.")]
    int maxActiveTrends = 5;

    [SerializeField]
    [Min(5f)]
    int minShortTimerSeconds = 15;

    [SerializeField]
    int maxShortTimerSeconds = 30;

    [SerializeField]
    int minLongTimerSeconds = 45;

    [SerializeField]
    int maxLongTimerSeconds = 90;

    [SerializeField]
    [Min(0f)]
    int minTrendCooldownLength = 3;

    [SerializeField]
    int maxTrendCooldownLength = 10;

    //Object pools for the TrendCards
    Queue<TrendCard> shortCardPool = new();
    Queue<TrendCard> longCardPool = new();

    //Temporary queue of CollectableObjects to be added back as options for selection
    Queue<string> pendingReturns = new();

    //The list of CollectableObjects that can be chosen as trends
    List<string> trendOptions = new();

    //The full list of CollectableObjects pulled from the master list
    readonly Dictionary<string, CollectableObjectInfo> fullObjectDict = new();

    //The set of TrendCards that are currently active
    readonly Dictionary<string, TrendCard> activeCards = new();

    RectTransform panelRect;
    HorizontalLayoutGroup horizontalLayoutGroup;

    bool isActive = true;
    bool stopRequested = false;

    //Controls the cooldown for placing new TrendCards.
    float trendCooldownTimer = 0;

    //The amount of space left to place TrendCards.
    float openFeedSpace = 0;

    //The width of a "long" TrendCard.
    int longCardWidth;

    //The width of a "short" TrendCard
    int shortCardWidth;

    [Header("Subscriber Interface for Trends")]
    [SerializeField]
    UnityEvent<string> OnAddTrendEvent;

    [SerializeField]
    UnityEvent<string> OnRemoveTrendEvent;

    private void Awake()
    {
        panelRect = GetComponent<RectTransform>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        
        maxTrendCooldownLength = Mathf.Max(minTrendCooldownLength + 1, maxTrendCooldownLength);

        longCardWidth = Mathf.FloorToInt(panelRect.rect.width * 0.3f);
        shortCardWidth = Mathf.FloorToInt(panelRect.rect.width * 0.2f);
        openFeedSpace = Mathf.FloorToInt(panelRect.rect.width);

        if (shortPanelPrefab == null)
            Debug.LogError("Missing ShortPanel Prefab object.");

        if (longPanelPrefab == null)
            Debug.LogError("Missing LongPanel Prefab object.");

        foreach (CollectableObjectInfo obj in masterObjectListSO.CollectableObjects)
        {
            fullObjectDict.Add(obj.name, obj);
            trendOptions.Add(obj.name);
        }
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (trendCooldownTimer <= 0)
        {
            if (activeCards.Count < maxActiveTrends)
            {
                if (openFeedSpace > shortCardWidth)
                {
                    AddTrendItem();

                    //Return any pending returns back into the set for selection
                    while (pendingReturns.TryDequeue(out string trendOption))
                    {
                        trendOptions.Add(trendOption);
                    }
                }

                else
                {
                   // Debug.LogFormat("No open space {0}.", openFeedSpace);
                }
            }

            else
            {
               // Debug.LogFormat("Active cardsd full {0}.", activeCards.Count);
            }
        }

        if (stopRequested)
        {
            ClearAllTrendItems();
            isActive = false;
            stopRequested = false;
        }

        if (trendCooldownTimer > 0)
            trendCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Add a random CollectableItem to the TrendFeed with a random score and lifetime.
    /// </summary>
    void AddTrendItem()
    {
        CollectableObjectInfo newTrendObject;
        TrendCard newTrendCard;
        
        int randObjIndex = Random.Range(0, trendOptions.Count);
        int objectLifetime = 0;

        if (Random.value > 0.5f)
            objectLifetime = Random.Range(minShortTimerSeconds, maxShortTimerSeconds);

        else
            objectLifetime = Random.Range(minLongTimerSeconds, maxLongTimerSeconds);

        //There are no options to choose from.
        if (trendOptions.Count == 0)
        {
            return;
        }

        newTrendObject = fullObjectDict[trendOptions[randObjIndex]];
        int objectScore = Random.Range(newTrendObject.MinPositiveScore, newTrendObject.MaxPositiveScore+1);
        
        trendOptions.RemoveAt(randObjIndex);

        //Fixed Creation Step for different card sizes.
        if (objectLifetime <= maxShortTimerSeconds)
        {
            if (!shortCardPool.TryDequeue(out newTrendCard))
            {
                newTrendCard = CreateTrendCard(shortPanelPrefab);
                newTrendCard.SetCardWidth(shortCardWidth);
                openFeedSpace -= shortCardWidth;
            }
        }

        else
        {
            if (!longCardPool.TryDequeue(out newTrendCard))
            {
                newTrendCard = CreateTrendCard(longPanelPrefab);
                newTrendCard.SetCardWidth(longCardWidth);
                openFeedSpace -= longCardWidth;
            }
        }

        //Ensure the object appears at the back of the HorizontalLayoutGroup
        newTrendCard.gameObject.transform.SetAsLastSibling();
        newTrendCard.gameObject.SetActive(true);
        newTrendCard.Activate(newTrendObject.ObjectName, objectScore, objectLifetime, RemoveTrendItem);
        
        activeCards.Add(newTrendObject.ObjectName, newTrendCard);
        openFeedSpace -= horizontalLayoutGroup.spacing;

        trendCooldownTimer = Random.Range(minTrendCooldownLength, maxTrendCooldownLength);

        OnAddTrendEvent.Invoke(newTrendObject.ObjectName);
    }

    /// <summary>
    /// Clear the current list of trending items. To be called when completing a level.
    /// </summary>
    void ClearAllTrendItems()
    {
        foreach (KeyValuePair<string, TrendCard> trendPair in activeCards)
        {
            RemoveTrendItem(trendPair.Value);
        }

        activeCards.Clear();
        pendingReturns.Clear();
    }

    /// <summary>
    /// Create a new TrendCard for the TrendFeed.
    /// </summary>
    /// <param name="cardPrefab"></param>
    /// <returns></returns>
    TrendCard CreateTrendCard(GameObject cardPrefab)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        TrendCard newTrendCard = cardObject.GetComponent<TrendCard>();

        cardObject.transform.SetParent(gameObject.transform);

        return newTrendCard;
    }

    /// <summary>
    /// Remove an Active TrendCard from the TrendFeed, hiding it and enabling it to be selected again.
    /// </summary>
    /// <param name="removeCard"></param>
    /// <param name="length"></param>
    void RemoveTrendItem(TrendCard removeCard)
    {
        activeCards.Remove(removeCard.ObjectName);
        removeCard.Deactivate();

        if (removeCard.CompareTag("ShortCard"))
        {
            shortCardPool.Enqueue(removeCard);
            openFeedSpace += shortCardWidth;
        }

        else
        {
            longCardPool.Enqueue(removeCard);
            openFeedSpace += longCardWidth;
        }

        openFeedSpace += horizontalLayoutGroup.spacing;
        pendingReturns.Enqueue(removeCard.ObjectName);
        OnRemoveTrendEvent.Invoke(removeCard.ObjectName);
    }

    /// <summary>
    /// Get the list of object names that are currently trending.
    /// </summary>
    /// <returns></returns>
    public List<string> GetActiveTrendObjectNames()
    {
        List<string> trendObjects = new();

        foreach (KeyValuePair<string, TrendCard> trendPair in activeCards)
        {
            trendObjects.Add(trendPair.Key);
        }

        return trendObjects;
    }

    /// <summary>
    /// Get the Score associated with an Object that is Trending.
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public int GetTrendScoreForObject(string objectName)
    {
        return activeCards.ContainsKey(objectName) ? activeCards[objectName].ObjectScore : 0;
    }

    /// <summary>
    /// Return true if the object specified is active in the TrendFeed.
    /// </summary>
    /// <param name="objectName">The name of the object to check for.</param>
    /// <returns></returns>
    public bool IsObjectTrending(string objectName)
    {
        return activeCards.ContainsKey(objectName);
    }

    /// <summary>
    /// Start the TrendFeed.
    /// </summary>
    public void StartTrendFeed()
    {
        isActive = true;
    }

    /// <summary>
    /// Completely halts the TrendFeed and clears the active trends.
    /// </summary>
    public void StopTrendFeed()
    {
        stopRequested = true;
    }
}
