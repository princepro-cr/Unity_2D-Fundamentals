# Zone Runner — 2D Platformer

A polished 2D platformer developed in Unity as part of a game development assignment. Players navigate through three distinct environments — Forest, Savanna, and Space — collecting coins, defeating enemies with a spike throw mechanic, and surviving increasingly challenging platforming sections.

---

## Features

### Gameplay
- Smooth, responsive player movement with jump feel (fall multiplier, short hop vs full jump)
- Spike throw mechanic — press **F** to throw a projectile that destroys enemies
- Coin collection system with win condition
- Lives system (3 lives) with Game Over condition
- Kill counter tracking enemies defeated
- Checkpoint system per zone

### Environments
- 3 distinct zones — Forest, Savanna, Space — each with unique lighting, music, and ground materials
- Smooth zone transition system with crossfading music and lighting
- Platform surface labels (Mossy Ground, Wooden Platform, Alien Surface)
- Physics materials per zone — grippy forest, normal savanna, slippery space

### Camera
- SmoothDamp follow with dead zone to reduce jitter
- Look-ahead in movement direction so players see what's coming
- Look-down on fall so players see their landing
- Camera bounds clamped to level extents
- Screen shake on death

### Audio
- Zone-specific background music with smooth crossfade on zone transition
- Full SFX set: coin collect, jump, land, hurt, die, win, spike throw

### UI / HUD
- Coins, Lives, and Kills counters using TextMeshPro
- Platform label display
- Win and Game Over screens with session stats (coins collected, enemies defeated)
- Canvas Scaler set to Scale With Screen Size (1920×1080)

### Polish
- Coin collect particle burst effect
- Leg swing walking animation
- Angry character design with eyebrow and mouth expressions
- Firefly particle ambience in the forest zone

---

## Technologies Used

- Unity 2D (URP)
- C#
- TextMeshPro
- Unity Physics Engine 2D
- Unity Particle System
- Unity Animation System
- Unity Audio Mixer

---

## Controls

| Action | Key |
|---|---|
| Move Left | A / Left Arrow |
| Move Right | D / Right Arrow |
| Jump | Space |
| Throw Spike | F |

---

## Getting Started

### Requirements
- Unity Hub
- Unity Editor (2022.3 LTS or newer recommended)

### Installation
1. Clone the repository
```bash
git clone https://github.com/yourusername/your-repository-name.git
```
2. Open the project using Unity Hub
3. Open `Assets/Scenes/SampleScene`
4. Press Play

---

## Project Structure

```
Assets/
├── Audio/
│   ├── Music/          # Zone background tracks
│   └── SFX/            # Sound effects
├── Animations/         # Animation clips and controllers
├── Prefabs/
│   ├── Collectibles/   # Coin prefab
│   ├── Enemies/        # SlimeEnemy prefab
│   └── Spike/          # Spike projectile prefab
├── Scenes/             # SampleScene
├── Scripts/
│   └── Managers/       # GameManager, ZoneLightingController, CheckpointManager
└── PhysicalMaterial2D/ # Physics materials per zone
```

---

## Game Design Highlights

- **Zone progression** — coins placed as breadcrumbs to guide players left to right through each zone
- **Difficulty ramp** — Forest (ground level, easy jumps) → Savanna (staircase platforms) → Space (large gaps, slippery surface)
- **Risk/reward** — coins placed near enemies reward players who use the spike throw
- **Audio feedback** — every player action has a corresponding sound effect for maximum responsiveness

---

## Future Improvements

- Additional levels and zones
- Mobile touch controls
- Enemy patrol AI improvements
- Power-ups (double jump, rapid fire spikes)
- Main menu and loading screen
- Leaderboard / high score system

---

## Developer

**Promise Semosa**  
Information Technology student passionate about software development, game development, mobile applications, and UI/UX design.

---

## License

This project is for educational and portfolio purposes.
