
# Multiplayer Emotes

To download the mod, go to the the page of [Multiplayer Emotes](https://www.nexusmods.com/stardewvalley/mods/2347) in NexusMods mod page.

In order to other players see the played emotes and they alse need to have this mod.

## Instalation

Follow these spteps to install the mod:
1. Download the mod [here](https://www.nexusmods.com/stardewvalley/mods/2347)
2. Extract the `.zip` in the `Mods` folder


## Configuration File
The mod allows  some configuration, and can be changed in the `config.json` file if you want. 
The config file is generated once Stardew Valley is launched at least once with the mod installed.

Available settings:

| Setting                     | Effect
| --------------------------- | -------------------------------
| `AnimateEmoteButtonIcon` | Default `true`. Enable or disable the emote menu button animation
| `ShowTooltipOnHover`      | Default `true`. Enable or disable the tooltip when hovering the emote menu button

## Console Commands
This mod adds some console commands to use with the SMAPI console. This can be useful in case a emote gets stuck playing or to stop playing emotes.

The commands are the following:
  
| command 				| parameters 							   | action
| --------------------- | -----------------------------------------|----------------
| `emote <value>`		| A integer representing the animation id. | Play the emote with the passed id
| `stop_emote` 		| None				                       | Stop any playing emote
| `stop_all_emotes` 	| None		                               | Stop any playing emote by other players
