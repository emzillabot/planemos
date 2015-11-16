﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * Subroutine that simulates text being loaded into memory
 * and displayed to a computer screen
 * @author emsharp
 */
public class TutorialTyperTF : MonoBehaviour {
	
	public float letterPause = 0.05f;
	public AudioClip startSound;

	// Read in characters of tutorial text first
	Text tutorialGameText;
	string textString;

	// Use this for initialization, each time new UI text
	// is displayed on the screen
	void Start () {

		tutorialGameText = GetComponent<Text> ();
		textString = tutorialGameText.text;
		tutorialGameText.text = "";
		StartCoroutine (TypeTutorialText ());
	}
	
	// Subroutine that simulates using computer terminal to "run" tutorial
	IEnumerator TypeTutorialText () {

		/*TODO Play startSound once indicating new page */

		foreach (char letter in textString.ToCharArray()) {
			tutorialGameText.text += letter;

			/*if(typeSound) {
				AudioClip.
			}*/
			yield return new WaitForSeconds(letterPause);
		}

		if (TutorialManagerTF.GAME_STATE == 0) {
			yield return new WaitForSeconds (1.0f);
			TutorialManagerTF.GAME_STATE = 1;
		}
	}
	
}