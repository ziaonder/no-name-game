using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private float time = 0.0f;
    public Material greenMat, redMat, whiteMat;
    [HideInInspector] public List<GameObject> cardsToMatch;
    // ints are representing the instanceIDs of the cards, and bool is either it is a match or not.
    public static event Action<int, int, bool> OnMatch;
    [SerializeField] private TextMeshProUGUI timerText;
    public int totalCards = 0;
    [SerializeField] private Material cardBackground;
    [SerializeField] private Texture[] cardTextures;

    private void OnEnable()
    {
        InputController.OnCardClick += GetCardsToMatch;
    }

    private void OnDisable()
    {
        InputController.OnCardClick -= GetCardsToMatch;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        timerText.text = "Time \n" + 0;
        cardBackground.mainTexture = cardTextures[Random.Range(0, 3)];
    }

    private void GetCardsToMatch(GameObject go)
    {
        switch (cardsToMatch.Count)
        {
            case 0:
                cardsToMatch.Add(go);
                break;
            case 1:
                cardsToMatch.Add(go);
                StartCoroutine(CompareCards(cardsToMatch[0], cardsToMatch[1]));

                // Clear the list so that the next pair can be added.
                cardsToMatch.Clear();
                break;
        }
    }

    private IEnumerator CompareCards(GameObject firstCardGO, GameObject secondCardGO)
    {
        while (firstCardGO.GetComponent<CardController>().CurrentState != CardController.State.Front ||
            secondCardGO.GetComponent<CardController>().CurrentState != CardController.State.Front)
        {
            yield return null;
        }
        
        string firstCardName = firstCardGO.GetComponent<CardController>().cardData.cardName;
        string secondCardName = secondCardGO.GetComponent<CardController>().cardData.cardName;
        if (firstCardName == secondCardName)
        {
            // Match!
            OnMatch?.Invoke(firstCardGO.GetComponent<CardController>().instanceID,
                secondCardGO.GetComponent<CardController>().instanceID, true);
            totalCards -= 2;
            if (totalCards == 0)
            {
                Debug.Log("Game Over");
            }
        }
        else
        {
            // Not match.
            OnMatch?.Invoke(firstCardGO.GetComponent<CardController>().instanceID,
                secondCardGO.GetComponent<CardController>().instanceID, false);
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
        timerText.text = "Time \n" + ((int)time).ToString();
    }
}
