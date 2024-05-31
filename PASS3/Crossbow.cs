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
	class Crossbow : Enemy
	{
		//list of targets
		protected List<Tower> targets;

		//range
		protected Rectangle range;

		//timer to recover from an attack
		protected Timer atkRecoveryTimer;

		/// <summary>
		/// ranged enemy with high def
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="targets"></param>
		public Crossbow(int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg, List<Tower> targets) : base(row, col, path, imgs, hpBarImg)
		{
			//get the targets
			this.targets = targets;

			//make the stats
			maxHp = 4000;
			hp = maxHp;
			def = 250;
			res = 10;
			atk = 400;
			atkSp = 2f;
			maxSpeed = 0.8f;
			speed = maxSpeed;

			//create the range
			range = new Rectangle((rect.X / Game1.UNIT) * Game1.UNIT - Game1.UNIT, (rect.Y / Game1.UNIT) * Game1.UNIT - Game1.UNIT, Game1.UNIT * 3, Game1.UNIT * 3);

			//attack timers
			attackTimer = new Timer(1000 * atkSp, false);
			atkRecoveryTimer = new Timer(1000, false);
		}

		//PRE: gameTime for game time
		//POST: 
		//DESC: update the enemy
		public override void Update(GameTime gameTime)
		{
			//update the timers
			attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			atkRecoveryTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			UpdateTimers(gameTime);

			//dont act if stunned
			if (!stunTimer.IsActive())
			{
				//move if not blocked and not recovering
				if (!isBlocked && !atkRecoveryTimer.IsActive())
				{
					//move
					Move();
				}

				//actiavte attack timer if not blocked or not recovering
				if (isBlocked || !atkRecoveryTimer.IsActive())
				{
					//activate timer
					attackTimer.Activate();
				}

				//attack if attack timer done
				if (attackTimer.IsFinished())
				{
					//loop through
					foreach (Tower i in targets)
					{
						//if target in range, attack
						if (i.Rect().Intersects(range))
						{
							//reset timers, attack
							atkRecoveryTimer.ResetTimer(true);
							attackTimer.ResetTimer(false);
							Attack();
						}
					}
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: move, and move the the range
		public override void Move()
		{
			//move and move the range
			base.Move();
			range.X = rect.X - Game1.UNIT;
			range.Y = rect.Y - Game1.UNIT;
		}

		//PRE: 
		//POST: 
		//DESC: attack in range or blocker
		protected override void Attack()
		{
			//if no blocker, attack a target in range
			if (blocker == null)
			{
				//loop through
				foreach (Tower i in targets)
				{
					//if in range, deal damage
					if (i.Rect().Intersects(range))
					{
						//deal damage
						i.TakeDmg(atk, false);
						break;
					}
				}
			}
			else
			{
				//damage the blocker
				blocker.TakeDmg(atk, false);
			}
		}
	}
}
