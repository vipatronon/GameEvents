using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes this object to listen to certain event
/// </summary>
public class GameEventListener : MonoBehaviour
{
    [Tooltip("Events to register with.")]
    public GameEvent[] gameEvents;

    [Tooltip("Response to invoke when Event is raised.")]
    [SerializeField]
    [Space(10f)]
    private ObjectEvent response;

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    private void OnEnable()
    {
        for (int i = 0; i < gameEvents.Length; i++)
        {
            gameEvents[i].RegisterListener(this);
        }
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    private void OnDisable()
    {
        for (int i = 0; i < gameEvents.Length; i++)
        {
            gameEvents[i].UnregisterListener(this);
        }
    }

    /// <summary>
    /// Calls its responses when the event gets raised
    /// </summary>
    public void OnEventRaised()
    {
        if (gameEvents != null)
        {
            for (int i = 0; i < gameEvents.Length; i++)
            {
                response.Invoke(gameEvents[i].Parameter);
            } 
        }
    }
}

/// <summary>
/// Class that represents a dynammic UnityEvent that receives an object of type GameObject as parameter
/// </summary>
[Serializable]
public class ObjectEvent : UnityEvent<object>
{

}
