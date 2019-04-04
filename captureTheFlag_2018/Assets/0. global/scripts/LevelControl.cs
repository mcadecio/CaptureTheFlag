using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControl : MonoBehaviour {

    public int index;
    public string levelName;

	public void OnTriggerEnter (Collider other) {
        //if (other.CompareTag("Player"))
       // {
            SceneManager.LoadScene(index);

            //SceneManager.LoadScene(levelName);
        //}
    }

    public void OnTriggerEnterStart()
    {
        SceneManager.LoadScene(index);
    }

    public void OnTriggerQuitGame()
    {
        Debug.Log("QUITTING GAME");
        Application.Quit();
    }
	public void OnTriggerViewLeaderboard(){
		SceneManager.LoadScene(index);
	}
}
