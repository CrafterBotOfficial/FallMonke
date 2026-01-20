<h1 align="center">FallMonke</h1>
<p align="center">
  <strong>FallMonke</strong> is a plugin/mod for <em>Gorilla Tag</em>.  
  It adds a whole new gamemode to Gorilla Tag, inspired by the Hex-a-gone gamemode from Fall Guys.
</p>

<p align="center">
  <img src="https://github.com/CrafterBotOfficial/FallMonke/blob/master/Marketing/HowToPlay.gif?raw=true" alt="FallMonke thumbnail" img width="auto" height="auto">
</p>

---

## Requirements
- Latest version of Utilla
- MonkeNotificationLib (Included)

## Build
To build Fall Monke from source use the following commands:
```bash
git clone https://github.com/CrafterbotOfficial/FallMonke.git
cd FallMonke
setx GORILLATAG_PATH "C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag" # For Windows only
dotnet build
dotnet build -c Release
```
or
```bash
git clone https://github.com/CrafterbotOfficial/FallMonke.git
cd FallMonke
dotnet nuget add source https://git.crafterbot.com/api/packages/GorillaTagModding/nuget/index.json
CI=TRUE dotnet build -c Release -o . # Note: This will not copy the mod into your plugins folder
```

## Legal
Use FallMonke at your own risk.

This product is not affiliated with Another Axiom Inc. or its videogames Gorilla Tag and Orion Drift and is not endorsed or otherwise sponsored by Another Axiom. Portions of the materials contained herein are property of Another Axiom. Â©2021 Another Axiom Inc. - https://www.anotheraxiom.com/fan-content-and-mod-policy
