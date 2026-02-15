# RealityRip | TreeHacks 2026

**Team Members:**
* Devanshu Pandya (UIUC)
* Julia Jiang (Stanford)
* Koichi Kimoto (Stanford)
* Rohan Godha (Georgia Tech)

## 🎯 Objective
An AI-native drone flight simulator that leverages Gaussian Splatting and Vision-Language Models (VLM) to automate disaster response training. By combining high-fidelity 3D reconstructions with local LLM-driven spatial intelligence, RealityRip transforms static scans into dynamic, mission-critical obstacle courses. 

## 💫 Motivation 
Traditional flight simulators rely on manual level design, which cannot scale to the unpredictability of disaster zones. Following events like earthquakes or structural collapses, first responders need immediate, accurate digital twins to plan rescue routes. RealityRip automates this by using VLMs to "see" a room scan, identify survivors/hazards, and generate optimal flight paths—all before a pilot even takes off.

## 🚀 Features

### 🧠 Semantic Mission Generation (VLM-Driven)
The core of RealityRip is its **Automated Goalpost System**. 
* **User Intent Prompting:** At startup, the system prompts the user for their motives (e.g., "Search for survivors in an earthquake zone"). 
* **Automated Placement:** Using a **local VLM (LLaVA/Moondream)** running on the **NVIDIA GX10**, the system identifies semantic anchors in the Gaussian Splat (e.g., "void under a collapsed table") and instantiates 3D goalposts automatically based on the user's intent.
* **Autonomous Pathfinding:** The system computes a smooth **Bezier Spline** trajectory through identified goals. If no specific path direction is provided, the **Mission Controller** generates a $C^1$ continuous optimal path for maximum search efficiency.

### 📍 Gaussian Splatting & Digital Twins
Instead of traditional low-poly assets, we use 3D reconstructions of real-world spaces.
* **Spatial Fidelity:** Navigates an exact 1:1 replica of physical environments.
* **Semantic Grounding:** The VLM identifies "Kitchens," "Pianos," or "Debris" directly from the splat data.


### 🎙️ Multi-Map Dynamic Commentary
* **Vibe Customization:** Users can toggle the commentator’s personality (e.g., "Gen Z Hacker" vs. "Tactical Dispatcher").
* **Map-Aware Personas:** * **Disaster Map:** High-stakes, serious, and instructive feedback focused on structural integrity and survivor safety.
    * **Stanford Engineering Quad:** Technical, O-notation roasts, and Stanford-specific humor.

### 🎮 Physics-Driven FPV Controller
Custom C# controller designed for the **RadioMaster Pocket** and standard gamepads.
* **Acro-Lite Mode:** Stabilization logic helps beginners navigate tight gaps in 3D scans.
* **Local Inference:** All AI commentary and goal generation are processed locally on the **GX10** to minimize latency and ensure data privacy in field operations.


## 🛠 Technical Stack
* **Engine:** Unity (C#) + **Unity Splines Package**
* **Local AI Engine:** Ollama / LocalAI (Llama 3, LLaVA-v1.5)
* **Hardware:** NVIDIA GX10 (Edge Inference), RadioMaster Pocket
* **Environment:** Gaussian Splatting 

## 🌐 Digital Twin Integration
The simulator functions as a **Digital Twin** of real-world environments. By utilizing Gaussian Splatting for spatial accuracy and real-time telemetry-driven AI, we create a closed-loop system where:
* **Spatial Fidelity:** The drone navigates an exact 1:1 replica of a physical room.
* **Physics Mirroring:** Rigidbody dynamics simulate real-world flight constraints (mass, drag, and thrust-to-weight ratios).
* **AI Observer:** The generative commentator acts as a digital twin of a flight instructor, providing objective telemetry analysis disguised as personality-driven feedback.


## 📐 Mathematical Triggers 
We use real-time physics data to drive the AI's understanding of the flight:
* **Jerk Detection:** $\Delta$ Angular Velocity / $\Delta$ Time (detects snapping turns).
* **Upside-Down Check:** Dot product of `transform.up` and `Vector3.down`.
* **Proximity Sensing:** Forward and side-channel `Raycasting` for "near miss" and "tight gap" detection.


## 📦 Setup & Installation
1. **Clone the repository.**
2. **Unity Version:** Open in Unity 2022.3+ or Unity 6.
3. **API Configuration:** Ensure you have your OpenAI API key set as an environment variable (OPENAI_API_KEY) on your system. The AIService.cs script will pull this automatically.
4. **Hardware:** Connect your RadioMaster Pocket via USB-C (Game Controller mode).
5. **Press Play** and fly!

---
*Developed for TreeHacks 2026 @ Stanford University.*