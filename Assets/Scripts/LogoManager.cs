using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogoManager : MonoBehaviour
{
    [Header("Data Listesi (Ön Yüzler İçin)")]
    public CardData[] allGameCards; 

    [Header("Logo Harflerini Sırasıyla Buraya Sürükle")]
    public GameObject[] cardPrefabs; 
    public Transform spawnParent;

    [Header("Ayarlar")]
    public float spacingX = 1.5f;      
    public float heightOffset = 2f;    
    public float cardScale = 0.5f;     

    [Header("Audio System")]
    public AudioSource audioSource; 
    public AudioClip clickSound;

    private WaitForSeconds _spawnDelay;

    void Awake()
    { 
        _spawnDelay = new WaitForSeconds(0.1f);
    }

    void Start()
    {
        StartCoroutine(SpawnLogoSequence());
    }

    IEnumerator SpawnLogoSequence()
    {
        int totalCards = cardPrefabs.Length; 
        
        float startX = -((totalCards - 1) * spacingX) / 2f;

        Vector3 targetScaleVec = Vector3.one * cardScale;
        Quaternion identityRot = Quaternion.identity;

        for (int i = 0; i < totalCards; i++)
        {
            float xPos = startX + (i * spacingX);
            Vector3 pos = new Vector3(xPos, heightOffset, 0); 

            GameObject cardObj = Instantiate(cardPrefabs[i], pos, identityRot, spawnParent);
            
            if (cardObj.TryGetComponent(out CardController cc) && cardObj.TryGetComponent(out SpriteRenderer sr))
            {
                Sprite letterSprite = sr.sprite; 

                CardData randomData = (allGameCards != null && allGameCards.Length > 0) 
                    ? allGameCards[Random.Range(0, allGameCards.Length)] 
                    : null;

                cc.manager = null; 
                cc.Initialize(randomData, letterSprite);
                
                cc.targetScale = targetScaleVec;

                cardObj.transform.localScale = Vector3.zero;
            }

            yield return StartCoroutine(AnimateCard(cardObj.transform, targetScaleVec));
            
            yield return _spawnDelay; 
        }
    }

    IEnumerator AnimateCard(Transform cardTrans, Vector3 targetScale)
    {
        float duration = 0.2f;
        float t = 0f;
        
        Vector3 startScale = Vector3.zero;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cardTrans.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        cardTrans.localScale = targetScale;
    }
    public void CardOpened(CardController card)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}