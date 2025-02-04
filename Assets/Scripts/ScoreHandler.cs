using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        scoreText.text = $"Score: {GetScore()}";
    }
    public int GetScore()
    {
        return score;
    }   
    
    public void IncrementScore(int incrementValue)
    {
        score += incrementValue;
    }
}
