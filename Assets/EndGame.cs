using System;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player has won");
            GameOverController gameOver = FindObjectOfType<GameOverController>();
            if (gameOver != null)
                gameOver.TriggerWin();
        }
    }
}
