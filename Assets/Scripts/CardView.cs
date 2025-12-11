using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private GameObject _scorePopupPrefab;
    [SerializeField] private RectTransform _tableRoot;

    private void Reset()
    {
        if (_rect == null)
            _rect = GetComponent<RectTransform>();
    }

    public RectTransform Rect => _rect;

    // Вылет в центр (подпрыг + полёт + поворот)
    public IEnumerator FlyToCenter(RectTransform target, float moveDuration, float maxRotateZ,
        float hopHeight, float hopDuration)
    {
        if (_rect == null || target == null)
            yield break;

        Vector2 slotPos = _rect.anchoredPosition;
        Vector2 hopPos = slotPos + Vector2.up * hopHeight;
        Vector2 endPos = target.anchoredPosition;

        float startZ = _rect.localEulerAngles.z;
        float endZ = startZ + Random.Range(-maxRotateZ, maxRotateZ);

        Sequence seq = DOTween.Sequence();

        // подпрыг вверх-вниз
        seq.Append(_rect.DOAnchorPos(hopPos, hopDuration).SetEase(Ease.OutQuad));
        seq.Append(_rect.DOAnchorPos(slotPos, hopDuration).SetEase(Ease.InQuad));

        // полёт + поворот
        seq.Append(_rect.DOAnchorPos(endPos, moveDuration).SetEase(Ease.OutCubic));
        seq.Join(_rect.DOLocalRotate(new Vector3(0f, 0f, endZ), moveDuration));

        yield return seq.WaitForCompletion();
    }

    // Подпрыг + popup +X над картой

    public IEnumerator PlayScoreAnimation(int addScore, float jumpHeight, float duration)
    {
        if (_rect == null)
            yield break;

        // 1. Прыжок карты (пока оставим твою версию или тоже переведём позже)
        Vector2 startPos = _rect.anchoredPosition;
        Vector2 upPos = startPos + Vector2.up * jumpHeight;

        Sequence jumpSeq = DOTween.Sequence();
        jumpSeq.Append(_rect.DOAnchorPos(upPos, duration * 0.5f).SetEase(Ease.OutQuad));
        jumpSeq.Append(_rect.DOAnchorPos(startPos, duration * 0.5f).SetEase(Ease.InQuad));

        // 2. Popup +X
        if (_scorePopupPrefab != null && _tableRoot != null)
        {
            GameObject popupGO = Object.Instantiate(_scorePopupPrefab, _tableRoot);
            RectTransform popupRect = popupGO.GetComponent<RectTransform>();
            CanvasGroup popupGroup = popupGO.GetComponent<CanvasGroup>();

            if (popupRect != null)
            {
                Vector2 popupStart = _rect.anchoredPosition + new Vector2(0f, 40f);
                Vector2 popupEnd = popupStart + new Vector2(0f, 30f);

                popupRect.anchoredPosition = popupStart;

                var tmp = popupGO.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                    tmp.text = $"+{addScore}";

                float popupDuration = 0.4f;

                float startZ = Random.Range(-6f, 6f);
                float endZ = startZ + Random.Range(-3f, 3f);
                popupRect.localEulerAngles = new Vector3(0f, 0f, startZ);

                // Sequence для popup
                Sequence popupSeq = DOTween.Sequence();

                // движение вверх
                popupSeq.Join(popupRect.DOAnchorPos(popupEnd, popupDuration).SetEase(Ease.OutQuad));

                // fade, если есть CanvasGroup
                if (popupGroup != null)
                {
                    popupGroup.alpha = 1f;
                    popupSeq.Join(popupGroup.DOFade(0f, popupDuration));
                }

                // поворот
                popupSeq.Join(popupRect.DOLocalRotate(
                    new Vector3(0f, 0f, endZ),
                    popupDuration
                ));

                popupSeq.OnComplete(() => Object.Destroy(popupGO));

                // ждём одновременно прыжок карты + popup
                Sequence total = DOTween.Sequence();
                total.Append(jumpSeq);
                total.Join(popupSeq);

                yield return total.WaitForCompletion();
                yield break;
            }
        }

        // Если popup не создался – просто ждём прыжок
        yield return jumpSeq.WaitForCompletion();
    }
}
