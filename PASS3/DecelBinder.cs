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
	class DecelBinder : Supporter
	{
		/// <summary>
		/// slows enemies hit, being a great help to other towers, uaually lower attack, but magic
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
		public DecelBinder(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//rect for test range
			Rectangle testRange;

			//get the range to test based on direction
			if (direction == Game1.UP)
			{
				//get range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 2) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else if (direction == Game1.RIGHT)
			{
				//get range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}
			else if (direction == Game1.DOWN)
			{
				//get range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else
			{
				//get range
				testRange = new Rectangle((col - 2) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}

			//get nodes in the range
			range = GetNodesInRange(testRange, nodeMap);
		}

		//PRE: 
		//POST: 
		//DESC: attack + slow
		public override void Attack()
		{
			//normal attack + slow
			base.Attack();
			enemiesToAttack[0].Slow(0.2f);
		}
	}
}
