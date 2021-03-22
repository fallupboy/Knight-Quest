using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    int startingSceneIndex = -1;

    public int GetStartingSceneIndex()
    {
        return startingSceneIndex;
    }

    /// <summary>
    /// This class allows to remember scene actions like pickups and mobs (e.g., when player picks up a coin and then dies, 
    /// scene will be reloader with remembering that coin pickup, so that coin won't be spawned again) 
    /// </summary>
    void Awake()
    {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
        ScenePersist[] scenePersists = FindObjectsOfType<ScenePersist>();
        if (scenePersists.Length <= 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            bool destroyMe = false;
            foreach (ScenePersist scenePersist in scenePersists)
            {
                if (scenePersist != this)
                {
                    if (scenePersist.GetStartingSceneIndex() != startingSceneIndex)
                    {
                        Destroy(scenePersist.gameObject);
                    }
                    else
                    {
                        destroyMe = true;
                    }
                }
            }

            if (destroyMe)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    void Update()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex != startingSceneIndex)
        {
            Destroy(gameObject);
        }
    }
}
