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
	class Tower
	{
		//tower data
		protected string name;
		protected string archetype;
		protected int maxHp;
		protected int atk;
		protected int def;
		protected int res;
		protected float redeploy;
		protected float cost;
		protected int block;
		protected float atkSp;
		protected int curHp;

		//rectangles for hitbox healthbar, and healthbar image
		protected Rectangle rect;
		protected Rectangle hpBarRec;
		protected Texture2D hpBarImg;

		//driection faced and tower range
		protected byte direction;
		protected TileNode[] range;

		//to store the enemies: blocked, in range, and attackable 
		protected List<Enemy> enemiesBlocked;
		protected List<Enemy> enemiesInRange;
		protected List<Enemy> enemiesToAttack;

		//animation sheets and data
		protected Texture2D[] animSheets;
		protected int[,] animSizes;
		protected Vector2 animLoc;
		protected Animation temp;
		protected Animation temp2;

		//attack cooldown and range display timer
		protected Timer atkSpTimer;
		protected Timer displayRangeTimer;

		/// <summary>
		/// towers are what the player places to defend the objective from the enemy, they attack and are attacked by the enemies
		/// towers may serve a wide range of purposes and functionalities
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
		public Tower (string name, string archetype, float[] stats, Texture2D[] animSheets, int[,] animSizes, Texture2D hpBarImg, byte direction, TileNode[,] nodeMap, int row, int col)
		{
			//get the stats for the tower
			this.name = name;
			this.archetype = archetype;
			maxHp = (int)stats[0];
			atk = (int)stats[1];
			def = (int)stats[2];
			res = (int)stats[3];
			redeploy = stats[4];
			cost = stats[5];
			block = (int)stats[6];
			atkSp = stats[7];
			curHp = maxHp;

			//to store the enemies: blocked (based on block), in range, and attackable 
			enemiesBlocked = new List<Enemy>(block);
			enemiesInRange = new List<Enemy>();
			enemiesToAttack = new List<Enemy>();

			//gets the direction the tower is to face
			this.direction = direction;

			//rect for testing for the nodes in the range of the tower
			Rectangle testRange;

			//get the test range based on direction
			if (direction == Game1.UP)
			{
				//get the test range
				testRange = new Rectangle(col * Game1.UNIT + 1, (row - 1) * Game1.UNIT + 1, Game1.UNIT / 2, Game1.UNIT / 2);
			}
			else if (direction == Game1.RIGHT)
			{
				//get the test range
				testRange = new Rectangle((col + 1) * Game1.UNIT, row * Game1.UNIT + 1, Game1.UNIT / 2, Game1.UNIT / 2);
			}
			else if (direction == Game1.DOWN)
			{
				//get the test range
				testRange = new Rectangle(col * Game1.UNIT + 1, (row + 1) * Game1.UNIT, Game1.UNIT / 2, Game1.UNIT / 2);
			}
			else
			{
				//get the test range
				testRange = new Rectangle((col - 1) * Game1.UNIT + 1, row * Game1.UNIT + 1, Game1.UNIT / 2, Game1.UNIT / 2);
			}

			//get the nodes in the test range
			range = GetNodesInRange(testRange, nodeMap);

			//store the rectangle of the tower
			rect = new Rectangle(col * Game1.UNIT + Game1.UNIT / 4, row * Game1.UNIT + Game1.UNIT / 4, Game1.UNIT / 2, Game1.UNIT / 2);
			hpBarRec = new Rectangle(rect.X, rect.Bottom, Game1.UNIT / 2, Game1.UNIT / 16);

			//get the animation info and images, and store them
			this.animSheets = animSheets;
			this.animSizes = animSizes;
			this.hpBarImg = hpBarImg;
			animLoc = rect.Location.ToVector2();
			animLoc.X -= animSheets[0].Width / 25;
			animLoc.Y -= animSheets[0].Height / 18;
			temp = new Animation(animSheets[0], animSizes[0, 0], animSizes[0, 1], animSizes[0, 2], 0, 21, 1, 500, animLoc, 0.5f, true);
			temp2 = new Animation(animSheets[2], animSizes[2, 0], animSizes[2, 1], animSizes[2, 2], 16, 21, Animation.ANIMATE_FOREVER, (int)(atkSp * 1000), animLoc, 0.5f, true);

			//timers for attack cooldown and displaying range
			atkSpTimer = new Timer(atkSp * 1000, true);
			displayRangeTimer = new Timer(5000, true);
		}

		//PRE: gameTimer for game time
		//POST: 
		//DESC: update the tower
		public virtual void Update(GameTime gameTime)
		{
			//update the range displaying timer
			displayRangeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//if the animation is not animating, remake it
			if (!temp.IsAnimating())
			{
				//get the new animation
				temp = new Animation(animSheets[1], animSizes[1, 0], animSizes[1, 1], animSizes[1, 2], 0, 21, Animation.ANIMATE_FOREVER, 500, animLoc, 0.5f, true);
			}

			//play the idle animation if no enemies to attack
			if (enemiesToAttack.Count > 0)
			{
				//update the animation and attack timer
				temp2.Update(gameTime);
				atkSpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

				//activate the attack timer
				atkSpTimer.Activate();

				//if the attack timer is finished, attack
				if (atkSpTimer.IsFinished())
				{
					//attack and reset the timer
					Attack();
					atkSpTimer.ResetTimer(true);
				}
			}
			else
			{
				//update the animation and reset the attack timer
				temp.Update(gameTime);
				atkSpTimer.ResetTimer(false);
			}
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw the tower
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			//flip the animation based on direction
			if (direction == 0 || direction == 1)
			{
				//draw based on if there are enemies
				if (enemiesToAttack.Count > 0)
				{
					//draw the tower
					temp2.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
				}
				else
				{
					//draw the tower
					temp.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
				}
			}
			else
			{
				//draw based on if there are enemies
				if (enemiesToAttack.Count > 0)
				{
					//draw the tower
					temp2.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
				}
				else
				{
					//draw the tower
					temp.Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
				}
			}

			//display the range temporarily
			if (!displayRangeTimer.IsFinished())
			{
				//highlight the tiles in range
				foreach (TileNode i in range)
				{
					//highlight a tile in range
					spriteBatch.Draw(hpBarImg, i.rect, Color.LightGray * 0.25f);
				}
			}

			//draw the hp bar
			spriteBatch.Draw(hpBarImg, hpBarRec, Color.CornflowerBlue);
		}

		//PRE: range rep. by rectangle, map of the game
		//POST: list of tiles in range
		//DESC: get nodes in rectangle range
		protected TileNode[] GetNodesInRange(Rectangle range, TileNode[,] nodeMap)
		{
			//the nodes in the test range
			List<TileNode> result = new List<TileNode>();

			//go though all the nodes and determine nodes in range
			foreach (TileNode i in nodeMap)
			{
				//if the node intersects the rectangle, add it
				if (i.rect.Intersects(range))
				{
					//add the node
					result.Add(i);
				}
			}

			//return the nodes in the test range
			return result.ToArray();
		}

		//PRE: 2 nodes lists
		//POST: combined node list
		//DESC: join 2 node arrays together
		protected TileNode[] JoinNodeArrays(TileNode[] array1, TileNode[] array2)
		{
			//the combined arrays
			List<TileNode> result = new List<TileNode>();

			//add the first array to the result
			result = array1.ToList();

			//add non duplicate nodes to the result from the second array
			foreach (TileNode i in array2)
			{
				//add if not dupe
				if (!result.Contains(i))
				{
					//add it
					result.Add(i);
				}
			}

			//return the combined arrays
			return result.ToArray();
		}

		//PRE: 
		//POST: 
		//DESC: deal damage to the enemy
		public virtual void Attack()
		{
			//deal damage to the enemy (non-magic)
			enemiesToAttack[0].TakeDmg(atk, false);
		}

		//PRE: damage to deal, if the attack is magic or not
		//POST:
		//DESC: deal damage to the tower, lowered by def and res
		public void TakeDmg(int dmg, bool isMagic)
		{
			//take the damage minus the defense, minimum 5%, or subtract magic damage by res as a percent, if the "damage" is not healing
			if (dmg > 0)
			{
				//deal damage based on if it is magic or not
				if (!isMagic)
				{
					//take the damage minus the defense, minimum 5%
					if (dmg - def < dmg * 0.05)
					{
						//damage is 5%
						dmg = (int)(dmg * 0.05);
					}
					else
					{
						//reduced damage
						dmg -= def;
					}
				}
				else
				{
					//subtract magic damage by res as a percent
					dmg = (int)(dmg * (100f - res) / 100f);
				}
			}

			//subtract the hp
			curHp -= dmg;

			//if overhealed, set health to max health
			if (curHp > maxHp)
			{
				curHp = maxHp;
			}

			//change the hp bar size 
			hpBarRec.Width = (int)((double)rect.Width * ((double)curHp / (double)maxHp));
		}

		//PRE: 
		//POST: return the int of hp
		//DESC: 
		public int GetHp()
		{
			//get the current hp
			return curHp;
		}

		//PRE: 
		//POST: return the int of max hp
		//DESC: 
		public int GetMaxHp()
		{
			//get the max hp
			return maxHp;
		}

		//PRE: 
		//POST: return the float of the respawn time
		//DESC: 
		public float GetRespawn()
		{
			//get redeploy time
			return redeploy;
		}

		//PRE: 
		//POST: return the name in a string
		//DESC: 
		public string GetName()
		{
			//return the name
			return name;
		}

		//PRE: 
		//POST: list of blocked enemies
		//DESC: 
		public List<Enemy> GetBlocked()
		{
			//get blocked enemies
			return enemiesBlocked;
		}

		//PRE: enemy to be added
		//POST: 
		//DESC: add enemy to blocked list
		public void AddBlocked(Enemy enemy)
		{
			//add an enemy to the blocked lit
			enemiesBlocked.Add(enemy);

			//remove from attack list if possible
			if (enemiesToAttack.Contains(enemy))
			{
				//remove
				enemiesToAttack.Remove(enemy);
			}

			//add it back, making it the higest priority
			enemiesToAttack.Insert(0, enemy);
		}

		//PRE: enemy to be removed
		//POST: 
		//DESC: remove enemy from blocked list
		public void RemoveBlocked(Enemy enemy)
		{
			//if the enemy is found, remove it
			for (int i = 0; i < enemiesBlocked.Count; i++)
			{
				//if the enemy is found, remove it
				if (enemiesBlocked[i] == enemy)
				{
					//remove the enemy
					enemiesBlocked.Remove(enemy);
					enemiesToAttack.Remove(enemy);
					break;
				}
			}
		}

		//PRE: enemy to add
		//POST: 
		//DESC: add the specified enemy to the attackable list
		public void AddToAttackable(Enemy enemy)
		{
			//add the enemy to the attackable list
			enemiesToAttack.Add(enemy);
		}

		//PRE: enemy to remove
		//POST: 
		//DESC: remove specified enemy from attackable
		public void RemoveFromAttackable(Enemy enemy)
		{
			//remove the enemy from the attackable list
			enemiesToAttack.Remove(enemy);
		}

		//PRE: 
		//POST: return the list of attackable enemies
		//DESC: get attackable enemies
		public List<Enemy> GetAttackable()
		{
			//get attackable enemies
			return enemiesToAttack;
		}

		//PRE: 
		//POST: node array of the range
		//DESC: return the nodes making up the range
		public TileNode[] GetRange()
		{
			//retun range
			return range;
		}

		//PRE: 
		//POST: bool saying if the character is at max block
		//DESC: retuns if the tower is at max block
		public bool IsMaxBlock()
		{
			//if the enemies blcoked count is equal to the max block, return true
			if (enemiesBlocked.Count >= block)
			{
				//return true
				return true;
			}

			//return not max
			return false;
		}

		//PRE: 
		//POST: 
		//DESC: some towers will have special effects on death, or need to clean up some data
		public virtual void DieEffects()
		{

		}

		//PRE: 
		//POST: the rectangle
		//DESC: get the rectangle
		public Rectangle Rect()
		{
			//get the rectangle
			return rect;
		}

		//PRE: 
		//POST: string of the archetype
		//DESC: return the archetype
		public string Archetype()
		{
			//get the archetype
			return archetype;
		}
	}
}
