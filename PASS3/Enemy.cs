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
	class Enemy
	{
		//store the enemy's stats: hp, defense, magic resistance, damage, attack speed, walk speed
		protected int maxHp;
		protected int hp;
		protected int def;
		protected int res;
		protected int atk;
		protected float atkSp;
		protected float maxSpeed = 1;
		protected float speed = 1;

		//bool indicating if the enemy can be attacked by ranged attacks or not
		protected bool isHidden = false;

		//location, rectangle, hp rectangle
		protected Vector2 loc;
		protected Rectangle rect;
		protected Rectangle hpBarRec;

		//path forwards
		protected NodeStack forwardPath;

		//images of the enemy
		protected Texture2D[] imgs;
		protected Texture2D hpBarImg;

		//next node to be travelled to
		protected TileNode curNode;

		//is the enemy blocked or not
		protected bool isBlocked = false;
		protected Tower blocker = null;

		//attack cooldown timer and walk "animation" timer
		protected Timer attackTimer;
		protected Timer walkTimer;

		//timers for stunning, slowing, and hiding the enemy
		protected Timer stunTimer;
		protected Timer slowTimer;
		protected Timer hideTimer;

		/// <summary>
		/// an enemy that walks along the given path, can be blocked and damaged by towers, can deal damage to towers, 
		/// will subtract the player's health if it walks through the exit
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <param name="path"></param>
		/// <param name="imgs"></param>
		/// <param name="hpBarImg"></param>
		public Enemy (int row, int col, List<TileNode> path, Texture2D[] imgs, Texture2D hpBarImg)
		{
			//enemy's stats
			maxHp = 3000;
			hp = maxHp;
			def = 100;
			res = 0;
			atk = 300;
			atkSp = 1;
			maxSpeed = 1;
			speed = maxSpeed;

			//create location and rectangles
			loc = new Vector2(col * Game1.UNIT + 9, row * Game1.UNIT + 9);
			rect = new Rectangle((int)loc.X, (int)loc.Y, 60, 60);
			hpBarRec = new Rectangle(rect.X, rect.Bottom, 60, 5);

			//create the path forwards
			forwardPath = new NodeStack();

			//add the path forwards in
			for (int i = path.Count - 1; i >= 0; i--)
			{
				//add the node
				forwardPath.Push(path[i]);
			}

			//get the images 
			this.imgs = imgs;
			this.hpBarImg = hpBarImg;

			//get the first node forwards
			curNode = forwardPath.Pop();

			//set the attack cooldown timer, and the walking cycle timer
			attackTimer = new Timer(1000 * atkSp, false);
			walkTimer = new Timer(1000, true);

			//set the stun, slow, hidden effect timers
			stunTimer = new Timer(1000, false);
			slowTimer = new Timer(1000, false);
			hideTimer = new Timer(500, false);
		}

		//PRE: gameTime for game's time
		//POST: 
		//DESC: update the enemy
		public virtual void Update(GameTime gameTime)
		{
			//update the timers
			UpdateTimers(gameTime);

			//do not do anything if stunned
			if (!stunTimer.IsActive())
			{
				//move if not blocked, attack if blocked
				if (!isBlocked)
				{
					//move and reset the attack timer
					Move();
					attackTimer.ResetTimer(false);
				}
				else
				{
					//update the attack timer and avtivate it
					attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
					attackTimer.Activate();

					//attack if timer is finished
					if (attackTimer.IsFinished())
					{
						//reset the timer and attack
						attackTimer.ResetTimer(true);
						Attack();
					}
				}
			}
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw the enemy
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			//draw the enemy depending on if it's attacking
			if (attackTimer.IsActive())
			{
				//draw the enemy
				spriteBatch.Draw(imgs[0], rect, Color.White);
			}
			else
			{
				//draw one of the enemy's walking frames
				if (walkTimer.GetTimeRemainingInt() > 250)
				{
					spriteBatch.Draw(imgs[1], rect, Color.White);
				}
				else
				{
					spriteBatch.Draw(imgs[2], rect, Color.White);
				}
			}

			//draw the hp bar
			spriteBatch.Draw(hpBarImg, hpBarRec, Color.Orange);
		}

		//PRE: 
		//POST: 
		//DESC: move the enemy
		public virtual void Move()
		{
			//move the enemy based on the location of it and the next node
			if (rect.Center.X < curNode.rect.Center.X)
			{
				//move the enemy right
				loc.X += speed;
				rect.X = (int)loc.X;
				hpBarRec.X = (int)loc.X;

				//if the enemy passes right of the center of the target node, set the current node to the next node
				if (rect.Center.X >= curNode.rect.Center.X && rect.Center.Y >= curNode.rect.Center.Y && rect.Center.Y <= curNode.rect.Center.Y)
				{
					//set the current node to the next
					SetCurNode();
				}
			}
			else if (rect.Center.X > curNode.rect.Center.X)
			{
				//move the enemy left
				loc.X -= speed;
				rect.X = (int)loc.X;
				hpBarRec.X = (int)loc.X;

				//if the enemy passes left of the center of the target node, set the current node to the next node
				if (rect.Center.X <= curNode.rect.Center.X && rect.Center.Y >= curNode.rect.Center.Y && rect.Center.Y <= curNode.rect.Center.Y)
				{
					//set the current node to the next
					SetCurNode();
				}
			}
			else if (rect.Center.Y < curNode.rect.Center.Y)
			{
				//move the enemy down
				loc.Y += speed;
				rect.Y = (int)loc.Y;
				hpBarRec.Y = (int)(loc.Y + rect.Height);

				//if the enemy passes down of the center of the target node, set the current node to the next node
				if (rect.Center.Y >= curNode.rect.Center.Y && rect.Center.X >= curNode.rect.Center.X && rect.Center.X <= curNode.rect.Center.X)
				{
					//set the current node to the next
					SetCurNode();
				}
			}
			else if (rect.Center.Y > curNode.rect.Center.Y)
			{
				//move the enemy up
				loc.Y -= speed;
				rect.Y = (int)loc.Y;
				hpBarRec.Y = (int)(loc.Y + rect.Height);

				//if the enemy passes up of the center of the target node, set the current node to the next node
				if (rect.Center.Y <= curNode.rect.Center.Y && rect.Center.X >= curNode.rect.Center.X && rect.Center.X <= curNode.rect.Center.X)
				{
					//set the current node to the next
					SetCurNode();
				}
			}
		}

		//PRE: gametime for the timers
		//POST: 
		//DESC: update important timers
		protected void UpdateTimers(GameTime gameTime)
		{
			//update the stun timer, slow timer, and walk timer
			stunTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			slowTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			hideTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
			walkTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			//reset statuses if timer done
			if (slowTimer.IsFinished())
			{
				speed = maxSpeed;
			}
			else if (hideTimer.IsFinished())
			{
				isHidden = false;
			}

			//reset walking "animation"
			if (walkTimer.IsFinished())
			{
				walkTimer.ResetTimer(true);
			}
		}

		//PRE: 
		//POST: 
		//DESC: set the next node
		private void SetCurNode()
		{
			//if no more node to walk over, set to null, otherwise, set to next node
			if (!forwardPath.IsEmpty())
			{
				//next node
				curNode = forwardPath.Pop();
			}
			else
			{
				//set to null 
				curNode = null;
			}
		}

		//PRE: new path replaing old path
		//POST: 
		//DESC: change the path the enemy is taking
		public void ChangePath(List<TileNode> newPath)
		{
			//reset the forward path
			forwardPath = new NodeStack();

			//push in the new path
			for (int i = newPath.Count - 1; i >= 0; i--)
			{
				//push
				forwardPath.Push(newPath[i]);
			}

			//set next node to next node
			curNode = forwardPath.Pop();
		}

		//PRE: 
		//POST: 
		//DESC: attack the blocker
		protected virtual void Attack()
		{
			//attack the blocker
			blocker.TakeDmg(atk, false);
		}

		//PRE: 
		//POST: 
		//DESC: stun the enemy
		public void Stun()
		{
			//turn on the stun timer
			stunTimer.ResetTimer(true);
		}

		//PRE: 
		//POST: 
		//DESC: slow the enemy
		public void Slow(float magnitude)
		{
			//replace the faster speed with the slower one
			if (speed > maxSpeed * magnitude)
			{
				speed = maxSpeed * magnitude;
			}

			//turn the timer on
			slowTimer.ResetTimer(true);
		}

		//PRE: 
		//POST: 
		//DESC: hide the enemy
		public void Hide()
		{
			//make the enemy hidden and turn the timer on
			isHidden = true;
			hideTimer.ResetTimer(true);
		}

		//PRE: 
		//POST: 
		//DESC: make the enemy unblocked
		public void ToggleBlocked()
		{
			//unblock if possible
			if (isBlocked)
			{
				//null
				blocker = null;
				isBlocked = false;
			}
		}

		//PRE: tower blocking
		//POST: 
		//DESC: blcok with blcoker
		public void ToggleBlocked(Tower blocker)
		{
			//set the blocker
			this.blocker = blocker;
			isBlocked = true;
		}

		//PRE: 
		//POST: get the blocker
		//DESC: 
		public Tower Blocker()
		{
			//get the blocker
			return blocker;
		}

		//PRE: 
		//POST: get if it is blocker
		//DESC: 
		public bool IsBlocked()
		{
			//return if it is blocked
			return isBlocked;
		}

		//PRE: 
		//POST: return if it is hidden
		//DESC: 
		public bool IsHidden()
		{
			//return if it is hidden
			return isHidden;
		}

		//PRE: damage to be dealt and if it is a magic attack
		//POST: 
		//DESC: take damage
		public void TakeDmg(int dmg, bool isMagic)
		{
			//take the damage minus the defense, minimum 5%, or subtract magic damage by res as a percent
			if (!isMagic)
			{
				//min 5% or substrct
				if (dmg - def < dmg * 0.05)
				{
					//min 5%
					dmg = (int)(dmg * 0.05);
				}
				else
				{
					//damage minus def
					dmg -= def;
				}
			}
			else
			{
				//percent resisted
				dmg = (int)(dmg * (100f - res) / 100f);
			}

			//take damage and adjust hp bar size
			hp -= dmg;
			hpBarRec.Width = (int)((double)rect.Width * ((double)hp / (double)maxHp));
		}

		//PRE: 
		//POST: boolean indicating if it is to be deleted or not
		//DESC: returns if the mob is to be deleted or not
		public virtual bool IsFinished()
		{
			//return to be deleted if dead or at the exit
			if (hp <= 0)
			{
				//award the kill
				Game1.currentPlayerKills++;
				return true;
			}
			else if (curNode == null && forwardPath.IsEmpty())
			{
				//subtract life
				Game1.health--;
				return true;
			}

			//not finished
			return false;
		}

		//PRE: 
		//POST: return rectangle
		//DESC: 
		public Rectangle Rect()
		{
			//return rectangle
			return rect;
		}

		//PRE: 
		//POST: return image
		//DESC: 
		public Texture2D Img()
		{
			//return image
			return imgs[0];
		}
	}
}
