# Merge Mortem — Case 1: The Coorg Study

A casual merge-puzzle game where merging clues is how you investigate a murder.

## How to play

1. Tap a generator (**Fabric**, **Prints**, or **Phone**) to drop a tier‑1 clue onto the board. Each tap costs 1 energy.
2. Tap one item, then tap an identical item (same chain, same tier) to merge them into the next tier.
3. When an item reaches tier 3 it glows yellow. Select it and press **File as evidence** to file it.
4. Filing reveals a clue about the case and updates the suspect chips.
5. File all three chains (Fabric, Prints, Phone) to complete Chapter 1 and uncover the Day‑1 hook.

If you run out of energy, a popup offers a simulated "Watch ad: +5 energy." Progress is saved after Chapter 1 completes — relaunching the game shows "See you tomorrow, detective" with options to simulate Chapter 2's teaser or replay the case from scratch.

## Unity version

`2022.3.67f2`

## Live link

https://riya-juyal.github.io/merge-mortem/

*(Pending manual GitHub repo creation and Pages setup — see below if not yet live.)*

## Project structure

- `Assets/Scenes/Main.unity` — single scene: Camera, EventSystem, and a `Bootstrap` GameObject.
- `Assets/Scripts/Data.cs` — the three evidence chains, tier labels, colors, and reveal text.
- `Assets/Scripts/GameState.cs` — pure game rules and state (board, energy, XP, merge/file logic). No UI code.
- `Assets/Scripts/GameUI.cs` — builds and renders the entire uGUI interface at runtime, handles input, modals, hints, and animations.
- `Assets/Scripts/Bootstrap.cs` — builds the Canvas at runtime and wires `GameState` to `GameUI`.
- `Assets/Editor/SceneSetup.cs` — creates `Main.unity` programmatically.
- `Assets/Editor/BuildScript.cs` — `BuildScript.BuildWebGL()` builds the WebGL player into `docs/` for GitHub Pages.

## Building from source

```
"<UnityEditorPath>/Unity" -batchmode -quit -projectPath . -executeMethod MergeMortemEditor.BuildScript.BuildWebGL -logFile build.log
```

Output is written to `docs/` with WebGL compression disabled, so it can be served directly by GitHub Pages.
