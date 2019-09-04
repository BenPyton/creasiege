# CreaSiege

Author: PELLETIER Benoît

CreaSiege is a prototype of a Besiege-like I made in 2 weeks during the 3rd year at Créajeux.

Demonstration in video:\
[![](http://img.youtube.com/vi/jMZckT97JpE/0.jpg)](https://youtu.be/jMZckT97JpE "Creasiege Demo")<br>

### Game Description :
- Besiege-like game.
- 1 player.
- Space vehicle construction and controling.
- Saving and loading of vehicles.
- Asset bundles for levels and vehicle parts.
- Key binding for active parts (like thrusters and weapons).
- Properties editing of parts.
- Vehicle destruction physic when vehicle take damages.

### Gameplay :
- You build a vehicle to fullfil your mission target in Edit mode, then in play mode you control your freshly constructed vehicle to try completing the mission. 

- In edit mode, you can select the part you want to add and place it with the mouse.\
There is a core block you cannot remove.\
You can also use the select tool, to select one part and edit it's properties (if there is any) or select multiple part and remove them with the delete key.\
You can use the erase tool to remove a part with a click (warning: a removed part remove also all connected parts to it).\
You can clear all your vehicle with the clear button.
- In play mode, you control your vehicle to complete your mission.\
By default, if no key was set to thrusters, they activate based on their orientation.\
And weapons follow the camera direction.

### Controls :
Default controls use an AZERTY keyboard.\
Use the F1 key to display the controls in-game.

### Technical informations :
The game was made with Unity 2018.3.0f2 and use asset bundles to manage levels and vehicle parts, so it's possible to add content without rebuilding the game.\
All assets (except the sky background) was made by me.
All code added to Unity (scripts and shaders) was made by me.