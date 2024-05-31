//Author: Benjamin Huynh
//File name: Program.cs
//Project name: PASS3
//Creation date: May 14, 2023
//Modified date: June 12, 2023
//Description: 
//OOP: each tower archetype comes from a parent type that stems from tower
//	   each enemy type comes from enemy (artist is a child of crossbow)
//	   these classes have inherited methods and data, and have overrides
//	   the other classes exist for organization and class interaction
//2D Arrays/Lists: used everywhere, storing maps, enemies, towers, tower data, projectiles, player data, etc
//File I/O: for player preferences, player data, tower data (and adding new tower data), sprite sheet sizes
//stacks/queues: nodestacks used to revert a path on the map and for enemies to follow, enemyqueues used to trickle enemies out
//recursion: pathfinding
//sorting/searching: linear search for most arrays/lists, insertion sort for most things (tower targets based on factors, data, etc)

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
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		//screen dimensions
		private int screenHeight;
		private int screenWidth;

		//define stream read/write for file I/O
		private static StreamWriter outFile;
		private static StreamReader inFile;

		//the length and width of a tile
		public const byte UNIT = 80;

		//grid dimensions
		public const byte ROW = 14;
		public const byte COL = 10;

		//gamestates
		private const byte MENU = 0;
		private const byte UNIT_MENU = 1;
		private const byte UNIT_CREATION = 2;
		private const byte INSTRUCTIONS = 3;
		private const byte PREPGAME = 4;
		private const byte GAME = 5;
		private const byte ENDGAME = 6;

		//max rounds for the game
		public const byte MAX_ROUNDS = 3;

		//directions
		public const byte UP = 0;
		public const byte RIGHT = 1;
		public const byte DOWN = 2;
		public const byte LEFT = 3;

		//the 8 types in the game
		public const byte CASTER = 0;
		public const byte DEFENDER = 1;
		public const byte GUARD = 2;
		public const byte MEDIC = 3;
		public const byte SNIPER = 4;
		public const byte SPECIALIST = 5;
		public const byte SUPPORTER = 6;
		public const byte VANGUARD = 7;

		//stores the current round
		public static int currentRound = 1;

		//the texture and rectangle shown while viewing tower stats
		Texture2D towerStatsBG;
		Rectangle bgRect;

		//mouse stats
		MouseState mouse;
		MouseState prevMouse;

		//keyboard states
		KeyboardState kb;
		KeyboardState prevKb;

		//current gamestate
		int gameState = MENU;

		//fonts
		SpriteFont regFont;
		SpriteFont littleFont;

		//songs
		public static Song[] songs = new Song[4];

		//all player data
		string playerName = null;
		string tempString = "";
		List<string> playerNames = new List<string>();
		List<int> playerWins = new List<int>();
		List<int> playerKills = new List<int>();
		List<List<string>> playerFavourites = new List<List<string>>();

		//player name location
		Vector2 playerNameLoc;

		//current player stats
		int currentPlayerWins = 0;
		public static int currentPlayerKills = 0;
		List<string> currentPlayerFavourites = new List<string>();

		//tower custom creation data
		byte creationStep = 0;
		string[] statTypes = new string[] { "Name", "Archetype", "Health", "Attack", "Defense", "Resistance", "Redeploy time", "Cost", "Block", "Attack interval" };
		string[] creationStats = new string[10];

		//level instance
		Level level;

		//tower data
		List<string> towerNames = new List<string>();
		List<string> towerTypes = new List<string>();
		List<float[]> towerStats = new List<float[]>();

		//tower animations & data, bg imgs, & hp bar img
		List<Texture2D[]> towerAnimSheets = new List<Texture2D[]>();
		List<int[,]> towerAnimSizes = new List<int[,]>();
		Texture2D[] bgImgs = new Texture2D[7];
		Texture2D hpBarImg;

		//tower type icons
		Texture2D[] towerTypeIcons = new Texture2D[9];

		//lists of tower cards
		List<TowerCard> towerCards = new List<TowerCard>();
		List<TowerCard> displayTowerCards = new List<TowerCard>();
		List<TowerCard> usedTowerCards = new List<TowerCard>(3);

		//tower being dragged and it's cost
		string dragging = " ";
		int towerCost = 0;

		//enemy images
		Texture2D[] enemyImgs = new Texture2D[13];

		//untility rectangles
		Rectangle util1Rec;
		Rectangle util2Rec;

		//tower icon rectangles
		Rectangle[] towerTypeIconRecs = new Rectangle[9];

		//logo image
		Texture2D titleImg;
		
		//button images
		Texture2D playImg;
		Texture2D creationImg;
		Texture2D intsImg;
		Texture2D exitImg;

		//arrow button images
		Texture2D[] deployArrowImgs = new Texture2D[4];

		//instruction images
		Texture2D[] instructionImgs = new Texture2D[3];

		//title rectangle and stats location
		Rectangle titleRec;
		Vector2 statsLoc;

		//rectangles for buttons
		Rectangle playButtonRec;
		Rectangle creationButtonRec;
		Rectangle intsButtonRec;
		Rectangle exitButtonRec;
		Rectangle backButtonRec;
		Rectangle continueButtonRec;

		//rects for arrows and cancel buttons
		Rectangle[] deployArrowRecs = new Rectangle[4];
		Rectangle deployCancelRec;

		//instruction rects
		Rectangle[] instructionRecs = new Rectangle[3];

		//menu buttons
		Button playButton;
		Button creationButton;
		Button intsButton;
		Button exitButton;
		Button backButton;
		Button continueButton;

		//dp timer and location
		Timer dpTimer;
		Vector2 dpLoc;

		//health and deployment points
		public static int health = 10;
		public static int deploymentPoints = 5;

		//bool indicating that the user is choosing their favourite towers
		bool isChoosingFaves = false;

		//bool indicating that the user is choosing the direction the tower is facing
		bool isChoosingDirection = false;

		//bool indicating whether the game should be sped up or not
		bool speedUp = false;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			IsMouseVisible = true;
			this.graphics.PreferredBackBufferWidth = 1120;
			this.graphics.PreferredBackBufferHeight = 880;
			this.graphics.ApplyChanges();
			screenWidth = this.graphics.GraphicsDevice.Viewport.Width;
			screenHeight = this.graphics.GraphicsDevice.Viewport.Height;
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			//load fonts
			regFont = Content.Load<SpriteFont>("Fonts/Regfont");
			littleFont = Content.Load<SpriteFont>("Fonts/LittleFont");

			//load background img and rect
			bgRect = new Rectangle(0, 0, screenWidth, screenHeight);
			towerStatsBG = Content.Load<Texture2D>("BG/TowerStatsBG");

			//load tile images
			bgImgs[0] = Content.Load<Texture2D>("BG/Spawn_64");
			bgImgs[1] = Content.Load<Texture2D>("BG/Exit_64");
			bgImgs[2] = Content.Load<Texture2D>("BG/Cobblestone_64");
			bgImgs[3] = Content.Load<Texture2D>("BG/Dirt_64");
			bgImgs[4] = Content.Load<Texture2D>("BG/Grass1_64");
			bgImgs[5] = Content.Load<Texture2D>("BG/Water_16");
			bgImgs[6] = Content.Load<Texture2D>("BG/Magma_16");

			//load enemy images
			enemyImgs[0] = Content.Load<Texture2D>("Enemies/Slug");
			enemyImgs[1] = Content.Load<Texture2D>("Enemies/CBow_Atk");
			enemyImgs[2] = Content.Load<Texture2D>("Enemies/CBow_W1");
			enemyImgs[3] = Content.Load<Texture2D>("Enemies/CBow_W2");
			enemyImgs[4] = Content.Load<Texture2D>("Enemies/Wiz_Atk");
			enemyImgs[5] = Content.Load<Texture2D>("Enemies/Wiz_W1");
			enemyImgs[6] = Content.Load<Texture2D>("Enemies/Wiz_W2");
			enemyImgs[7] = Content.Load<Texture2D>("Enemies/Fire_Atk");
			enemyImgs[8] = Content.Load<Texture2D>("Enemies/Fire_W1");
			enemyImgs[9] = Content.Load<Texture2D>("Enemies/Fire_W2");
			enemyImgs[10] = Content.Load<Texture2D>("Enemies/Luc_Atk");
			enemyImgs[11] = Content.Load<Texture2D>("Enemies/Luc_W1");
			enemyImgs[12] = Content.Load<Texture2D>("Enemies/Luc_W2");

			//hp img
			hpBarImg = new Texture2D(graphics.GraphicsDevice, 1, 1);
			hpBarImg.SetData(new[] { Color.White });

			//try to open the files
			try
			{
				//open the file to read
				inFile = File.OpenText("MainTowerStats.txt");

				//read the data from the file until there is none left
				while (!inFile.EndOfStream)
				{
					//read the lines in the file and convert to the required data types
					string[] dataLine = inFile.ReadLine().Split(',');

					towerNames.Add(dataLine[0]);

					dataLine[1] = CheckTowerType(dataLine[1]);
					towerTypes.Add(dataLine[1]);

					float[] tempStats = new float[8];

					for (int i = 0; i < tempStats.Length; i++)
					{
						tempStats[i] = (float)Convert.ToDouble(dataLine[i + 2]);
					}

					towerStats.Add(tempStats);
				}

				//if the file was being read, close reading
				if (inFile != null)
				{
					//close reading the file
					inFile.Close();
				}

				//open the file to read
				inFile = File.OpenText("SpriteSheetSizes.txt");

				//read the data from the file until there is none left
				while (!inFile.EndOfStream)
				{
					//read the lines in the file and convert to the required data types
					string[] dataLine = inFile.ReadLine().Split('|');

					int[,] temp2D = new int[3, 3];
					string[] tempRow = new string[3];

					for (int i = 0; i < dataLine.Length; i++)
					{
						tempRow = dataLine[i].Split(',');
						
						for (int j = 0; j < tempRow.Length; j++)
						{
							temp2D[i, j] = Convert.ToInt32(tempRow[j]);
						}
					}

					towerAnimSizes.Add(temp2D); 
				}

				//if the file was being read, close reading
				if (inFile != null)
				{
					//close reading the file
					inFile.Close();
				}

				//open the file to read
				inFile = File.OpenText("Player&Stats.txt");/////////////////////////////////////////////////////////////

				//read the data from the file until there is none left
				while (!inFile.EndOfStream)
				{
					//read the lines in the file and convert to the required data types
					string[] dataLine = inFile.ReadLine().Split(',');

					playerNames.Add(dataLine[0]);
					playerWins.Add(Convert.ToInt32(dataLine[1]));
					playerKills.Add(Convert.ToInt32(dataLine[2]));

					List<string> tempFavs = new List<string>();

					for (int i = 3; i < dataLine.Length; i++)
					{
						tempFavs.Add(dataLine[i]);
					}

					playerFavourites.Add(tempFavs);
				}

				//if the file was being read, close reading
				if (inFile != null)
				{
					//close reading the file
					inFile.Close();
				}
			}
			catch (FileNotFoundException fnf)
			{
				//file not found error feedback
				Console.WriteLine("ERROR: file not found." + fnf);
			}
			catch (EndOfStreamException eos)
			{
				//stream ended error feedback
				Console.WriteLine("ERROR: read past file." + eos);
			}
			catch (FormatException fe)
			{
				//bad format error feedback
				Console.WriteLine("ERROR: bad format." + fe);
			}
			catch (Exception e)
			{
				//general error feedback
				Console.WriteLine("ERROR: error? what???" + e);
			}
			finally
			{
				//if the file was being read, close reading
				if (inFile != null)
				{
					//close reading the file
					inFile.Close();
				}
			}

			//towerAnimSheets[0, 0] = Content.Load<Texture2D>("Characters/"+characterName+"_Deploy");

			for (int i = 0; i < towerNames.Count; i++)
			{
				try
				{
					//try loading animations for characters
					Texture2D[] sheets = new Texture2D[3];
					sheets[0] = Content.Load<Texture2D>("Characters/" + towerNames[i] + "_Deploy");
					sheets[1] = Content.Load<Texture2D>("Characters/" + towerNames[i] + "_Idle");
					sheets[2] = Content.Load<Texture2D>("Characters/" + towerNames[i] + "_Attack");
					towerAnimSheets.Add(sheets);
				}
				catch
				{
					//load default animation instead
					Texture2D[] sheets = new Texture2D[3];
					sheets[0] = Content.Load<Texture2D>("Characters/Melantha_Deploy");
					sheets[1] = Content.Load<Texture2D>("Characters/Melantha_Idle");
					sheets[2] = Content.Load<Texture2D>("Characters/Melantha_Attack");
					towerAnimSheets.Add(sheets);
				}
			}

			//player name loaction
			playerNameLoc = new Vector2(screenWidth / 4, screenHeight / 4);

			//load icon iamges
			towerTypeIcons[0] = Content.Load<Texture2D>("Icons/caster");
			towerTypeIcons[1] = Content.Load<Texture2D>("Icons/defender");
			towerTypeIcons[2] = Content.Load<Texture2D>("Icons/guard");
			towerTypeIcons[3] = Content.Load<Texture2D>("Icons/medic");
			towerTypeIcons[4] = Content.Load<Texture2D>("Icons/sniper");
			towerTypeIcons[5] = Content.Load<Texture2D>("Icons/specialist");
			towerTypeIcons[6] = Content.Load<Texture2D>("Icons/supporter");
			towerTypeIcons[7] = Content.Load<Texture2D>("Icons/vanguard");
			towerTypeIcons[8] = Content.Load<Texture2D>("Icons/Star");

			//load icon rectangles
			for (int i = 0; i < towerTypeIconRecs.Length; i++)
			{
				towerTypeIconRecs[i] = new Rectangle(13 * Game1.UNIT, i * Game1.UNIT, Game1.UNIT, Game1.UNIT);
			}

			//temp to store animation sizes
			int[] temp = new int[3];

			for (int i = 0; i < towerNames.Count; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					temp[j] = towerAnimSizes[i][1, j];
				}

				//make a new tower card
				towerCards.Add(new TowerCard(towerNames[i], towerTypes[i], towerStats[i], towerAnimSheets[i][1], temp, regFont, regFont, towerStatsBG, bgRect, -50, -50)); //10, i
			}

			//dp timer and loc
			dpTimer = new Timer(1000, true);
			dpLoc = new Vector2(0, 760);

			//laod logo
			titleImg = Content.Load<Texture2D>("BG/Menu");

			//load button images
			playImg =  Content.Load<Texture2D>("Buttons/Start");
			creationImg = Content.Load<Texture2D>("Buttons/Create");
			intsImg = Content.Load<Texture2D>("Buttons/Instructions");
			exitImg = Content.Load<Texture2D>("Buttons/Exit");

			//load deployment arrows
			deployArrowImgs[0] = Content.Load<Texture2D>("Buttons/Arrow_Up");
			deployArrowImgs[1] = Content.Load<Texture2D>("Buttons/Arrow_Right");
			deployArrowImgs[2] = Content.Load<Texture2D>("Buttons/Arrow_Down");
			deployArrowImgs[3] = Content.Load<Texture2D>("Buttons/Arrow_Left");

			//load instruction images
			instructionImgs[0] = Content.Load<Texture2D>("BG/Int1");
			instructionImgs[1] = Content.Load<Texture2D>("BG/Int2");
			instructionImgs[2] = Content.Load<Texture2D>("BG/Int3");

			//title and stats rec and loc
			titleRec = new Rectangle(0, 0, titleImg.Width * 12, titleImg.Height * 12);
			statsLoc.X = titleRec.Right;
			statsLoc.Y = 0;

			//button rectangles
			playButtonRec = new Rectangle(UNIT * 5, UNIT * 4, UNIT * 4, UNIT);
			creationButtonRec = new Rectangle(UNIT * 5, UNIT * 5, UNIT * 4, UNIT);
			intsButtonRec = new Rectangle(UNIT * 5, UNIT * 6, UNIT * 4, UNIT);
			exitButtonRec = new Rectangle(UNIT * 5, UNIT * 7, UNIT * 4, UNIT);
			backButtonRec = new Rectangle(UNIT * 1, UNIT * 8, UNIT * 2, UNIT);
			continueButtonRec = new Rectangle(UNIT * 11, UNIT * 8, UNIT * 2, UNIT);

			//arrow rectangles
			deployArrowRecs[UP] = new Rectangle(-UNIT * 5, -UNIT * 5, UNIT / 4 * 3, UNIT / 8 * 9);
			deployArrowRecs[RIGHT] = new Rectangle(-UNIT * 5, -UNIT * 5, UNIT / 8 * 9, UNIT / 4 * 3);
			deployArrowRecs[DOWN] = new Rectangle(-UNIT * 5, -UNIT * 5, UNIT / 4 * 3, UNIT / 8 * 9);
			deployArrowRecs[LEFT] = new Rectangle(-UNIT * 5, -UNIT * 5, UNIT / 8 * 9, UNIT / 4 * 3);

			//cancel rectangle
			deployCancelRec = new Rectangle(deployArrowRecs[RIGHT].X, deployArrowRecs[UP].Y, UNIT / 8 * 9, UNIT / 8 * 9);

			//instruction recs
			instructionRecs[0] = new Rectangle(0, 0, instructionImgs[0].Width * 3 / 4, instructionImgs[0].Height* 3 / 4);
			instructionRecs[1] = new Rectangle(0, instructionRecs[0].Height, instructionImgs[1].Width* 3 / 4, instructionImgs[1].Height* 3 / 4);
			instructionRecs[2] = new Rectangle(instructionRecs[0].Width, 0, instructionImgs[2].Width* 3 / 4, instructionImgs[2].Height* 3 / 4);

			//menu buttons
			playButton = new Button(playImg, playButtonRec, Color.White * 0.5f);
			creationButton = new Button(creationImg, creationButtonRec, Color.White * 0.5f);
			intsButton = new Button(intsImg, intsButtonRec, Color.White * 0.5f);
			exitButton = new Button(exitImg, exitButtonRec, Color.White * 0.5f);
			backButton = new Button(deployArrowImgs[3], backButtonRec, Color.White * 0.5f);
			continueButton = new Button(deployArrowImgs[1], continueButtonRec, Color.White * 0.5f);

			//util recs
			util1Rec = new Rectangle(960, 800, Game1.UNIT, Game1.UNIT);
			util2Rec = new Rectangle(1040, 800, Game1.UNIT, Game1.UNIT);

			//songs
			songs[0] = Content.Load<Song>("Songs/Menu");
			songs[1] = Content.Load<Song>("Songs/Minima");
			songs[2] = Content.Load<Song>("Songs/Signore");
			songs[3] = Content.Load<Song>("Songs/Lucien");

			//songs are repeating
			MediaPlayer.IsRepeating = true;

			//play the song
			MediaPlayer.Play(songs[0]);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			//get the current and previous state of the mouse
			prevMouse = mouse;
			mouse = Mouse.GetState();

			//get the current and previous state of the keyboard
			prevKb = kb;
			kb = Keyboard.GetState();

			//perform logic based on the game's state
			switch (gameState)
			{
				case MENU:
					//update the menu
					UpdateMenu();
					break;
				case UNIT_MENU:
					//update menu
					UpdateUnitMenu(gameTime);
					break;
				case UNIT_CREATION:
					//update unit creation
					UpdateUnitCreation();
					break;
				case INSTRUCTIONS:
					//go to the menu if enter is pressed
					if (Keyboard.GetState().IsKeyDown(Keys.Enter))
					{
						//go to the menu
						gameState = MENU;
					}
					break;
				case PREPGAME:
					//go to the game if enter is pressed
					level.UpdatePrep(mouse, prevMouse, kb, prevKb);
					if (Keyboard.GetState().IsKeyDown(Keys.Enter) && level.PathIsValid())
					{
						for (int i = 0; i < usedTowerCards.Count; i++)
						{
							usedTowerCards[i].Move(10, i);
						}

						//go to the game
						gameState = GAME;
					}
					break;
				case GAME:
					//update the game
					UpdateGame(gameTime);
					break;
				case ENDGAME:
					//update the endgame
					UpdateEndgame();
					break;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			//make the background grey
			GraphicsDevice.Clear(Color.Gray);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			switch (gameState)
			{
				case MENU:
					//draw menu
					DrawMenu();
					break;
				case UNIT_MENU:
					//draw unit menu
					DrawUnitMenu();
					break;
				case UNIT_CREATION:
					//draw the back and continue buttons
					backButton.Draw(spriteBatch);
					continueButton.Draw(spriteBatch);

					//draw the question string
					spriteBatch.DrawString(regFont, statTypes[creationStep] + tempString, playerNameLoc, Color.White);
					break;
				case INSTRUCTIONS:
					//draw the instructions
					for (int i = 0; i < instructionRecs.Length; i++)
					{
						spriteBatch.Draw(instructionImgs[i], instructionRecs[i], Color.White);
					}
					break;
				case PREPGAME:
					//draw the prepgame
					level.Draw(spriteBatch);
					break;
				case GAME:
					//draw the game
					DrawGame();
					break;
				case ENDGAME:
					//draw the endgame
					DrawEndgame();
					break;
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}

		//PRE: 
		//POST: 
		//DESC: update the menu
		private void UpdateMenu()
		{
			//upon startup, ask player for a name
			if (playerName == null)
			{
				if (prevKb != kb)
				{
					//let the player choose a name/account
					Letters();
					Numbers();
					if (kb.IsKeyDown(Keys.Back) && tempString.Length > 0)
					{
						tempString = tempString.Substring(0, tempString.Length - 1);
					}
				}

				//player submist name
				if (kb.IsKeyDown(Keys.Enter))
				{
					playerName = tempString;

					for (int i = 0; i < playerNames.Count; i++)
					{
						//if names match, set stats to that account
						if (playerName == playerNames[i])
						{
							currentPlayerKills = playerKills[i];
							currentPlayerWins = playerWins[i];
							currentPlayerFavourites = playerFavourites[i];
						}
					}
				}
			}
			else
			{
				//update the menu buttons
				playButton.Update(mouse);
				creationButton.Update(mouse);
				intsButton.Update(mouse);
				exitButton.Update(mouse);

				//if buttons are pressed, do their thing
				if (prevMouse.LeftButton != ButtonState.Pressed)
				{
					if (playButton.IsDown())
					{
						//go to the game
						level = new Level(screenHeight, screenWidth, bgImgs, enemyImgs, hpBarImg, towerNames, towerTypes, towerStats, towerAnimSheets, towerAnimSizes);
						gameState = UNIT_MENU;
					}
					else if (creationButton.IsDown())
					{
						//go create a character
						tempString = "";
						creationStep = 0;
						gameState = UNIT_CREATION;
					}
					else if (intsButton.IsDown())
					{
						//go to instructions
						gameState = INSTRUCTIONS;
					}
					else if (exitButton.IsDown())
					{
						//leave the game
						Exit();
					}
				}
			}
		}

		//PRE: game time for game
		//POST: 
		//DESC: 
		private void UpdateUnitMenu(GameTime gameTime)
		{
			//go to the prepgame if enetr is pressed
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				//go to the game and play song
				MediaPlayer.Play(songs[currentRound]);
				gameState = PREPGAME;
			}

			//update the display cards
			for (int j = 0; j < displayTowerCards.Count; j++)
			{
				displayTowerCards[j].Update(gameTime, mouse, prevMouse);
			}

			//update the roster cards
			for (int j = 0; j < usedTowerCards.Count; j++)
			{
				usedTowerCards[j].Update(gameTime, mouse, prevMouse);
			}

			for (int i = 0; i < towerTypeIconRecs.Length; i++)
			{
				//if a class icon is clicked, show the towers of that type
				if (towerTypeIconRecs[i].Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
				{
					//clear the cards displayed
					displayTowerCards.Clear();

					//add new cards to the display
					foreach (TowerCard j in towerCards)
					{
						if ((i == j.Type() && !usedTowerCards.Contains(j)) || (i == 8 && currentPlayerFavourites.Contains(j.Name())))
						{
							displayTowerCards.Add(j);
						}
					}

					//sort tower costs
					TowerCostSort(displayTowerCards);

					//display display cards
					for (int j = 0; j < displayTowerCards.Count; j++)
					{
						displayTowerCards[j].Move(j / 13, j % 13);
					}
				}
			}

			//if the star is right clicked, choose faves or stop
			if (towerTypeIconRecs[8].Contains(mouse.Position) && mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)
			{
				if (isChoosingFaves)
				{
					//choose
					isChoosingFaves = false;
				}
				else
				{
					//stop choosing
					isChoosingFaves = true;
				}
			}

			foreach (TowerCard i in displayTowerCards)
			{
				//if the mouse clicks on a card in the menu, add it to the team
				if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && i.Rect().Contains(mouse.Position))
				{
					if (!isChoosingFaves && !usedTowerCards.Contains(i) && usedTowerCards.Count < 12)
					{
						//add card to roster, remove from display, sort costs
						usedTowerCards.Add(i);
						displayTowerCards.Remove(i);
						TowerCostSort(displayTowerCards);
						TowerCostSort(usedTowerCards);

						//move display around
						for (int j = 0; j < displayTowerCards.Count; j++)
						{
							displayTowerCards[j].Move(j / 13, j % 13);
						}

						//move roster around
						for (int j = 0; j < usedTowerCards.Count; j++)
						{
							Console.WriteLine(usedTowerCards.Count);
							usedTowerCards[j].Move(10, j);
						}

						break;
					}

					//if faves being chosen, add them or remove them if they are already faved
					if (isChoosingFaves)
					{
						if (!currentPlayerFavourites.Contains(i.Name()))
						{
							//add to faves
							currentPlayerFavourites.Add(i.Name());
						}
						else
						{
							//remove from faves
							currentPlayerFavourites.Remove(i.Name());
						}
					}
				}
			}

			foreach (TowerCard i in usedTowerCards)
			{
				//if the mouse clicks on a tower card in the roster, remove it and place it back into the menu
				if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && i.Rect().Contains(mouse.Position))
				{
					//removes from roster
					usedTowerCards.Remove(i);

					//add to display if the cards there are the same type
					if (displayTowerCards.Count > 0 && displayTowerCards[0].Type() == i.Type())
					{
						//add to display
						displayTowerCards.Add(i);
						TowerCostSort(displayTowerCards);

						for (int j = 0; j < displayTowerCards.Count; j++)
						{
							displayTowerCards[j].Move(j / 13, j % 13);
						}
					}

					//shift the roster cards to organize
					for (int j = 0; j < usedTowerCards.Count; j++)
					{
						//move the card into place
						usedTowerCards[j].Move(10, j);
					}

					break;
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: update unit creation
		private void UpdateUnitCreation()
		{
			//update the buttons that allow the user to go back/continue
			backButton.Update(mouse);
			continueButton.Update(mouse);

			//let the user type out stats
			if (prevKb != kb)
			{
				//add to string letter is applicable
				if (creationStep <= 1)
				{
					Letters();
				}

				//delete from string
				if (kb.IsKeyDown(Keys.Back) && tempString.Length > 0)
				{
					tempString = tempString.Substring(0, tempString.Length - 1);
				}

				//add to string numbers
				Numbers();
			}

			//go back a step if backbutton is pressed, or go forwards if continue and string is not empty
			if (backButton.IsDown())
			{
				if (creationStep == 0)
				{
					//go back to menu
					gameState = MENU;
				}
				else
				{
					//go back a step
					creationStep--;
					tempString = "";
				}
			}
			else if (continueButton.IsDown() && prevMouse.LeftButton != ButtonState.Pressed)
			{
				//finalize everything on the last step
				if (creationStep == 9)
				{
					//add to stats
					creationStats[creationStep] = tempString;

					//check if type is ok
					creationStats[1] = CheckTowerType(creationStats[1]);

					//add tower data to the other tower datas
					towerNames.Add(creationStats[0]);
					towerTypes.Add(creationStats[1]);

					float[] tempStats = new float[8];

					for (int i = 0; i < tempStats.Length; i++)
					{
						tempStats[i] = (float)Convert.ToDouble(creationStats[i + 2]);
					}

					towerStats.Add(tempStats);

					//add an animation
					towerAnimSizes.Add(towerAnimSizes[0]);
					towerAnimSheets.Add(towerAnimSheets[0]);

					int[] temp = new int[3];

					for (int i = 0; i < 3; i++)
					{
						temp[i] = towerAnimSizes[towerNames.Count - 1][1, i];
					}

					//make a new tower card
					towerCards.Add(new TowerCard(towerNames[towerNames.Count - 1], towerTypes[towerNames.Count - 1], towerStats[towerNames.Count - 1], towerAnimSheets[towerNames.Count - 1][1], temp, regFont, regFont, towerStatsBG, bgRect, -50, -50));

					//write to towers the new tower
					WriteToTowers();

					//go back to the menu
					gameState = MENU;
				}
				else if (tempString.Length > 0)
				{
					//add to stats holder, clear string, next step
					creationStats[creationStep] = tempString;
					tempString = "";
					creationStep++;
				}
			}
		}

		//PRE: gametime for time
		//POST: 
		//DESC: update the game
		private void UpdateGame(GameTime gameTime)
		{
			Console.WriteLine("DP: " + deploymentPoints);

			//if the player is choosing a direction, have the arrows for them to choose or cancel
			if (isChoosingDirection)
			{
				if (mouse.LeftButton == ButtonState.Pressed)
				{
					if (deployArrowRecs[UP].Contains(mouse.Position))
					{
						//add tower up
						level.AddTower(dragging, UP, deployArrowRecs[UP].Bottom / Game1.UNIT, deployArrowRecs[UP].X / Game1.UNIT);
						deploymentPoints -= towerCost;
						FinishDeploy();
					}
					else if (deployArrowRecs[RIGHT].Contains(mouse.Position))
					{
						//add tower right
						level.AddTower(dragging, RIGHT, deployArrowRecs[UP].Bottom / Game1.UNIT, deployArrowRecs[UP].X / Game1.UNIT);
						deploymentPoints -= towerCost;
						FinishDeploy();
					}
					else if (deployArrowRecs[DOWN].Contains(mouse.Position))
					{
						//add tower down
						level.AddTower(dragging, DOWN, deployArrowRecs[UP].Bottom / Game1.UNIT, deployArrowRecs[UP].X / Game1.UNIT);
						deploymentPoints -= towerCost;
						FinishDeploy();
					}
					else if (deployArrowRecs[LEFT].Contains(mouse.Position))
					{
						//add tower left
						level.AddTower(dragging, LEFT, deployArrowRecs[UP].Bottom / Game1.UNIT, deployArrowRecs[UP].X / Game1.UNIT);
						deploymentPoints -= towerCost;
						FinishDeploy();
					}
					else if (deployCancelRec.Contains(mouse.Position))
					{
						//cancel
						FinishDeploy();
					}
				}
			}
			else
			{
				//speed up the game if space is pressed
				if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
				{
					if (speedUp != true)
					{
						//speed up
						speedUp = true;
					}
					else
					{
						//slow down
						speedUp = false;
					}
				}

				//if dp timer is finsihed, add dp, reset timer
				if (dpTimer.IsFinished())
				{
					deploymentPoints++;
					dpTimer.ResetTimer(true);
				}

				//if the player clicks on the utilities, prepare to place them
				if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
				{
					if (util1Rec.Contains(mouse.Position))
					{
						//water
						dragging = "util1";
						towerCost = 5;
					}
					else if (util2Rec.Contains(mouse.Position))
					{
						//lava
						dragging = "util2";
						towerCost = 10;
					}
				}

				foreach (TowerCard i in usedTowerCards)
				{
					//update towercards
					i.Update(gameTime, mouse, prevMouse);

					//if mouse pressed prepare for deployment
					if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
					{
						if (i.Rect().Contains(mouse.Position))
						{
							//prepare for deployment
							dragging = i.Name();
							towerCost = i.Cost();
							Console.WriteLine("Cost: "+i.Cost());
						}
					}

					//update the colour based on status
					if (level.GetDownTowerNames().Contains(i.Name()))
					{
						i.SetColour(2);
					}
					else if (level.GetActiveTowerNames().Contains(i.Name()) || deploymentPoints < i.Cost())
					{
						i.SetColour(1);
					}
					else
					{
						i.SetColour(0);
					}
				}

				//if dragging a tower, make sure there is not a tower already there, it is in bounds, it can be afforded,
				//it is not deployed already,etc. or if it is a util, make sure not dragging on the same util
				if (dragging != " " 
					&& !level.GetActiveTowerNames().Contains(dragging)
					&& !level.GetDownTowerNames().Contains(dragging)
					&& towerCost <= deploymentPoints
					&& mouse.LeftButton == ButtonState.Released
					&& mouse.Position.Y < 800 
					&& level.TileType(mouse.Y / UNIT, mouse.X / UNIT) != Level.ENDPOINT
					&& level.TileType(mouse.Y / UNIT, mouse.X / UNIT) != Level.SPAWN
					&& ((!level.TileHasTower(mouse.Y / Game1.UNIT, mouse.X / Game1.UNIT) && dragging != "util1" && dragging != "util2")
					|| (dragging == "util1" && level.TileType(mouse.Y / UNIT, mouse.X / UNIT) != Level.WATER)
					|| (dragging == "util2" && level.TileType(mouse.Y / UNIT, mouse.X / UNIT) != Level.LAVA)))
				{
					//check if dragging util or tower, deploy accordingly
					if (dragging == "util1")
					{
						//deploy water
						deploymentPoints -= towerCost;
						dragging = " ";
						level.AddObstacle(5, mouse.X / Game1.UNIT, mouse.Y / Game1.UNIT);
					}
					else if (dragging == "util2")
					{
						//deploy lava
						deploymentPoints -= towerCost;
						dragging = " ";
						level.AddObstacle(6, mouse.X / Game1.UNIT, mouse.Y / Game1.UNIT);
					}
					else
					{
						//move the deployment arrows to the deployment location
						deployArrowRecs[UP].X = (mouse.X / Game1.UNIT) * Game1.UNIT + 10;
						deployArrowRecs[UP].Y = (mouse.Y / Game1.UNIT) * Game1.UNIT - Game1.UNIT;
						deployArrowRecs[RIGHT].X = deployArrowRecs[UP].Right;
						deployArrowRecs[RIGHT].Y = deployArrowRecs[UP].Bottom;
						deployArrowRecs[DOWN].X = deployArrowRecs[UP].X;
						deployArrowRecs[DOWN].Y = deployArrowRecs[UP].Bottom + 60;
						deployArrowRecs[LEFT].X = deployArrowRecs[UP].X - 90;
						deployArrowRecs[LEFT].Y = deployArrowRecs[UP].Bottom;

						//move the cancel placement button to the deployment location
						deployCancelRec.X = deployArrowRecs[RIGHT].X;
						deployCancelRec.Y = deployArrowRecs[DOWN].Y;

						//choose direction
						isChoosingDirection = true;
					}
				}
				else if (towerCost > deploymentPoints || (mouse.Position.Y > 800 && mouse.LeftButton == ButtonState.Released))
				{
					//not dragging anything 
					dragging = " ";
					towerCost = 0;
				}

				//update level and dp timer
				level.UpdateGame(gameTime);
				dpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

				//double update if speed up
				if (speedUp)
				{
					level.UpdateGame(gameTime);
					dpTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
				}
			}

			//if level is done
			if (level.IsLevelDone() || health <= 0)
			{
				//play menu song, go to endgame
				MediaPlayer.Play(songs[0]);
				gameState = ENDGAME;
			}
		}

		//PRE: 
		//POST: 
		//DESC: unpdate endgame
		private void UpdateEndgame()
		{
			//if continuing, write to save, reset dp, etc
			if (Keyboard.GetState().IsKeyDown(Keys.Enter))
			{
				//save, reset dp
				WriteToSave();
				deploymentPoints = 5;

				//depending on level and health
				if (health < 0)
				{
					//lose, go to menu on round 1
					currentRound = 1;
					gameState = MENU;
				}
				else if (currentRound <= 3)
				{
					//go to the prepgame and next level
					health += 10;
					MediaPlayer.Play(songs[currentRound]);
					gameState = PREPGAME;
				}
				else
				{
					//reset towercard colours
					foreach (TowerCard i in usedTowerCards)
					{
						i.SetColour(0);
					}

					//reset tower cards
					usedTowerCards.Clear();
					displayTowerCards.Clear();

					//increase wins, go to round 1 and menu
					currentPlayerWins++;
					currentRound = 1;
					gameState = MENU;
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw menu
		private void DrawMenu()
		{
			//draw string our buttons and logo and stats
			if (playerName == null)
			{
				//ask for name
				spriteBatch.DrawString(regFont, "Your name: " + tempString, playerNameLoc, Color.White);
			}
			else
			{
				//show stats, buttons, logo
				spriteBatch.Draw(titleImg, titleRec, Color.White);
				spriteBatch.DrawString(littleFont, "Total kills: " + currentPlayerKills + " Total wins: " + currentPlayerWins, statsLoc, Color.White);
				playButton.Draw(spriteBatch);
				creationButton.Draw(spriteBatch);
				intsButton.Draw(spriteBatch);
				exitButton.Draw(spriteBatch);
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw unit menu
		private void DrawUnitMenu()
		{
			//draw type icons
			for (int i = 0; i < towerTypeIconRecs.Length - 1; i++)
			{
				spriteBatch.Draw(towerTypeIcons[i], towerTypeIconRecs[i], Color.White);
			}

			//make the favourites tab glow yellow if choosing favourites
			if (isChoosingFaves)
			{
				spriteBatch.Draw(towerTypeIcons[8], towerTypeIconRecs[8], Color.Yellow);
			}
			else
			{
				spriteBatch.Draw(towerTypeIcons[8], towerTypeIconRecs[8], Color.White);
			}

			//draw every tower card displayed
			foreach (TowerCard i in displayTowerCards)
			{
				i.Draw(spriteBatch);
			}

			//draw every tower card to be used in game
			foreach (TowerCard i in usedTowerCards)
			{
				i.Draw(spriteBatch);
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw game
		private void DrawGame()
		{
			//draw level
			level.Draw(spriteBatch);

			//draw towercards
			foreach (TowerCard i in usedTowerCards)
			{
				i.Draw(spriteBatch);
			}

			//draw arrows
			for (int i = 0; i < deployArrowRecs.Length; i++)
			{
				spriteBatch.Draw(deployArrowImgs[i], deployArrowRecs[i], Color.Purple * 0.5f);
			}

			//draw cancel button
			spriteBatch.Draw(exitImg, deployCancelRec, Color.Red);

			//draw tp and health info
			spriteBatch.DrawString(regFont, "DP: " + deploymentPoints + "   Health: " + health, dpLoc, Color.White);

			//draw utils
			spriteBatch.Draw(bgImgs[5], util1Rec, Color.White);
			spriteBatch.Draw(bgImgs[6], util2Rec, Color.White);

			//make a lime highlight when mouse is pressed
			if (mouse.LeftButton == ButtonState.Pressed)
			{
				Rectangle temp = new Rectangle(mouse.X / Game1.UNIT * Game1.UNIT, mouse.Y / Game1.UNIT * Game1.UNIT, Game1.UNIT, Game1.UNIT);

				spriteBatch.Draw(hpBarImg, temp, Color.Lime * 0.25f);
			}
		}

		//PRE: 
		//POST: 
		//DESC: draw endgame
		private void DrawEndgame()
		{
			//draw message based on win/loss
			if (health > 0)
			{
				//win, congradulate and inform
				spriteBatch.DrawString(regFont, "Your health: " + health, playerNameLoc, Color.White);

				if (currentRound == MAX_ROUNDS + 1)
				{
					spriteBatch.DrawString(littleFont, "\n\n        A winner is you!", playerNameLoc, Color.White);
				}
			}
			else
			{
				//loss
				spriteBatch.DrawString(regFont, "You have perished!", playerNameLoc, Color.White);
			}
		}

		//PRE: 
		//POST: stirng of tower type
		//DESC: check tower type
		private string CheckTowerType(string type)
		{
			//if type is not found, default
			if (!Caster.GetChildren().Contains(type) &&
				!Defender.GetChildren().Contains(type) &&
				!Guard.GetChildren().Contains(type) &&
				!Medic.GetChildren().Contains(type) &&
				!Sniper.GetChildren().Contains(type) &&
				!Specialist.GetChildren().Contains(type) &&
				!Supporter.GetChildren().Contains(type) &&
				!Vanguard.GetChildren().Contains(type))
			{
				return "StandardBearer";
			}

			//return old type if ok
			return type;
		}

		//PRE: 
		//POST: 
		//DESC: write player info to save
		private void WriteToSave() //Dr. Fubar,2,416,Melantha,Kroos
		{
			//if player name exists, replace current stats
			if (playerNames.Contains(playerName))
			{
				for (int i = 0; i < playerNames.Count; i++)
				{
					//player is found? replace stats
					if (playerNames[i] == playerName)
					{
						//replace stats
						playerKills[i] = currentPlayerKills;
						playerWins[i] = currentPlayerWins;
						playerFavourites[i] = currentPlayerFavourites;
					}
				}
			}
			else
			{
				//add stats to the end
				playerNames.Add(playerName);
				playerKills.Add(currentPlayerKills);
				playerWins.Add(currentPlayerWins);
				playerFavourites.Add(currentPlayerFavourites);
			}

			//create file
			outFile = File.CreateText("Player&Stats.txt");

			for (int i = 0; i < playerNames.Count; i++)
			{
				//write data to file
				outFile.Write(playerNames[i]);
				outFile.Write("," + playerWins[i]);
				outFile.Write("," + playerKills[i]);

				for (int j = 0; j < playerFavourites[i].Count; j++)
				{
					Console.WriteLine(playerFavourites[i].Count + " florida");
					outFile.Write("," + playerFavourites[i][j]);
				}

				//prepare for next line
				outFile.WriteLine("");
			}

			//close file
			outFile.Close();
		}

		//PRE: 
		//POST: 
		//DESC: add tower data
		private void WriteToTowers()
		{
			//try writing to files
			try
			{
				//add text to the end of the tower file
				outFile = File.AppendText("MainTowerStats.txt");
				outFile.Write("\n" + towerNames[towerNames.Count - 1]);
				outFile.Write("," + towerTypes[towerNames.Count - 1]);

				//write the tower stats out
				for (int i = 0; i < towerStats[towerNames.Count - 1].Length; i++)
				{
					outFile.Write("," + towerStats[towerNames.Count - 1][i]);
				}

				//close file
				outFile.Close();

				//write out the spritesheet sizes
				outFile = File.AppendText("SpriteSheetSizes.txt");
				outFile.Write("\n" + towerAnimSizes[towerNames.Count - 1][1, 0]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][1, 1]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][1, 2]);
				outFile.Write("|" + towerAnimSizes[towerNames.Count - 1][1, 0]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][1, 1]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][1, 2]);
				outFile.Write("|" + towerAnimSizes[towerNames.Count - 1][2, 0]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][2, 1]);
				outFile.Write("," + towerAnimSizes[towerNames.Count - 1][2, 2]);

				//close file
				outFile.Close();
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Couldn't find the file, dunno where it went");
			}
			catch
			{

			}
		}

		//Pre: list containing the cards of the towers
		//Post: vals will be a ascending sorted array of ints
		//Desc: Sort all the elements of vals in ascending order
		private void TowerCostSort(List<TowerCard> cards)
		{
			//Store the current value being inserted
			TowerCard temp;

			//Store the index the value will be inserted at
			int j;

			//Assume the first element is sorted, now go through each unsorted element
			for (int i = 1; i < cards.Count; i++)
			{
				//Store the next element to be inserted
				temp = cards[i];

				//Shift all "sorted" elements that are greater than the new value to the right
				for (j = i; j > 0 && cards[j - 1].Cost() > temp.Cost(); j--)
				{
				    cards[j] = cards[j - 1];
				}

				//The insertion location has been found, now insert the value
				cards[j] = temp;
			}
		}

		//PRE: 
		//POST: 
		//DESC: help clean up/finish deployment
		private void FinishDeploy()
		{
			//indicate dragging nothing
			dragging = " ";

			//hide the deployment arrows
			deployArrowRecs[UP].X = -400;
			deployArrowRecs[UP].Y = -400;
			deployArrowRecs[RIGHT].X = -400;
			deployArrowRecs[RIGHT].Y = -400;
			deployArrowRecs[DOWN].X = -400;
			deployArrowRecs[DOWN].Y = -400;
			deployArrowRecs[LEFT].X = -400;
			deployArrowRecs[LEFT].Y = -400;

			//hide the cancel button
			deployCancelRec.X = -400;
			deployCancelRec.Y = -400;

			//indicate no longer choosing direction
			isChoosingDirection = false;
		}

		//PRE: 
		//POST: 
		//DESC: add letters to a string
		private void Letters()
		{
			//add correspoding letters
			if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift))
			{
				if (kb.IsKeyDown(Keys.A))
				{
					tempString += 'A';
				}
				else if (kb.IsKeyDown(Keys.B))
				{
					tempString += 'B';
				}
				else if (kb.IsKeyDown(Keys.C))
				{
					tempString += 'C';
				}
				else if (kb.IsKeyDown(Keys.D))
				{
					tempString += 'D';
				}
				else if (kb.IsKeyDown(Keys.E))
				{
					tempString += 'E';
				}
				else if (kb.IsKeyDown(Keys.F))
				{
					tempString += 'F';
				}
				else if (kb.IsKeyDown(Keys.G))
				{
					tempString += 'G';
				}
				else if (kb.IsKeyDown(Keys.H))
				{
					tempString += 'H';
				}
				else if (kb.IsKeyDown(Keys.I))
				{
					tempString += 'I';
				}
				else if (kb.IsKeyDown(Keys.J))
				{
					tempString += 'J';
				}
				else if (kb.IsKeyDown(Keys.K))
				{
					tempString += 'K';
				}
				else if (kb.IsKeyDown(Keys.L))
				{
					tempString += 'L';
				}
				else if (kb.IsKeyDown(Keys.M))
				{
					tempString += 'M';
				}
				else if (kb.IsKeyDown(Keys.N))
				{
					tempString += 'N';
				}
				else if (kb.IsKeyDown(Keys.O))
				{
					tempString += 'O';
				}
				else if (kb.IsKeyDown(Keys.P))
				{
					tempString += 'P';
				}
				else if (kb.IsKeyDown(Keys.Q))
				{
					tempString += 'Q';
				}
				else if (kb.IsKeyDown(Keys.R))
				{
					tempString += 'R';
				}
				else if (kb.IsKeyDown(Keys.S))
				{
					tempString += 'S';
				}
				else if (kb.IsKeyDown(Keys.T))
				{
					tempString += 'T';
				}
				else if (kb.IsKeyDown(Keys.U))
				{
					tempString += 'U';
				}
				else if (kb.IsKeyDown(Keys.V))
				{
					tempString += 'V';
				}
				else if (kb.IsKeyDown(Keys.W))
				{
					tempString += 'W';
				}
				else if (kb.IsKeyDown(Keys.X))
				{
					tempString += 'X';
				}
				else if (kb.IsKeyDown(Keys.Y))
				{
					tempString += 'Y';
				}
				else if (kb.IsKeyDown(Keys.Z))
				{
					tempString += 'Z';
				}
			}
			else if (kb.IsKeyDown(Keys.Space))
			{
				tempString += ' ';
			}
			else
			{
				if (kb.IsKeyDown(Keys.A))
				{
					tempString += 'a';
				}
				else if (kb.IsKeyDown(Keys.B))
				{
					tempString += 'b';
				}
				else if (kb.IsKeyDown(Keys.C))
				{
					tempString += 'c';
				}
				else if (kb.IsKeyDown(Keys.D))
				{
					tempString += 'd';
				}
				else if (kb.IsKeyDown(Keys.E))
				{
					tempString += 'e';
				}
				else if (kb.IsKeyDown(Keys.F))
				{
					tempString += 'f';
				}
				else if (kb.IsKeyDown(Keys.G))
				{
					tempString += 'g';
				}
				else if (kb.IsKeyDown(Keys.H))
				{
					tempString += 'h';
				}
				else if (kb.IsKeyDown(Keys.I))
				{
					tempString += 'i';
				}
				else if (kb.IsKeyDown(Keys.J))
				{
					tempString += 'j';
				}
				else if (kb.IsKeyDown(Keys.K))
				{
					tempString += 'k';
				}
				else if (kb.IsKeyDown(Keys.L))
				{
					tempString += 'l';
				}
				else if (kb.IsKeyDown(Keys.M))
				{
					tempString += 'm';
				}
				else if (kb.IsKeyDown(Keys.N))
				{
					tempString += 'n';
				}
				else if (kb.IsKeyDown(Keys.O))
				{
					tempString += 'o';
				}
				else if (kb.IsKeyDown(Keys.P))
				{
					tempString += 'p';
				}
				else if (kb.IsKeyDown(Keys.Q))
				{
					tempString += 'q';
				}
				else if (kb.IsKeyDown(Keys.R))
				{
					tempString += 'r';
				}
				else if (kb.IsKeyDown(Keys.S))
				{
					tempString += 's';
				}
				else if (kb.IsKeyDown(Keys.T))
				{
					tempString += 't';
				}
				else if (kb.IsKeyDown(Keys.U))
				{
					tempString += 'u';
				}
				else if (kb.IsKeyDown(Keys.V))
				{
					tempString += 'v';
				}
				else if (kb.IsKeyDown(Keys.W))
				{
					tempString += 'w';
				}
				else if (kb.IsKeyDown(Keys.X))
				{
					tempString += 'x';
				}
				else if (kb.IsKeyDown(Keys.Y))
				{
					tempString += 'y';
				}
				else if (kb.IsKeyDown(Keys.Z))
				{
					tempString += 'z';
				}
			}
		}

		//PRE: 
		//POST: 
		//DESC: add numbers to sting
		public void Numbers()
		{
			//add corresponding numbers
			if (kb.IsKeyDown(Keys.D1))
			{
				tempString += '1';
			}
			else if (kb.IsKeyDown(Keys.D2))
			{
				tempString += '2';
			}
			else if (kb.IsKeyDown(Keys.D3))
			{
				tempString += '3';
			}
			else if (kb.IsKeyDown(Keys.D4))
			{
				tempString += '4';
			}
			else if (kb.IsKeyDown(Keys.D5))
			{
				tempString += '5';
			}
			else if (kb.IsKeyDown(Keys.D6))
			{
				tempString += '6';
			}
			else if (kb.IsKeyDown(Keys.D7))
			{
				tempString += '7';
			}
			else if (kb.IsKeyDown(Keys.D8))
			{
				tempString += '8';
			}
			else if (kb.IsKeyDown(Keys.D9))
			{
				tempString += '9';
			}
			else if (kb.IsKeyDown(Keys.D0))
			{
				tempString += '0';
			}
			else if (kb.IsKeyDown(Keys.OemPeriod))
			{
				tempString += '.';
			}
		}
	}
}
