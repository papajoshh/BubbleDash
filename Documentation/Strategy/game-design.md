# BUBBLE DASH - GAME DESIGN DOCUMENT

## CORE CONCEPT

### Game Genre
**Hybrid**: Idle Bubble Shooter + Endless Runner
- Primary Mechanic: Bubble shooting with physics
- Secondary Mechanic: Automatic character progression
- Tertiary Mechanic: Idle/offline progression

### Target Audience
- **Primary**: Casual mobile gamers (25-45 años)
- **Secondary**: Idle game enthusiasts
- **Demographics**: Global, iOS/Android users with disposable income

## CORE GAMEPLAY LOOP

### Primary Loop (2-3 minutes)
1. **Shoot bubbles** to create combinations and clear obstacles
2. **Character advances** automatically, speed increases with successful shots
3. **Collect coins/gems** from bubble combinations and distance traveled
4. **Hit obstacle or miss too many shots** → Game Over
5. **Spend currency** on upgrades and return to step 1

### Secondary Loop (Daily)
1. **Complete daily challenges** for bonus rewards
2. **Check idle progression** and collect offline earnings
3. **Upgrade character/abilities** with accumulated resources
4. **Participate in events** or limited-time challenges

### Meta Loop (Weekly/Monthly)
1. **Prestige system**: Reset progress for permanent bonuses
2. **Seasonal content**: New bubble types, characters, or areas
3. **Leaderboards**: Compete for weekly/monthly rankings

## CORE MECHANICS

### 1. Bubble Shooting System
- **Physics-based trajectory** with aiming guide
- **Color matching**: 3+ same colors to pop
- **Special bubbles**: 
  - Rainbow (matches any color)
  - Bomb (clears area)
  - Multiplier (increases points)
  - Speed (boosts character movement)

### 2. Momentum System (UNIQUE SELLING POINT)
- **Base character speed**: 1x
- **Consecutive successful shots**: +0.1x speed per hit (max 3x)
- **Miss or hit obstacle**: Reset to base speed
- **Speed affects**: Distance scoring, coin collection rate, obstacle avoidance

### 3. Idle Progression
- **Offline earnings**: Character continues moving at reduced speed
- **Idle multipliers**: Upgradeable offline efficiency
- **Auto-shooter**: Purchasable automatic bubble shooting
- **Prestige bonuses**: Permanent improvements across resets

### 4. Obstacle System
- **Static obstacles**: Walls, barriers requiring specific bubble combinations
- **Moving obstacles**: Platforms, rotating barriers
- **Dynamic obstacles**: Generated based on player skill level
- **Escape mechanisms**: Special bubble combinations to avoid/destroy obstacles

## PROGRESSION SYSTEMS

### Character Upgrades
1. **Movement Speed**: Base speed increase
2. **Bubble Capacity**: Carry more special bubbles
3. **Aim Assist**: Better trajectory prediction
4. **Luck**: Higher chance for special bubbles
5. **Idle Efficiency**: Faster offline progression

### Prestige System
- **Reset requirement**: Reach certain distance milestone
- **Prestige currency**: Earned based on total distance/score
- **Permanent bonuses**: 
  - Starting speed bonus
  - Increased special bubble spawn rate
  - Higher coin multipliers
  - Exclusive character skins

### Daily Challenges
- **Distance goals**: "Travel 5000 meters"
- **Combo challenges**: "Get 10 consecutive hits"
- **Collection tasks**: "Collect 500 coins in single run"
- **Special combinations**: "Use 5 bomb bubbles in one game"

## MONETIZATION INTEGRATION

### Ad Placement Strategy
- **Rewarded Video**: 
  - Double coins after game over
  - Revive with full momentum
  - Unlock daily bonus chest
- **Interstitial**: Between games (every 3rd game over)
- **Banner**: In upgrade menus only

### In-App Purchase Integration
- **Premium Bubbles**: Special effects and abilities
- **Character Skins**: Cosmetic upgrades with minor stat bonuses
- **Currency Packs**: Coins and gems at discounted rates
- **Ad Removal**: $2.99 removes all non-rewarded ads
- **Starter Pack**: $0.99 early-game boost package

### Battle Pass Integration
- **Free Track**: 10 tiers with basic rewards
- **Premium Track**: $4.99, 20 tiers with exclusive content
- **Duration**: 4 weeks per season
- **Progression**: Points earned through distance and challenges

## TECHNICAL REQUIREMENTS

### Performance Targets
- **60 FPS** on mid-range Android devices (2019+)
- **30 FPS minimum** on low-end devices
- **Loading times**: <3 seconds between games
- **Memory usage**: <500MB RAM

### Platform Specific Features
- **Android**: Google Play Games integration, cloud save
- **Touch controls**: Intuitive aim and shoot mechanics
- **Haptic feedback**: On bubble pops and collisions
- **Portrait orientation**: One-handed gameplay

## USER EXPERIENCE PRINCIPLES

### Accessibility
- **Simple controls**: Tap to aim, release to shoot
- **Clear visual feedback**: Obvious bubble trajectories and combinations
- **Scalable UI**: Works on different screen sizes
- **Color-blind friendly**: Alternative indicators for bubble types

### Retention Mechanics
- **Quick sessions**: Games last 2-5 minutes maximum
- **Immediate gratification**: Satisfying bubble pop effects
- **Progression visibility**: Clear upgrade paths and benefits
- **Social features**: Leaderboards and achievement sharing

## RISK MITIGATION

### Technical Risks
- **Physics complexity** → Use Unity's built-in physics with optimization
- **Performance on old devices** → Adjustable quality settings
- **Touch responsiveness** → Extensive mobile testing

### Design Risks
- **Mechanic complexity** → Start simple, iterate based on feedback
- **Monetization balance** → A/B test ad frequency and pricing
- **Player retention** → Strong idle mechanics and daily rewards

---

**Design Philosophy**: "Easy to learn, satisfying to master, rewarding to return to"