using UnityEngine;
public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
    public void PlaySoundFXAtPosition(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(_audioSource, spawnTransform.position, Quaternion.identity);
        _audioSource.clip = audioClip;
        _audioSource.volume = volume;
        _audioSource.Play();
        float clipLength = audioClip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
