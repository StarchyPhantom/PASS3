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
	class Projectile
	{
		//Vector2 origin;
		Vector2 loc;
		Vector2 destination;

		//speed
		Vector2 speed;

		//rectangle
		Rectangle rect;

		//damage
		int atk;

		/// <summary>
		/// projectile moving from one place to another
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="destination"></param>
		/// <param name="atk"></param>
		public Projectile(Vector2 origin, Vector2 destination, int atk)
		{
			//set speed, rec, and loc
			speed = (destination - origin) / 90;
			rect = new Rectangle((int)origin.X - 5, (int)origin.Y - 5, 10, 10);
			loc = origin;

			//get destination
			this.destination = destination;

			//get attack
			this.atk = atk;
		}

		//PRE: 
		//POST: 
		//DESC: update the projectile
		public void Update()
		{
			//move it move it
			loc += speed;
			rect.Location = loc.ToPoint();
		}

		//PRE: 
		//POST: return rect
		//DESC: 
		public Rectangle Rect()
		{
			//return rect
			return rect;
		}

		//PRE: 
		//POST: return destination point
		//DESC: 
		public Point DestPoint()
		{
			//return point
			return destination.ToPoint();
		}

		//PRE: 
		//POST: return int damage
		//DESC: 
		public int Damage()
		{
			//return damage
			return atk;
		}
	}
}
