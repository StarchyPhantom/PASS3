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
	class Medic : Tower
	{
		//list of towers in rnage to be healed
		protected List<Tower> healTargets;

		/// <summary>
		/// medics do not attack, medics heal and prioritize the towers with the lowest health percentage
		/// </summary>
		/// <param name="name"></param>
		/// <param name="archetype"></param>
		/// <param name="stats"></param>
		/// <param name="animSheets"></param>
		/// <param name="animSizes"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="direction"></param>
		/// <param name="nodeMap"></param>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="healTargets"></param>
		public Medic(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col, List<Tower> healTargets) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//get the targets from level
			this.healTargets = healTargets;

			//rect to test for nodes in range
			Rectangle testRange;

			//get the range based on direction
			if (direction == Game1.UP)
			{
				//get the range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 3) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else if (direction == Game1.RIGHT)
			{
				//get the range
				testRange = new Rectangle(col * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}
			else if (direction == Game1.DOWN)
			{
				//get the range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, row * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else
			{
				//get the range
				testRange = new Rectangle((col - 3) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}

			//get the nodes in range
			range = GetNodesInRange(testRange, nodeMap);
		}

		//PRE: 
		//POST: string of children
		//DESC: get the children of the class
		public static string[] GetChildren()
		{
			//return the children
			return new string[] { "Medic" };
		}

		//PRE: game's time gameTime
		//POST: 
		//DESC: update the tower
		public override void Update(GameTime gameTime)
		{
			//update the range displaying timer
			displayRangeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//if the animation is not animating, remake it
			if (!temp.IsAnimating())
			{
				//get the new animation
				temp = new Animation(animSheets[1], animSizes[1, 0], animSizes[1, 1], animSizes[1, 2], 0, 21, Animation.ANIMATE_FOREVER, 500, animLoc, 0.5f, true);
			}

			//play the idle animation if no targets to heal
			if (healTargets.Count > 0)
			{
				//update the animation and attack timer
				temp2.Update(gameTime);
				atkSpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

				//activate the attack timer
				atkSpTimer.Activate();

				//if the attack timer is finished, attack
				if (atkSpTimer.IsFinished())
				{
					//attack and reset the timer
					ReorganizeTowers();
					Attack();
					atkSpTimer.ResetTimer(true);
				}
			}
			else
			{
				//update the animation and reset the attack timer
				temp.Update(gameTime);
				atkSpTimer.ResetTimer(false);
			}
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw the medic
		public override void Draw(SpriteBatch spriteBatch)
		{
			//flip the animation based on direction
			if (direction == 0 || direction == 1)
			{
				//draw based on if there are targets
				if (healTargets.Count > 0)
				{
					//draw the tower
					temp2.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
				}
				else
				{
					//draw the tower
					temp.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
				}
			}
			else
			{
				//draw based on if there are targets
				if (healTargets.Count > 0)
				{
					//draw the tower
					temp2.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
				}
				else
				{
					//draw the tower
					temp.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
				}
			}

			//display the range temporarily
			if (!displayRangeTimer.IsFinished())
			{
				//highlight the tiles in range
				foreach (TileNode i in range)
				{
					//highlight a tile in range
					spriteBatch.Draw(hpBarImg, i.rect, Color.LightGray * 0.25f);
				}
			}

			//draw the hp bar
			spriteBatch.Draw(hpBarImg, hpBarRec, Color.CornflowerBlue);
		}

		//PRE: 
		//POST: 
		//DESC: insertion sort the towers by health %
		protected void ReorganizeTowers()
		{
			//Store the current value being inserted
			Tower temp;

			//Store the index the value will be inserted at
			int j;

			//Assume the first element is sorted, now go through each unsorted element
			for (int i = 1; i < healTargets.Count; i++)
			{
				//Store the next element to be inserted
				temp = healTargets[i];

				//Shift all "sorted" elements that are greater than the new value to the right
				for (j = i; j > 0 && (double)healTargets[j - 1].GetHp() / (double)healTargets[j - 1].GetMaxHp() > (double)temp.GetHp() / (double)temp.GetMaxHp(); j--)
				{
					healTargets[j] = healTargets[j - 1];
				}

				//The insertion location has been found, now insert the value
				healTargets[j] = temp;
			}
		}

		//PRE: 
		//POST: 
		//DESC: heal the target tower
		public override void Attack()
		{
			//heal the targeted tower
			healTargets[0].TakeDmg(-atk, false);
		}

		//PRE: 
		//POST: 
		//DESC: set the heal targets list to null
		public override void DieEffects()
		{
			//nullify the heal targets list
			healTargets = null;
		}
	}
}
