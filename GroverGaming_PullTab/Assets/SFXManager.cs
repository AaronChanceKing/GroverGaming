using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    [SerializeField] AudioSource _audio;
    [SerializeField] AudioSource _slotAudio;

    [Header("Sounds")]
    [SerializeField] AudioClip _goodClick;
    [SerializeField] AudioClip _badClick;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayOnShot(AudioClip audio)
    {
        _audio.PlayOneShot(audio);
    }

    public void GoodClick()
    {
        _audio.PlayOneShot(_goodClick);
    }
    public void BadClick()
    {
        _audio.PlayOneShot(_badClick);
    }
    public void SpinClick()
    {
        _slotAudio.volume = 0.9f;
        _slotAudio.Play();
    }
    public void SpinSoundReduce()
    {
        _slotAudio.volume -= .3f;
    }
    public void SpinStop()
    {
        _slotAudio.Stop();
    }
}
