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
	class Dreadnought : Guard
	{
		/// <summary>
		/// high single target dps, avergae all around, will win 1v1s any day
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
		public Dreadnought(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{

		}
	}
}
