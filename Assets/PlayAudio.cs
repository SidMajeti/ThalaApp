using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    AudioSource audioSource;
    //public AudioClip audioClip;
    // Start is called before the first frame update
    const float start = 0.6f;
    const float end = 1.25f;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        AudioClip np = MakeSubclip(audioSource.clip, start, end);
        audioSource.clip = np;
    }

    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        /* Return the sub clip */
        return newClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
