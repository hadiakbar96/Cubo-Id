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

    // True when player pressed skip during typing
    private bool skipTyping = false;

    // True when player pressed Space/Click after typing finished (to advance)
    private bool advancePressed = false;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // --- INTRO TEXT PHASE ---
        for (int i = 0; i < introLines.Length; i++)
        {
            advancePressed = false;

            yield return StartCoroutine(TypeText(introText, introLines[i]));

            if (!skipTyping)
            {
                yield return new WaitUntil(() => advancePressed);
            }

            skipTyping = false;
            advancePressed = false;
        }

        // Hide intro text, show dialogue box
        introText.gameObject.SetActive(false);
        dialogueBox.SetActive(true);
        audioPlaying = true;

        // --- DIALOGUE BOX PHASE ---
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            advancePressed = false;

            yield return StartCoroutine(TypeText(dialogueText, dialogueLines[i]));

            if (!skipTyping)
            {
                yield return new WaitUntil(() => advancePressed);
            }

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
                // Show full line instantly, skip typing animation
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
            // Skip typing and advance in one press
            skipTyping = true;
        }
        else
        {
            // Advance to next line after typing finished
            advancePressed = true;
        }
    }
}