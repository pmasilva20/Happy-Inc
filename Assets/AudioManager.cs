using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AudioClip levelComplete;

    [SerializeField]
    public AudioClip makelink;

    [SerializeField]
    public AudioClip removeLink;

    [SerializeField]
    public AudioClip newBubble;

    [SerializeField]
    public AudioClip newNode;

    [SerializeField]
    public AudioClip removeNode;

    private List<AudioSource> sources = new List<AudioSource>();


    public void playSoundEffect(AudioClip clip)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 0.4f;
        audioSource.Play();
        sources.Add(audioSource);
    }


    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < sources.Count; i++)
        {
            AudioSource source = sources[i];
            if (!source.isPlaying)
            {
                sources.Remove(source);
                Destroy(source);
            }
        }
    }
}
