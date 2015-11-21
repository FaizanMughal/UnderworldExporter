﻿using UnityEngine;
using System.Collections;

public class Fountain : object_base {
//Code for handing fountain behaviour.

	public override bool use ()
	{
		if (playerUW.playerInventory.ObjectInHand=="")
		{
			if ((objInt.isEnchanted==true) &&(objInt.Link>=512))
			{
				playerUW.PlayerMagic.CastEnchantment(playerUW.gameObject,objInt.Link-512);
			}
			ml.text= playerUW.StringControl.GetString (1,237);
			return true;
		}
		else
		{
			return ActivateByObject(playerUW.playerInventory.GetGameObjectInHand());		
		}
	}


}