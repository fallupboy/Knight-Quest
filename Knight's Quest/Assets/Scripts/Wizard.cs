using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Wizard : MonoBehaviour
{
    [SerializeField] GameObject levelPassBlocker, hint;
    [SerializeField] int amountOfCoinsToPass = 10;
    [SerializeField] int amountOfCrystalsToPass = 1;

    Animator myAnimator;

    bool canTalkWithWizard;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hint.SetActive(true);
        canTalkWithWizard = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hint.SetActive(false);
        canTalkWithWizard = false;
    }

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canTalkWithWizard)
        {
            if (CrossPlatformInputManager.GetButtonDown("Submit"))
            {
                if (FindObjectsOfType<DialogueController>().Length < 1) { return; }
                else
                {
                    FindObjectOfType<DialogueController>().OpenDialogueWindow();
                }
            }
        }
    }

    public int GetAmountOfCoinsToPass()
    {
        return amountOfCoinsToPass;
    }

    public int GetAmountOfCrystalsToPass()
    {
        return amountOfCrystalsToPass;
    }

    public void CheckLevelPassBlocker()
    {
        if (levelPassBlocker.activeSelf)
        {
            StartCoroutine(CastSpell());
        }
    }

    IEnumerator CastSpell()
    {
        myAnimator.SetTrigger("castSpell");
        yield return new WaitForSeconds(3f);
        levelPassBlocker.SetActive(false);
    }
}
