﻿using UnityEngine;
using System.Collections;

public class HealthFlask : GuiBase {
	/*The health, mana and poisoned state indicators*/
	public UITexture[] LevelImages=new UITexture[13];
	public float Level;
	public float MaxLevel;
	public float FlaskLevel;

	private float PreviousLevel;
	private float PreviousMaxLevel;

	public bool isHealthDisplay;
	private bool Poisoned;

	void Update () {
		if (isHealthDisplay==true)
		{//Health flask
			Level=playerUW.CurVIT;
			MaxLevel=playerUW.MaxVIT;
		}
		else
		{//mana flask
			Level=playerUW.PlayerMagic.CurMana;
			MaxLevel=playerUW.PlayerMagic.MaxMana;
		}

		if ((isHealthDisplay) && (playerUW.Poisoned!=Poisoned))
		{
			Poisoned = playerUW.Poisoned;
			UpdatePoisonDisplay();
		}

		if ((Level!=PreviousLevel) || (MaxLevel!=PreviousMaxLevel))
		{
			PreviousLevel=Level;
			PreviousMaxLevel=MaxLevel;
			if (MaxLevel>0)
			{
				FlaskLevel = (Level/MaxLevel) * 13.0f; 
				
				for (int i = 0; i <13; i++)
				{
					LevelImages[i].enabled=(i<FlaskLevel);
				}
			}
		}
	}


	void UpdatePoisonDisplay()
	{
		//Debug.Log("Updating poison display");
		for (int i = 0; i <13; i++)
		{
			if (Poisoned==true)
			{//Load the poisoned versions of the flask images.
				LevelImages[i].mainTexture=Resources.Load <Texture2D> ("HUD/Flask/Flasks_"+ (i+50).ToString("0000"));
			}
			else
			{//Load the healthy versions of the flask images.
				LevelImages[i].mainTexture=Resources.Load <Texture2D> ("HUD/Flask/Flasks_"+ (i).ToString("0000"));
			}
		}
	}

	void OnClick()
	{
		string output=""; 
		if (isHealthDisplay == true)
		{
			//Your current vitality is out of 
			if (playerUW.Poisoned==true)
			{
				output = "You are [barely/mildly/badly] poisoned\n";
			}
			output= output + "Your current vitality is " +playerUW.CurVIT + " out of " + playerUW.MaxVIT;
		}
		else
		{//your current mana points are 
			output= output + "Your current mana points are " +playerUW.PlayerMagic.CurMana + " out of " + playerUW.PlayerMagic.MaxMana;
		}

		playerUW.GetMessageLog().text =output;
	}
}
