
# Custom Emojis

This mod allows to add custom emojis that can be sent to other players. The custom emojis will be added to the bottom of the already existing vanilla emojis.

The accepted image formats are:
* `.png`
* `.jpg`/`.jpeg`
* `.gif` *(does not animate)*
* `.bmp`

To know what changed between versions you can go to the [Release Notes](release-notes.md).

>**Note:** In order to other players see and use the emojis they will also need to have this mod.

## Instalation

To download the mod, go to the the page of [Custom Emojis](https://www.nexusmods.com/stardewvalley/mods/2435) in the NexusMods mod page.

This mod requires [SMAPI](https://smapi.io/). All the help to install SMAPI and the troubleshooting help can be found in the "Player guide".

Follow these spteps to install the mod:
1. Download the mod [here](https://www.nexusmods.com/stardewvalley/mods/2435)
2. Extract the `.zip` in the `Mods` folder


## Configuration File
The mod allows  some configuration, and can be changed in the `config.json` file if you want. 
The config file is generated once Stardew Valley is launched at least once with the mod installed.

Available settings:

| Setting                     | Default Value   | Effect
|-----------------------------|:---------------:|-------------------------------
| `AllowClientEmojis`       | `true`         | Enable or disable the emote menu button animation
| `AllowClientEmojis`       | `".png"`       | Allows to specify one or multiple image formats that will be searched and added from the custom emojis folder.<br>Accepted values: `".png"`, `".jpg"`, `".jpeg"`, `".gif"`, `".bmp"`<br>

For example, if you would want to search and add all compatible image formats:
```json
{
  "AllowClientEmojis": true,
  "ImageExtensions": [
    ".png",
    ".jpg",
    ".jpeg",
    ".gif",
    ".bmp"
  ]
}
```

## Console Commands
This mod adds some console commands to use with the SMAPI console.

The commands are the following:
  
| Command 				  | Action
| ----------------- | -----------------------------------------
| `reload_emojis` | Reload the game emojis with the new ones found in the mod folder.