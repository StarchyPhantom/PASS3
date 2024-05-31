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
	class Level
	{
		//max rounds and unit
		const byte MAX_ROUNDS = Game1.MAX_ROUNDS;
		const byte UNIT = Game1.UNIT;

		//tile constants
		public const byte SPAWN = 0;
		public const byte ENDPOINT = 1;
		public const byte BLOCK = 2;
		public const byte PATH = 3;
		public const byte ROOF = 4;
		public const byte WATER = 5;
		public const byte LAVA = 6;

		//enemy constants
		const byte SLUG = 0;
		const byte CROSSBOW = 1;
		const byte GHOST = 2;
		const byte ARTIST = 3;
		const byte LUCIEN = 4;

		//max spawn time
		const int MAX_ENEMY_SPAWN_TIME = 8000;

		//screen width and screen height, and bvackground images
		int screenHeight;
		int screenWidth;
		Texture2D[] bgImgs;

		//current map used
		byte[,] curMap;

		//maps to be used
		List<byte[,]> maps = new List<byte[,]>{ new byte[,]{{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, ENDPOINT},
									{BLOCK, BLOCK, BLOCK, PATH, BLOCK, PATH, PATH, PATH, PATH, PATH, PATH, PATH, PATH, PATH},
									{BLOCK, BLOCK, BLOCK, PATH, BLOCK, BLOCK, BLOCK, PATH, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, PATH},
									{BLOCK, BLOCK, BLOCK, PATH, BLOCK, BLOCK, BLOCK, PATH, PATH, PATH, PATH, PATH, PATH, PATH},
									{SPAWN, PATH, PATH, PATH, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK}},

									new byte[,]{{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, ROOF, ROOF, ROOF, ROOF, ROOF, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{SPAWN, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, ENDPOINT},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, ROOF, ROOF, ROOF, ROOF, ROOF, ROOF, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK}},

									new byte[,]{{SPAWN, PATH, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{PATH, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK},
									{BLOCK, BLOCK, ROOF, ROOF, ROOF, ROOF, LAVA, LAVA, ROOF, ROOF, ROOF, ROOF, BLOCK, BLOCK},
									{BLOCK, BLOCK, ROOF, BLOCK, BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK, BLOCK, ROOF, BLOCK, BLOCK},
									{BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK},
									{BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK},
									{BLOCK, BLOCK, ROOF, BLOCK, BLOCK, BLOCK, LAVA, LAVA, BLOCK, BLOCK, BLOCK, ROOF, BLOCK, BLOCK},
									{BLOCK, BLOCK, ROOF, ROOF, ROOF, ROOF, LAVA, LAVA, ROOF, ROOF, ROOF, ROOF, BLOCK, BLOCK},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, PATH},
									{BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, BLOCK, PATH, ENDPOINT}}};

		//costs of tiles
		float[] tileCosts = new float[] { 0.5f,
										  0.5f,
										  999f,
										  1f,
										  0.5f, 
										  2.5f, 
										  5f};

		//enemies in each level
		int[,] levelEnemies = new int[,] { { 20, 10, 0, 5, 0 }, 
										   { 30, 20, 10, 10, 0 },
										   { 50, 25, 25, 25, 1} };

		//start and end nodes
		TileNode start;
		TileNode end;

		//node map
		TileNode[,] nodeMap = new TileNode[10, 14];

		//main path taken and the undostack
		List<TileNode> mainPath = new List<TileNode>();
		NodeStack undoStack = new NodeStack();

		//open and closed lists
		List<TileNode> open = new List<TileNode>();
		List<TileNode> closed = new List<TileNode>();

		//size of the map as a vector
		Vector2 mapSize = new Vector2(Game1.COL, Game1.ROW);

		//enemy images and hp bar image
		Texture2D[] enemyImgs;
		Texture2D hpBarImg;

		//enemies to spawn and active
		EnemyQueue enemiesToSpawn = new EnemyQueue();
		List<Enemy> enemiesActive = new List<Enemy>();

		//enemy span timer
		Timer enemySpawnTimer;
		int enemySpawnTime = MAX_ENEMY_SPAWN_TIME;

		//enemy queue recs
		Rectangle[] enemyQueueRecs = new Rectangle[3];

		//tower data
		List<string> towerNames;
		List<string> towerTypes;
		List<float[]> towerStats;
		List<Texture2D[]> towerAnimSheets;
		List<int[,]> towerAnimSizes;

		//lists to keep track of towers
		List<string> activeTowerNames = new List<string>();
		List<string> downTowerNames = new List<string>();
		List<Tower> towers = new List<Tower>();
		List<Tower> downTowers = new List<Tower>();
		List<Timer> respawnTimers = new List<Timer>();

		//lists for projectiles
		List<Projectile> friendlyProjectiles = new List<Projectile>();
		List<Projectile> enemyProjectiles = new List<Projectile>();

		//towers in range
		List<List<Tower>> towersInRange = new List<List<Tower>>();

		/// <summary>
		/// the level manager of the game
		/// </summary>
		/// <param name="screenHeight"></param>
		/// <param name="screenWidth"></param>
		/// <param name="bgImgs"></param>
		/// <param name="enemyImgs"></param>
		/// <param name="hpBarImg"></param>
		/// <param name="towerNames"></param>
		/// <param name="towerTypes"></param>
		/// <param name="towerStats"></param>
		/// <param name="towerAnimSheets"></param>
		/// <param name="towerAnimSizes"></param>
		public Level(int screenHeight, int screenWidth,
			Texture2D[] bgImgs,
			Texture2D[] enemyImgs,
			Texture2D hpBarImg,
			List<string> towerNames, List<string> towerTypes, List<float[]> towerStats, 
			List<Texture2D[]> towerAnimSheets, List<int[,]> towerAnimSizes)
		{
			//set data
			this.bgImgs = bgImgs;
			this.enemyImgs = enemyImgs;
			this.hpBarImg = hpBarImg;
			this.screenHeight = screenHeight;
			this.screenWidth = screenWidth;
			this.towerNames = towerNames;
			this.towerTypes = towerTypes;
			this.towerStats = towerStats;
			this.towerAnimSheets = towerAnimSheets;
			this.towerAnimSizes = towerAnimSizes;

			//set spawn timer
			enemySpawnTimer = new Timer(enemySpawnTime, false);

			//set current map
			curMap = maps[Game1.currentRound - 1];

			//set nodemap
			for (int i = 0; i < Game1.COL; i++)
			{
				for (int j = 0; j < Game1.ROW; j++)
				{
					nodeMap[i, j] = new TileNode(i, j, curMap[i, j], bgImgs[curMap[i, j]], mapSize);

					//set start and end
					if (curMap[i, j] == SPAWN)
					{
						start = nodeMap[i, j];
						enemyQueueRecs[0] = new Rectangle(j * UNIT, i * UNIT, UNIT / 2, UNIT / 2);
						enemyQueueRecs[1] = new Rectangle(j * UNIT, i * UNIT + UNIT / 2, UNIT / 2, UNIT / 2);
						enemyQueueRecs[2] = new Rectangle(j * UNIT + UNIT / 2, i * UNIT + UNIT / 2, UNIT / 2, UNIT / 2);
					}
					else if (curMap[i, j] == ENDPOINT)
					{
						end = nodeMap[i, j];
					}
				}
			}

			//redo path
			ReDoPath();
		}

		//PRE: mouse and kb
		//POST: 
		//DESC: update the level pregame
		public void UpdatePrep(MouseState mouse, MouseState prevMouse, KeyboardState kb, KeyboardState prevKb)
		{
			//if a dirt tile is clicked, turn it into a path tile
			if (mouse.LeftButton == ButtonState.Pressed)
			{
				//y
				for (int i = 0; i < Game1.COL; i++)
				{

					//x
					for (int j = 0; j < Game1.ROW; j++)
					{
						//turn dirt into path if valid
						if (nodeMap[i, j].rect.Contains(mouse.Position) && curMap[i, j] == BLOCK)
						{
							//record the change, update the path
							undoStack.Push(nodeMap[i, j]);
							curMap[i, j] = PATH;
							nodeMap[i, j].SetTileType(PATH);
						}
					}
				}
			}

			//undo a path tile back into a dirt tile
			if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.Z) && prevKb.IsKeyUp(Keys.Z) && !undoStack.IsEmpty())
			{
				//pop from the undo stack and place back the tile
				curMap[undoStack.Top().row, undoStack.Top().col] = BLOCK;
				nodeMap[undoStack.Top().row, undoStack.Top().col].SetTileType(BLOCK);
				undoStack.Pop();
			}

			//when the enter key is pressed, recalculate the path and start spawning enemies
			if (kb.IsKeyDown(Keys.Enter))
			{
				//redo the path
				ReDoPath();

				//if path is vaild, spawn enemies
				if (PathIsValid())
				{
					//textures of the enemies
					Texture2D[] slugImgs = new Texture2D[] { enemyImgs[0] };
					Texture2D[] CBowImgs = new Texture2D[] { enemyImgs[1], enemyImgs[2], enemyImgs[3] };
					Texture2D[] ArtImgs = new Texture2D[] { enemyImgs[4], enemyImgs[5], enemyImgs[6] };
					Texture2D[] GhostImgs = new Texture2D[] { enemyImgs[7], enemyImgs[8], enemyImgs[9] };
					Texture2D[] LucImgs = new Texture2D[] { enemyImgs[10], enemyImgs[11], enemyImgs[12] };

					//temp holding list
					List<Enemy> tempEnemyMix = new List<Enemy>();

					///
					///add each enemy to the list
					///
					for (int i = 0; i < levelEnemies[Game1.currentRound - 1, SLUG]; i++)
					{
						tempEnemyMix.Add(new Slug(start.row, start.col, mainPath, slugImgs, hpBarImg));
					}

					for (int i = 0; i < levelEnemies[Game1.currentRound - 1, CROSSBOW]; i++)
					{
						tempEnemyMix.Add(new Crossbow(start.row, start.col, mainPath, CBowImgs, hpBarImg, towers));
					}

					for (int i = 0; i < levelEnemies[Game1.currentRound - 1, ARTIST]; i++)
					{
						tempEnemyMix.Add(new Artist(start.row, start.col, mainPath, ArtImgs, hpBarImg, towers, enemyProjectiles));
					}

					for (int i = 0; i < levelEnemies[Game1.currentRound - 1, GHOST]; i++)
					{
						tempEnemyMix.Add(new Ghost(start.row, start.col, mainPath, GhostImgs, hpBarImg, towers));
					}

					for (int i = 0; i < levelEnemies[Game1.currentRound - 1, LUCIEN]; i++)
					{
						tempEnemyMix.Add(new Lucien(start.row, start.col, mainPath, LucImgs, hpBarImg, enemyProjectiles));
					}

					//random numbers
					Random rng = new Random();

					//randomly add enemies to the queue
					while (tempEnemyMix.Count > 0)
					{
						Enemy temp = tempEnemyMix[rng.Next(0, tempEnemyMix.Count)];
						enemiesToSpawn.Enqueue(temp);
						tempEnemyMix.Remove(temp);
					}

					//reset timer
					enemySpawnTime = MAX_ENEMY_SPAWN_TIME;
					enemySpawnTimer.ResetTimer(true);
				}
			}
		}

		//PRE: gametime for gametime
		//POST: 
		//DESC: update the level
		public void UpdateGame(GameTime gameTime)
		{
			//update the enemy spawning timer
			enemySpawnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

			for (int i = 0; i < respawnTimers.Count; i++)
			{
				//update respawn timers
				respawnTimers[i].Update(gameTime.ElapsedGameTime.TotalMilliseconds);

				//if the respawn timer for a tower is done, make them available to deploy
				if (respawnTimers[i].IsFinished())
				{
					//set to be able to deploy
					downTowers.RemoveAt(i);
					downTowerNames.RemoveAt(i);
					respawnTimers.RemoveAt(i);
					break;
				}
			}

			for (int i = 0; i < towers.Count; i++) 
			{
				//update towers
				towers[i].Update(gameTime);

				//handle the knockout of a tower
				if (towers[i].GetHp() <= 0)
				{
					foreach (Enemy j in towers[i].GetBlocked())
					{
						//unblock the blocked enemies
						j.ToggleBlocked();
					}

					//do any special actions the tower might have at death
					towers[i].DieEffects();

					foreach (List<Tower> j in towersInRange)
					{
						//remove tower if null
						if (j == null)
						{
							towersInRange.Remove(j);
						}
					}

					//shift the tower to the down list, start respawn
					downTowerNames.Add(towers[i].GetName());
					activeTowerNames.Remove(towers[i].GetName());
					downTowers.Add(towers[i]);
					respawnTimers.Add(new Timer(towers[i].GetRespawn() * 1000, true));
					towers.Remove(towers[i]);
					break;
				}
			}

			//spawn an enemy when the timer is finished and there are still enemies left to spawn
			if (enemySpawnTimer.IsFinished() && enemiesToSpawn.Size() > 0)
			{
				//spawn enemy, reset timer
				enemiesActive.Add(enemiesToSpawn.Dequeue());
				enemySpawnTime = (int)(enemySpawnTime * 0.99);
				enemySpawnTimer.ResetTimer(true, enemySpawnTime);
			}

			foreach (Enemy i in enemiesActive)
			{
				//update or handle the enemy's death
				if (i.IsFinished())
				{
					foreach (Tower j in towers)
					{
						//checks if the enemy was blocked
						if (i.IsBlocked())
						{
							//find the tower blocking the enemy
							if (i.Blocker() == j)
							{
								//remove the no longer blocked enemy
								j.RemoveBlocked(i);
							}
						}

						//remove from attackable if was in attackable
						if (j.GetAttackable().Contains(i))
						{
							j.RemoveFromAttackable(i);
						}
					}

					//remove the enemy
					enemiesActive.Remove(i);
					break;
				}
				else
				{
					//update the enemy
					i.Update(gameTime);
				}

				foreach (TileNode j in mainPath)
				{
					//do effect based on tile type
					if (i.Rect().Intersects(j.rect))
					{
						if (j.tileType == LAVA)
						{
							//deal damage to enemy
							i.TakeDmg(5, true);
						}
						else if (j.tileType == WATER)
						{
							//slow the enemy
							i.Slow(0.6f);
						}
						else if (j.tileType == ROOF)
						{
							//hide the enmy
							i.Hide();
						}
					}
				}

				foreach (Tower j in towers)
				{
					//if the enemy and tower intersect, handle blocking
					if (i.Rect().Intersects(j.Rect()))
					{
						//only block the enemy if the enemy isn't blocked and the tower does not have max block yet
						if (!i.IsBlocked() && !j.IsMaxBlock())
						{
							j.AddBlocked(i);
							i.ToggleBlocked(j);
						}
					}

					//if the enemy is in the range of the tower, make the enemy attackable to the tower, or if not in range, unattackable
					if (IsEnemyInRange(i, j) && !j.GetAttackable().Contains(i) && !i.IsHidden())
					{
						//add the enemy to the attackable list
						j.AddToAttackable(i);
					}
					else if (!IsEnemyInRange(i,j) && !j.Rect().Intersects(i.Rect()))
					{
						//remove the enemy from the attackable list
						j.RemoveFromAttackable(i);
					}
				}

				//if a projectile hits an enemy, explode and deal full damage
				foreach (Projectile j in friendlyProjectiles)
				{
					//if the projectile hits an enemy
					if (i.Rect().Intersects(j.Rect()))
					{
						//get rid of the projectile and explode
						friendlyProjectiles.Remove(j);
						Explode(j.DestPoint(), j.Damage(), true);
						break;
					}
				}
			}

			foreach (Projectile i in friendlyProjectiles)
			{
				//update the projectile
				i.Update();

				//if the projectile doesn't hit anything, explode, and deal partial damage
				if (i.Rect().Contains(i.DestPoint()))
				{
					//get rid of the projectile and explode
					friendlyProjectiles.Remove(i);
					Explode(i.DestPoint(), (int)(i.Damage() * 0.9), true);
					break;
				}
			}

			foreach (Projectile i in enemyProjectiles)
			{
				//update the projectile
				i.Update();

				//if the projectile doesn't hit anything, explode, and deal partial damage
				if (i.Rect().Contains(i.DestPoint()))
				{
					//get rid of the projectile and explode
					enemyProjectiles.Remove(i);
					Explode(i.DestPoint(), i.Damage(), false);
					break;
				}
			}
		}

		//PRE: spritebatch to draw
		//POST: 
		//DESC: draw the level
		public void Draw(SpriteBatch spriteBatch)
		{
			//draw map
			for (int i = 0; i * Game1.UNIT < screenHeight - Game1.UNIT; i++)
			{
				for (int j = 0; j * Game1.UNIT < screenWidth; j++)
				{
					spriteBatch.Draw(bgImgs[curMap[i, j]], nodeMap[i, j].rect, Color.White);
				}
			}

			//draw enemies
			foreach (Enemy i in enemiesActive)
			{
				i.Draw(spriteBatch);
			}

			//draw towers
			foreach (Tower i in towers)
			{
				i.Draw(spriteBatch);
			}

			//draw friendly projectiles
			foreach (Projectile i in friendlyProjectiles)
			{
				spriteBatch.Draw(bgImgs[0], i.Rect(), Color.Green);
			}

			//draw enemy projectiles
			foreach (Projectile i in enemyProjectiles)
			{
				spriteBatch.Draw(bgImgs[0], i.Rect(), Color.Purple);
			}

			//draw enemy queue
			for (int i = 0; i < enemyQueueRecs.Length && i < enemiesToSpawn.Size(); i++)
			{
				spriteBatch.Draw(enemiesToSpawn.Peek(i).Img(), enemyQueueRecs[i], Color.White);
			}
		}

		//PRE:
		//POST: a boolean indicating level completion
		//DESC: checks if the level is done or not
		public bool IsLevelDone()
		{
			//says the level is done if there are no mobs left onscreen or to spawn
			if (enemiesToSpawn.Size() == 0 && enemiesActive.Count == 0)
			{
				if (Game1.currentRound < MAX_ROUNDS)
				{
					//new map
					curMap = maps[Game1.currentRound];

					//redo node map
					for (int i = 0; i < Game1.COL; i++)
					{
						for (int j = 0; j < Game1.ROW; j++)
						{
							nodeMap[i, j] = new TileNode(i, j, curMap[i, j], bgImgs[curMap[i, j]], mapSize);

							//set start and end
							if (curMap[i, j] == SPAWN)
							{
								start = nodeMap[i, j];
								enemyQueueRecs[0] = new Rectangle(j * UNIT, i * UNIT, UNIT / 2, UNIT / 2);
								enemyQueueRecs[1] = new Rectangle(j * UNIT + UNIT / 2, i * UNIT, UNIT / 2, UNIT / 2);
								enemyQueueRecs[2] = new Rectangle(j * UNIT + UNIT / 2, i * UNIT + UNIT / 2, UNIT / 2, UNIT / 2);
							}
							else if (curMap[i, j] == ENDPOINT)
							{
								end = nodeMap[i, j];
							}
						}
					}

					//redo path
					ReDoPath();

					//clear all lists
					friendlyProjectiles.Clear();
					enemyProjectiles.Clear();
					activeTowerNames.Clear();
					downTowerNames.Clear();
					towers.Clear();
					downTowers.Clear();
					respawnTimers.Clear();
					towersInRange.Clear();
				}

				//increase round
				Game1.currentRound++;
				return true;
			}

			//not done
			return false;
		}

		//PRE: name, direction, location
		//POST: 
		//DESC: add a tower
		public void AddTower(string name, byte direction, int row, int col)
		{
			//loop through the towers
			for (int i = 0; i < towerNames.Count; i++)
			{
				//if the names match, add the correspoding tower type
				if (name == towerNames[i])
				{
					//find the corresponding tower typeand add a tower
					switch (towerTypes[i])
					{
						case "Marksman":
							towers.Add(new Marksman(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "Swordmaster":
							towers.Add(new Swordmaster(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "Artilleryman":
							towers.Add(new Artilleryman(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col, friendlyProjectiles));
							break;
						case "Medic":
							towersInRange.Add(new List<Tower>());
							towers.Add(new MedicMedic(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col, towersInRange[towersInRange.Count - 1]));
							break;
						case "Ambusher":
							towers.Add(new Ambusher(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "Protector":
							towers.Add(new Protector(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "Dreadnought":
							towers.Add(new Dreadnought(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "DecelBinder":
							towers.Add(new DecelBinder(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "CoreCaster":
							towers.Add(new CoreCaster(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						case "StandardBearer":
							towers.Add(new StandardBearer(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
						default:
							towers.Add(new Tower(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i], towerAnimSizes[i], hpBarImg, direction, nodeMap, row, col));
							break;
					}

					//int tempCount = towers.Count;

					//find towers in the ranges of the medics
					for (int j = 0; j < towers.Count; j++)
					{
						//if the tower is a medic, search for towers in its range
						if (Medic.GetChildren().Contains(towers[j].Archetype()))
						{
							foreach (Tower k in towers)
							{
								foreach (TileNode l in towers[j].GetRange())
								{
									for (int m = 0; m < towersInRange.Count(); m++)
									{
										//if tower is in range add it
										if (l.rect.Intersects(k.Rect()) && !towersInRange[m].Contains(k))
										{
											//add tower
											towersInRange[m].Add(k);
										}
									}
								}
							}
						}
					}

					//inidcate active
					activeTowerNames.Add(name);
				}
			}
		}

		//PRE: the obstacle wanted and its location
		//POST: 
		//DESC: adds an obstacle to the curMap
		public void AddObstacle(byte obstacle, int col, int row)
		{
			//if the obstacle deployed is lava or water, deploy them
			if (obstacle == LAVA)
			{
				//deploy lava on the curMap
				curMap[row, col] = LAVA;
				nodeMap[row, col].SetTileType(LAVA);
			}
			else
			{
				//deploy water on the curMap
				curMap[row, col] = WATER;
				nodeMap[row, col].SetTileType(WATER);
			}

			//recalculate the shortest path
			ReDoPath();

			//change the paths of the enemies
			enemiesToSpawn.ChangePath(mainPath);

			Console.WriteLine(mainPath.Count);
		}

		//PRE: location
		//POST: boolean saying if there is a tower or not
		//DESC: sees if the tile already has a tower
		public bool TileHasTower(int row, int col)
		{
			//if the node has a tower
			return nodeMap[row, col].HasTower(towers);
		}

		//PRE: enemy, tower
		//POST: bool saying inrange or not
		//DESC: detects if an enemy is in the range of a tower
		private bool IsEnemyInRange(Enemy i, Tower j)
		{
			//loop through tiles to see if an enemy is in it
			foreach (TileNode k in j.GetRange())
			{
				if (i.Rect().Intersects(k.rect))
				{
					//enemy in range
					return true;
				}
			}

			//enemy not
			return false;
		}

		//PRE: 
		//POST: list of strings of names active
		//DESC: get the names of the towers active
		public List<string> GetActiveTowerNames()
		{
			//names of towers active
			return activeTowerNames;
		}

		//PRE: 
		//POST: list of strings of names downed
		//DESC: get the names of the towers downed
		public List<string> GetDownTowerNames()
		{
			//names of towers downed
			return downTowerNames;
		}

		//PRE: origin, damage, team
		//POST: 
		//DESC: exlpode and deal damage
		public void Explode(Point origin, int dmg, bool isFriendly)
		{
			//create a rectangle that will check for targets in range
			Rectangle explosion = new Rectangle(origin.X - 120, origin.Y - 120, 240, 240);

			//deal damage based on affiliation
			if (isFriendly)
			{
				foreach (Enemy i in enemiesActive)
				{
					//damage enemy in explosion
					if (explosion.Contains(i.Rect()))
					{
						i.TakeDmg(dmg, true);
					}
				}
			}
			else
			{
				foreach (Tower i in towers)
				{
					//damage tower in explosion
					if (explosion.Contains(i.Rect()))
					{
						i.TakeDmg(dmg, true);
					}
				}
			}
		}

		//PRE: location
		//POST: returns tile type of tile
		//DESC: 
		public byte TileType(int row, int col)
		{
			//return tile type
			return curMap[row, col];
		}

		//PRE: 
		//POST: 
		//DESC: redo the path
		private void ReDoPath()
		{
			//loop through the y axis tiles
			for (int i = 0; i < Game1.COL; i++)
			{
				//loop through the x axis tiles
				for (int j = 0; j < Game1.ROW; j++)
				{
					//reset adjacents
					nodeMap[i, j].SetAdjacents(nodeMap);
					Console.WriteLine("AD"+nodeMap[i, j].adjacents.Count);
				}
			}

			//set new h, find new path
			SetHCost(nodeMap, end.row, end.col);
			mainPath = FindPath(start, end);
		}

		//PRE: 
		//POST: bool saying if the path is valid
		//DESC: 
		public bool PathIsValid()
		{
			//if there is something in main path, then it is valid
			if (mainPath.Count > 0)
			{
				return true;
			}

			//not valid
			return false;
		}

		//PRE: start and end nodes
		//POST: vaild tile node list path
		//DESC: finds a path
		private List<TileNode> FindPath(TileNode start, TileNode end)
		{
			//the result list
			List<TileNode> result = new List<TileNode>();

			//set a min f, index, and current node
			float minF = 99999f;
			int minIndex = 0;
			TileNode curNode;

			//reset the costs
			start.g = 0;
			start.f = start.g + start.h;

			//clear the node lists
			open.Clear();
			closed.Clear();

			//add the start to the open list
			open.Add(start);

			//while searching
			while (true)
			{
				minF = 10000f;

				//try finding the tile with the lowest f cost in the open list
				for (int i = 0; i < open.Count; i++)
				{
					if (open[i].f < minF)
					{
						minF = open[i].f;
						minIndex = i;
					}
				}

				//set curnode to that tile and move it to the closed list
				curNode = open[minIndex];
				open.RemoveAt(minIndex);
				closed.Add(curNode);

				//break if the current id i the end id
				Console.WriteLine(curNode.id + " f: " + curNode.f + " open list count: " + open.Count);
				if (curNode.id == end.id)
				{
					end = curNode;
					break;
				}

				//have a comparison node
				TileNode compNode;

				for (int i = 0; i < curNode.adjacents.Count; i++)
				{
					//make the comparison node one of the adjacent nodes
					compNode = curNode.adjacents[i];

					//check if the compnode is not a block and doesn't exist in the closed list
					if (compNode.tileType != BLOCK && TileExists(closed, compNode) == -1)
					{
						//make a new g
						float newG = GetGCost(compNode, curNode);

						//if the tile doesn't exist in the open list
						if (TileExists(open, compNode) == -1)
						{
							//compnode's parent is curnode
							compNode.parent = curNode;

							//calculate new g and f
							compNode.g = newG;
							compNode.f = compNode.g + compNode.h;

							//add compnode to the open list
							open.Add(compNode);
						}
						else if (newG < compNode.g)
						{
							//compnode's parent is curnode
							compNode.parent = curNode;

							//calculate new g and f
							compNode.g = newG;
							compNode.f = compNode.g + compNode.h;
						}
					}
				}

				//stop if no more in the open list
				if (open.Count == 0)
				{
					break;
				}
			}

			//if the end tile is in the closed list
			if (TileExists(closed, end) != -1)
			{
				//pathnode = end node
				TileNode pathNode = end;

				//climb up the chain of parents until no more
				while (pathNode != null)
				{
					//insert into resilts
					result.Insert(0, pathNode);

					//go to next parent
					pathNode = pathNode.parent;
				}
			}

			//return the path
			return result;
		}

		//PRE: current location and target location
		//POST: float h cost
		//DESC: gets the h cost
		private float GetHCost(int row, int col, int targetRow, int targetCol)
		{
			return (float)Math.Abs(targetRow - row) * 10f + (float)Math.Abs(targetCol - col) * 10f;
		}

		//PRE: node map, target location
		//POST: 
		//DESC: sets the h cost
		private void SetHCost(TileNode[,] nodeMap, int targetRow, int targetCol)
		{
			for (int i = 0; i < Game1.COL; i++)
			{
				for (int j = 0; j < Game1.ROW; j++)
				{
					//set h cost and set f
					nodeMap[i, j].h = GetHCost(i, j, targetRow, targetCol);
					nodeMap[i, j].f = nodeMap[i, j].g + nodeMap[i, j].h;
				}
			}
		}

		//PRE: comparison node, parent node
		//POST: g cost
		//DESC: gets the g cost between tiles
		private float GetGCost(TileNode compNode, TileNode parentNode)
		{
			return parentNode.g + 10f * tileCosts[compNode.tileType];
		}

		//PRE: list of nodes and the ndoe checked
		//POST: gets the id of the node if it exists
		//DESC: gets the id if the tile exists in a list
		private int TileExists(List<TileNode> nodeList, TileNode checkNode)
		{
			for (int i = 0; i < nodeList.Count; i++)
			{
				//if id = id
				if (nodeList[i].id == checkNode.id)
				{
					//exists, return id
					return i;
				}
			}

			//does not exist
			return -1;
		}
	}
}
