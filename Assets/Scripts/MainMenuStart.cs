using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuStart : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform newSpawn;

    public UnityEvent IntroFinished;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("GeneratorIsOn") == 1)
        {
            if (player)
            {
                player.position = newSpawn.position;
                player.rotation = newSpawn.rotation;
                IntroFinished.Invoke();
            }
        }
    }

    public void GeneratorStart()
    {
        SetGenerator(true);
    }

    public void ResetIntro()
    {
        SetGenerator(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetGenerator(bool generator)
    {
        if (generator)
        {
            PlayerPrefs.SetInt("GeneratorIsOn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("GeneratorIsOn", 0);
        }
    }
}
