# 🪙 COIN PREFAB SETUP GUIDE

## Creating the Coin Prefab

### 1. GameObject Setup
1. Create Empty GameObject: **GameObject > Create Empty**
2. Name it: **"CoinPrefab"**
3. Reset Transform: Position (0,0,0), Rotation (0,0,0), Scale (1,1,1)

### 2. Visual Components
1. **Add Sprite Renderer**:
   - Add Component > Rendering > Sprite Renderer
   - Sprite: Use any circular/coin sprite from free assets
   - Color: Yellow (#FFD700) or Gold (#FFC107)
   - Sorting Layer: "Pickups" or "Default"
   - Order in Layer: 5 (above bubbles)

### 3. Add Scripts
1. **Coin Script**:
   - Add Component > Scripts > Coin
   - Settings:
     - Coin Value: 1
     - Rotation Speed: 180
     - Bob Speed: 2
     - Bob Height: 0.2
     - Magnet Range: 3
     - Magnet Speed: 8
     - Lifetime: 20

### 4. Collider Setup
1. **CircleCollider2D** (auto-added by script):
   - Is Trigger: ✓
   - Radius: 0.3

### 5. Optional Visual Polish
1. **Particle System** (Optional):
   - Add as child GameObject
   - Name: "CoinSparkle"
   - Settings:
     - Duration: 5
     - Looping: ✓
     - Start Lifetime: 1
     - Start Speed: 0.5
     - Start Size: 0.1
     - Emission Rate: 5
     - Shape: Circle, Radius 0.3
     - Color: Yellow gradient

2. **Trail Renderer** (Optional):
   - Add Component > Effects > Trail Renderer
   - Time: 0.2
   - Width: 0.1 to 0
   - Material: Sprites-Default
   - Color: Yellow fade to transparent

### 6. Save as Prefab
1. Drag to: **Assets/Prefabs/Gameplay/**
2. Name: **"CoinPrefab"**

## Configuring CoinSystem

### 1. Find CoinSystem in Scene
- Should be on a GameObject (possibly GameManager or separate)

### 2. Assign Coin Prefab
- Drag **CoinPrefab** to `Coin Prefab` field
- Configure spawn settings:
  - Coin Spawn Chance: 0.3 (30%)
  - Min Coins Per Spawn: 1
  - Max Coins Per Spawn: 3
  - Coin Spacing: 1
  - Use Patterns: ✓

## Testing Coins

### 1. Play Mode Tests
- [ ] Coins spawn with obstacles
- [ ] Coins rotate and bob
- [ ] Magnetic attraction works within range
- [ ] Collection adds to coin counter
- [ ] UI updates correctly
- [ ] Sound plays on collection
- [ ] Visual effect on collection
- [ ] Coins auto-destroy after lifetime

### 2. Visual Debugging
- In Scene view, selected coins show:
  - Yellow circle = Magnet range
  - Gizmos help debug collection issues

## Common Issues

### Coins Not Spawning
- Check CoinSystem has prefab assigned
- Verify spawn chance > 0
- Check ObstacleGenerator calls SpawnCoinsAtPosition

### Coins Not Collecting
- Ensure Player has "Player" tag
- Check coin collider Is Trigger = true
- Verify player has collider

### No Collection Effect
- Check SimpleEffects instance exists
- Verify ShowScorePopup works
- Check SimpleSoundManager exists

---
**Created**: Dec 31, 2024  
**Status**: Ready for Unity setup