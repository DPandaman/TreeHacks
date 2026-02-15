# RealityRip | TreeHacks 2026

**Team Members:**
* [Member 1 Name]
* [Member 2 Name]
* [Member 3 Name]
* [Member 4 Name]

---

## 🎯 Objective
An AI-native drone flight simulator that combines high-fidelity **Gaussian Splatting** environments with a real-time **Generative AI Flight Commentator**. We bridge the gap between realistic drone physics and interactive, personality-driven feedback to create a more engaging pilot training experience.

---

## 🚀 Features

### 🧠 Real-Time Generative Commentary
Using **OpenAI's GPT-4o**, the simulator monitors drone physics (velocity, angular acceleration, proximity) to provide live feedback.
* **Persona:** A sarcastic Gen Z flight instructor.
* **Context-Aware:** Detects events such as smooth turns, crashes, near misses, and idling/stuck.

### 📍 Semantic Landmark Navigation
Users can map a 3D space and set goal landmarks. The **Goal Manager** tracks progress and triggers celebratory (or mocking) commentary upon arrival.
* **Dynamic Goals:** Fly to the "Piano," "Kitchen," or "Tree" based on the environment scan.

### 🎮 Physics-Driven FPV Controller
Custom C# controller designed for the **RadioMaster Pocket** (or standard gamepads).
* **Acro-Lite Mode:** Includes stabilization logic to help beginners navigate complex 3D scans.
* **Precision Physics:** Built on Unity’s Rigidbody system with specific lift and torque calculations.

### ✨ Gaussian Splatting Environments
Instead of traditional low-poly assets, we use 3D reconstructions of real-world spaces, providing an immersive training ground for pilots.

---

## 🛠 Technical Stack
* **Engine:** Unity (C#)
* **AI Engine:** OpenAI API (GPT-4o)
* **Input:** Unity Input System (RadioMaster/Joystick support)
* **Environment:** Gaussian Splatting / 3D Reconstruction

---

### 🌐 Digital Twin Integration
The simulator functions as a **Digital Twin** of real-world environments. By utilizing Gaussian Splatting for spatial accuracy and real-time telemetry-driven AI, we create a closed-loop system where:
* **Spatial Fidelity:** The drone navigates an exact 1:1 replica of a physical room.
* **Physics Mirroring:** Rigidbody dynamics simulate real-world flight constraints (mass, drag, and thrust-to-weight ratios).
* **AI Observer:** The generative commentator acts as a digital twin of a flight instructor, providing objective telemetry analysis disguised as personality-driven feedback.

---

## 📐 Mathematical Triggers 
We use real-time physics data to drive the AI's understanding of the flight:
* **Jerk Detection:** $\Delta$ Angular Velocity / $\Delta$ Time (detects snapping turns).
* **Upside-Down Check:** Dot product of `transform.up` and `Vector3.down`.
* **Proximity Sensing:** Forward and side-channel `Raycasting` for "near miss" and "tight gap" detection.

---

## 📦 Setup & Installation
1. **Clone the repository.**
2. **Unity Version:** Open in Unity 2022.3+ or Unity 6.
3. **API Configuration:** Select the `AI_Manager` object.
   * Paste your `OPENAI_API_KEY` into the Inspector field.
4. **Hardware:** Connect your RadioMaster Pocket via USB-C (Game Controller mode).
5. **Press Play** and fly!

---
*Developed for TreeHacks 2026 @ Stanford University.*