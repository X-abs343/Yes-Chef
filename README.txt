# Yes Chef! - Technical Prototype

A fast-paced kitchen management prototype built in Unity 6000.3.10f1 adhering to strict SOLID principles, scalable systems architecture, and responsive procedural gameplay juice.

---

## Gameplay & Controls
- WASD / Arrow Keys: Move the Chef
- E / Space: Interact / Pick up / Prepare / Deliver
- Esc: Pause / Resume

### Kitchen Stations
- Refrigerator: Dispenses raw ingredients (Vegetables, Cheese, Meat).
- Cutting Table: Prepares Vegetables (2.0s duration with active wobble).
- Stove: Features 2 independent burner slots for cooking Meat (6.0s duration with active sizzle).
- Customer Delivery Windows (4x): Shows required ingredients and waiting time. Scoring: $\text{Base Values} - \lfloor \text{Elapsed Seconds} \rfloor$.
- Trash Bin: Clears current held ingredient.

---

## Architecture & SOLID Principles

### 1. Single Responsibility Principle (SRP)
- PlayerController: Pure kinematic locomotion and physics handling.
- PlayerInventory: Manages single-slot item attachment, detachment, and lifecycle.
- PlayerVisualFeedback: Decoupled presentation layer handling procedural character animations (locomotion bob, stop-squash, celebration jump, and sad discard tilt).
- InputReader: Handles hardware abstraction via the New Input System and broadcasts C# events.

### 2. Open/Closed Principle (OCP)
- BaseStation: Encapsulates common workstation functionality, collision, and procedural interaction bump feedback. Concrete stations (`FridgeStation`, `CuttingStation`, `StoveStation`, `DeliveryWindow`, `TrashStation`) extend behavior without altering existing code.

### 3. Liskov Substitution Principle (LSP)
- All interactive kitchen surfaces inherit from `BaseStation` and implement `IInteractable`. Any station can seamlessly process `CanInteract(inventory)` and `Interact(inventory)` interchangeably.

### 4. Interface Segregation Principle (ISP)
- `IInteractable` isolates player-object interaction contracts from internal station data, progress tracking, or order evaluation logic.

### 5. Dependency Inversion Principle (DIP)
- `PlayerInteraction` depends entirely on high-level abstractions (`IInteractable`, `PlayerInventory`) rather than concrete station classes.
- UI systems subscribe to broadcast events (`OnScoreChanged`, `OnTimerTick`, `OnOrderDeliveredScore`) rather than polling game state inside `Update()`.

---

## Polish & Game Feel Highlights
- Procedural Character Juice: Sine-wave walking bob, dynamic squash-and-stretch on deceleration, and $360^\circ$ jump rotations on successful delivery.
- World-Space UI: Compact ticket cards with type-safe color-coded icons, countdown timers, and world-space cooking progress bars.
- Floating Score Feedback: Bouncy `+Score` (Green/Gold) and `-Score` (Red) popups over delivery windows.
- Camera Feedback: Screen shake on invalid interactions or negative score orders.

---

## Persistence & Delivery
- High Score: Serialized and persisted across sessions using `PlayerPrefs`.
- Target Engine: Unity 6000.3.10f1.