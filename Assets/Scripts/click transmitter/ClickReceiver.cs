using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickReceiver : MonoBehaviour
{
    //when a clickReceiver is activated by a clickTransmitter, it performs actions on whatever it is attached to.

    //if moveTarget and rotateAxis are null, moves along one axis away from click transmitter

    //lockdown target is the gameobject which has another clickReceiver, that you wish to disable for lockdownDuration seconds.
    public GameObject moveTarget, lockdownTarget;
    public float moveDuration = 1;
    public float pauseDuration = 2;
    public float lockdownDuration = 0;
    public float rotateDegrees = 0;
    public Vector3 startPosition;

    [System.Serializable]
    public class Sound
    {
        public AudioClip audioClip;
        public float playDelay = 0;
        public Modes playOnMode;

    }
    public List<Sound> sounds;

    private float timeCounter = 0;
    private Quaternion startRotation;
    private AudioSource audioSource;
    private Vector3 transformUp;

    public enum Modes
    {
        resting,
        going,
        pausing,
        returning,
        lockdown
    }
    private Modes mode = Modes.resting;

    void Start()
    {
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startRotation = transform.rotation;
        transformUp = new Vector3(0, 0, 1);

        if (moveTarget == null && rotateDegrees == 0)
        {
            Debug.Log("Warning: rotateDegrees==0 and no moveTarget assigned to a ClickReceiver.");
            moveTarget = gameObject;
        }
    }

    private void SetMode(Modes newMode, bool playSound)
    {
        mode = newMode;
        if (playSound)
            PlaySound(newMode);
    }

    private void SetMode(Modes newMode)
    {
        SetMode(newMode, true);
    }

    private void SetGoingPosition()
    {
        if (moveTarget != null)
        {
            float t = timeCounter / moveDuration;
            transform.position = Vector3.Slerp(startPosition, moveTarget.transform.position, t);
            transform.rotation = Quaternion.Lerp(startRotation, moveTarget.transform.rotation, t);
        }
        else
        {
            float ratio = Time.deltaTime / moveDuration;
            transform.Rotate(transformUp, rotateDegrees * ratio);
        }
    }

    private void SetReturningPosition()
    {
        if (moveTarget != null)
        {
            float t = timeCounter / moveDuration;
            transform.position = Vector3.Slerp(moveTarget.transform.position, startPosition, t);
            transform.rotation = Quaternion.Lerp(moveTarget.transform.rotation, startRotation, t);
        }
        else
        {
            float ratio = Time.deltaTime / moveDuration;
            transform.Rotate(transformUp, rotateDegrees * ratio * -1);
        }
    }

    void Update()
    {
        if (mode == Modes.resting)
            return;

        timeCounter += Time.deltaTime;

        if (mode == Modes.going)
        {
            if (timeCounter < moveDuration)
                SetGoingPosition();
            else
            {
                timeCounter = 0;
                if (moveTarget != null)
                {
                    transform.position = moveTarget.transform.position;
                    transform.rotation = moveTarget.transform.rotation;
                }
                SetMode(Modes.pausing);
            }
        }
        else if (mode == Modes.pausing && timeCounter > pauseDuration)
        {
            timeCounter = 0;
            SetMode(Modes.returning);
        }
        else if (mode == Modes.returning)
        {
            if (timeCounter < moveDuration)
                SetReturningPosition();
            else
            {
                timeCounter = 0;
                transform.position = startPosition;
                transform.rotation = startRotation;

                SetMode(Modes.resting);
            }
        }
    }

    private AudioSource GetAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        return audioSource;
    }

    private void PlaySound(Modes mode)
    {
        GetAudioSource().Stop();
        if (sounds == null)
            return;

        foreach (Sound sound in sounds)
            if (sound.playOnMode == mode)
                StartCoroutine(PlaySoundAfterDelay(sound.audioClip, sound.playDelay));
    }

    public void Activate()
    {
        if (mode == Modes.lockdown)
        {
            PlaySound(Modes.lockdown);
        }
        else if (mode == Modes.resting)
        {
            SetMode(Modes.going);
            timeCounter = 0;

            if (lockdownTarget != null)
            {
                ClickReceiver clickReceiver = lockdownTarget.GetComponent<ClickReceiver>();
                if (clickReceiver == null)
                    Debug.LogError("lockdownTarget has no clickReceiver. target=" + lockdownTarget);
                else
                    StartCoroutine(clickReceiver.LockdownForDuration(lockdownDuration));
            }
        }
    }

    private IEnumerator LockdownForDuration(float duration)
    {
        if (duration == 0)
            Debug.LogError("DisableForDuration got duration of zero.");
        if (mode != Modes.resting)
            Debug.LogError("LockdownForDuration happened when mode was not resting. mode=" + mode);

        Modes lastMode = mode;
        SetMode(Modes.lockdown, false);
        yield return new WaitForSeconds(duration);
        SetMode(lastMode,false);
    }

    private IEnumerator PlaySoundAfterDelay(AudioClip audioClip, float playDelay)
    {
        //this is necessary instead of AudioSource.PlayDelay so that multiple audioClips can be assigned to the same
        //audioSource all at once without overwriting each other.
        yield return new WaitForSeconds(playDelay);
        GetAudioSource().clip = audioClip;
        GetAudioSource().Stop();
        GetAudioSource().Play();
    }
}