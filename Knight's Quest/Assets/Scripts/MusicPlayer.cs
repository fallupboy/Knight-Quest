using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] audioList;
    AudioSource myAudioSource;

    bool musicPlayerOn = false;
    int currentSceneIndex;

    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<MusicPlayer>().Length;
        if (numMusicPlayers > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            myAudioSource.clip = audioList[0];
            CheckMusicPlayer();
            
        }

        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            myAudioSource.clip = audioList[1];
            CheckMusicPlayer();
        }
        
        if (SceneManager.GetActiveScene().name == "Level 4")
        {
            myAudioSource.clip = audioList[2];
            CheckMusicPlayer();
        }
        
        if (SceneManager.GetActiveScene().name == "End Game Success")
        {
            myAudioSource.clip = audioList[3];
            CheckMusicPlayer();
        }
    }

    // prevent Play() function from looping every frame
    private void CheckMusicPlayer()
    {
        if (!musicPlayerOn)
        {
            myAudioSource.Play();
            musicPlayerOn = true;
        }
        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            musicPlayerOn = false;
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
    }
}
