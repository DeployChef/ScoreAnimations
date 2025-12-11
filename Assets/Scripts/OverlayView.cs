using DG.Tweening;
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

        yield return _group.DOFade(1f, duration).WaitForCompletion();
    }

    public IEnumerator FadeOut(float duration)
    {
        if (_group == null) yield break;

        yield return _group.DOFade(0f, duration).WaitForCompletion();
        _group.blocksRaycasts = false;
        _group.gameObject.SetActive(false);
    }
}
