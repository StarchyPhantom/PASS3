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
	class Ambusher : Specialist
	{
		/// <summary>
		/// Usually no block, inflicts stun on targets, attacks all targets in range
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
		public Ambusher(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//rectangle to grab nodes in range, grab a special node to make the unique range pattern
			Rectangle testRange;
			TileNode[] specialRange = new TileNode[1];

			//get the range that will be tested
			testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));

			//this try/catch is here just in case the special node does not exist
			try
			{
				//get the node based on direction
				if (direction == Game1.UP)
				{
					//get the node
					specialRange[0] = nodeMap[row - 2, col];
				}
				else if (direction == Game1.RIGHT)
				{
					//get the node
					specialRange[0] = nodeMap[row, col + 2];
				}
				else if (direction == Game1.DOWN)
				{
					//get the node
					specialRange[0] = nodeMap[row + 2, col];
				}
				else
				{
					//get the node
					specialRange[0] = nodeMap[row, col - 2];
				}
			}
			catch
			{
				//get the backup node
				specialRange[0] = nodeMap[row, col];
			}

			//get the nodes in range
			range = JoinNodeArrays(GetNodesInRange(testRange, nodeMap), specialRange);
		}

		//PRE: 
		//POST: 
		//DESC: overrides the attack to attack all enemies in range and stun them
		public override void Attack()
		{
			//loop through the enemies in range
			foreach (Enemy i in enemiesToAttack)
			{
				//damage the enemy and stun them
				i.TakeDmg(atk, false);
				i.Stun();
			}
		}
	}
}
