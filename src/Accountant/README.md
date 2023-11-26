## GilGoblin.Accountant

`GilGoblin.Accountant` is a service that calculates the costs and profits for profitability of items in Final Fantasy XIV, and stores it in the database available from the package `GilGoblin.Database`.

The accountant service pre-calculates values for recipes such as cost and profitability. Performance is a large issue if these calculations are done at runtime. The calculations are performed by this service and stored for quick reference in the future.

The `GilGoblin.Accountant` package read and updates the database, and performs no other functionality. This package is
intended to be used in conjunction with other GilGoblin packages to calculate profitability for in-game
crafting in the game *FFXIV*.

The entire project, GilGoblin, can be viewed on Github:
https://github.com/NickReinlein/GilGoblin

## Special Thanks

Goblin
Icon:   <a href="https://www.freepik.com/icon/elf_196867#fromView=search&term=goblin&page=1&position=30&track=ais">
Designed by
Roundicons</a>