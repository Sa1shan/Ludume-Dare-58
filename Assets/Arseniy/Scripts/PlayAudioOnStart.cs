using UnityEngine;

public class PlayAudioOnStart : MonoBehaviour
{
    [Header("Audio Source для проигрывания")]
    [SerializeField] private AudioSource audioSource;

    [Header("Аудио клип")]
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource или AudioClip не назначен!");
        }
    }
}