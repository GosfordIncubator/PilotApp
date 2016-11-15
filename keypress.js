var keypress = require('keypress');
var ip = '192.168.1.69';
var arDrone = require('ar-drone');
var client = arDrone.createClient({"ip" : "192.168.1." + process.argv[2]});
// client.takeoff();
// client.stop();
// // client.land();

var flying = false;
var parallel = 0.0;		   //forwards +ve
var transverse = 0.0;	  //right +ve
var lateral = 0.0;		    //rotation - clockwise +ve

var droneData = new Object();
var deltaTime;              //time between droneData updates in seconds
var prevTime;                //figure used to calculate delta time in milliseconds
var curTime = Date.now();   //ditto

var position = {
    x: 0,
    y: 0,
    z: 0
}

// make `process.stdin` begin emitting "keypress" events
keypress(process.stdin);

// listen for the "keypress" event
process.stdin.on('keypress', function (ch, key) {
	// console.log('got "keypress"', key);
	// if (key && key.name == 'c') {
	// 	flying = !flying;
	// 	if (flying) {
	// 		client.land();
	// 	} else {
	// 		client.takeoff();
	// 	}
 //  	}
 	if(key) {
 		switch(key.name) {
			case 'c':
				if(flying) {
					client.land();
				} else {
					client.takeoff();
					client.stop();
				}
				flying = !flying;
				break;
			case 's':
				parallel = 0;
				transverse = 0;
				lateral = 0;
				client.stop();
				break;
			case 'w':
				parallel += 0.1;
				if (parallel > 1) {
					parallel = 1;
				}
				break;
			case 'x':
				parallel -= 0.1;
				if (parallel < -1) {
					parallel = -1;
				}
				break;
			case 'd':
				transverse += 0.1;
				if (transverse > 1) {
					transverse = 1;
				}
				break;
			case 'a':
				transverse -= 0.1;
				if (transverse < -1) {
					transverse = -1;
				}
				break;
			case 'e':
				lateral += 0.1;
				if (lateral > 1) {
					lateral = 1;
				}
				break;
			case 'q':
				lateral -= 0.1;
				if (lateral < -1) {
					lateral = -1;
				}
				break;
            case 'b':
                console.log(droneData.demo.batteryPercentage);
                break;
            case 'i':
                console.log(droneData.demo.rotation.yaw * 180 / Math.PI);
                break;
            case 'o':
                position.x = 0;
                position.y = 0;
                break;
			case 'p':
				console.log(position.x, position.y, position.z);
				break;
			default:
				break;
		}

		//apply movement
		if (parallel >= 0) {
			client.front(parallel);
		} else {
			client.back(-parallel);
		}
		if (transverse >= 0) {
			client.right(transverse);
		} else {
			client.left(-transverse);
		}
		if (lateral >= 0) {
			client.clockwise(lateral);
		} else {
			client.counterClockwise(-lateral);
		}

		if(parallel == 0 && transverse == 0 /*&& lateral == 0*/) {
			client.stop();
		}
	}
});

process.stdin.setRawMode(true); //making this false causes console-style input
process.stdin.resume();

//read nav data into variable
function updateData(navData) {
    //update droneData
	droneData = navData;
    //calculate deltaTime
    prevTime = curTime;
    curTime = Date.now();
    deltaTime = (curTime - prevTime) / 1000;
    //update position data based on deltaTime and velocities
    if (flying) {
        try {
            var yawRadians = droneData.demo.rotation.yaw * 180 / Math.PI;
            var localX = droneData.demo.xVelocity * deltaTime;
            var localY = droneData.demo.yVelocity * deltaTime;
            position.z = droneData.demo.altitude;
            position.x += Math.cos(yawRadians)*localX + Math.sin(yawRadians)*localY;
            position.y += Math.sin(yawRadians)*localX + Math.cos(yawRadians)*localY;

        } catch(err) {
            client.stop();
            client.land();
            console.log("Error calculating velocity.");
            console.log(err);
        }
    }
}
client.on('navdata', updateData);
