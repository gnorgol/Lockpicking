using System.Collections;
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


    private bool hasPlayedHintSound = false;

    public float unlockAngle;
    public float pickRotation;
    private float lockRotation;
    public bool canRotate;
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



    public bool hasPlayedBreakSound = false;

    private void HandleLockpicking()
    {
        // Rotation du crochet
        if (canRotate) {
        float rotationInput = Input.GetAxis("Horizontal");
        pickRotation = Mathf.Clamp(pickRotation - rotationInput * Time.deltaTime * 50f, -180f, 0f);
        lockpickImage.transform.rotation = Quaternion.Euler(0, 0, pickRotation);
        }

        // Rotation de la serrure
        if (Input.GetKey(KeyCode.Space))
        {
            if (canRotate)
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
                        if (!hasPlayedBreakSound)
                        {
                            Debug.Log("Lock broken!");
                            PlaySound(breakSound);


                            hasPlayedBreakSound = true;
                        }
                    }
                }
            }

        }
        else
        {
            lockRotation = Mathf.Lerp(lockRotation, 0f, Time.deltaTime * 5f);
            lockInsideImage.transform.rotation = Quaternion.Euler(0, 0, lockRotation);
            canRotate = true;
            hasPlayedBreakSound = false; // Reset the flag when the player stops rotating
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
            if (!hasPlayedHintSound)
            {
                PlaySound(hintSound);
                hasPlayedHintSound = true;
            }
        }
        else
        {
            hasPlayedHintSound = false;
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
        StartCoroutine(PlayTurningSoundsWithDelay());
    }

    private IEnumerator PlayTurningSoundsWithDelay()
    {
        int soundIndex = Random.Range(0, turningSounds.Length);
        PlaySound(turningSounds[soundIndex]);
        yield return new WaitForSeconds(0.5f); // Délai de 0.5 secondes entre les sons
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
