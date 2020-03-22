# Dynamic Ad Placement Tutorial Development Log


This document is a record of the development of a tutorial to show how to implement Dynamic Ad Placemment with deltaDNA

Goals
* Use simple game to demonstrate concept
* Add Unity Ads 
* Add deltaDNA
* Use deltaDNA to achieve 
    * Dynamic Ad Placement
    * Dynamic Ad Rewards
    * Personalised Placement and Rewards to targeted player segments.

## 1.0.0  Create Empty Project
- Added GitHub repository for project
- Created Empty Unity Project

## 1.0.1 Create Simple Game
- Setup Camera and Canvas
- Setup Playfield
- Setup HUD Displays
- Setup Main Menu Text
- Setup StartButton and Game State
- Load Levels
- Populate and Start Level
- Initialise Hud
- Snake Movement
- Add Bodyparts
- Collision Detection
- Eat Food
- Level Progression
- Game Over and Reset

Minimum Viable Simple Game achieved. 
There are still more game mechanics to implement e.g. (coins, poison time limits etc..). However, there is enough functionality to move forward and implement deltaDNA analysis, Unity Ads and deltaDNA CRM to personalize the user experience. 

## 1.0.2 Add Basic deltaDNA event recording. 
- Add game to deltaDNA. 
- Download, Add and Start deltaDNA SDK
- Automated session and device events recording (gameStarted & clientDevice)