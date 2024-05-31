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
	class Artist : Crossbow
	{
		//list of projectiles
		List<Projectile> projectiles;

		/// <summary>
		/// casts splash damage explosions, high res, low def, spread your towers out!
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="targets"></param>
		/// <param name="projectiles"></param>
		public Artist(int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg, List<Tower> targets, List<Projectile> projectiles) : base(row, col, path, imgs, hpBarImg, targets)
		{
			//make stats
			maxHp = 4000;
			hp = maxHp;
			def = 100;
			res = 50;
			atk = 300;
			atkSp = 3f;
			maxSpeed = 0.8f;
			speed = maxSpeed;

			//attack timer
			attackTimer = new Timer(1000 * atkSp, false);

			//get projectile list
			this.projectiles = projectiles;
		}

		//PRE: 
		//POST: 
		//DESC: attack with projectiles
		protected override void Attack()
		{
			//if not blocked, fire at a tower in range, otherwise fire at the blocker
			if (blocker == null)
			{
				//loop through until a target is found
				foreach (Tower i in targets)
				{
					//if in range
					if (i.Rect().Intersects(range))
					{
						//fire an explosive
						projectiles.Add(new Projectile(rect.Center.ToVector2(), i.Rect().Center.ToVector2(), atk));
						break;
					}
				}
			}
			else
			{
				//fire an explosive at the blocker
				projectiles.Add(new Projectile(rect.Center.ToVector2(), blocker.Rect().Center.ToVector2(), atk));
			}
		}
	}
}
