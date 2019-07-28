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

Now you can put any data you want in your Memory.stats object and it will be fed to grafana.

