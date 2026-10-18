using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip incorrectClip;
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip endClip;

    private void OnEnable()
    {
        GameEvents.OnItemPickedUp += HandleItemPickedUp;
        GameEvents.OnItemPlaced += HandleItemPlaced;
        GameEvents.OnLevelStarted += HandleLevelStarted;
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnItemPickedUp -= HandleItemPickedUp;
        GameEvents.OnItemPlaced -= HandleItemPlaced;
        GameEvents.OnLevelStarted -= HandleLevelStarted;
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandleItemPickedUp(TablewareItem item) => PlaySfx("pickup");
    private void HandleItemPlaced(TablewareItem item, bool correct) => PlaySfx(correct ? "correct" : "incorrect");
    private void HandleLevelStarted() => PlaySfx("start");
    private void HandleLevelCompleted(SessionData data) => PlaySfx("end");

    public void PlaySfx(string eventName)
    {
        if (source == null) return;

        AudioClip clip = null;
        switch (eventName)
        {
            case "pickup": clip = pickUpClip; break;
            case "correct": clip = correctClip; break;
            case "incorrect": clip = incorrectClip; break;
            case "start": clip = startClip; break;
            case "end": clip = endClip; break;
        }
        if (clip != null) source.PlayOneShot(clip);
    }
}