#if(UNITY_EDITOR)

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public class GameEventUtility : EditorWindow
    {
        private const string findListenersMenuName = "Find Listeners";
        private const string findRaisersMenuName = "Find Raisers";
        private const string confirmDialog = "Ok";
        private const string maisInfoDialog = "Details";
        private static string selectedGameEventName = string.Empty;
        private static GameEvent selectedGameEvent;
        private static SearchType searchType;
        private static List<GameObject> foundRaisersInScene = new List<GameObject>();
        private static List<GameObject> foundRaisersInPrefabs = new List<GameObject>();
        private static List<GameObject> foundListenersInScene = new List<GameObject>();
        private static List<GameObject> foundListenersInPrefabs = new List<GameObject>();

        /// <summary>
        /// Method that validates if the selected asset is a GameEvent indeed
        /// </summary>
        /// <returns></returns>
        [MenuItem("Assets/Event System/Find Listeners", validate = true)]
        private static bool FindListenersValidation()
        {
            return Selection.activeObject is GameEvent;
        }

        /// <summary>
        /// Method that finds the listeners of determinate GameEvent
        /// </summary>
        [MenuItem("Assets/Event System/Find Listeners")]
        private static void FindListeners()
        {
            selectedGameEvent = Selection.activeObject as GameEvent;
            selectedGameEventName = Selection.activeObject.name;
            searchType = SearchType.Listeners;

            Debug.Log($"<b>Finding listeners of event:{selectedGameEventName}...</b>");
            foundListenersInScene = FindListenersInScene();
            foundListenersInPrefabs = FindListenersInPrefabs();

            if (foundListenersInScene.Count == 0 && foundListenersInPrefabs.Count == 0)
            {
                EditorUtility.DisplayDialog(findListenersMenuName, $"None listener found for event {selectedGameEventName} in scene or prefabs", confirmDialog);
            }
            else
            {
                bool openWindow = EditorUtility.DisplayDialog(findListenersMenuName, $"Search finished for event {selectedGameEventName}.\n\n{foundListenersInScene.Count} listener(s) in scene.\n{foundListenersInPrefabs.Count} listener(s) in prefabs(s).\n\n", confirmDialog, maisInfoDialog);

                if (!openWindow)
                {
                    OpenDetailsWindow();
                }
            }

            Debug.Log("<b>End of search</b>");
        }

        /// <summary>
        /// Method that validates if the selected asset is a GameEvent indeed
        /// </summary>
        /// <returns></returns>
        [MenuItem("Assets/Event System/Find Raisers", validate = true)]
        private static bool FindRaisersValidation()
        {
            return Selection.activeObject is GameEvent;
        }

        /// <summary>
        /// Method that finds all raises of Selection.activeObject GameEvent
        /// </summary>
        [MenuItem("Assets/Event System/Find Raisers")]
        private static void FindRaisers()
        {
            selectedGameEvent = Selection.activeObject as GameEvent;
            selectedGameEventName = Selection.activeObject.name;
            searchType = SearchType.Raisers;

            Debug.Log($"<b>Finding raisers of event {selectedGameEventName}...</b>");
            var raisersInScene = FindRaisersInScene();
            var raisersInPrefabs = FindRaisersInPrefabs();

            if (raisersInScene == 0 && raisersInPrefabs == 0)
            {
                EditorUtility.DisplayDialog(findRaisersMenuName, $"None raiser found for event {selectedGameEventName} in scene or prefabs", confirmDialog);
            }
            else
            {
                bool openWindow = EditorUtility.DisplayDialog(findRaisersMenuName, $"Search finished for event {selectedGameEventName}.\n\n{raisersInScene} raiser(s) in scene.\n{raisersInPrefabs} raiser(s) in prefab(s)\n\n", confirmDialog, maisInfoDialog);

                if (!openWindow)
                {
                    OpenDetailsWindow();
                }
            }

            Debug.Log("<b>End of search</b>");
        }

        /// <summary>
        /// Finds all GameObjects in scene that has an GameEventListeners component attached to it
        /// with the selected asset in Selection.activeObject
        /// </summary>
        /// <returns></returns>
        private static List<GameObject> FindListenersInScene()
        {
            Debug.Log("<b><color=teal>References in scene</color></b>");

            var componentsInProject = Resources.FindObjectsOfTypeAll<GameEventListener>();
            var componentsInScene = new List<GameEventListener>();

            foreach (var component in componentsInProject)
            {
                if (!AssetDatabase.GetAssetOrScenePath(component).Contains(".prefab"))
                    componentsInScene.Add(component);
            }

            var filteredListeners = new List<GameObject>();

            foreach (var listener in componentsInScene)
            {
                var listerenerType = listener.GetType();

                var fields = listerenerType.GetFields();

                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        GameEvent[] gameEvents = (GameEvent[])field.GetValue(listener);
                        var rodiscreison = Selection.activeObject;

                        if (gameEvents != null)
                        {
                            foreach (var gameEvent in gameEvents)
                            {
                                if (gameEvent.name.Equals(selectedGameEventName))
                                {
                                    Debug.Log($"<color=teal>{listener.name}</color>");
                                    filteredListeners.Add(listener.gameObject);
                                }
                            }
                        }
                    }
                }
            }

            return filteredListeners;
        }

        /// <summary>
        /// Finds all prefabs in project that has an GameEventListeners component attached to it
        /// with the selected asset in Selection.activeObject
        /// </summary>
        /// <returns></returns>
        private static List<GameObject> FindListenersInPrefabs()
        {
            Debug.Log("<b><color=navy>References in prefabs</color></b>");

            var filteredListeners = new List<GameObject>();
            var assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in assetPaths)
            {
                if (path.Contains(".prefab"))
                {
                    var rawAsset = AssetDatabase.LoadMainAssetAtPath(path);

                    var asset = rawAsset as GameObject;

                    if (asset != null)
                    {
                        var assetComponents = asset.GetComponents<GameEventListener>();

                        if (assetComponents.Length > 0)
                        {
                            foreach (var listener in assetComponents)
                            {
                                if (listener.gameEvents.Length > 0)
                                {
                                    foreach (var gameEvent in listener.gameEvents)
                                    {
                                        if (gameEvent.name.Equals(selectedGameEventName))
                                        {
                                            filteredListeners.Add(listener.gameObject);
                                            Debug.Log($"<color=navy>{asset.name}</color>");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return filteredListeners;
        }

        /// <summary>
        /// Finds all GameObjects in scene that has some reference in any script attached to it that
        /// raises the Selection.activeObject GameEvent
        /// </summary>
        /// <returns></returns>
        private static int FindRaisersInScene()
        {
            Debug.Log("<b><color=teal>References in scene</color></b>");

            var componentsInProject = Resources.FindObjectsOfTypeAll<Component>();
            var componentsInScene = new List<Component>();

            foreach (var component in componentsInProject)
            {
                if (!AssetDatabase.GetAssetOrScenePath(component).Contains(".prefab"))
                    componentsInScene.Add(component);
            }

            var foundInSceneCount = CheckRaiserComponents(componentsInScene.ToArray(), "teal", ref foundRaisersInScene);

            return foundInSceneCount;
        }

        /// <summary>
        /// Finds all prefabs that has some reference in any script attached to it that
        /// raises the Selection.activeObject GameEvent
        /// </summary>
        /// <returns></returns>
        private static int FindRaisersInPrefabs()
        {
            var foundInPrefabsCount = 0;

            Debug.Log("<b><color=navy>References in prefabs</color></b>");

            var filteredListeners = new List<GameEventListener>();
            var assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in assetPaths)
            {
                if (path.Contains(".prefab"))
                {
                    var rawAsset = AssetDatabase.LoadMainAssetAtPath(path);

                    var asset = rawAsset as GameObject;

                    if (asset != null)
                    {
                        var components = asset.GetComponents<Component>();

                        foundInPrefabsCount += CheckRaiserComponents(components, "navy", ref foundRaisersInPrefabs);
                    }
                }
            }

            return foundInPrefabsCount;
        }

        /// <summary>
        /// Checks if any given component has some reference to the Selection.activeObject GameEvent
        /// </summary>
        /// <param name="components"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static int CheckRaiserComponents(Component[] components, string color, ref List<GameObject> foundObjects)
        {
            var foundCount = 0;
            var selectionObject = Selection.activeObject.ToString();

            if (components.Length > 0)
            {
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        var fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ToList();

                        var scriptNamespace = component.GetType().BaseType.Namespace;

                        if (!string.IsNullOrEmpty(scriptNamespace))
                        {
                            if (component.GetType().BaseType.Namespace.Contains("Assets")) //or get by custom namespace
                            {
                                var componentBase = component.GetType().BaseType;

                                while (!componentBase.Equals(typeof(MonoBehaviour)))
                                {
                                    fields.AddRange(componentBase.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ToList());

                                    componentBase = componentBase.BaseType;

                                    if (componentBase.GetType().Equals(typeof(object)))
                                        break;
                                }
                            }

                            foreach (var field in fields)
                            {
                                if (field.FieldType == typeof(GameEvent))
                                {
                                    var value = field.GetValue(component);

                                    if (value != null)
                                    {
                                        if (value.ToString() == selectionObject)
                                        {
                                            Debug.Log($"<color={color}>{component.name}</color>");
                                            foundCount++;
                                            foundObjects.Add(component.gameObject);
                                        }
                                    }
                                }

                                //TODO: Pensar em uma forma de perguntar por um tipo generico ao inves de implementacoes concretas...
                                if (field.FieldType == typeof(UnityEvent) || field.FieldType == typeof(ObjectEvent))
                                {
                                    var unityEvent = field.GetValue(component) as UnityEventBase;

                                    for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                                    {
                                        if (unityEvent.GetPersistentTarget(i).ToString() == selectionObject)
                                        {
                                            Debug.Log($"<color={color}>{component.name}</color>");
                                            foundCount++;
                                            foundObjects.Add(component.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return foundCount;
        }

        /// <summary>
        /// Opens this window in Unity
        /// </summary>    
        private static void OpenDetailsWindow()
        {
            var window = GetWindow(typeof(GameEventUtility));
            window.minSize = new Vector2(500f, 400f);
            window.Show();
        }

        /// <summary>
        /// Whenever the window is drawn
        /// </summary>
        private void OnGUI()
        {

            //TODO: Scrollview

            GUILayout.Label("GameEvent Finder", EditorStyles.boldLabel);
            GUILayout.Label($"Reference details of {searchType} for GameEvent {selectedGameEventName}");
            EditorGUILayout.ObjectField(selectedGameEvent, typeof(GameEvent), false);
            GUILayout.Space(15f);

            var foundObjectsInScene = new List<GameObject>();
            var foundObjectsInPrefabs = new List<GameObject>();

            if (searchType.Equals(SearchType.Raisers))
            {
                foundObjectsInScene = foundRaisersInScene;
                foundObjectsInPrefabs = foundRaisersInPrefabs;
            }
            else
            {
                foundObjectsInScene = foundListenersInScene;
                foundObjectsInPrefabs = foundListenersInPrefabs;
            }

            GUILayout.Label("References in scene");

            foreach (var gameObject in foundObjectsInScene)
            {
                if (GUILayout.Button(gameObject.name))
                {
                    EditorGUIUtility.PingObject(gameObject);
                }
            }

            GUILayout.Space(15f);
            GUILayout.Label("References in prefabs");

            foreach (var gameObject in foundObjectsInPrefabs)
            {
                if (GUILayout.Button(gameObject.name))
                {
                    EditorGUIUtility.PingObject(gameObject);
                }
            }

            GUILayout.Space(10f);
        }

        /// <summary>
        /// Whenever this window becomes disabled
        /// </summary>
        private void OnDisable()
        {
            foundRaisersInScene.Clear();
            foundRaisersInPrefabs.Clear();
            foundListenersInScene.Clear();
            foundListenersInPrefabs.Clear();
            selectedGameEventName = string.Empty;
        }
    }
#endif