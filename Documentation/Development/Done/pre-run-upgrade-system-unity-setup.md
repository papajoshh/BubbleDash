# PRE-RUN UPGRADE SYSTEM - UNITY IMPLEMENTATION GUIDE

## 🎯 OVERVIEW
Esta guía implementa el **nuevo sistema de upgrades pre-run** donde los jugadores preparan su build ANTES de empezar la partida, no durante el gameplay.

---

## 🔄 NEW FLOW SUMMARY

```
🏠 Main Menu → ⬆️ Upgrades (Pre-Run) → 🎮 Start Game → 🎯 Pure Gameplay → 💀 Game Over → 🏠 Back to Menu
```

### ✅ BENEFITS:
- **No timer pressure** durante decisiones de upgrade
- **Economía lógica** - gastas coins de runs anteriores
- **Gameplay puro** - durante run solo te concentras en jugar
- **UX más limpia** - separación clara preparación vs ejecución

---

## 📋 PREREQUISITES
- ✅ Timer system implementado (Sprint 1)
- ✅ UpgradeSystem, CoinSystem funcionando
- ✅ Scripts actualizados con nuevos cambios
- ✅ DOTween en el proyecto

---

## 🏠 STEP 1: CREATE MAIN MENU UI

### 1.1 Main Menu Panel Container
```
1. En UI Canvas: Right-click → UI → Panel
2. Rename a "MainMenuPanel"
3. Configure Panel Image:
   ├── Color: Dark Blue Semi-transparent (0, 50, 100, 200)
   └── Raycast Target: ✓ checked
4. Configure RectTransform:
   ├── Anchor: Stretch (preset)
   ├── Left: 0, Top: 0, Right: 0, Bottom: 0
   └── Pivot: 0.5, 0.5
```

### 1.2 Background Image (Optional)
```
1. En MainMenuPanel: Right-click → UI → Image
2. Rename a "BackgroundImage"
3. Configure Image:
   ├── Source Image: (gradient or game logo background)
   ├── Color: Light Blue (100, 150, 255, 100)
   └── Image Type: Simple
4. Configure RectTransform:
   ├── Anchor: Stretch (preset)
   ├── Left: 0, Top: 0, Right: 0, Bottom: 0
   └── Send to Back: ✓
```

### 1.3 Title Container
```
1. En MainMenuPanel: Right-click → UI → Empty
2. Rename a "TitleContainer"
3. Configure RectTransform:
   ├── Anchor: Top-Center (preset)
   ├── Pos X: 0, Pos Y: -100
   ├── Width: 400, Height: 150
   └── Pivot: 0.5, 1
```

### 1.4 Game Title Text
```
1. En TitleContainer: Right-click → UI → Text - TextMeshPro
2. Rename a "GameTitleText"
3. Configure TextMeshPro:
   ├── Text: "BUBBLE DASH"
   ├── Font Size: 48
   ├── Alignment: Center Middle
   ├── Color: White (255, 255, 255, 255)
   ├── Font Style: Bold
   └── Auto Size: Off
4. Configure RectTransform:
   ├── Anchor: Stretch (preset)
   ├── Left: 0, Top: 0, Right: 0, Bottom: 0
   └── Pivot: 0.5, 0.5
```

### 1.5 Buttons Container
```
1. En MainMenuPanel: Right-click → UI → Empty
2. Rename a "ButtonsContainer"
3. Configure RectTransform:
   ├── Anchor: Center (preset)
   ├── Pos X: 0, Pos Y: 0
   ├── Width: 300, Height: 400
   └── Pivot: 0.5, 0.5
4. Add Component: Vertical Layout Group
   ├── Spacing: 20
   ├── Child Alignment: Upper Center
   ├── Control Child Size Width: ✓ checked
   ├── Control Child Size Height: ❌ unchecked
   ├── Use Child Scale: ✓ checked
   └── Child Force Expand: Width ✓, Height ❌
```

### 1.6 Menu Buttons
```
Para cada botón (StartGame, Upgrades, Settings, Quit):

1. En ButtonsContainer: Right-click → UI → Button - TextMeshPro
2. Rename a "[Action]Button" (ej: "StartGameButton")
3. Configure Button:
   ├── Interactable: ✓ checked
   ├── Transition: Color Tint
   ├── Normal Color: Blue (100, 150, 255, 255)
   ├── Highlighted Color: Light Blue (150, 200, 255, 255)
   ├── Pressed Color: Dark Blue (50, 100, 200, 255)
   └── Selected Color: Blue (100, 150, 255, 255)
4. Configure Button Text:
   ├── Text: "🎮 START GAME" / "⬆️ UPGRADES" / "⚙️ SETTINGS" / "❌ QUIT"
   ├── Font Size: 20
   ├── Alignment: Center Middle
   ├── Color: White (255, 255, 255, 255)
   └── Font Style: Bold
5. Add Component: Layout Element
   ├── Min Height: 60
   └── Preferred Height: 60

Crear los 4 botones:
- StartGameButton: "🎮 START GAME"
- UpgradesButton: "⬆️ UPGRADES"
- SettingsButton: "⚙️ SETTINGS"
- QuitButton: "❌ QUIT"
```

### 1.7 Info Container
```
1. En MainMenuPanel: Right-click → UI → Empty
2. Rename a "InfoContainer"
3. Configure RectTransform:
   ├── Anchor: Bottom-Center (preset)
   ├── Pos X: 0, Pos Y: 50
   ├── Width: 600, Height: 100
   └── Pivot: 0.5, 0
4. Add Component: Vertical Layout Group
   ├── Spacing: 10
   ├── Child Alignment: Middle Center
   └── Child Force Expand: Width ❌, Height ❌
```

### 1.8 Info Texts
```
1. Total Coins Text:
   En InfoContainer: Create TextMeshPro
   ├── Name: "TotalCoinsText"
   ├── Text: "💰 Coins: 1,247"
   ├── Font Size: 24
   ├── Alignment: Center Middle
   ├── Color: Yellow (255, 255, 0, 255)
   └── Font Style: Bold

2. High Score Text:
   En InfoContainer: Create TextMeshPro
   ├── Name: "HighScoreDisplayText"
   ├── Text: "🏆 Best Score: 15,432"
   ├── Font Size: 18
   ├── Alignment: Center Middle
   ├── Color: Light Gray (200, 200, 200, 255)
   └── Font Style: Normal

3. Version Text:
   En InfoContainer: Create TextMeshPro
   ├── Name: "GameVersionText"
   ├── Text: "v1.0.0"
   ├── Font Size: 14
   ├── Alignment: Center Middle
   ├── Color: Gray (150, 150, 150, 255)
   └── Font Style: Normal
```

### 1.9 Add MainMenuUI Component
```
1. Select MainMenuPanel GameObject
2. Add Component: MainMenuUI.cs
3. Configure en Inspector:
   ├── Main Menu Panel: Drag MainMenuPanel (self)
   ├── Start Game Button: Drag StartGameButton
   ├── Upgrades Button: Drag UpgradesButton
   ├── Settings Button: Drag SettingsButton
   ├── Quit Button: Drag QuitButton
   ├── Total Coins Text: Drag TotalCoinsText
   ├── Game Version Text: Drag GameVersionText
   ├── High Score Display Text: Drag HighScoreDisplayText
   ├── Game Title Text: Drag GameTitleText
   ├── Logo Image: (opcional, si agregaste imagen)
   ├── Button Animation Delay: 0.1
   └── Title Animation Duration: 1
```

---

## ⬆️ STEP 2: UPDATE UPGRADE SYSTEM

### 2.1 Modify Upgrade Panel Access
```
1. Select UpgradePanel GameObject (debe existir de implementación anterior)
2. Verify UpgradeUI.cs component está configurado
3. NO CHANGES needed en la estructura - solo comportamiento
```

### 2.2 Remove Upgrade Button from Gameplay HUD
```
1. Select UIManager GameObject
2. En UIManager component:
   ├── upgradeButton field should be empty/removed
   └── No upgrade button reference needed
3. Remove/Delete upgrade button from HUD if exists
```

### 2.3 Test Upgrade Flow
```
TESTING CHECKLIST:
✅ Main Menu shows upgrade button
✅ Clicking upgrades opens upgrade menu
✅ Upgrade menu shows coin count
✅ Can purchase upgrades with coins
✅ Closing upgrade menu returns to main menu
✅ Starting game applies purchased upgrades
❌ NO upgrade access during gameplay
```

---

## 💀 STEP 3: UPDATE GAME OVER SCREEN

### 3.1 Remove Upgrades Button from GameOverTimerUI
```
1. Select GameOverTimerPanel GameObject
2. En ButtonsContainer:
   ├── DELETE "UpgradesButton" if exists
   ├── Keep only: PlayAgainButton, MainMenuButton
   └── Adjust layout spacing if needed
3. En GameOverTimerUI component:
   ├── Remove upgradesButton reference
   ├── Should only have: playAgainButton, mainMenuButton
   └── Save changes
```

### 3.2 Update Button Layout
```
En GameOverTimerPanel → ButtonsContainer:
1. Adjust Horizontal Layout Group:
   ├── Spacing: 30 (más espacio entre 2 botones)
   ├── Child Alignment: Middle Center
   └── Child Force Expand: Width ✓, Height ❌
2. Verify button sizes look balanced
```

---

## 🎮 STEP 4: INTEGRATE WITH GAME FLOW

### 4.1 Setup Initial State
```
1. En Hierarchy: 
   ├── MainMenuPanel: SetActive(true) initially
   ├── UpgradePanel: SetActive(false) initially
   ├── GameOverTimerPanel: SetActive(false) initially
   └── HUD elements: SetActive(false) initially

2. Game should start showing Main Menu by default
```

### 4.2 Configure GameManager Integration
```
1. Select GameManager GameObject
2. Verify currentState starts as "Menu"
3. autoStartGame should be FALSE (no auto-start)
4. Game starts only when MainMenu → Start Game clicked
```

### 4.3 Flow Testing
```
COMPLETE FLOW TEST:
1. 🏠 Game opens → Main Menu visible
2. ⬆️ Click Upgrades → Upgrade menu opens, coins visible
3. 💰 Purchase upgrades → Coins reduce, upgrades apply
4. ❌ Close upgrades → Returns to main menu
5. 🎮 Click Start Game → Game starts with upgrades applied
6. ⏱️ Play until timer expires → Game Over Timer screen
7. 🔄 Click Play Again → Game restarts (skip menu)
8. 🏠 Click Main Menu → Returns to main menu
```

---

## 🔧 STEP 5: CLEANUP & OPTIMIZATION

### 5.1 Remove Obsolete Elements
```
ELEMENTOS A ELIMINAR:
├── Upgrade button from gameplay HUD
├── upgradeButton references in UIManager
├── UpgradesButton from GameOverTimerUI
└── Any upgrade-related pause logic during gameplay

ELEMENTOS A MANTENER:
├── UpgradeSystem (funcional)
├── UpgradeUI (funcional, pero solo desde main menu)
├── Timer system (sin pausas para upgrades)
└── Coin system (funcional)
```

### 5.2 Verify Timer System
```
TIMER BEHAVIOR AFTER CHANGES:
✅ Timer starts when game starts
✅ Timer only pauses with ESC key (manual pause)
❌ Timer NO pausa para upgrade menus (ya no existen en gameplay)
✅ Game Over por timer funciona normal
✅ Head Start upgrade extiende timer correctamente
```

### 5.3 Audio & Visual Polish
```
AUDIO INTEGRATION:
1. Main menu button clicks play sound
2. Upgrade purchase plays coin sound
3. Game start plays start sound
4. Navigation sounds consistent

VISUAL POLISH:
1. Button hover animations work
2. Menu transitions smooth
3. Upgrade menu animations work
4. Game Over screen animations work
```

---

## 🧪 STEP 6: TESTING & VALIDATION

### 6.1 Complete Flow Testing
```
🏠 MAIN MENU TEST:
├── All buttons responsive
├── Coin count updates correctly
├── High score displays correctly
├── Version text shows
└── Animations play smoothly

⬆️ UPGRADE MENU TEST:
├── Opens from main menu only
├── Shows current coin count
├── Can purchase upgrades
├── Upgrades persist between sessions
├── Returns to main menu properly
└── NO ACCESS during gameplay

🎮 GAMEPLAY TEST:
├── Game starts from main menu
├── Upgrades applied correctly
├── Timer system works
├── NO upgrade button visible
├── Pure gameplay experience
└── ESC only pauses (no upgrade access)

💀 GAME OVER TEST:
├── Shows correct stats
├── Adds coins to total
├── Play Again works
├── Main Menu return works
└── NO upgrade button present
```

### 6.2 Performance Testing
```
MEMORY & PERFORMANCE:
├── No memory leaks from animations
├── Smooth transitions between states
├── UI responsive on mobile
├── No frame drops during menu transitions
└── DOTween cleanup proper
```

### 6.3 Edge Cases
```
EDGE CASE TESTING:
├── Start game with 0 coins (should work)
├── Buy max level upgrades (should disable buttons)
├── Rapid button clicking (should not break)
├── Back/forth between menus (should work smoothly)
└── Game Over → immediate Play Again (should work)
```

---

## 🎨 STEP 7: VISUAL CUSTOMIZATION (Optional)

### 7.1 Theme Consistency
```
COLOR SCHEME:
├── Primary: Blue (100, 150, 255)
├── Secondary: Light Blue (150, 200, 255)
├── Accent: Yellow (255, 255, 0) for coins
├── Text: White (255, 255, 255)
└── Background: Dark Blue (0, 50, 100, 200)
```

### 7.2 Enhanced Animations
```
MAIN MENU ENTRANCE:
├── Title slides down + bounce
├── Buttons scale in sequence
├── Coin count fades in
└── Background subtle pulse

UPGRADE MENU:
├── Panel scales in with bounce
├── Items slide in from right
├── Purchase button pulse
└── Coin decrease animation
```

---

## 🔧 TROUBLESHOOTING

### Common Issues:

**❌ Upgrade menu doesn't open from main menu:**
```
Solution: Check MainMenuUI.upgradesButton onClick listener
Verify: UpgradeUI.OpenUpgradeMenuFromMainMenu() is called
```

**❌ Coins don't update in main menu:**
```
Solution: Verify CoinSystem.OnCoinsChanged event subscription
Check: MainMenuUI.UpdateCoinDisplay() is called
```

**❌ Game Over screen still shows upgrade button:**
```
Solution: Update GameOverTimerUI prefab/setup
Remove: upgradesButton reference completely
```

**❌ Timer still pauses for non-existent upgrade menu:**
```
Solution: Remove PauseGameSilent() calls related to upgrades
Check: Only ESC key pauses game now
```

**❌ Upgrades purchased in main menu don't apply in game:**
```
Solution: Verify UpgradeSystem.ApplyStartingUpgrades() called in GameManager.StartGame()
Check: PlayerPrefs save/load for upgrades working
```

---

## ✅ SUCCESS CRITERIA

### Must Work:
- [ ] Main menu shows on game start
- [ ] Can access upgrades ONLY from main menu
- [ ] Upgrade purchases work and persist
- [ ] Start game applies upgrades correctly
- [ ] Game Over returns to main menu option
- [ ] NO upgrade access during gameplay
- [ ] Timer system unaffected by upgrade removal

### Should Work:
- [ ] Smooth animations between menus
- [ ] Coin count updates in real-time
- [ ] High score displays correctly
- [ ] All button interactions responsive

### Nice to Have:
- [ ] Sound effects for all interactions
- [ ] Visual polish and consistent theme
- [ ] Smooth performance on mobile

---

## 📚 FILE REFERENCES

**Scripts Created/Modified:**
- `/Assets/Scripts/UI/MainMenuUI.cs` (NEW)
- `/Assets/Scripts/UI/GameOverTimerUI.cs` (MODIFIED - removed upgrades button)
- `/Assets/Scripts/UI/UpgradeUI.cs` (MODIFIED - main menu integration)
- `/Assets/Scripts/UI/UIManager.cs` (MODIFIED - removed upgrade button)
- `/Assets/Scripts/Managers/GameManager.cs` (MINOR UPDATES)

**Documentation:**
- `/Documentation/Development/upgrade-flow-redesign.md` (Design rationale)
- `/Documentation/Development/pre-run-upgrade-system-unity-setup.md` (This guide)

---

**STATUS**: ✅ Ready for Unity Implementation  
**ESTIMATED TIME**: 60-90 minutes  
**COMPLEXITY**: Medium-High - Complete UI flow redesign  
**DEPENDENCIES**: Timer system, UpgradeSystem, CoinSystem functional

## 🎯 FINAL NOTES

Este nuevo flow mejora significativamente la UX:
- **Decisiones sin presión** de tiempo
- **Economía clara** - gastas lo que tienes de runs anteriores  
- **Gameplay puro** - run sin interrupciones
- **Separación clara** entre preparación y ejecución

El resultado es un roguelite más auténtico donde preparas tu build estratégicamente antes de cada run, en lugar de tomar decisiones apresuradas durante el gameplay.