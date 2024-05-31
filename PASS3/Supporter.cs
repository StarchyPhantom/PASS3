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
	class Supporter : Tower
	{
		/// <summary>
		/// supporters are here to provide support, in this case, to slow enemies and deal magic damage
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
		public Supporter(string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col) : base(name, archetype, stats, animSheets, animSizes, hpBarImg, direction, nodeMap, row, col)
		{

		}

		//PRE: 
		//POST: string of children
		//DESC: get the children of the class
		public static string[] GetChildren()
		{
			return new string[] { "DecelBinder"/*, "Bard"*/ };
		}

		//PRE: 
		//POST: 
		//DESC: override to deal magic damage
		public override void Attack()
		{
			//deal magic damage
			enemiesToAttack[0].TakeDmg(atk, true);
		}
	}
}
