using UnityEngine;
using Watermelon;

public class TestL : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(Aa),0.3f);
    }

    void Aa()
    {
        GameController.LoadLevel(LevelController.MaxReachedLevelIndex);
    }
}
