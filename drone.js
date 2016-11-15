var arDrone = require('ar-drone');
var keypress = require('keypress');

function Drone(ip) {
    this.client = arDrone.createClient(ip);

    this.parallel = 0;
    this.transverse = 0;
    this.lateral = 0;

    this.droneData = new Object();
    this.deltaTime;
    this.prevTime;
    this.curTime = Date.now();

    this.position = {
        x: 0,
        y: 0,
        z: 0
    }

    this.updateData = function(navData) {
        //update drone data
        this.droneData = navData;
        //calculate delta time
        this.prevTime = this.curTime;
        this.curTime = Date.now();
        this.deltaTime = this.curTime - this.prevTime;
        console.log(this.deltaTime);

        //update position
        if (this.droneData.droneState.flying) {
            var yawRadians = this.droneData.demo.rotation.yaw * 180 / Math.PI;
            var localX = this.droneData.demo.xVelocity * deltaTime;
            var localY = this.droneData.demo.yVelocity * deltaTime;
            this.position.z = this.droneData.demo.altitude;
            this.position.x += Math.cos(yawRadians)*localX + Math.sin(yawRadians)*localY;
            this.position.y += Math.sin(yawRadians)*localX + Math.cos(yawRadians)*localY;
        }
    }
    this.client.on('navdata', this.updateData);
}

var drone10 = new Drone("192.168.1.1");

// make `process.stdin` begin emitting "keypress" events
keypress(process.stdin);
process.stdin.setRawMode(true); //making this false causes console-style input

process.stdin.on('keypress', function (ch, key) {
    if (key && key.name == "b") {
        console.log(drone10.droneData.demo.batteryPercentage);
    }
})
