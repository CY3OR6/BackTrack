using System;
using UnityEngine;
using UnityEngine.Audio;


public enum SoundType
{
    CardFlip,
    CardMatch,
    CardMismatch,
    GameStart,
    GameEnd,
    ButtonClick
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundsSO SO;
    private static SoundManager instance;
    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            source = GetComponent<AudioSource>();
        }
    }

    public static void PlaySound(SoundType soundType, AudioSource audioSource = null, float volume = 1)
    {
        SoundList soundList = instance.SO.sounds[(int)soundType];

        if (soundList.sounds.Length == 0) return;

        AudioClip[] clips = soundList.sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (audioSource != null)
        {
            audioSource.outputAudioMixerGroup = soundList.mixer;
            audioSource.clip = randomClip;
            audioSource.volume = volume * soundList.volume;
            audioSource.Play();
        }
        else
        {
            instance.source.outputAudioMixerGroup = soundList.mixer;
            instance.source.PlayOneShot(randomClip, soundList.volume * volume);
        }
    }

}


[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0f, 1f)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip[] sounds;

}
