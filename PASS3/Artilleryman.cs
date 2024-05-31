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
	class Artilleryman : Sniper
	{
		//list keeping track of projectiles
		List<Projectile> projectiles;

		/// <summary>
		/// an explosive-firing tower that fires a projectile instead of htting enemies in range
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
		/// <param name="projectiles"></param>
		public Artilleryman(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col, List<Projectile> projectiles) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{
			//rectangle to grab nodes in range
			Rectangle testRange;

			//get the range based on direction
			if (direction == Game1.UP)
			{
				//get the range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, (row - 4) * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), 360);
			}
			else if (direction == Game1.RIGHT)
			{
				//get the range
				testRange = new Rectangle(col * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, 360, (int)(Game1.UNIT * 2.5));
			}
			else if (direction == Game1.DOWN)
			{
				//get the range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, row * Game1.UNIT + 1, (int)(Game1.UNIT * 2.5), 360);
			}
			else
			{
				//get the range
				testRange = new Rectangle((col - 4) * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, 360, (int)(Game1.UNIT * 2.5));
			}

			//get the nodes in the range
			range = GetNodesInRange(testRange, nodeMap);

			//get the projectiles from level
			this.projectiles = projectiles;
		}

		//PRE: 
		//POST: 
		//DESC: overries the attack to firing an explosive towards the enemy
		public override void Attack()
		{
			//fires a projectile towards the enemy
			projectiles.Add(new Projectile(rect.Center.ToVector2(), enemiesToAttack[0].Rect().Center.ToVector2(), atk));
		}
	}
}
