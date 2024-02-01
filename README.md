# SpaceBlasters
 An Online 2D Multiplayer Action Game

## General Theme
This is a game where the  multiplayer shooter and platforming genre are combined. By utilizing different movement mechanics, player can escape from enemies or choose to assault them. In SpaceBlasters to join a game u must either join a lobby or queue for a match by using UGS(Unity Gaming Services). Either way game will start and the round timer will start counting down. The round timer determines the match duration and the player who won the game is selected by the killing score at the end of the match.

## Player Mechanics
To maintain the player mechanics on code side, state pattern is implemented. This way, each mechanic can be designed and implemented separatly. These mechanics are heavily based on movement such as running, idle, jetpack, jumping and falling. Movement is client authoritive in SpaceBlasters so by manipulating the client transform on implemented states, the transform is synchronized.

  
![run](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/77959b37-9903-47c1-87e9-77a6df39b3d1)
![jump](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/782f0e5a-5edf-4051-acaa-d68370b4bc49)
![idle](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/2880396f-6c8c-4d47-b62a-ed289db9f4b2)

## Shop and NetworkVariables
To make the gameplay more engaging, at the end of each round a shop panel gets activated. On this panel, player can upgrade characters via buying items(improvements). Currently there are 3 types of items in the shop. These items are armour upgrade, weapon upgrade and jetpack upgrade. Armour item is basically an increasement on a network variable that manipulates the recevied damage data. Weapon item is on contrast as it increases the value of the damage output of the weapon. The jetpack upgrade is also a networkvariable but this networkvariable works with jetpack state. This variable is used as a fuel indicator for whether we can transition to jetpack state or to exit jetpack state on lack of fuel.   

![Shop](https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/b0441df0-3ddf-4d62-8c08-2c06b1c62380)

To be able to earn currency to buy the upgrades, player can either pick the coins that are randomly generated on map or they can kill other players and scavange the other player's currency. The currency is also a networkvariable defined on the currency repo class. After killing an enemy, the skull of the enemy must be collected to increase the kill count. 

## Network Connection
In SpaceBlasters there are 2 network models implemented. These are listen server and dedicated server. On listen server model, one of the clients creates a lobby allocation and a join code. With this join code other clients can join the game via lobby panel. The other method which is used in matchmaking allocates a dedicated server on unity game server hosting service when client queues for match and sends ticket to matchmaking service. To utilize the functionalities on different scenes and throught the game, the singleton pattern is implemented. Each network class(server,client,host) instantiates their own game manager and methods for client connection. 

<img src="https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/162e10f3-cb75-45a4-9f56-1379cad3d88f" width=500 height=300/>
<img src="https://github.com/Enesozdogan/SpaceBlasters/assets/72387932/903174bc-8d72-42de-a926-a82270d3db80" width=500 height=300/>

