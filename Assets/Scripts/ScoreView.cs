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

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            int current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            _scoreText.text = current.ToString();

            yield return null;
        }

        _scoreText.text = to.ToString();
    }
}
