using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float delayBetweenScenes = 1f;
    [SerializeField] Image fadingImage;
    Animator myAnimator;

    private void Start()
    {
        myAnimator = fadingImage.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == FindObjectOfType<Player>().myButtomCollider)
        {
            EndLevel();
        }
    }

    private void EndLevel()
    {
        StartCoroutine(LevelEndCoroutine());
    }

    IEnumerator LevelEndCoroutine()
    {
        myAnimator.SetBool("isFadedIn", false);
        yield return new WaitForSeconds(delayBetweenScenes);
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }
}
