using System.Collections;
using TMPro;
using UnityEngine;

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

        // Подпрыг вверх
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / hopDuration;
            t = Mathf.Clamp01(t);
            _rect.anchoredPosition = Vector2.Lerp(slotPos, hopPos, t);
            yield return null;
        }

        // Возврат вниз
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / hopDuration;
            t = Mathf.Clamp01(t);
            _rect.anchoredPosition = Vector2.Lerp(hopPos, slotPos, t);
            yield return null;
        }

        // Полёт к центру с поворотом
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            t = Mathf.Clamp01(t);

            _rect.anchoredPosition = Vector2.Lerp(slotPos, endPos, t);
            float z = Mathf.Lerp(startZ, endZ, t);
            _rect.localEulerAngles = new Vector3(0f, 0f, z);

            yield return null;
        }

        _rect.anchoredPosition = endPos;
        _rect.localEulerAngles = new Vector3(0f, 0f, endZ);
    }

    // Подпрыг + popup +X над картой
    public IEnumerator PlayScoreAnimation(int addScore, float jumpHeight, float duration)
    {
        if (_rect == null)
            yield break;

        Vector2 startPos = _rect.anchoredPosition;
        Vector2 upPos = startPos + Vector2.up * jumpHeight;

        // Подпрыг
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 0.5f);
            t = Mathf.Clamp01(t);
            _rect.anchoredPosition = Vector2.Lerp(startPos, upPos, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 0.5f);
            t = Mathf.Clamp01(t);
            _rect.anchoredPosition = Vector2.Lerp(upPos, startPos, t);
            yield return null;
        }

        _rect.anchoredPosition = startPos;

        // Popup +X
        if (_scorePopupPrefab != null && _tableRoot != null)
        {
            GameObject popupGO = Instantiate(_scorePopupPrefab, _tableRoot);
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
                float pt = 0f;

                float startZ = Random.Range(-6f, 6f);
                float endZ = startZ + Random.Range(-3f, 3f);
                popupRect.localEulerAngles = new Vector3(0f, 0f, startZ);

                while (pt < 1f)
                {
                    pt += Time.deltaTime / popupDuration;
                    pt = Mathf.Clamp01(pt);

                    popupRect.anchoredPosition = Vector2.Lerp(popupStart, popupEnd, pt);

                    if (popupGroup != null)
                        popupGroup.alpha = 1f - pt;

                    float z = Mathf.Lerp(startZ, endZ, pt);
                    popupRect.localEulerAngles = new Vector3(0f, 0f, z);

                    yield return null;
                }

                Destroy(popupGO);
            }
        }
    }
}
