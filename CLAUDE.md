# BUBBLE DASH - PROJECT CONTEXT

## PROJECT OVERVIEW
**Game Name**: Bubble Dash  
**Genre**: Idle Bubble Shooter + Endless Runner Hybrid  
**Platform**: Android ONLY (Google Play) - No iOS/Mac development  
**Development Timeline**: Weekend Project (48-72 hours)  
**Team**: Solo Developer (Programmer only)  
**Target**: Generate revenue from day 1

## CURRENT STATUS
- ✅ Market research completed
- ✅ Strategic plan defined
- 🔄 Documentation structure created
- ⏳ Core development pending
- ⏳ Monetization integration pending
- ⏳ Art assets integration pending

## CORE GAME CONCEPT
**Unique Selling Point**: Hybrid game combining bubble shooting with idle progression
- Players shoot bubbles while character auto-advances
- "Momentum System": More consecutive bubble pops = faster character movement
- Idle progression continues when offline
- Daily challenges and prestige system for retention

## TECHNICAL SPECIFICATIONS
- **Engine**: Unity 2022.3 LTS
- **Target Platform**: Android API 21+
- **Art Style**: Minimalist/Low Poly (using free Unity assets)
- **Monetization**: Hybrid (Ads 70% + IAP 25% + Battle Pass 5%)

## KEY DIFFERENTIATORS
1. **Momentum System**: Unique mechanic linking bubble popping to movement speed
2. **Idle Integration**: Offline progression keeps players engaged
3. **Minimalist Aesthetic**: Feature, not limitation
4. **Multi-monetization**: Multiple revenue streams from launch

## IMMEDIATE PRIORITIES
1. **Core Mechanics Implementation**: Bubble shooting + character movement
2. **Gameplay Loop**: Score system + combo mechanics
3. **Basic UI**: Functional interface for testing
4. **Monetization Framework**: Ads + IAP integration setup

## DEVELOPMENT GUIDELINES FOR CLAUDE
When working on this project:

### 1. ALWAYS READ FIRST
- Check `/Documentation/Development/current-sprint.md` for active tasks
- Review `/Documentation/Technical/architecture.md` for code structure
- Consult `/Documentation/Strategy/game-design.md` for design decisions

### 2. CODE STANDARDS
- Follow Unity C# conventions
- Use meaningful component names (PlayerController, BubbleManager, etc.)
- Comment only when business logic is complex
- Prioritize performance for mobile (object pooling, efficient updates)

### 3. ART CONSTRAINTS
- **NO custom art creation** - use only free Unity Asset Store resources
- Maintain consistent minimalist style
- 3-4 color palette maximum
- Low poly aesthetic for performance

### 4. DECISION PRIORITY
1. **Functionality first**: Get it working
2. **Mobile performance**: Keep it smooth
3. **Monetization ready**: Prepare for ads/IAP
4. **Polish last**: Only after core works

### 5. TESTING APPROACH
- Test on mobile resolution (1080x1920)
- Verify touch controls work properly
- Check performance on lower-end devices
- Validate monetization integration points

## PROJECT STRUCTURE
```
FirstArcade/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # Game mechanics
│   │   ├── UI/             # Interface controllers
│   │   ├── Monetization/   # Ads & IAP
│   │   └── Managers/       # Game state management
│   ├── Prefabs/           # Reusable objects
│   ├── Materials/         # Minimalist materials
│   └── Scenes/           # Game scenes
├── Documentation/        # Project documentation
└── CLAUDE.md            # This context file
```

## CRITICAL SUCCESS METRICS
- **Day 1**: Core mechanics working
- **Day 2**: Complete gameplay loop
- **Day 3**: Monetization + Polish
- **Week 1**: Android build ready
- **Month 1**: 1,000 downloads, $50 revenue

## EMERGENCY PROTOCOLS
If stuck for >2 hours on any feature:
1. Simplify the approach
2. Check Unity Asset Store for solutions
3. Implement minimal viable version
4. Document for post-launch improvement

## REVENUE PROJECTIONS
- **Conservative**: $10-30/day at 1K DAU
- **Realistic**: $50-150/day at 5K DAU
- **Optimistic**: $100-300/day at 10K DAU

---
**Last Updated**: 2024-12-30  
**Next Milestone**: Core Mechanics Implementation  
**Priority**: HIGH - Start development immediately