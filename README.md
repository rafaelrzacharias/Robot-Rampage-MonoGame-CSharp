# Robot Rampage MonoGame CSharp

Please visit: http://www.monogame.net/2017/03/01/monogame-3-6/

Download and install MonoGame for Visual Studio.

Download the complete project with solution and placement art, build and enjoy!

To try the game, please find the executable file in Robot Rampage MonoGame CSharp/bin/DesktopGL/AnyCPU/Debug/

NOTES:
- This is a top-down 2D tank shooter.
- The most interesting feature on this project is the enemy AI. It was implemented a pathfinding with A* to give enemy tanks a way to pursue the player throughout the map and also avoid collisions with walls.
- Another great feature is the Camera2D that was implemented. This game has a world that is larger than the screen. Players can navigate to the edges of the screen and a camera system will autimatically pan and follow the player.
- A TileMap was implemented. This offers the ability to create large worlds efficiently, while keeping the memory footprint small. The implications of this on level design and world creation cannot be understated.
- Two axis where used independently! Players use the keyboard (WASD keys) or the gamePad (left thumbstick) to move the tank around and the keyboard (numpad 1-9) or the gamePad (right thumbstick) to aim and fire with the turret.
- It expands on the particle system created for the Asteroid Assault project, because now more effects can be created and also reused.
- Players have the ability to use multiple tank weapons. This creates new gameplay possibilities, causes a different feel when using each weapon and also each one has a different damage and behavior.
- A per-tile-collision was implemented. Players and enemies collide with tiles and react accordingly.
- A GoalManager was implemented. The player has an objective: to find and destroy all ComputerTerminals that are "creating" the new enemy tanks to chase the player. When all terminals are destroyed, the player proceeds to the next level.
- Simple game progression was implemented, as players beat each new wave, the next one comes with more ComputerTerminals that requires the player to disable them, each terminal spawning more enemies.
