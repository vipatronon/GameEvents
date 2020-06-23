using UnityEngine;

public class ListenersExemple : MonoBehaviour
{
    public void ReachedRight()
    {
        Debug.Log("Has reached right!");
        // Do more stuff maybe?
    }

    public void ReachedLeft()
    {
        Debug.Log("Has reached left!");
        // Do more stuff maybe?
    }
}
