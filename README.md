# HydraX

HydraX is an asset decompiler for Call of Duty: Black Ops 3 I have been working on since March of 2018, its aim is to accurately decompile more specialized and specific files such as AI files, tables, etc. that Treyarch didn't/couldn't provide with the Call of Duty: Black Ops 3 Mod Tools. If you're interested in extracting Models, Images, etc. check out [Greyhound](https://github.com/Scobalula/Greyhound), a fork of Wraith Archon.

HydraX can decompile/export the following asset types:

| Asset Type                       | Settings Group                   |
|----------------------------------|----------------------------------|
| localize                         | Raw File                         |
| xcam                             | Misc                             |
| attachmentcosmeticvariant        | Weapon                           |
| footsteptable                    | Misc                             |
| playersoundstable                | Misc                             |
| sharedweaponsounds               | Misc                             |
| playerfxtable                    | Misc                             |
| xmodelalias                      | Misc                             |
| impactsoundstable                | Misc                             |
| impactsfxtable                   | Misc                             |
| entityfximpacts                  | Misc                             |
| entityfximpacts                  | Misc                             |
| surfacefxtable                   | Misc                             |
| surfacesounddef                  | Misc                             |
| tracer                           | Weapon                           |
| laser                            | Misc                             |
| flametable                       | Weapon                           |
| customizationtable               | Misc                             |
| shellshock                       | Physics                          |
| ttf                              | Raw File                         |
| aitype                           | AI                               |
| vehiclesounddef                  | Misc                             |
| vehiclefxdef                     | Misc                             |
| tagfx                            | Misc                             |
| animselectortable                | AI                               |
| behaviorstatemachine             | AI                               |
| physpreset                       | Physics                          |
| physconstraints                  | Physics                          |
| rumble                           | Physics                          |
| weaponcamo                       | Weapon                           |
| zbarrier                         | Misc                             |
| behaviortree                     | AI                               |
| structuredtable                  | Meta Data                        |
| animstatemachine                 | AI                               |
| attachment                       | Attachment                       |
| animmappingtable                 | AI                               |
| sound                            | Sound                            |
| vehicle                          | Misc                             |
| rawfile                          | Raw File                         |
| scriptparsetree                  | Raw File                         |
| scriptbundle                     | Script Bundle                    |
| attachmentunique                 | Weapon                           |
| beam                             | Misc                             |
| weapon                           | Weapon                           |
| stringtable                      | Meta Data                        |

For assets that are exported to GDTs, Hydra will bundle them into different GDTs in the source_data folder of the export directory.

Scripts and LUA files can be exported, but cannot be decompiled. To dissassemble LUA files, use Jari's [LUA Dissassembler](https://github.com/JariKCoding/T7-8-LuaDissassembler).

# Credits

* Harry Bo21 - Tons of testing and feedback (especially on the AI files)
* raptroes - Testing and Feedback
* RDV/Ardivee - Acoustix 
* [Hydra Logo](https://thenounproject.com/term/hydra/1389034/)

# Using HydraX

Simply launch Black Ops 3 and the level of the assets you want to export, click Load Game, and export the assets you need or all (exporting is multithreaded and so Export All should be very fast for most modern systems).

# Requirements

* Windows 10 64bit officially tested, but Windows 7+ 64bit should work
* .NET Framework Version 4.7.1
* General understanding of how to use the assets you want to work with

# License/Disclaimers

HydraX is licensed under GPL 3.0. It is provided in the hope it is useful to you but comes WITH NO WARRANTY. The user assumes full responsibility for any damages caused.

HydraX was developed for the users of the Black Ops III Mod Tools to provide some files/information Treyarch couldn't/didn't include with the Mod Tools. All work was done on legally obtained copies of Black Ops III and the Black Ops III Mod Tools. Most of the files it exports, are only useful to those using Black Ops III's Mod Tools. HydraX does not and will never rebuild FFs or provide methods to modify game content. I don't support hacking!