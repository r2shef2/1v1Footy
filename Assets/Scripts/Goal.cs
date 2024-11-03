using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int playerGoalNumber;
    private Action<int> GoalScored;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameController.BALL_TAG)
        {
            GoalScored.Invoke(playerGoalNumber);
        }
    }

    public void SetGoalScoredAction(Action<int> argGoalScored)
    {
        GoalScored = argGoalScored;
    }
}
