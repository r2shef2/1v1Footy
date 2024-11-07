using System;
using System.Collections;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public int outOfBoundsNumber;
    public Vector3 resetPositionOne; // Reset position for one side
    public Vector3 resetPositionTwo;
    public Vector3 ballResetPosition; // Unique reset position for the ball// Reset position for the other side
    private Action<int> OutOfBoundsTriggered;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameController.BALL_TAG)
        {
            // Invoke the out of bounds action, passing the outOfBoundsNumber
            OutOfBoundsTriggered.Invoke(outOfBoundsNumber);

            SoundManager.Instance.PlayOutOfBoundsSound();
        }
    }

    public void SetOutOfBoundsAction(Action<int> argOutOfBoundsTriggered)
    {
        OutOfBoundsTriggered = argOutOfBoundsTriggered;
    }
}
