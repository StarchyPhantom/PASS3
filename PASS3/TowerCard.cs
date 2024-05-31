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
	class TowerCard
	{
		//carc stats
		string name;
		string archetype;
		int type;
		float[] stats;

		//card rectangle
		Rectangle cardRect;

		//text locs
		Vector2 nameLoc;
		Vector2 statsLoc1;
		Vector2 statsLoc2;

		//display the stats or not
		bool displayStats = false;

		//card animation
		Animation cardAnim;

		//fonts
		SpriteFont regFont;
		SpriteFont titleFont;

		//background img and rect
		Texture2D bgImg;
		Rectangle bgRect;

		//the colour
		Color colour;

		/// <summary>
		/// allows selection of tower and display of stats
		/// </summary>
		/// <param name="name"></param>
		/// <param name="archetype"></param>
		/// <param name="stats"></param>
		/// <param name="animSheet"></param>
		/// <param name="animSize"></param>
		/// <param name="regFont"></param>
		/// <param name="titleFont"></param>
		/// <param name="bgImg"></param>
		/// <param name="bgRect"></param>
		/// <param name="row"></param>
		/// <param name="col"></param>
		public TowerCard (string name, string archetype, float[] stats, Texture2D animSheet, int[] animSize, SpriteFont regFont, SpriteFont titleFont, Texture2D bgImg, Rectangle bgRect, int row, int col)
		{
			//get name and archetype
			this.name = name;
			this.archetype = archetype;

			//find type
			if (Caster.GetChildren().Contains(archetype))
			{
				type = Game1.CASTER;
			}
			else if (Defender.GetChildren().Contains(archetype))
			{
				type = Game1.DEFENDER;
			}
			else if (Guard.GetChildren().Contains(archetype))
			{
				type = Game1.GUARD;
			}
			else if (Sniper.GetChildren().Contains(archetype))
			{
				type = Game1.SNIPER;
			}
			else if (Medic.GetChildren().Contains(archetype))
			{
				type = Game1.MEDIC;
			}
			else if (Vanguard.GetChildren().Contains(archetype))
			{
				type = Game1.VANGUARD;
			}
			else if (Specialist.GetChildren().Contains(archetype))
			{
				type = Game1.SPECIALIST;
			}
			else if (Supporter.GetChildren().Contains(archetype))
			{
				type = Game1.SUPPORTER;
			}
			else
			{
				//invalid type
				type = -1;
			}

			//get stats
			this.stats = stats;

			//cardrect and it's anim
			cardRect = new Rectangle(col * Game1.UNIT, row * Game1.UNIT, Game1.UNIT, Game1.UNIT);
			cardAnim = new Animation(animSheet, animSize[0], animSize[1], animSize[2], 0, 0, Animation.ANIMATE_FOREVER, 500, cardRect.Location.ToVector2(), 0.2f, true);
			
			//get fonts
			this.regFont = regFont;
			this.titleFont = titleFont;

			//get bg image and rec
			this.bgImg = bgImg;
			this.bgRect = bgRect;

			//make locations
			nameLoc = new Vector2(Game1.UNIT / 8, Game1.UNIT / 8);
			statsLoc1 = new Vector2(Game1.UNIT / 2, Game1.UNIT * 2);
			statsLoc2 = new Vector2((int)(Game1.UNIT * 2.5), Game1.UNIT * 5);

			//colour is white
			colour = Color.White;
		}

		//PRE: 
		//POST: 
		//DESC: update the towercard
		public void Update(GameTime gameTime, MouseState mouse, MouseState prevMouse)
		{
			//update the card animation
			cardAnim.Update(gameTime);

			//display stats if right clicked
			if (cardRect.Contains(mouse.Position))
			{
				if (mouse.RightButton == ButtonState.Released && prevMouse.RightButton == ButtonState.Pressed)
				{
					displayStats = true;
				}
			}

			//stop displaying if clicked off
			if (!cardRect.Contains(mouse.Position) && (mouse.LeftButton == ButtonState.Pressed || mouse.RightButton == ButtonState.Pressed))
			{
				displayStats = false;
			}
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw card info
		public void Draw(SpriteBatch spriteBatch)
		{
			//draw the card
			cardAnim.Draw(spriteBatch, colour);

			//disp;ay the stats if the stats are to be displayed
			if (displayStats == true)
			{
				spriteBatch.Draw(bgImg, bgRect, Color.White * 0.75f);
				spriteBatch.DrawString(regFont, name, nameLoc, Color.White);
				spriteBatch.DrawString(regFont, "      Health " + stats[0] + "\n" +
												"      Attack " + stats[1] + "\n" +
												"    Defense " + stats[2] + "\n" +
												"Resistance " + stats[3], statsLoc1, Color.White);
				spriteBatch.DrawString(regFont, "Redeploy " + stats[4] + "\n" +
												"       Cost " + stats[5] + "\n" +
												"     Block " + stats[6] + "\n" +
												"     Speed " + stats[7], statsLoc2, Color.White);
			}
		}

		//PRE: status of the tower
		//POST: 
		//DESC: change the colour based on tower status
		public void SetColour(byte status)
		{
			//downed = red, expensive/deployed = grey, available = white
			if (status == 0)
			{
				colour = Color.White;
			}
			else if (status == 1)
			{
				colour = Color.Gray;
			}
			else
			{
				colour = Color.Red;
			}
		}

		//PRE: 
		//POST: get the name in a string
		//DESC: 
		public string Name()
		{
			//return name
			return name;
		}

		//PRE: 
		//POST: get the rectangle
		//DESC: 
		public Rectangle Rect()
		{
			//get the rectangle
			return cardRect;
		}

		//PRE: location to go
		//POST: 
		//DESC: move the card elsewhere
		public void Move(int row, int col)
		{
			//move the card
			cardRect.X = col * Game1.UNIT;
			cardRect.Y = row * Game1.UNIT;
			cardAnim.TranslateTo(cardRect.X, cardRect.Y);
		}

		//PRE: 
		//POST: return type as int
		//DESC: 
		public int Type()
		{
			//return tower type
			return type;
		}

		//PRE: 
		//POST: return int cost of tower
		//DESC: 
		public int Cost()
		{
			//return cost
			return (int)stats[5];
		}
	}
}
