using UnityEngine;
public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void PlaySoundFXAtPosition(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource tempAudioSource = Instantiate(audioSource, spawnTransform.position, Quaternion.identity);
        this.audioSource.clip = audioClip;
        this.audioSource.volume = volume;
        this.audioSource.Play();
        float clipLength = audioClip.length;
        Destroy(tempAudioSource.gameObject, clipLength);
    }
}
