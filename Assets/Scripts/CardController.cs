using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Ayarlar")]
    public CardManager manager;
    public Vector3 targetScale = Vector3.one;
    public bool isMatched = false;

    private CardData _data;
    private Sprite _backSprite;
    private SpriteRenderer sr;
    
    private bool isHovered = false;
    private Vector3 basePos;
    private bool isFront = false;
    
    private readonly Vector3 hoverOffset = new Vector3(0, 0, -0.2f);
    
    private const float STOP_THRESHOLD_SQR = 0.0001f; 

    public int CardID
    {
        get { return _data != null ? _data.id : -1; }
    }

    public void Initialize(CardData data, Sprite backSkin)
    {
        _data = data;
        _backSprite = backSkin;
        if(sr == null) sr = GetComponent<SpriteRenderer>(); 
        sr.sprite = backSkin;
        isFront = false;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        basePos = transform.localPosition;
        isFront = false; 
        targetScale = transform.localScale; 
    }

    void Update()
    {
        Vector3 finalPos = isHovered ? basePos + hoverOffset : basePos;

        Vector3 currentPos = transform.localPosition;

        if ((currentPos - finalPos).sqrMagnitude > STOP_THRESHOLD_SQR)
        {
            transform.localPosition = Vector3.Lerp(currentPos, finalPos, Time.deltaTime * 5f);
        }
        else if (currentPos != finalPos)
        {
            transform.localPosition = finalPos;
        }


        Vector3 finalScale = isHovered ? targetScale * 0.9f : targetScale;
        Vector3 currentScale = transform.localScale;

        if ((currentScale - finalScale).sqrMagnitude > STOP_THRESHOLD_SQR)
        {
            transform.localScale = Vector3.Lerp(currentScale, finalScale, Time.deltaTime * 5f);
        }
        else if (currentScale != finalScale)
        {
            transform.localScale = finalScale;
        }
    }

    public void OnPointerClick(PointerEventData eventData) => HandleClick();
    void OnMouseDown() => HandleClick();

    void HandleClick()
    {
        if (manager == null) { StartCoroutine(FlipAnimation(!isFront)); return; }
        if (isMatched || manager.IsBusy) return;

        if (!isFront)
        {
            StartCoroutine(FlipAnimation(true));
            manager.CardOpened(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => isHovered = true;
    public void OnPointerExit(PointerEventData eventData) => isHovered = false;
    void OnMouseEnter() => isHovered = true;
    void OnMouseExit() => isHovered = false;

    public void SetBackFace()
    {
        sr.sprite = _backSprite;
        isFront = false;
    }

    public void FlipCard()
    {
        if (_data != null) sr.sprite = _data.frontSprite;
        isFront = true;
    }

    public IEnumerator FlipAnimation(bool toFront)
    {
        float duration = 0.4f;
        float halfDuration = duration / 2;
        float t = 0f;

        Quaternion startRot = transform.rotation;
        Quaternion midRot = Quaternion.Euler(0, 90, 0);
        Quaternion endRot = Quaternion.Euler(0, 0, 0);

        while (t < 1f)
        {
            t += Time.deltaTime / halfDuration;
            transform.rotation = Quaternion.Lerp(startRot, midRot, t);
            yield return null;
        }

        if (toFront) FlipCard(); else SetBackFace();

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / halfDuration;
            transform.rotation = Quaternion.Lerp(midRot, endRot, t);
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }
}