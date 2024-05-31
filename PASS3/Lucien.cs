using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS3
{
	class Lucien : Enemy
	{
		//game projectiles
		List<Projectile> projectiles;

		//self explosion timer
		Timer explodeTimer;

		/// <summary>
		/// The final boss, explodes on self, deals massive damage and boasts high def, res and health
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="projectiles"></param>
		public Lucien(int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg, List<Projectile> projectiles) : base(row, col, path, imgs, hpBarImg)
		{
			//make stats
			maxHp = 10000;
			hp = maxHp;
			def = 400;
			res = 40;
			atk = 1000;
			atkSp = 4f;
			maxSpeed = 0.2f;
			speed = maxSpeed;

			//get projectile list
			this.projectiles = projectiles;

			//make timers
			attackTimer = new Timer(1000 * atkSp, false);
			explodeTimer = new Timer(20000, true);
		}

		//PRE: game time for the game time time
		//POST: 
		//DESC: update Lucien
		public override void Update(GameTime gameTime)
		{
			//update normally + timer
			explodeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			base.Update(gameTime);
		}

		//PRE: 
		//POST: 
		//DESC: do an additional explosion attack
		protected override void Attack()
		{
			//attack normally
			base.Attack();

			//if the explosion timer is done, epxlode
			if (explodeTimer.IsFinished())
			{
				//reset the timer and explode on self
				projectiles.Add(new Projectile(rect.Center.ToVector2(), rect.Center.ToVector2(), atk / 2));
				explodeTimer.ResetTimer(true);
			}
		}

		//PRE: 
		//POST: boolean indicating if it is to be deleted or not
		//DESC: returns if the mob is to be deleted or not
		public override bool IsFinished()
		{
			//return to be deleted if dead or at the exit
			if (hp <= 0)
			{
				//award the kill
				Game1.currentPlayerKills++;
				return true;
			}
			else if (curNode == null && forwardPath.IsEmpty())
			{
				//subtract life
				Game1.health -= 15;
				return true;
			}

			//not done yet lol
			return false;
		}
	}
}
