using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] private AudioClip goalSound;
    [SerializeField, Range(0.1f, 3f)] private float goalSoundPitch = 1.0f;

    [SerializeField] private List<AudioClip> hardKickSounds;
    [SerializeField, Range(0.1f, 3f)] private float hardKickPitch = 1.0f;

    [SerializeField] private List<AudioClip> softKickSounds;
    [SerializeField, Range(0.1f, 3f)] private float softKickPitch = 1.0f;

    [SerializeField] private List<AudioClip> hitGrassSounds;
    [SerializeField, Range(0.1f, 3f)] private float hitGrassPitch = 1.0f;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Range(0.1f, 3f)] private float jumpSoundPitch = 1.0f;

    [SerializeField] private AudioClip outOfBoundsSound;
    [SerializeField, Range(0.1f, 3f)] private float outOfBoundsPitch = 1.0f;

    [SerializeField] private AudioClip stunSound;
    [SerializeField, Range(0.1f, 3f)] private float stunSoundPitch = 1.0f;

    [SerializeField] private AudioClip countdownSound;
    [SerializeField, Range(0.1f, 3f)] private float countdownSoundPitch = 1.0f;

    [SerializeField] private AudioClip timerEndSound;
    [SerializeField, Range(0.1f, 3f)] private float timerEndPitch = 1.0f;

    [SerializeField] private AudioClip grappleSound;
    [SerializeField, Range(0.1f, 3f)] private float grappleSoundPitch = 1.0f;

    [SerializeField] private AudioClip grappleHitSound;
    [SerializeField, Range(0.1f, 3f)] private float grappleHitSoundPitch = 1.0f;

    [SerializeField] private AudioClip freezeSound;
    [SerializeField, Range(0.1f, 3f)] private float freezeSoundPitch = 1.0f;

    [SerializeField] private AudioClip unfreezeSound;
    [SerializeField, Range(0.1f, 3f)] private float unfreezeSoundPitch = 1.0f;

    [SerializeField] private AudioClip playerSpeedUpSound;
    [SerializeField, Range(0.1f, 3f)] private float playerSpeedUpPitch = 1.0f;

    [SerializeField] private AudioClip playerSlowDownSound;
    [SerializeField, Range(0.1f, 3f)] private float playerSlowDownPitch = 1.0f;

    [SerializeField] private AudioClip buttonSound;
    [SerializeField, Range(0.1f, 3f)] private float buttonSoundPitch = 1.0f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundEffectsSource;
    [SerializeField] private AudioSource musicSource;

    private bool soundEffectsEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffect(AudioClip clip, float pitch)
    {
        if (soundEffectsEnabled && clip != null)
        {
            soundEffectsSource.pitch = pitch;
            soundEffectsSource.PlayOneShot(clip);
            soundEffectsSource.pitch = 1.0f; // Reset to default
        }
    }

    public void PlayRandomSoundEffect(List<AudioClip> clips, float pitch)
    {
        if (soundEffectsEnabled && clips != null && clips.Count > 0)
        {
            int index = Random.Range(0, clips.Count);
            soundEffectsSource.pitch = pitch;
            soundEffectsSource.PlayOneShot(clips[index]);
            soundEffectsSource.pitch = 1.0f; // Reset to default
        }
    }

    // Methods to play specific sounds using Inspector-defined pitch values
    public void PlayGoalSound() => PlaySoundEffect(goalSound, goalSoundPitch);
    public void PlayHardKickSound() => PlayRandomSoundEffect(hardKickSounds, hardKickPitch);
    public void PlaySoftKickSound() => PlayRandomSoundEffect(softKickSounds, softKickPitch);
    public void PlayHitGrassSound() => PlayRandomSoundEffect(hitGrassSounds, hitGrassPitch);
    public void PlayJumpSound() => PlaySoundEffect(jumpSound, jumpSoundPitch);
    public void PlayOutOfBoundsSound() => PlaySoundEffect(outOfBoundsSound, outOfBoundsPitch);
    public void PlayStunSound() => PlaySoundEffect(stunSound, stunSoundPitch);
    public void PlayCountdownSound() => PlaySoundEffect(countdownSound, countdownSoundPitch);
    public void PlayTimerEndSound() => PlaySoundEffect(timerEndSound, timerEndPitch);
    public void PlayGrappleSound() => PlaySoundEffect(grappleSound, grappleSoundPitch);
    public void PlayGrappleHitSound() => PlaySoundEffect(grappleHitSound, grappleHitSoundPitch);
    public void PlayFreezeSound() => PlaySoundEffect(freezeSound, freezeSoundPitch);
    public void PlayUnfreezeSound() => PlaySoundEffect(unfreezeSound, unfreezeSoundPitch);
    public void PlayPlayerSpeedUpSound() => PlaySoundEffect(playerSpeedUpSound, playerSpeedUpPitch);
    public void PlayPlayerSlowDownSound() => PlaySoundEffect(playerSlowDownSound, playerSlowDownPitch);
    public void PlayButtonSound() => PlaySoundEffect(buttonSound, buttonSoundPitch);

}
