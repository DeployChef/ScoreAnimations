using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class ComboView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _comboText;

    private void Reset()
    {
        if (_group == null)
            _group = GetComponent<CanvasGroup>();
        if (_comboText == null)
            _comboText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Awake()
    {
        if (_group != null)
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _group.interactable = false;
        }
        gameObject.SetActive(false);
    }

    public IEnumerator Show(string text, float fadeDuration, float showTime)
    {
        if (_group == null)
            yield break;

        gameObject.SetActive(true);
        _group.blocksRaycasts = false;

        if (_comboText != null)
            _comboText.text = text;

        yield return _group.DOFade(1f, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(showTime);
        yield return _group.DOFade(0f, fadeDuration).WaitForCompletion();

        gameObject.SetActive(false);
    }
}