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

## To publish

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true
dotnet publish -c Release -r win-x86 --self-contained true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true

# Mac
dotnet publish -c Release -r osx-arm64 --self-contained true
```

## Lib
```xaml
  <PackageReference Include="EEIP" Version="1.6.0.26419" /> 
```