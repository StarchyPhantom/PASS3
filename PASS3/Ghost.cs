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
	class Ghost : Crossbow
	{
		/// <summary>
		/// cannot be blocked, low defensive stats
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="targets"></param>
		public Ghost(int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg, List<Tower> targets) : base(row, col, path, imgs, hpBarImg, targets)
		{
			//set stats
			maxHp = 5000;
			hp = maxHp;
			def = 100;
			res = 10;
			atk = 500;
			atkSp = 2f;
			maxSpeed = 0.5f;
			speed = maxSpeed;

			//set range
			range = new Rectangle((rect.X / Game1.UNIT) * Game1.UNIT, (rect.Y / Game1.UNIT) * Game1.UNIT, Game1.UNIT, Game1.UNIT);

			//is blocked so it cannot be blocked
			isBlocked = true;

			//set attack timer
			attackTimer = new Timer(1000 * atkSp, true);
		}

		//PRE: game time for the time of the game
		//POST: 
		//DESC: update enemy
		public override void Update(GameTime gameTime)
		{
			//upadte timers
			UpdateTimers(gameTime);
			attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//do not do anything if stunned
			if (!stunTimer.IsActive())
			{
				//move 
				Move();

				//attack if in range
				if (attackTimer.IsFinished())
				{
					foreach (Tower i in targets)
					{
						//is in range? attack
						if (range.Contains(i.Rect()))
						{
							//attack, reset timer
							attackTimer.ResetTimer(true);
							Attack();
						}
					}
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: move normally, move range
		public override void Move()
		{
			//move noramlly with the range
			base.Move();
			range.X += Game1.UNIT;
			range.Y += Game1.UNIT;
		}
	}
}
