# Reads data from Screeps and Display it in a grafana server
Made for a very easy usage.

![Screenshot](https://i.imgur.com/37MNA0o.png "Screenshot")

## Setup

1. Clone the project
2. Copy `.env.example` into a `.env` file. Edit this file and replace XXXXX with what you want your admin password to be.
3. Copy `.env.server.example` into `.env.screepsplus`
4. Update this file to connect to your server (it's already setup for screepsplus) and update the credentials
5. Start the containers with `docker-compose up`
6. Connect to grafana on localhost:9120 with the username admin and the password you have setup.
7. Add a new Datasource named graphite with the url `http://graphite`

![Graphite](https://i.imgur.com/0G4qdC7.png)

Now you can put any data you want in your `Memory.stats` object and it will be fed to grafana.

## Premade Screeps Script

1. You can use this premade script to automatically create your `Memory.stats` object with some useful information
https://raw.githubusercontent.com/devnixs/screeps-stats-grafana/master/scripts/stats-manager.ts
2. Use it with 
```typescript
import {StatsManager} from 'stats-manager.ts'
StatsManager.runForAllRooms();
```

## Premade Grafana Dashboard

1. You can use this premade grafana dashboard to display all the important metrics
https://raw.githubusercontent.com/devnixs/screeps-stats-grafana/master/scripts/grafana-dashboard.json

2. Simply import it as a new dashboard in grafana.

![Import](https://i.imgur.com/qDQs3ew.png)
