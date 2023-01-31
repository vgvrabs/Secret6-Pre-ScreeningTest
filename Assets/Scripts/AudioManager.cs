using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<AudioManager>();
    }
    
    private AudioSource[] SFX {
        get {
            return GetComponentsInChildren<AudioSource>();
        }
    }

    public void PlaySound(string name, float delay) {
        foreach (AudioSource a in SFX) {
            if (a.name.Contains(name)) {
                a.Play();
            }
        }
    }
}
