using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class TrendEvent : UnityEvent<string>
{
}

/// <summary>
/// Singleton Instance to manage the list of active TrendCards influencing Player Collectable score.
/// </summary>
public class TrendFeed : MonoBehaviour
{
    [HideInInspector]
    [Tooltip("ScriptableObject containing the full list of collectable objects.")]
    public TrendObjectList masterObjectListSO;

    [SerializeField]
    [Tooltip("ScriptableObject containing the full list of collectable objects.")]
    TrendObjectList objectListSmall;

    [SerializeField]
    [Tooltip("ScriptableObject containing the full list of collectable objects.")]
    TrendObjectList objectListMedium;

    [SerializeField]
    [Tooltip("ScriptableObject containing the full list of collectable objects.")]
    TrendObjectList objectListLarge;

    [SerializeField]
    GameObject cardPrefab = null;

    [SerializeField]
    [Tooltip("The maximum number of trends that can be on-screen.")]
    int maxActiveTrends = 5;

    [SerializeField]
    [Min(5f)]
    int minShortTimerSeconds = 10;

    [SerializeField]
    int maxShortTimerSeconds = 15;

    [SerializeField]
    int minLongTimerSeconds = 20;

    [SerializeField]
    int maxLongTimerSeconds = 30;

    [SerializeField]
    [Min(0f)]
    int minTrendCooldownLength = 3;

    [SerializeField]
    int maxTrendCooldownLength = 10;

    [SerializeField]
    [Range(0f, 1f)]
    float negativeTrendChance = 0.15f;
    
    //Object pools for the TrendCards
    Queue<TrendCard> cardPool = new();

    //Temporary queue of CollectableObjects to be added back as options for selection
    Queue<string> pendingReturns = new();

    //The list of CollectableObjects that can be chosen as trends
    List<string> trendOptions = new();

    //The full list of CollectableObjects pulled from the master list
    readonly Dictionary<string, TrendObjectInfo> fullObjectDict = new();

    //The set of TrendCards that are currently active
    readonly Dictionary<string, TrendCard> activeCards = new();

    RectTransform panelRect;
    HorizontalLayoutGroup horizontalLayoutGroup;

    bool isActive = true;
    bool stopRequested = false;

    //Controls the cooldown for placing new TrendCards.
    float trendCooldownTimer = 0;

    int cardWidth;

    [Header("Subscriber Interface for Trends")]
    public TrendEvent OnAddTrendEvent;
    public TrendEvent OnRemoveTrendEvent;

    private void Awake()
    {
        masterObjectListSO = objectListSmall;
        panelRect = GetComponent<RectTransform>();
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();

        maxTrendCooldownLength = Mathf.Max(minTrendCooldownLength + 1, maxTrendCooldownLength);

        int totalPadding = horizontalLayoutGroup.padding.right * maxActiveTrends;
        cardWidth = Mathf.FloorToInt((panelRect.rect.width - totalPadding) / maxActiveTrends);

        if (cardPrefab == null)
            Debug.LogError("Missing ShortPanel Prefab object.");

        foreach (TrendObjectInfo obj in masterObjectListSO.TrendObjects)
        {
            fullObjectDict.Add(obj.ObjectName, obj);
            trendOptions.Add(obj.ObjectName);
        }
    }

    private void Start()
    {
        if (OnAddTrendEvent == null)
        {
            OnAddTrendEvent = new();
        }

        if (OnRemoveTrendEvent == null)
        {
            OnRemoveTrendEvent = new();
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
                AddTrendItem();

                //Return any pending returns back into the set for selection
                while (pendingReturns.TryDequeue(out string trendOption))
                {
                    trendOptions.Add(trendOption);
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
        TrendObjectInfo newTrendObject;
        
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

        int objectScore;

        if (Random.value < negativeTrendChance)
            objectScore = Random.Range(newTrendObject.MinNegativeScore, newTrendObject.MaxNegativeScore + 1);
        else
            objectScore = Random.Range(newTrendObject.MinPositiveScore, newTrendObject.MaxPositiveScore + 1);
        
        trendOptions.RemoveAt(randObjIndex);

        if (!cardPool.TryDequeue(out TrendCard newTrendCard))
        {
            newTrendCard = CreateTrendCard(cardPrefab);
            newTrendCard.SetCardWidth(cardWidth);
        }

        //Ensure the object appears at the back of the HorizontalLayoutGroup
        newTrendCard.gameObject.transform.SetAsLastSibling();
        newTrendCard.gameObject.SetActive(true);
        newTrendCard.Activate(newTrendObject.ObjectName, objectScore, objectLifetime, newTrendObject.ObjectSprite, RemoveTrendItem);
        
        activeCards.Add(newTrendObject.ObjectName, newTrendCard);

        trendCooldownTimer = Random.Range(minTrendCooldownLength, maxTrendCooldownLength);

        OnAddTrendEvent.Invoke(newTrendObject.ObjectName);
    }

    /// <summary>
    /// Clear the current list of trending items. To be called when completing a level.
    /// </summary>
    public void ClearAllTrendItems()
    {
        foreach (KeyValuePair<string, TrendCard> trendPair in activeCards)
        {
            ClearTrendItem(trendPair.Value);
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
        cardObject.transform.localPosition = Vector3.zero;
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

        cardPool.Enqueue(removeCard);
        pendingReturns.Enqueue(removeCard.ObjectName);
        OnRemoveTrendEvent.Invoke(removeCard.ObjectName);
    }

    /// <summary>
    /// Remove an Active TrendCard from the TrendFeed, hiding it and enabling it to be selected again.
    /// </summary>
    /// <param name="removeCard"></param>
    /// <param name="length"></param>
    void ClearTrendItem(TrendCard removeCard) {
        removeCard.Deactivate();

        cardPool.Enqueue(removeCard);
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
    /// Set the new list of Trending Objects for the current level to pull from.
    /// </summary>
    /// <param name="newObjectList"></param>
    public void SetTrendObjectList(int newLevel)
    {
        switch (newLevel) {
            case 0:
                masterObjectListSO = objectListSmall;
                break;
            case 1:
                masterObjectListSO = objectListMedium;
                break;
            case 2:
                masterObjectListSO = objectListLarge;
                break;
        }
        fullObjectDict.Clear();
        trendOptions.Clear();

        foreach (TrendObjectInfo obj in masterObjectListSO.TrendObjects)
        {
            fullObjectDict.Add(obj.ObjectName, obj);
            trendOptions.Add(obj.ObjectName);
        }
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
