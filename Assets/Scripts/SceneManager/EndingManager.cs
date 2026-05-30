using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("Dialogue Box")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] dialogueLines;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public string gameSceneName = "StartScreen";

    [Header("TypingSound")]
    public AudioSource typingAudio;
    public AudioClip typingClip;

    private bool isTyping = false;
    private bool audioPlaying = false;

    // True when the player pressed skip during typing
    private bool skipTyping = false;

    // True when the player pressed Space/Click AFTER typing finished (to advance)
    private bool advancePressed = false;

    void Start()
    {
        StartCoroutine(PlayEndingSequence());
    }

    IEnumerator PlayEndingSequence()
    {
        audioPlaying = true;

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            advancePressed = false;

            yield return StartCoroutine(TypeText(dialogueText, dialogueLines[i]));

            // If the player already pressed skip (which skips AND advances in one press),
            // don't wait for another press — just move straight to the next line.
            if (!skipTyping)
            {
                // Typing finished naturally — wait for the player to press to advance
                yield return new WaitUntil(() => advancePressed);
            }

            // Reset for next line
            skipTyping = false;
            advancePressed = false;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator TypeText(TextMeshProUGUI textComponent, string line)
    {
        isTyping = true;
        skipTyping = false;

        textComponent.text = "";

        int soundCounter = 0;

        foreach (char letter in line)
        {
            if (skipTyping)
            {
                // Show the full line instantly and stop — no extra press needed to advance
                textComponent.text = line;
                break;
            }

            textComponent.text += letter;

            if (letter != ' ' && typingClip != null && typingAudio != null && audioPlaying)
            {
                soundCounter++;
                if (soundCounter % 2 == 0)
                {
                    typingAudio.PlayOneShot(typingClip);
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space);

        if (!pressed) return;

        if (isTyping)
        {
            // First press during typing → skip to end of current line AND advance
            skipTyping = true;
        }
        else
        {
            // Press after typing finished → advance to next line
            advancePressed = true;
        }
    }
}