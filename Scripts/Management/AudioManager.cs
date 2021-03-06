using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
public class AudioManager : MonoBehaviour
{
    //GameObjects
    public AudioMixer audioMixer;
    public GameObject sfxManager;
    public GameObject musicManager;
    public Slider musicSlider;
    public Slider sfxSlider;
    //Variables
    public AudioClip[] bgmClips;
    public AudioClip[] soundFXClips;
    Dictionary<ClipNames, AudioClip> AudioClips = new Dictionary<ClipNames, AudioClip>();
    public enum ClipNames
    {
        BattleSong,
        MenuSong,
        Punch,
        Kick,
        Block,
        Dodge,
        CriticalHit,
        Stagger,
        BossMusic
    }
    public enum ClipType
    {
        Music,
        SFX
    }
    //gets called in pauseman before the sliders get turned off
    public void InitializeAudio()
    {
        //find objects
        sfxManager = GameObject.Find("SFXManager");
        musicManager = GameObject.Find("MusicManager");
        musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
        sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
        //Set the slider values
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1);
        //add music
        AudioClips.Add(ClipNames.MenuSong, bgmClips[0]);
        AudioClips.Add(ClipNames.BattleSong, bgmClips[1]);
        AudioClips.Add(ClipNames.BossMusic, bgmClips[2]);
        //add sfx
        AudioClips.Add(ClipNames.Punch, soundFXClips[0]);
        AudioClips.Add(ClipNames.Kick, soundFXClips[1]);
        AudioClips.Add(ClipNames.Block, soundFXClips[2]);
        AudioClips.Add(ClipNames.Dodge, soundFXClips[3]);
        AudioClips.Add(ClipNames.CriticalHit, soundFXClips[4]);
        AudioClips.Add(ClipNames.Stagger, soundFXClips[5]);
    }
    public void PlayClip(ClipNames clipName, ClipType clipType)
    {
        if(clipType == ClipType.Music)
        {
            musicManager.GetComponent<AudioSource>().clip = AudioClips[clipName];
            musicManager.GetComponent<AudioSource>().Play();
        }
        else
            sfxManager.GetComponent<AudioSource>().PlayOneShot(AudioClips[clipName]);      
    }
    //Functions that adjust the volume
    public void AdjustMusic(float musicValue)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(musicValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicValue);
        musicManager.GetComponent<AudioSource>().volume = musicValue;
    }
    public void AdjustSFX(float sFXValue)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sFXValue) * 20);
        PlayerPrefs.SetFloat("EffectsVolume", sFXValue);
        sfxManager.GetComponent<AudioSource>().volume = sFXValue;
    }
    public void TestSFX()
    {
        sfxManager.GetComponent<AudioSource>().PlayOneShot(soundFXClips[2]);
    }
    public void StopMusic()
    {
        musicManager.GetComponent<AudioSource>().Stop();
    }
}
