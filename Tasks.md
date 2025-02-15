# ORNITHO Prototype Tasks

## Player Character Implementation

### Movement Mechanics
- [x] #48 Player movement: Walking (Implemented in PlayerMovement.cs)
- [x] #49 Player movement: Running (Implemented in PlayerMovement.cs)
- [x] #50 Player movement: Crouching (Implemented in PlayerMovement.cs)
- [x] #51 Player movement: Walk while crouching (Implemented in PlayerMovement.cs)
- [x] #52 Player movement: Swimming (Implemented in PlayerMovement.cs with WaterZone.cs)
- [x] #54 Player movement: Turning around quickly (Implemented in MouseLook.cs)
- [ ] #53 Player trigger: Opening a door

### Visual and Audio Feedback
- [ ] #60 Player character temporary POC sounds
- [ ] List of human NPC sounds (POC)
- [ ] Character sounds (INCOMPLETE)

## Dinosaur AI Implementation

### Utahraptor Behavior
- [x] #44 Ambush predator stalking; waiting for the player to get in the light (Implemented in UtahraptorAI.cs)
- [x] #45 Aggressive trigger: Running after the player (Implemented in UtahraptorAI.cs)
- [x] #46 Close-by visual cone (Implemented in UtahraptorAI.cs)
- [x] #47 Far-away visual cone (Implemented in UtahraptorAI.cs)

### Environment Interaction
- [ ] #55 Flickering light dinosaur jumpscare
- [ ] #56 Environmental changes: Rain
- [ ] #57 Environmental changes: Rising water level due to rain

## Sound Design Implementation
- [ ] #59 Spinosaurus temporary POC sounds (Story Points: 5 - "1 day")
- [ ] #61 Opening a door temporary POC sound (Story Points: 1 - "1 hour")
- [ ] #62 Main music temporary POC sounds (Story Points: 1 - "1 hour")

## Environment and Lighting
- [ ] #82 POC main light (Story Points: 3 - "Half a day")
- [ ] #83 Flickering light (Story Points: 2 - "2 hours")

## Implementation Plan

### Batch 1 - Swimming and Water Environment ✓
1. ✓ Create water area with proper colliders and triggers
2. ✓ Implement swimming mechanics in PlayerMovement.cs
3. ✓ Add water effects and physics
4. ✓ Position Spinosaurus near water area

### Batch 2 - Sound Implementation (Next)
1. Set up audio manager
2. Implement basic sound effects
3. Add ambient music system
4. Create door interaction sounds

### Batch 3 - Lighting and Environment
1. Implement main lighting system
2. Add flickering light effects
3. Create rain particle system
4. Implement water level changes

## Notes
- Movement mechanics are complete, including swimming
- AI behavior systems are functional
- Focus needed on sound design implementation
- Lighting system needs both main and dynamic (flickering) implementation
- Water environment and swimming mechanics are now implemented 