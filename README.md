# DanceTools
A plugin with an in-game console that allows the host to spawn in items, spawn enemies, set credits, and more (to be added :^])<br>
(Only the host needs this plugin as of right now)<br>
The console can also be used while you are dead

## Installation
Download the zip and extract `DanceTools.dll` and `/DanceTools/` folder inside your plugins folder!<br>
**You need the `/DanceTools/` folder with the included assetbundle (`dancetoolsconsole`) for the console to work.**

## Plugin Info
Currently there are 6 commands that are usable with the in-game console, and 2 chat commands (from older version)<br>
Default key to open the console is `~` (Under your escape button) <- This can be changed in the config!<br>
**Most commands are HOST ONLY. Some can be used by client such as `clear` to clear the console.<br> 
In the future, there will be some client side commands**

Each command has its own description and a way to see how to use it.<br>
The `help` command will give you a list of all available commands.<br>
Using `help <another command name>` will give you a description of that command.<br>
Example: `help item` will show what the item command does and how to use it all within the console!

### Current list of commands
`clear`, `close`, `enemy`,`god`, `help`, `item`, `lights`, `tester (does nothing)`, `setcredits`

<details>
  <summary><b>Command Usage/Descriptions</b></summary><br>
	
* Arguments that have a `?` infront can be omitted and are optional.

|Command         |Usage   |Description   |
|----------------|--------|--------------|
|clear  	 |`clear` | Clears the console log
|close           |`close` | Closes the console UI. Use this in case of bug/getting stuck
|enemy           |`enemy name ?amount ?onme`| Spawns X amount of enemies inside random vents. Use command without arguments to see list of all enemies available
|god		 |`god` | Toggles Godmode for the host
|help            |`help ?command`| Without arguments shows list of commands, if used with an argument, it will show that commands description. 
|item            |`item itemID/name ?amount ?value ?weight(buggy)`| Spawns X amount of items on top of your (or spectated) player with a specified value.
|lights 	 |`lights on/off` | Changes the lights inside
|tester          |`tester`| Command for me to play around with. does nothing as of right now
|setcredits      |`setcredits amount`| Sets the groups credits to a specified amount

</details>

<details>
  <summary><b> Vanilla Item List (v49) (Click to expand)</b> </summary>

| ItemID | Item Name |
| ----------- | ----------- |
0 | Binoculars
1 | Boombox
2 | box
3 | Flashlight
4 | Jetpack
5 | Key
6 | Lockpicker
7 | Apparatus
8 | Mapper
9 | Pro-flashlight
10 | Shovel
11 | Stun grenade
12 | Extension ladder
13 | TZP-Inhalant
14 | Walkie-talkie
15 | Zap gun
16 | Magic 7 ball
17 | Airhorn
18 | Bell
19 | Big bolt
20 | Bottles
21 | Brush
22 | Candy
23 | Cash register
24 | Chemical jug
25 | Clown horn
26 | Large axle
27 | Teeth
28 | Dust pan
29 | Egg beater
30 | V-type engine
31 | Golden cup
32 | Fancy lamp
33 | Painting
34 | Plastic fish
35 | Laser pointer
36 | Gold bar
37 | Hairdryer
38 | Magnifying glass
39 | Metal sheet
40 | Cookie mold pan
41 | Mug
42 | Perfume bottle
43 | Old phone
44 | Jar of pickles
45 | Pill bottle
46 | Remote
47 | Ring
48 | Toy robot
49 | Rubber Ducky
50 | Red soda
51 | Steering wheel
52 | Stop sign
53 | Tea kettle
54 | Toothpaste
55 | Toy cube
56 | Hive
57 | Radar-booster
58 | Yield sign
59 | Shotgun
60 | Ammo
61 | Spray paint
62 | Homemade flashbang
63 | Gift
64 | Flask
65 | Tragedy
66 | Comedy
67 | Whoopie cushion
</details>

## Known Issues/Bugs
- Items that are spawned have no value when scanned. These items still sell for the correct value.
- Items that are spawned while in the ship/pre-game will sometimes go through the ship until landed.
- Sometimes console won't open if Steam overlay is opened or tabbed out of the game. (Fix is to tab in and out again)
- Weight parameter doesn't work properly for `item` command
- Some inputs are recorded while you have the console open (ie typing `god` will also drop your active item)

## Special Thanks to:
[MrMiinxx](https://www.youtube.com/watch?v=4Q7Zp5K2ywI) - YouTube tutorial for how to make a plugin<br>
[GameMaster Plugin](https://thunderstore.io/c/lethal-company/p/GameMasterDevs/GameMaster/) - Great Plugin with great examples<br>
<b>And a big thanks to everyone who has given feedback and suggestions. <3<b>

## Other
Feel free to use code from this plugin

## Changelog
### Version 1.1.4
- Added the ability to change the console opacity as per request
	- This can be done through the config using HEX color
 		- This also means you can change the color of the background if you wish
- Changed how the weight parameter works to make sure it doesn't set weight of items to 0 if nothing specified
	- Weight parameter is still buggy.
### Version 1.1.3
- Added `god` command
- Added `lights` command
- Added new functionality to Item command:
  	- You can now type the name of the item to spawn it instead of ID. Both ways work
  	- Weight is now a parameter. <- Doesn't work properly, set it to 0 or leave it for normal weight.

### Version 1.1.2
- Reworked `enemy` command:
  	- Can now use names to spawn enemies (Uses in-game names (Eyeless dog = Mouth Dog))
	- All enemies should be available to be spawned in regardless of level
	- Added outside enemies to the spawnable list
- Fixed empty spaces counting as empty characters in the console
- Console can now be opened in the main menu. Should be avaiable anywhere.
- (WIP!!) External command implementation for plugin developers.
	- Check `ExternalCommands` folder in the github repo
- Removed chat commands (old way of spawning items/enemies through chat)
- Enemies should actually actually now spawn in random vents (lol)
### Version 1.1.1
- Added the option to change the default console key in the config file.
- Fixed an issue with the enemy "onme" function to spawn an enemy correctly.
### Version 1.1.0
- Added Console UI with new commands
	- `clear`, `close`, `help`, `tester (does nothing)`, `setcredits`
- Moved `item` and `enemy` commands to the console and fixed bunch of bugs
- Added config file. As of right now, it only has console customization for colors. Will be adding more in the future
- Keeping the chat commands for the time being for anyone who still may want to use them.
