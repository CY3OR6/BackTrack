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

    public string Symbol { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }
    public bool CanInteract => !IsFlipped && !IsMatched;

    public void Initialize(string symbol, GameConfig gameConfig)
    {
        Symbol = symbol;
        config = gameConfig;
        symbolText.text = symbol;
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanInteract) return;

        Flip();
        GameEvents.OnCardFlipped?.Invoke(this);
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
    }

    public void SetMatched()
    {
        IsMatched = true;
        GetComponent<CanvasGroup>().alpha = 0.7f;
    }

    public IEnumerator ResetAfterMismatch()
    {
        yield return new WaitForSeconds(config.mismatchResetDelay);
        Flip();
    }
}
