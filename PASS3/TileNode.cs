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
	class TileNode
	{
		//cost values
		public float f = 0f;
		public float g = 0f;
		public float h = 0f;
		
		//parent tile
		public TileNode parent = null;

		//id, location and type
		public int id;
		public int row;
		public int col;
		public byte tileType;

		//adjacent tiles
		public List<TileNode> adjacents = new List<TileNode>();

		//rectangle and tile image
		public Rectangle rect;
		public Texture2D img;

		/// <summary>
		/// a terrain tile, enemies pathfind on these
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="tileType"></param>
		/// <param name="img"></param>
		/// <param name="mapSize"></param>
		public TileNode (int row, int col, byte tileType, Texture2D img, Vector2 mapSize)
		{
			//get the row
			this.row = row;
			this.col = col;

			//get the id
			id = row * (int)mapSize.Y + col;

			//get the type and image
			this.tileType = tileType;
			this.img = img;

			//create the rectangle
			rect = new Rectangle(col * Game1.UNIT, row * Game1.UNIT, Game1.UNIT, Game1.UNIT);
		}

		//PRE: type to set to
		//POST: 
		//DESC: sets tile to a type
		public void SetTileType(byte type)
		{
			tileType = type;
		}

		//PRE: tile map of the game
		//POST: 
		//DESC: stes adjacents to the tile
		public void SetAdjacents(TileNode[,] map)
		{
			//clear current adjacents
			adjacents.Clear();

			//loop through row and col until adjacents are found
			for (int i = row - 1; i <= row + 1; i++)
			{
				for (int j = col - 1; j <= col + 1; j++)
				{
					//must be N/E/S/W to the tile
					if (tileType != 2 && ((row == i && col != j) || (row != i && col == j)))
					{
						//do not add blocks
						if (i >= 0 && i < Game1.COL && 
							j >= 0 && j < Game1.ROW && 
							map[i, j].tileType != 2)
						{
							//add tile
							adjacents.Add(map[i, j]);
						}
					}
				}
			}
		}

		//PRE: list of towers
		//POST: bool saying if there's a tower or not
		//DESC: sees if there's a toer on it or not
		public bool HasTower(List<Tower> towers)
		{
			//loop through towers
			foreach (Tower i in towers)
			{
				//if there is a tower, true
				if (i.Rect().Intersects(rect))
				{
					//true
					return true;
				}
			}

			//no tower!
			return false;
		}
	}
}
