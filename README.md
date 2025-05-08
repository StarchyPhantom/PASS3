# Tower defense game

How to run: 
Navigate as so and find the executable PASS3.exe
PASS3/PASS3/bin/Debug/PASS3.exe

How to view code (Visual Studio 2019 recomended):
The PASS3.sln file in the same file as this readme if using Visual Studio or software that supports .sln
Otherwise
PASS3/PASS3
and access any of the C# files
Recommend files are Game1.cs and Level.cs, containing the most of the important logic

Features custom character creation which uses a default character texture unfortunatly, this is beacuse Monogame requires XNB files for images
and there is no easy way to convert files to XNB on the fly. 
You create your own map, which enemies will use pathfinding to navigate.

Honestly, this project was made to be a hybrid of a proof of concept and framework for a more complete project, 
due to the limited enemies and unused tower class archetypes.
