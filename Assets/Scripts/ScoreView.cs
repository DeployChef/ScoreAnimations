using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;


public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Reset()
    {
        if (_scoreText == null)
            _scoreText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetInstant(int value)
    {
        if (_scoreText == null) return;
        _scoreText.text = value.ToString();
    }

    public IEnumerator AnimateScore(int from, int to, float duration)
    {
        if (_scoreText == null)
            yield break;

        int current = from;

        // создаём tween, который меняет current от from до to
        Tween tween = DOTween.To(
            () => current,
            x =>
            {
                current = x;
                _scoreText.text = current.ToString();
            },
            to,
            duration
        ).SetEase(Ease.OutQuad);

        // ждём завершения твина внутри корутины
        yield return tween.WaitForCompletion();

        _scoreText.text = to.ToString();
    }
}
