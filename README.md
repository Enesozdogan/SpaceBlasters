# SpaceBlasters
 An Online 2D Multiplayer Action Game

## General Theme
This is a game where the  multiplayer shooter and platforming genre are combined. By utilizing different movement mechanics, player can escape from enemies or choose to assault them. In SpaceBlasters to join a game u must either join a lobby or queue for a match by using UGS(Unity Gaming Services). Either way game will start and the round timer will start counting down. The round timer determines the match duration and the player who won the game is selected by the killing score at the end of the match.

## Player Mechanics
To maintain the player mechanics on code side, state pattern is implemented. This way, each mechanic can be designed and implemented separatly. These mechanics are heavily based on movement such as running, idle, jetpack, jumping and falling. Movement is client authoritive in SpaceBlasters so by manipulating the client transform on implemented states, the transform is synchronized.

  
![run](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/77959b37-9903-47c1-87e9-77a6df39b3d1)
![jump](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/782f0e5a-5edf-4051-acaa-d68370b4bc49)
![idle](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/2880396f-6c8c-4d47-b62a-ed289db9f4b2)
