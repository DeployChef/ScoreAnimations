using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FinalScoreView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Reset()
    {
        if (_group == null)
            _group = GetComponent<CanvasGroup>();
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

    public IEnumerator Show(int totalScore, float fadeDuration, float showTime)
    {
        if (_group == null)
            yield break;

        gameObject.SetActive(true);
        _group.blocksRaycasts = true;

        if (_scoreText != null)
            _scoreText.text = totalScore.ToString();

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

        // Небольшая пауза, пока окно висит
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
        _group.blocksRaycasts = false;
        _group.interactable = false;
        gameObject.SetActive(false);
    }
}