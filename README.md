# To run it

```bash
git clone https://github.com/Elfgador/keyence-camera-connector.git

cd keyence-camera-connector

dotnet run
```


# Program Actions
- Connect to camera
- Run program N°1
- Print result
- Close connection

## Run simulator with docker

```bash

docker pull iotechsys/ethernetip-sim:1.0.4-arm64

docker run -i --rm --name=ethernetip-sim --network=EthernetIp iotechsys/ethernetip-sim:1.0.4-arm64

# Get ip
docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' ethernetip-sim

# Stop
docker stop ethernetip-sim

```