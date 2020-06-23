using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An asset that represents any event int the game
/// </summary>
[CreateAssetMenu]
public class GameEvent : ScriptableObject 
{
    private List<GameEventListener> eventListeners = new List<GameEventListener>();
    [HideInInspector]
    public object Parameter {get; set;}

    /// <summary>
    /// Tells all listeners whenever this events gets raised
    /// </summary>
    public void Raise()
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised();
    }

    /// <summary>
    /// Register the listener to listen whenever thes events gets raised
    /// </summary>
    /// <param name="listener">Listener.</param>
    [HideInInspector]
    public void RegisterListener(GameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    /// <summary>
    /// Unregister the listener to not listen this event anymore
    /// </summary>
    /// <param name="listener">Listener.</param>
    [HideInInspector]
    public void UnregisterListener(GameEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
