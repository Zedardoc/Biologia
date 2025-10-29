using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    public AudioClip musicsong;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music.clip = musicsong;
        music.Play();
    }
}
