# HydraX

HydraX is an asset decompiler for Call of Duty: Black Ops 3. The aim of HydraX is to accurately decompile more specialized and specific files such as AI files, tables, etc. that Treyarch didn't/couldn't provide with the Call of Duty: Black Ops 3 Mod Tools. If you're interested in extracting Models, Images, etc. check out [Greyhound](https://github.com/Scobalula/Greyhound), a fork of Wraith Archon.

HydraX can decompile/export the following asset types:

| Asset Type/Pool                                              |
|--------------------------------------------------------------|
| ZBarrier                                                     |
| AnimationMappingTable                                        |
| AnimationSelectorTable                                       |
| AnimationStateMachine                                        |
| BehaviorTree                                                 |
| CustomizationTable (An associated assets such as body types) |
| Localize (Localized Strings)                                 |
| PhysPreset                                                   |
| RawFile (including ttf (fonts))                              |
| Rumble                                                       |
| Sound (Aliases)                                              |
| StringTable                                                  |
| StructuredTable                                              |
| WeaponCamo                                                   |
| XCam                                                         |

For assets that are exported to GDTs, Hydra will bundle them into different GDTs in the source_data folder of the export directory:

* camo_assets = Weapon Camos 
* character_assets = Character Assets (Customization Tables, etc)
* misc_assets = ZBarriers 
* physic_assets = Physics Presets and Rumble
* table_assets = Unused, for future ImpactFX, etc. assets
* xcams_assets = XCams

Scripts and LUA files can be exported, but cannot be decompiled. To dissassemble LUA files, use Jari's [LUA Dissassembler](https://github.com/JariKCoding/T7-8-LuaDissassembler).

# Credits

* Harry Bo21 - Tons of testing and feedback (especially on the AI files)
* raptroes - Testing and Feedback
* DTZxPorter - Heuristic Scan Info and Tips from Wraith
* RDV/Ardivee - Acoustix 
* [Hydra Logo](https://thenounproject.com/term/hydra/1389034/)

# Using HydraX

Simply launch Black Ops 3 and the level of the assets you want to export, click Load Game, and export the assets you need or all (exporting is multithreaded and so Export All should be very fast for most modern systems).

# Requirements

* Windows 10 64bit officially tested, but Windows 7+ 64bit should work
* .NET Framework Version 4.6.1
* General understanding of how to use the assets you want to work with

# License/Disclaimers

HydraX is licensed under GPL 3.0. It is provided in the hope it is useful to you but comes WITH NO WARRANTY. The user assumes full responsibility for any damages caused.

HydraX was developed for the users of the Black Ops III Mod Tools to provide some files/information Treyarch couldn't/didn't include with the Mod Tools. All work was done on legally obtained copies of Black Ops III and the Black Ops III Mod Tools. Most of the files it exports, are only useful to those using Black Ops III's Mod Tools. HydraX does not and will never rebuild FFs or provide methods to modify game content. I don't support hacking!

## Support Me

If you use HydraX in any of your projects, it would be appreciated if you credit me, a huge amount of time and work (especially on more intricate assets such as aliases) went into developing it and fine tuning it, and a simple credit isn't too much to ask for. While I obviously won't come running after you, I do keep track of people who at the very least do not provide credit. ;)

If you'd like to support me even more, considering buying me a coffee (I drink a lot of it :x):

[![Donate](https://img.shields.io/badge/Donate-PayPal-yellowgreen.svg)](https://www.paypal.me/scobalula)
