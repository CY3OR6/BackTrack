using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;
    [SerializeField] private TMP_Text symbolText;

    [Header("Config")]
    [SerializeField] private GameConfig config;

    // Properties
    public string Symbol { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }
    public bool CanInteract => !IsFlipped && !IsMatched;

    private Coroutine flipCoroutine;


    private void Start()
    {
        SetFlipState(false, immediate: true);
    }

    public void Initialize(string symbol, GameConfig gameConfig)
    {
        Symbol = symbol;
        config = gameConfig;
        symbolText.text = symbol;

        SetFlipState(false, immediate: true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanInteract) return;

        Flip();
        GameEvents.OnCardFlipped?.Invoke(this);

        Debug.Log($"Card clicked: {Symbol}");
    }

    public void Flip()
    {
        IsFlipped = !IsFlipped;
        frontFace.transform.DOLocalRotate(
            new Vector3(0, !IsFlipped ? 180 : 0, 0),
            0.5f
        ).SetEase(Ease.OutBack);

        backFace.transform.DOLocalRotate(
           new Vector3(0, IsFlipped ? 180 : 0, 0),
          0.5f
       ).SetEase(Ease.OutBack);

        SetFlipState(IsFlipped);
    }

    private void SetFlipState(bool flipped, bool immediate = false)
    {
        IsFlipped = flipped;
        frontFace.SetActive(flipped);
        backFace.SetActive(!flipped);

        if (immediate)
        {
            transform.rotation = Quaternion.Euler(0, flipped ? 180 : 0, 0);
        }
    }

    public void SetMatched()
    {
        IsMatched = true;
        // Visual feedback for matched cards
        GetComponent<CanvasGroup>().alpha = 0.7f;
    }

    public IEnumerator ResetAfterMismatch()
    {
        yield return new WaitForSeconds(config.mismatchResetDelay);
        Flip();
    }
}
