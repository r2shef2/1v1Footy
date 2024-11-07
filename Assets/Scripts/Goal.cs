using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int playerGoalNumber;
    private Action<int> GoalScored;
    public ParticleSystem goalEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameController.BALL_TAG)
        {
            SoundManager.Instance.PlayGoalSound();

            GoalScored.Invoke(playerGoalNumber);

            goalEffect.transform.position = collision.GetContact(0).point;

            // resize particle to face side of goal
            Vector3 newScaleOfParticle = new Vector3(goalEffect.transform.localScale.x, goalEffect.transform.localScale.y, goalEffect.transform.localScale.z);
            newScaleOfParticle.z = playerGoalNumber == 1 ? 1 : -1;
            goalEffect.transform.localScale = newScaleOfParticle;
            goalEffect.Play();
        }
    }

    public void SetGoalScoredAction(Action<int> argGoalScored)
    {
        GoalScored = argGoalScored;
    }
}
