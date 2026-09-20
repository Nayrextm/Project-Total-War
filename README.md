⚓Project Total War \ Naval Combat Prototype

📌 Overview
This project is a technical prototype focused strictly on robust systems engineering and clean code architecture. Rather than building a polished vertical slice with UI/UX, this repository serves as a sandbox for implementing advanced Object-Oriented Programming (OOP) principles, SOLID design, and highly optimized gameplay mechanics.

⚙️ Core Technical Focus
Clean Architecture: Strict adherence to SOLID principles. Game logic is fully decoupled using the Service Locator pattern, Dependency Injection (via entity initializers), and event-driven interfaces.

Advanced Weapon Systems: Custom Fire Control System (FCS) managing multiple turret controllers. Features include real-time ballistic trajectory calculations and State Machine-driven firing modes (Salvo, Sequential, Burst).

Camera & Visual Physics: Advanced camera handling using Cinemachine for seamless free-look targeting and dynamic framing. Implemented procedural visualization of ship behavior, including dynamic heeling (roll) mechanics during maneuvers based on vessel mass and rudder speed.

Performance & Optimization: Designed with Zero-GC (Garbage Collection) allocation in Update loops. Implements custom Object Pooling for projectiles and optimized target-finding fallbacks (e.g., using mathematical Plane.Raycast instead of physics colliders for water intersections).

Decoupled Input: A dedicated Input Service (Controller/Possession pattern) allows seamless switching between Player and AI control without altering the underlying ship systems.
