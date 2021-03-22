using UnityEngine;
using UnityEngine.UI;

public class FadingScreen : MonoBehaviour
{
    [SerializeField] Image fadingImage;
    Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        fadingImage.gameObject.SetActive(true);
        myAnimator = fadingImage.GetComponent<Animator>();
        myAnimator.SetBool("isFadedIn", true);
    }

}
