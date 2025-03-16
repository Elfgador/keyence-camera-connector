

## To run it

#### Clone
```bash
git clone https://github.com/Elfgador/keyence-camera-connector.git
```
#### Run
```bash
cd keyence-camera-connector
dotnet run
```

## To publish

### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

### Mac
```bash
dotnet publish -c Release -r osx-arm64 --self-contained true
```

## Lib
```xaml
  <PackageReference Include="EEIP" Version="1.6.0.26419" /> 
```

## Simulator
```bash
# Only run on linux
sudo docker pull cpppo/scada

sudo docker run -p 192.168.1.43:44818:44818 -d cpppo/scada
```