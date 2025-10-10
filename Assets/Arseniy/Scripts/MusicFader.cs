using UnityEngine;
using System.Collections;

public class MusicFader : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Основной источник музыки
    [SerializeField] private float fadeDuration = 2f; // Время плавного перехода (в секундах)
    [SerializeField] private AudioClip audioClip;
    
    private Coroutine currentFadeCoroutine;

    /// <summary>
    /// Плавно переключает музыку с текущей на новую.
    /// </summary>
    /// <param name="newClip">Новый аудиоклип.</param>
    public void ChangeMusic()
    {
        if (audioSource == null || audioClip == null)
        {
            Debug.LogWarning("AudioSource или новый AudioClip не назначен!");
            return;
        }

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeMusic());
    }

    private IEnumerator FadeMusic()
    {
        float startVolume = audioSource.volume;

        // Плавное затухание текущей музыки
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();

        // Переключение на новый трек
        audioSource.clip = audioClip;
        audioSource.Play();

        // Плавное увеличение громкости до исходной
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
        currentFadeCoroutine = null;
    }
}