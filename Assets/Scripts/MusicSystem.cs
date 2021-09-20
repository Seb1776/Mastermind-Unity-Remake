using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   class MusicSystem
*	This class handles everything music related
*
*	Start(), void
*	Update(), void
*	GetSong(), void
*	DeactivateAnimatorDelay(float, AudioClip), IEnumerator
*	PlaySoundEffect(AudioClip), void
*/
public class MusicSystem : MonoBehaviour
{
	public enum MusicState {Decoding, Decode_Faster, Last_Decode, Win}
	public MusicState currentState;

	public bool changeToControl;
	[Range(0, 2)]
	public float changeToControlDelta;
	public bool changeToAnticipation;
	[Range(0, 2)]
	public float changeToAnticipationDelta;
	public bool changeToWin;
	[Range(0, 2)]
	public float changeToWinDelta;
	public AudioSource sourceSFX;

	GameManager manager;
	AudioClip stealth;
	AudioClip control;
	AudioClip anticipation;
	AudioClip win;
	AudioSource source;

	/*
	*   Start()
	*   Gets executed at the first frame of execution
	*
	*   Returns: void
	*/
	void Start()
	{	
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}

	/*
	*   Update()
	*   Gets executed every frame
	*
	*   Returns: void
	*/
	void Update()
	{
		if (changeToControl && source.clip != control)
		{
			if (source.volume > 0f)
			{
				if (changeToWinDelta < 0)
				{
					changeToControlDelta = Mathf.Abs(changeToControlDelta);
					source.volume -= Time.deltaTime / changeToControlDelta;
				}

				else
					source.volume -= Time.deltaTime * changeToControlDelta;
			}

			else if (source.volume <= 0f)
			{
				source.clip = control;
				source.volume = 1f;
				source.Play();
				manager.turnsWarningText.text = "5 TURNS LEFT!!!";
				manager.turnsWarning.SetBool("show", true);
				StartCoroutine(DeactivateAnimatorDelay(2f, manager.turnsWarning));
				manager.cameraWinAnimation.SetBool("faster", true);
				currentState = MusicState.Decode_Faster;
				changeToControl = false;
			}
		}

		if (changeToAnticipation && source.clip != anticipation)
		{
			if (source.volume > 0f)
			{
				if (changeToAnticipationDelta < 0)
				{
					changeToAnticipationDelta = Mathf.Abs(changeToAnticipationDelta);
					source.volume -= Time.deltaTime / changeToAnticipationDelta;
				}

				else
					source.volume -= Time.deltaTime * changeToAnticipationDelta;
			}

			else if (source.volume <= 0f)
			{
				source.clip = anticipation;
				source.volume = 1f;
				source.Play();
				manager.turnsWarningText.text = "LAST TURN!!!";
				manager.turnsWarning.SetBool("show", true);
				StartCoroutine(DeactivateAnimatorDelay(2f, manager.turnsWarning));
				manager.cameraWinAnimation.SetBool("last", true);
				currentState = MusicState.Last_Decode;
				changeToAnticipation = false;
			}
		}

		if (changeToWin && source.clip != win)
		{
			if (source.volume > 0f)
			{
				if (changeToWinDelta < 0)
				{
					changeToWinDelta = Mathf.Abs(changeToWinDelta);
					source.volume -= Time.deltaTime / changeToWinDelta;
				}

				else
					source.volume -= Time.deltaTime * changeToWinDelta;
			}

			else if (source.volume <= 0f)
			{
				source.clip = win;
				source.volume = 1f;
				source.Play();
				manager.TriggerConfetti();
				manager.winTextAnimator.SetBool("show", true);
				manager.cameraWinAnimation.SetBool("win", true);
				currentState = MusicState.Win;
				changeToWin = false;
			}
		}
	}

	/*
	*	GetSong()
	*	Based on a name, it will search all 4 song phases in the game files
	*
	*	Returns: void
	*/
	public void GetSong(string songName)
	{
		stealth = Resources.Load<AudioClip>("Music/" + songName + "_stealth");
		control = Resources.Load<AudioClip>("Music/" + songName + "_control");
		anticipation = Resources.Load<AudioClip>("Music/" + songName + "_lastwish");
		win = Resources.Load<AudioClip>("Music/" + songName + "_win");
		source = GetComponent<AudioSource>();

		source.loop = true;
		source.clip = stealth;
		source.Play();
	}

	/*
	*	DeactivateAnimatorDelay()
	*	Deactivates an animator after some seconds
	*/
	IEnumerator DeactivateAnimatorDelay(float delay, Animator _anim)
	{
		yield return new WaitForSeconds(delay);

		_anim.SetBool("show", false);
	}

	/*
	*	PlaySoundEffect()
	*	Plays a sound effect
	*
	*	Arguments: clipToPlay - The sfx to play
	*
	*	Returns: void
	*/
	public void PlaySoundEffect(AudioClip clipToPlay)
	{
		sourceSFX.PlayOneShot(clipToPlay);
	}
}
