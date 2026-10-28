using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [Header("Data System")]
    public CardData[] allCardData; 
    public GameObject cardPrefab;  
    public Sprite globalBackSkin;  

    [Header("Audio System")]
    public AudioSource audioSource;
    public AudioClip matchSound;    
    public AudioClip clickSound;

    [Header("Other Things")]
    public Transform spawnParent;
    public int rows = 2;
    public int cols = 4;

    public int cardsToUse = 4; 

    public float cardW = 2f;
    public float cardH = 3f;

    public float spacingX = 3f;
    public float spacingY = 4f;

    [Range(0f, 1f)]
    public float cameraMargin = 0.1f; 

    private List<CardData> deck;
    private List<CardController> openedCards;

    private WaitForSeconds _waitSpawnDelay;
    private WaitForSeconds _waitMatchCheckDelay;
    private WaitForSeconds _waitTurnDelay;

    private Camera _mainCam; 

    private bool isBusy = false; 
    public bool IsBusy => isBusy;

    void Awake()
    {
        _mainCam = Camera.main;

        _waitSpawnDelay = new WaitForSeconds(0.05f);
        _waitMatchCheckDelay = new WaitForSeconds(0.75f);
        _waitTurnDelay = new WaitForSeconds(0.5f);

        int totalCards = rows * cols;
        deck = new List<CardData>(totalCards);
        openedCards = new List<CardController>(2);
    }

    void Start()
    {
        StartCoroutine(SpawnCardsSequential());
    }

    IEnumerator SpawnCardsSequential()
    {
        deck.Clear();

        List<int> availableIndices = new List<int>(allCardData.Length);
        for (int i = 0; i < allCardData.Length; i++)
        {
            availableIndices.Add(i);
        }

        int actualCardsToUse = Mathf.Min(cardsToUse, allCardData.Length);
        
        for (int i = 0; i < actualCardsToUse; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int dataIndex = availableIndices[randomIndex];
            
            CardData selected = allCardData[dataIndex];
            availableIndices.RemoveAt(randomIndex);

            deck.Add(selected);
            deck.Add(selected);
        }

        for (int i = deck.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }

        FitCameraToGrid();

        int index = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (index >= deck.Count) yield break;

                float xPos = (c * spacingX) - ((cols - 1) * spacingX / 2f);
                float yPos = (r * -spacingY) + ((rows - 1) * spacingY / 2f);
                
                Vector3 pos = new Vector3(xPos, yPos, 0);

                GameObject cardObj = Instantiate(cardPrefab, pos, Quaternion.identity, spawnParent);
                
                CardController cc = cardObj.GetComponent<CardController>();
                cc.manager = this;
                cc.Initialize(deck[index], globalBackSkin);

                yield return StartCoroutine(AnimateCard(cardObj.transform));

                yield return _waitSpawnDelay;
                index++;
            }
        }
    }

    void FitCameraToGrid()
    {
        if (_mainCam == null) return;

        float totalW = cols * cardW + (cols - 1) * spacingX;
        float totalH = rows * cardH + (rows - 1) * spacingY;

        totalW *= (1f + cameraMargin);
        totalH *= (1f + cameraMargin);

        float halfH = totalH / 2f;
        float halfW = totalW / 2f;
        Vector3 center = spawnParent ? spawnParent.position : Vector3.zero;

        if (_mainCam.orthographic)
        {
            float neededSizeByHeight = halfH;
            float neededSizeByWidth = halfW / _mainCam.aspect;

            _mainCam.orthographicSize = Mathf.Max(neededSizeByHeight, neededSizeByWidth);
            _mainCam.transform.position = new Vector3(center.x, center.y, _mainCam.transform.position.z);
        }
        else
        {
            float vFovRad = _mainCam.fieldOfView * Mathf.Deg2Rad;
            float distanceByHeight = halfH / Mathf.Tan(vFovRad / 2f);

            float hFovRad = 2f * Mathf.Atan(Mathf.Tan(vFovRad / 2f) * _mainCam.aspect);
            float distanceByWidth = halfW / Mathf.Tan(hFovRad / 2f);

            float requiredDistance = Mathf.Max(distanceByHeight, distanceByWidth);

            Vector3 camPos = _mainCam.transform.position;
            camPos.x = center.x;
            camPos.y = center.y;
            camPos.z = center.z - requiredDistance; 

            _mainCam.transform.position = camPos;
        }
    }

    IEnumerator AnimateCard(Transform cardTrans)
    {
        float duration = 0.2f;
        float t = 0f;
        
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cardTrans.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        cardTrans.localScale = endScale;
    }

    public void CardOpened(CardController card)
    {
        if (isBusy) return;
        if (openedCards.Contains(card)) return; 
        if (card.isMatched) return;

        openedCards.Add(card);

        if (openedCards.Count > 2)
        {
            var last = openedCards[openedCards.Count - 1];
            StartCoroutine(last.FlipAnimation(false));
            openedCards.RemoveAt(openedCards.Count - 1);
            return;
        }

        if (openedCards.Count == 2)
        {
            CheckMatch(); 
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }        
    }

    private void CheckMatch()
    {
        if (openedCards[0].CardID == openedCards[1].CardID)
        {
            openedCards[0].isMatched = true;
            openedCards[1].isMatched = true;

            if (audioSource != null && matchSound != null)
            {
                audioSource.PlayOneShot(matchSound);
            }

            openedCards.Clear();
        }
        else
        {
            StartCoroutine(CloseCardsAnimated());
        }
    }

    IEnumerator CloseCardsAnimated()
    {
        isBusy = true;
        
        yield return _waitMatchCheckDelay; 

        foreach (var card in openedCards)
        {
            card.StartCoroutine(card.FlipAnimation(false));
        }

        yield return _waitTurnDelay; 

        openedCards.Clear();
        isBusy = false;
    }
}