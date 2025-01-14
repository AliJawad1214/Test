using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip cardClick;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip missMatchSound;

    private AudioSource audioSource;

    public static SoundManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(buttonClick);
    }

    public void PlayCardClick()
    {
        audioSource.PlayOneShot(cardClick);
    }

    public void PlaymatchSound()
    {
        audioSource.PlayOneShot(matchSound);
    }

    public void PlaymissMatchSound()
    {
        audioSource.PlayOneShot(missMatchSound);
    }
}
