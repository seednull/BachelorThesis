// ============================================================
// Project            :  BachelorThesis
// File               :  .\modules\Game\scripts\Elements\powerups.cs
// Copyright          :  
// Author             :  -
// Created on         :  Montag, 9. Dezember 2013 13:59
//
// Editor             :  TorqueDev v. 1.2.3430.42233
//
// Description        :  Creates a powerup after a random amount of time.
//                    :  It will have an effect like extra LP, AP, damage etc.
//                    :  This file handles the start function of the schedule chain
//					  :  and the Power Up class PowerUpSprite.
// ============================================================

function createPowerUp()
{
	%up = new Sprite( PowerUpSprite );
	%up.type = getRandom(0, 6);
	%up.Size = "5 5";
	%up.createCircleCollisionShape(1.5);
	%pos = getRandom(0, 4);
	
	%up.Image = "Game:PowerUp" @ getPowerUpType(%up.type);
	
	%up.Position = getPowerUpPosition(%pos);
	
	%up.SceneLayer = 8;
	%up.SceneGroup = 25;
	
	%up.setBodyType(dynamic);
	
	%up.setCollisionCallback(true);
	%up.setCollisionGroups("1 2");
	
	Level.add(%up);
	
	$powerUp = %up;
	
	
}

function getPowerUpPosition(%pos)
{
	switch(%pos)
	{
		case 0:
			return "0 0";
		case 1:
			return "-90 -90";
		case 2:
			return "-90 90";
		case 3:
			return "90 90";
		case 4:
			return "90 -90";
	}
}

function PowerUpSprite::onCollision(%this, %obj, %details)
{
	if (%obj.SceneGroup == $character.SceneGroup || %obj.SceneGroup == $enemy.SceneGroup)
	{
		schedule(1, 0, deleteObj, %this);
		$powerUpSchedule = schedule(getRandom($averagePowerUpPause / 2, $averagePowerUpPause * 1.5), 0, createPowerUp);
		
		activateUpgrade(%this.type, %obj);
		alxPlay("Game:PowerUp");
	}
}

function getPowerUpType(%i)
{
	switch(%i)
	{
		case 0:
			return "HP";
		case 1:
			return "MP";
		case 2:
			return "DD";
		case 3:
			return "HMP";
		case 4:
			return "DA";
		case 5:
			return "SI";
		case 6:
			return "CR";
	}
}

function activateUpgrade(%i, %obj)
{
	switch(%i)
	{
		case 0:
			%obj.addHP(50);
		case 1:
			%obj.addMP(1.5);
		case 2:
			%obj.projectileDamage *= 2;
			%obj.doubleDamage = true;
			%obj.setBlendColor("1 0 0");
			$deactivateDoubleDamageSchedule = schedule(10000, 0, deactivateDoubleDamage, %obj);
			$ddObject = %obj;
		case 3:
			%obj.MPCostFactor /= 2;
			%obj.setBlendColor("0 0 1");
			$deactivateHalfMPSchedule = schedule(10000, 0, deactivateHalfMP, %obj);
			$hmpObject = %obj;
		case 4:
			%obj.setBlendColor("1 1 0");
			$deactivatePowerUpResetColorSchedule = %obj.schedule(10000, resetBlendColor);
			$daObject2 = %obj;
			if (%obj.SceneGroup == $character.SceneGroup)
			{
				$enemy.projectileDamage /= 2;
				$deactivateDoubleArmorSchedule = schedule(10000, 0, deactivateDoubleArmor, $enemy);
				$daObject = $enemy;
			}
			else
			{
				$character.projectileDamage /= 2;
				$deactivateDoubleArmorSchedule = schedule(10000, 0, deactivateDoubleArmor, $character);
				$daObject = $character;
			}
		case 5:
			%obj.maxSpeed *= 1.5;
			%obj.setBlendColor("1 1 1");
			%obj.acceleration = %obj.maxSpeed / 5;
			$deactivateIncreasedSpeedSchedule = schedule(15000, 0, deactivateSpeedIncrease, %obj);
			$siObject = %obj;
		case 6:
			if (%obj.SceneGroup == $character.SceneGroup)
				addScore(mMax(500 * $level, 250));
	}
}

function deactivateDoubleDamage(%obj)
{
	%obj.projectileDamage /= 2;
	%obj.doubleDamage = false;
	%obj.resetBlendColor();
}

function deactivateHalfMP(%obj)
{
	%obj.MPCostFactor *= 2;
	%obj.resetBlendColor();
}

function deactivateDoubleArmor(%obj)
{
	%obj.projectileDamage *= 2;
}

function deactivateSpeedIncrease(%obj)
{
	%obj.maxSpeed /= 1.5;
	%obj.acceleration = %obj.maxSpeed / 5;
	%obj.resetBlendColor();
}