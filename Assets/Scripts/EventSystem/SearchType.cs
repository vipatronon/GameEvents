#if(UNITY_EDITOR)

/// <summary>
/// Represents a type of search in the GameEvent find tool
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Searchs for listeners of a GameEvent
    /// </summary>
    Listeners,

    /// <summary>
    /// Searchs for raisers of a GameEvent
    /// </summary>
    Raisers
}

#endif