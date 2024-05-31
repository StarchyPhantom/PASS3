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
	class Slug : Enemy
	{
		/// <summary>
		/// basic melee enemy with low stats
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		public Slug(int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg) : base(row, col, path, imgs, hpBarImg)
		{
			//make stats
			maxHp = 2000;
			hp = maxHp;
			def = 100;
			res = 0;
			atk = 100;
			atkSp = 1;
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw the enemy
		public override void Draw(SpriteBatch spriteBatch)
		{
			//draw depending on if attacking or not
			if (attackTimer.IsActive())
			{
				//draw enemy
				spriteBatch.Draw(imgs[0], rect, Color.Red);
			}
			else
			{
				//draw based on cycle
				if (walkTimer.GetTimeRemainingInt() > 250)
				{
					//draw enemy
					spriteBatch.Draw(imgs[0], rect, Color.White);
				}
				else
				{
					//draw enemy
					spriteBatch.Draw(imgs[0], rect, Color.White * 0.9f);
				}
			}

			//draw the health bar
			spriteBatch.Draw(hpBarImg, hpBarRec, Color.Orange);
		}
	}
}
