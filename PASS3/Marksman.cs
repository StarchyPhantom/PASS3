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
	class Marksman : Sniper
	{
		/// <summary>
		/// marksmans are towers that like to sit back and assist with single targets
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
		public Marksman(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//rect to test range
			Rectangle testRange;

			//get test range depending on direction
			if (direction == Game1.UP)
			{
				//get test range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 3) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else if (direction == Game1.RIGHT)
			{
				//get test range
				testRange = new Rectangle(col * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}
			else if (direction == Game1.DOWN)
			{
				//get test range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, row * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), (int)(Game1.UNIT * 3.5));
			}
			else
			{
				//get test range
				testRange = new Rectangle((col - 3) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, (int)(Game1.UNIT * 3.5), (int)(Game1.UNIT * 2.5));
			}

			//get nodes in range
			range = GetNodesInRange(testRange, nodeMap);
		}
	}
}
