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
	class EnemyQueue
	{
		//the internal list and the size of it
		List<Enemy> enemyList;
		int size;

		/// <summary>
		/// queue holding enemies, differs slightly from a traditional queue
		/// </summary>
		public EnemyQueue ()
		{
			//set the list and size
			enemyList = new List<Enemy>();
			size = 0;
		}

		//PRE: enemy to add
		//POST: 
		//DESC: add the enemy
		public void Enqueue(Enemy enemy)
		{
			//add enemy, increase size
			enemyList.Add(enemy);
			size++;
		}

		//PRE: enemy to remove
		//POST: enemy
		//DESC: remove enemy and return it
		public Enemy Dequeue()
		{
			//return null if empty
			if (IsEmpty())
			{
				return null;
			}

			//return enemy, lower size, remove enemy
			Enemy temp = enemyList[0];
			enemyList.RemoveAt(0);
			size--;

			//return enemy
			return temp;
		}

		//PRE: index to peek at
		//POST: 
		//DESC: special, take a peek anywhere
		public Enemy Peek(int index)
		{
			//enemy peeked
			return enemyList[index];
		}

		//PRE: 
		//POST: num elements in queue
		//DESC: 
		public int Size()
		{
			//num elements
			return size;
		}

		//PRE: 
		//POST: bool saying if empty or not
		//DESC: 
		public bool IsEmpty()
		{
			//if not empty, return false
			if (size != 0)
			{
				return false;
			}

			//is empty? retun true
			return true;
		}

		//PRE: 
		//POST: 
		//DESC: change the paths of all enemies inside
		public void ChangePath(List<TileNode> path)
		{
			//change the enemies paths
			foreach (Enemy i in enemyList)
			{
				//change the enemy's path
				i.ChangePath(path);
			}
		}
	}
}
