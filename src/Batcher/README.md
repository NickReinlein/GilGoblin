## GilGoblin.Batcher

GilGoblin.Batcher is a package that can help split up large jobs into batches of smaller jobs. For example, instead of
having 14,000 item Ids in a list, it can split it into 140 lists with 100 entries each, which would align with page size
for a GET call

The batcher performs small operations on the input data but performs no other functionality. This package is
intended to be used in conjunction with other GilGoblin packages to calculate profitability for in-game
crafting in the game *FFXIV*.

The entire project, GilGoblin, can be viewed on Github:
https://github.com/NickReinlein/GilGoblin

## Special Thanks

Goblin
Icon:   <a href="https://www.freepik.com/icon/elf_196867#fromView=search&term=goblin&page=1&position=30&track=ais">
Designed by
Roundicons</a>