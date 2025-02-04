using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public ScoreHandler handleScore;
    public GameObject shuffleText;
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
