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
	class CoreCaster : Caster
	{
		/// <summary>
		/// Core casters are to deal magic damage from a range
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
		public CoreCaster(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//rect to get the nodes in range and a special node
			Rectangle testRange;
			TileNode[] specialRange = new TileNode[1];

			//just in case, get a backup range
			testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));

			//this try/catch is here just in case the special node does not exist
			try
			{
				//get the range based on direction
				if (direction == Game1.UP)
				{
					//get the range
					testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 2) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));
					specialRange[0] = nodeMap[row - 3, col];
				}
				else if (direction == Game1.RIGHT)
				{
					//get the range
					testRange = new Rectangle(col * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));
					specialRange[0] = nodeMap[row, col + 3];
				}
				else if (direction == Game1.DOWN)
				{
					//get the range
					testRange = new Rectangle((col - 1) * Game1.UNIT + 1, row * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));
					specialRange[0] = nodeMap[row + 3, col];
				}
				else
				{
					//get the range
					testRange = new Rectangle((col - 2) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 2.5));
					specialRange[0] = nodeMap[row, col - 3];
				}
			}
			catch
			{
				//get the backup special
				specialRange[0] = nodeMap[row, col];
			}

			//get the nodes in range
			range = JoinNodeArrays(GetNodesInRange(testRange, nodeMap), specialRange);
		}
	}
}
