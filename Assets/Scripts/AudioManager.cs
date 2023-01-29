using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        else if (instance != this) {
            Destroy(gameObject);
        }
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
