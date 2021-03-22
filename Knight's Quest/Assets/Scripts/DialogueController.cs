using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] GameObject[] dialoguePhrases;
    [SerializeField] GameObject errorPhrase, successPhrase, nextButton, payButton, doneButton, closeButton;

    int phraseCounter = 0;

    public void OpenDialogueWindow()
    {
        dialogueCanvas.SetActive(true);
        dialoguePhrases[phraseCounter].SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextPhrase()
    {
        dialoguePhrases[phraseCounter].SetActive(false);
        phraseCounter++;
        dialoguePhrases[phraseCounter].SetActive(true);

        if (phraseCounter == dialoguePhrases.Length - 1)
        {
            nextButton.SetActive(false);
            payButton.SetActive(true);
        }
    }

    public void Purchase()
    {
        payButton.SetActive(false);
        nextButton.SetActive(false);

        if (successPhrase != null)
        {
            dialoguePhrases[phraseCounter].SetActive(false);
            closeButton.SetActive(false);
            nextButton.SetActive(false);
            payButton.SetActive(false);
            successPhrase.SetActive(true);
            doneButton.SetActive(true);
        }
        else
        {
            FinishDialogueSuccessfuly();
        }
    }

    public void FinishDialogueSuccessfuly()
    {
        CloseDialogueWindow();
        FindObjectOfType<Wizard>().CheckLevelPassBlocker();
        Destroy(gameObject);
    }

    public void FailDialogue()
    {
        dialoguePhrases[phraseCounter].SetActive(false);
        errorPhrase.SetActive(true);
    }

    public void CloseDialogueWindow()
    {
        if (payButton.activeSelf)
        {
            payButton.SetActive(false);
            nextButton.SetActive(true);
        }

        if (errorPhrase.activeSelf)
        {
            errorPhrase.SetActive(false);
        }
        else
        {
            doneButton.SetActive(false);
            dialoguePhrases[phraseCounter].SetActive(false);
        }

        phraseCounter = 0;
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}
