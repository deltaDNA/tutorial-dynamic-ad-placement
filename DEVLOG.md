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
- Add some more events. sdkConfigured, missionStarted, missionCompleted, missionFailed

TODO - Come back later and add some more detail to mission events (cost, poison, snake length, snake speed ....)

## 1.0.3 Get Unity Ads installed and running.
- Install Unity Ads through Unity Package Manager
- Updated project to Unity 2019.3.1f1 to take advantage of latest Ads SDK
- Updated Unity Ads to v3.4.4 in Unity Package Manager
- Included Unity Ads library 
- Configured GameId, PlacementId and testmode
- Setup 4 x Ads listeners
- Initialized Unity Ads
- Added ShowAd() method, currently hardwired to occur on every failed mission.

## 1.0.4 Currency rewards and AdImpression events
- Implement currency (spend on levels, reward from ads and levels)
- Implement mission reward celebration 

## 1.0.5 DeltaDNA campaigns control Ad
- Ad Placement, Reward value, Frequency controlled dynamically.
- Create Game Parameters and Actions to send data to client
- Create Campaign to AB Test and send Ad control data to client
- Implement Callbacks in game and move ShowAd command inside the Campaign callback.
- Apply Reward Amount if player watched ad to completion
- Record adImpression event 

## 1.0.6 Clean Up
- Add mission cost and rewards to mission events.
- Add celebration coin drop 

