using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    [SerializeField] AudioSource _audio;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayOnShot(AudioClip audio)
    {
        _audio.PlayOneShot(audio);
    }
}
