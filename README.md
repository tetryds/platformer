# Platformer Game

## Implementation

The platformer 2d template game has been implemented entirely from scratch.

> No AI was used at any point during the development of this project.

The UI was not reimplemented yet.

### Player Control

Features:
* Walking (A/W)
* Jumping (Spacebar)
* Dashing (Left shift)

> In order to kill monsters, use dashing. This was added to make the game more challenging.

The player implementation can be found at `Assets\Scripts\Gameplay\Player\PlayerController.cs` and uses an event based state machine.
The state machine does not include jumping state, because the behavior of the jumping state does not change substantially from walking.

### Game Control

The game control can be found under `Assets\Scripts\Gameplay\Player\GameplayManager.cs` and also uses an event based state machine.
It also wires up events for other systems such as enemies, tokens and score.

Using an event based system the code can be easily extended and modules can be added trivially.
The event architecture also allows us to easily implement edit mode tests for each of the systems.

### Tools

Multiple tools have been added to `Assets\Scripts\Tools\`. Each of them is meant to solve a specific problem in development.
These tools are flexible and can be shared across multiple projects.

## Tests

>⚠️ Ensure that the build profile `Windows.Test` is active before running the tests. This was made to not pollute main scene with test scene artifacts and structure.

There are E2E playmode tests and a few edit mode tests.

The edit mode tests are mostly for the core state machine implementation, which is derived from my own state machine tool https://github.com/tetryds/unity-tools/tree/master/tetryds.Tools/Lib/StateMachine

All tests can be executed from the Test Runner view.
