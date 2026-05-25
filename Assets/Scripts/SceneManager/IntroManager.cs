using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Intro Text")]
    public TextMeshProUGUI introText;

    [TextArea]
    public string[] introLines;

    [Header("Dialogue Box")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] dialogueLines;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public string gameSceneName = "LevelScreen";

    [Header("TypingSound")]
    public AudioSource typingAudio;
    public AudioClip typingClip;

    private bool isTyping = false;
    private bool audioPlaying = false;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // Play intro text
        
        for (int i = 0; i < introLines.Length; i++)
        {
            yield return StartCoroutine(TypeText(introText, introLines[i]));

            yield return new WaitUntil(() =>
                Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space));
        }

        // Hide intro text
        introText.gameObject.SetActive(false);

        // Show dialogue box
        dialogueBox.SetActive(true);
        audioPlaying = true;

        // DIALOGUE BOX PHASE
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(TypeText(dialogueText, dialogueLines[i]));

            yield return new WaitUntil(() =>
                Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space));
        }

        // Load game scene
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator TypeText(TextMeshProUGUI textComponent, string line)
    {
        isTyping = true;

        textComponent.text = "";

        int soundCounter = 0;

        foreach (char letter in line)
        {
            textComponent.text += letter;

            // Play sound only for non-space characters
            if (letter != ' ' && typingClip != null && typingAudio != null && audioPlaying == true) 
            {
                soundCounter++;

                // Play sound every 2 characters
                if (soundCounter % 2 == 0)
                {
                    typingAudio.PlayOneShot(typingClip);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}