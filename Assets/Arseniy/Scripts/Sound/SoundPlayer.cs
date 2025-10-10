using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Tooltip("Аудиоклипы для проигрывания по очереди")]
    public List<AudioClip> clips = new List<AudioClip>();

    [Tooltip("Громкость проигрывания")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [SerializeField] private AudioSource audioSource;

    

    /// <summary>
    /// Запустить проигрывание списка аудиоклипов один за другим
    /// </summary>
    public void PlaySounds()
    {
        if (clips.Count == 0)
        {
            Debug.LogWarning("SoundPlayer: Нет аудиоклипов для проигрывания");
            return;
        }
        
        audioSource.PlayOneShot(clips[0]);
    }

    private IEnumerator PlaySequence()
    {
        foreach (var clip in clips)
        {
            if (clip == null) continue;

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);

            // Ждем пока проиграется
            yield return new WaitForSeconds(clip.length);
        }
    }

    /// <summary>
    /// Можно вызвать с любого места, передав список клипов
    /// </summary>
    public void PlaySoundsList(List<AudioClip> newClips)
    {
        clips = newClips;
        PlaySounds();
    }

    /// <summary>
    /// Можно вызвать с одного клипа
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        PlaySoundsList(new List<AudioClip> { clip });
    }
}