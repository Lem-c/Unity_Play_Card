# Unity Card Game Documentation

## Overview
This Unity-based card game combines gameplay with a user interface and animation-driven experiences. It supports a tutorial mode for beginners and randomized gameplay for repeatable fun. The game is designed to be expandable and uses several components for modular and efficient implementation.

---

## Techniques Used

### 1. **Game Flow Management**
   - **Scripts**: `GameManager.cs`, `persistentGM.cs`, `Round.cs`
   - Centralized control of the game logic, including:
     - Player turn management.
     - Card pool handling using the `Round` class.
     - Tutorial and regular gameplay differentiation.
     - Persistent data between sessions using `persistentGM.cs`.

### 2. **Player Data Persistence**
   - **Scripts**: `PlayerData.cs`, `EnterUIManager.cs`
   - Player progress is saved and loaded using JSON serialization, ensuring data persistence across sessions.

### 3. **User Interface (UI) Management**
   - **Scripts**: `UIManager.cs`, `EnterUIManager.cs`
   - Dynamic UI updates for:
     - Player and bot information.
     - Game status and chip counts.
     - Interactive buttons for actions like revealing cards and exiting.

### 4. **Animation System**
   - **Scripts**: `AnimationManager.cs`, `UIManager.cs`
   - Smooth animations for card reveals and other game events.
   - Event-driven design with `AnimationCompleted` for seamless transitions.

### 5. **Tutorial Mode**
   - **Scripts**: `Round.cs`, `GameManager.cs`, `TutorialScenario.cs`, `TutorialScenarioFactory.cs`
   - Step-by-step guided gameplay with fixed card sequences to teach the player basic mechanics.

### 6. **Randomized Gameplay**
   - **Scripts**: `Round.cs`, `persistentGM.cs`, `NewCardPoolManager.cs`
   - Randomized card pools and player count to ensure replayability.

### 7. **Card Pool Management**
   - **Scripts**: `CardPoolManager.cs`, `NewCardPoolManager.cs`
   - Handles creating and randomizing card pools for different player counts.

### 8. **Card Representation and Visualization**
   - **Scripts**: `CardUI.cs`
   - Visual representation of cards with dynamic updates for suits and values.

---

## Interface Documentation

### AnimationManager.cs
Manages animations during the game.
- **Methods**:
  - `PlayAnimation(string animationName)`: Triggers an animation by name.
  - `OnAnimationFinished()`: Event callback when an animation ends.

### GameManager.cs
Controls the game logic and flow.
- **Methods**:
  - `StartGame()`: Initializes a new game.
  - `StartTutorialRound(int scenarioIndex)`: Sets up a tutorial round.
  - `DrawCards()`: Draws cards for the player and bot.
  - `RevealBotCard()`: Reveals the bot's card.
  - `UpdatePlayerData(bool playerLeave)`: Updates and saves player data.

### persistentGM.cs
Provides global persistence across scenes.
- **Fields**:
  - `playerCountQueue`: Queue of upcoming player counts.
  - `boolQueue`: Queue for boolean-based decisions.
  - `isChosenRound`: Tracks special round conditions.

### PlayerData.cs
Stores and handles player-specific data.
- **Fields**:
  - `playerName`: Name of the player.
  - `gems`: Player's gem count.
  - `isTutorial`: Tracks whether the tutorial is active.
  - `tutorialRound`: Current tutorial step.

### Round.cs
Manages individual rounds of the game.
- **Methods**:
  - `DrawCards(out Card playerCard, out Card botCard, bool isChosenRound)`: Draws cards for player and bot, optionally favoring player wins.
  - `EnableTutorialMode(int scenarioIndex)`: Configures a round for tutorial gameplay.

### EnterUIManager.cs
Handles the main menu UI.
- **Methods**:
  - `LoadMainGameScene()`: Transitions to the main game.
  - `UpdateUI()`: Updates player information on the main menu.

### UIManager.cs
Manages in-game UI elements.
- **Methods**:
  - `UpdatePlayerName(string name)`: Updates the displayed player name.
  - `UpdateChips(int chips, bool isPlayer)`: Updates the chip count for player or bot.
  - `ShowWinImage()`, `ShowLoseImage()`: Displays the win/lose results.
  - `PlayCardRevealAnimation()`: Plays the card reveal animation.
  - `SetRevealButtonInteractable(bool interactable)`: Enables/disables the reveal button.

### CardPoolManager.cs
Handles the creation and selection of card pools.
- **Methods**:
  - `GetRandomThreePeoplePool()`: Gets a pool for three-player games.
  - `GetRandomFivePeoplePool()`: Gets a pool for five-player games.
  - `ResetThreePeoplePools()`, `ResetFivePeoplePools()`: Resets selected pools.

### NewCardPoolManager.cs
Enhanced card pool management.
- **Methods**:
  - `GetRandomPoolForPeople(int peopleCount)`: Gets a pool for a specified number of players.
  - `ResetPoolsForPeople(int peopleCount)`: Resets selected pools for specific player counts.

### CardUI.cs
Displays cards visually in the game.
- **Methods**:
  - `SetCard(int cardValue, string suit)`: Sets the card value and suit.

### TutorialScenario.cs
Defines individual tutorial scenarios.
- **Fields**:
  - `scenarioIndex`: Unique identifier for the scenario.
  - `tutorialPool`: Pool of cards for the scenario.
  - `tutorialBotDrawSequence`: Fixed bot card draw sequence.
  - `tutorialPlayerDrawSequence`: Fixed player card draw sequence.

### TutorialScenarioFactory.cs
Creates and manages tutorial scenarios.
- **Methods**:
  - `GetScenario(int scenarioIndex)`: Retrieves a specific tutorial scenario.

---

## File Structure
- `Assets/Scripts`:
  - **Core Scripts**:
    - `GameManager.cs`
    - `persistentGM.cs`
    - `Round.cs`
  - **UI Scripts**:
    - `UIManager.cs`
    - `EnterUIManager.cs`
  - **Animation Scripts**:
    - `AnimationManager.cs`
  - **Data Scripts**:
    - `PlayerData.cs`
  - **Card Management Scripts**:
    - `CardPoolManager.cs`
    - `NewCardPoolManager.cs`
    - `CardUI.cs`
  - **Tutorial Scripts**:
    - `TutorialScenario.cs`
    - `TutorialScenarioFactory.cs`

---

## Getting Started
1. Clone or download the project.
2. Open the project in Unity.
3. Set up the scenes and make sure all assets are linked properly.
4. Press Play to start the game.

---

## Contributing
If you'd like to contribute, please fork the repository and submit a pull request. Ensure your code adheres to Unity and C# best practices.

---

## License
This project is licensed under the MIT License. See the LICENSE file for details.

