// ============================================================
// Project            :  Bachelor Thesis
// File               :  ..\GitHub\BachelorThesis\modules\Game\scripts\Elements\projectile.cs
// Copyright          :  
// Author             :  -
// Created on         :  Samstag, 2. November 2013 22:08
//
// Editor             :  TorqueDev v. 1.2.3430.42233
//
// Description        :  
//                    :  
//                    :  
// ============================================================

//classes:
//Projectile -> Sprite
/*
Fields:
owner - the object that shot the projectile

*/

///create a new Projectile at %position, flying at a direction of %direction degrees at %speed
///you may add %addVeloX and %addVeloY to the speed (speed from shooting chara e.g.)
///also you will have to provide the %owner of the projectile
function createProjectile(%position, %direction, %speed, %addVeloX, %addVeloY, %owner)
{
	%shot = new Sprite(Projectile);
	%shot.setBodyType( dynamic );
	%shot.Position = %position;
	%shot.Size = "0.7 0.7";
	%shot.setLinearVelocityPolar(%direction, %speed);
	%shot.SceneLayer = 11;
	%shot.SceneGroup = 3;
	%shot.Image = "Game:Projectile";
	%shot.createCircleCollisionShape(0.35);
	%shot.setCollisionGroups( "0 1 4" );
	%shot.setCollisionCallback( true );
	%shot.owner = %owner;
	
	//adjust color Value and Collision Groups
	if (%owner.SceneGroup == $character.SceneGroup)
	{
		%shot.setBlendColor(0.5, 0.5, 1);
		%shot.setCollisionGroups(1);
		%enemy = $enemy;
	}
	else
	{
		%shot.setBlendColor(1, 0.5, 0.5);
		%shot.setCollisionGroups(2);
		%enemy = $character;
	}
	if (%owner.doubleDamage)
		%shot.setBlendColor("1 0 0");
	
	//%shot.setLinearVelocity(%newVeloX SPC %newVeloY);
	Level.add(%shot);
	
	if (isOnScreen(%position))
		alxPlay("Game:shot");
}

///is called upon a collision with the enemy
function Projectile::onCollision(%this, %obj, %details)
{
	%glare = showGlare(%this.Position, %this.Size, 200);
	Level.add(%glare);
	
	
	if (%obj.SceneGroup == 1 || %obj.SceneGroup == 2)
	{
		%obj.addHP(-%this.owner.projectileDamage);
		%obj.flash();
		alxPlay("Game:hit");
		if (%obj.SceneGroup == 1)
			addScore(10);
	}
		
	
	//delete the Projectile
	schedule(16, 0, deleteObj, %this);
}