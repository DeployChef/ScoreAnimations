using DG.Tweening;
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

        yield return _group.DOFade(1f, fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(showTime);
        yield return _group.DOFade(0f, fadeDuration).WaitForCompletion();

        _group.blocksRaycasts = false;
        _group.interactable = false;
        gameObject.SetActive(false);
    }
}