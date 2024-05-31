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
	class StandardBearer : Vanguard
	{
		//timer for dp
		Timer dpTimer;

		/// <summary>
		/// generates dp over time, cheap, kinda weak
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
		public StandardBearer(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//set timer for dp
			dpTimer = new Timer(5000, true);
		}

		//PRE: game time for the uhh, game timeeee
		//POST: 
		//DESC: 
		public override void Update(GameTime gameTime)
		{
			//update normally, and update the dp timer
			base.Update(gameTime);
			dpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//generate dp when timer is done
			if (dpTimer.IsFinished())
			{
				//generate dp, reste timer
				Game1.deploymentPoints++;
				dpTimer.ResetTimer(true);
			}
		}
	}
}
