# Forge install bootstrapper

Adapted from [bangbang93/forge-install-bootstrapper](https://github.com/bangbang93/forge-install-bootstrapper/tree/f075721af43b95925de18e5103d86fbbfe65a43e).
The bootstrapper retains PCL's existing installer compatibility logic and uses
`https://maven.minecraftforge.net/` for installer-managed downloads.

Rebuild the embedded `Plain Craft Launcher 2/Resources/forge-installer.jar` with
Python 3 and JDK 17 or newer:

```powershell
python .github/scripts/build_forge_bootstrapper.py
python .github/scripts/check_network_policy.py
```

The build downloads its compiler classpath from Forge's official Maven repository,
checks its pinned SHA-256, and produces Java 8 compatible bytecode. CI rebuilds it
before compiling the launcher so the shipped helper matches the reviewed source.
