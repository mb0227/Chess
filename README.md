# Chess Game Project

## Overview
This project is a Chess Game developed in WPF using C# with the .NET Framework (Version 4.8). It is designed to provide a fully functional chess-playing experience, complete with both multiplayer and Player vs Computer modes. The game adheres to FIDE rules and incorporates several advanced features, including time controls and undo functionality. The main goal of this project was to understand the use and implementation of data structures in real-world applications.

## Features
- Multiplayer mode for two players.
- Chess960 mode for multiplayer games.
- Player vs Computer mode with three difficulty levels: Easy, Medium, and Hard.
- Time controls: 1m, 3m, 5m, and 10m.
- Full adherence to FIDE rules.
- Undo functionality.
- Offer draw feature.
- Special moves like En Passant and Castling.

## Installation Instructions
1. Download Visual Studio from https://visualstudio.microsoft.com/
2. Install .NET Desktop Development when Visual Studio Installer is opened.
3. Install Git if you don't already have it.
4. Clone the repository or from GitHub using:
   ```bash
   git clone https://github.com/mb0227/Chess.git
   ```
   You can simply download from this link too.
5. Open the project in Visual Studio.
6. Ensure you have .NET Framework 4.8 installed.
7. Build and run the solution to start the game.

## Run without Visual Studio
To run this project without installing the setup file follow these steps.

### To play this game in Windows:
1. Install the Chess.zip file from https://github.com/mb0227/Chess/releases/tag/Chess. It is also available in github repository of this project as a zip file named as Setup.zip, install from any one of these.
2. Extract the folder.
3. Run Chess.exe, now you are Good to go!

### To play this game in Linux:
1. Install wine, wine-mono and it relevant dependencies.
For Example: For Ubuntu
```bash
   sudo apt update
   sudo apt install wine
```

Or if this does not work try:

```bash
sudo apt install --install-recommends winehq-stable
```

2. Install the Chess.zip file from https://github.com/mb0227/Chess/releases/tag/Chess. It is also available in github repository of this project as a zip file named as Setup.zip, install from any one of these.
3. Extract the folder.
4. Open terminal in that directory.
5. Run command
```bash
   wine Chess.exe
```
6. You are good to go!


## Gameplay
1. **Multiplayer Mode**: Allows two players to play locally, taking turns.
1. **Fisher Random Chess**: Allows two players to play locally, taking turns.
2. **Player vs Computer Mode**: Choose a difficulty level and play against the AI.
3. **Time Controls**: Select from 1m, 3m, 5m, or 10m for competitive play.
4. **Undo Feature**: Revert your last move.
5. **Offer Draw**: Propose a draw to your opponent.
6. **Special Moves**: Perform En Passant, Castling, and other advanced chess moves seamlessly.

## Repository Link
[GitHub Repository](https://github.com/mb0227/Chess)

## References
- **Chess.com**: Official website providing chess tutorials, resources, and game analysis. [Chess.com](https://www.chess.com)
- **Lichess**: Free and open-source online chess platform for learning strategies and practicing games. [LiChess](https://lichess.org)
- **GeeksforGeeks**: "Chess Game Algorithm and Implementation Guide." [GeeksforGeeks](https://www.geeksforgeeks.org)
- **FIDE**: Official governing body of chess rules and regulations. [FIDE](https://www.fide.com)
- **YouTube - Chess Notations**: "Chess Notations Tutorial" by Chess Talk. [Youtube](https://www.youtube.com/watch?v=b6PR885Rgb8)
- **YouTube - WPF**: "WPF Tutorial" by Kampa Plays. [YouTube](https://www.youtube.com/watch?v=t9ivUosw_iI&list=PLih2KERbY1HHOOJ2C6FOrVXIwg4AZ-hk1)
- **Green Chess - Images**: Chess piece images and visual resources. [Green Chess](https://greenchess.net/info.php?item=downloads)