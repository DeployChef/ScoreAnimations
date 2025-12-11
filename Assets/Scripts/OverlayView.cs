using System.Collections;
using UnityEngine;

public class OverlayView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;

    private void Reset()
    {
        if (_group == null)
            _group = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeIn(float duration)
    {
        if (_group == null) yield break;

        _group.gameObject.SetActive(true);
        _group.blocksRaycasts = true;

        float t = 0f;
        float start = _group.alpha;
        float end = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);
            _group.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        _group.alpha = end;
    }

    public IEnumerator FadeOut(float duration)
    {
        if (_group == null) yield break;

        float t = 0f;
        float start = _group.alpha;
        float end = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);
            _group.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        _group.alpha = end;
        _group.blocksRaycasts = false;
        _group.gameObject.SetActive(false);
    }
}
