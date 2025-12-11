using System.Collections;
using UnityEngine;
using TMPro;

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

        // Fade-in
        float t = 0f;
        float start = 0f;
        float end = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            _group.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        _group.alpha = 1f;
        yield return new WaitForSeconds(showTime);

        // Fade-out
        t = 0f;
        start = _group.alpha;
        end = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            t = Mathf.Clamp01(t);
            _group.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        _group.alpha = 0f;
        gameObject.SetActive(false);
    }
}