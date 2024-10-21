using UnityEngine;
using UnityEngine.UI;

public class Lockpicking : MonoBehaviour
{
    public Image lockImage;
    public Image lockInsideImage;
    public Image lockpickImage;
    public AudioClip[] turningSounds;
    public AudioClip[] pickSounds;
    public AudioClip hintSound;
    public AudioClip unlockSound;
    public AudioClip breakSound;

    public float unlockAngle;
    public float pickRotation;
    private float lockRotation;
    private bool canRotate;
    public bool isLockpicking;
    private float nextCheckTime;

    private void Start()
    {
        InitializeLockpicking();
        
    }

    private void Update()
    {
        if (isLockpicking)
        {
            HandleLockpicking();
        }
    }

    private void InitializeLockpicking()
    {
        isLockpicking = true;
        lockRotation = 0f;
        pickRotation = 0f;
        unlockAngle = Random.Range(-180f, 0);
        canRotate = true;
    }



    private void HandleLockpicking()
    {
        // Rotation du crochet
        float rotationInput = Input.GetAxis("Horizontal");
        pickRotation = Mathf.Clamp(pickRotation - rotationInput * Time.deltaTime * 50f, -180f, 0f);
        lockpickImage.transform.rotation = Quaternion.Euler(0, 0, pickRotation);

        // Rotation de la serrure
        if (Input.GetKey(KeyCode.Space) && canRotate)
        {
            lockRotation = Mathf.Lerp(lockRotation, -90f, Time.deltaTime * 5f);
            lockInsideImage.transform.rotation = Quaternion.Euler(0, 0, lockRotation);
            PlayTurningSounds();

            if (Mathf.Abs(lockRotation) > 45f)
            {
                if (Mathf.Abs(pickRotation - unlockAngle) < 10f)
                {
                    if (Mathf.Abs(lockRotation) > 80f)
                    {
                        Unlock();
                    }
                }
                else
                {
                    canRotate = false;
                    PlaySound(breakSound);
                }
            }
        }
        else
        {
            lockRotation = Mathf.Lerp(lockRotation, 0f, Time.deltaTime * 5f);
            lockInsideImage.transform.rotation = Quaternion.Euler(0, 0, lockRotation);
            canRotate = true;
        }

        // Vérifier périodiquement si l'angle est correct
        if (Time.time > nextCheckTime)
        {
            CheckLockpickAngle();
            nextCheckTime = Time.time + 0.5f;
        }
    }

    private void CheckLockpickAngle()
    {
        if (Mathf.Abs(pickRotation - unlockAngle) < 10f)
        {
            PlaySound(hintSound);
        }
    }

    private void Unlock()
    {
        isLockpicking = false;
        PlaySound(unlockSound);
        Debug.Log("Unlocked!");
    }

    private void PlayTurningSounds()
    {
        int soundIndex = Random.Range(0, turningSounds.Length);
        PlaySound(turningSounds[soundIndex]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    public void StopLockpicking()
    {
        isLockpicking = false;
        Debug.Log("Lockpicking cancelled");
    }
}
