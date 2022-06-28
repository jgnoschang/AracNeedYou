using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{

    public List<AudioSource> AudioSourceEmCena = new List<AudioSource>();
    public int indexMusic;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void disableAllMusics()
    {
        AudioSourceEmCena[1].Stop();//musica intro
    }

    public void playAudio(int index)
    {
        int random;

        

        if (index == 0)
        {
            if (AudioSourceEmCena[0].isPlaying == false)
                AudioSourceEmCena[0].Play();
        }
        if (index == 1)
        {
            if (AudioSourceEmCena[1].isPlaying == false)
                AudioSourceEmCena[1].Play();
        }

        if (index == 2)
        {
            if (AudioSourceEmCena[2].isPlaying == false)
                AudioSourceEmCena[2].Play();
        }

        if (index == 3)
        {
            if (AudioSourceEmCena[3].isPlaying == false)
                AudioSourceEmCena[3].Play();
        }

        if (index == 4)
        {
            random = Random.Range(0, 2);
            if (random == 0)
                AudioSourceEmCena[4].Play();
            else if (random == 1)
                AudioSourceEmCena[5].Play();
        }


    }
}
