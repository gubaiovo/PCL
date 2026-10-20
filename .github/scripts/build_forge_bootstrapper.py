"""Rebuild the embedded bootstrapper against Forge's official installer classes."""

from pathlib import Path
import hashlib
import subprocess
import tempfile
import urllib.request
import zipfile

ROOT = Path(__file__).resolve().parents[2]
INSTALLER_URL = (
    'https://maven.minecraftforge.net/net/minecraftforge/forge/1.17.1-37.0.2/'
    'forge-1.17.1-37.0.2-installer.jar'
)
INSTALLER_SHA256 = '1deef7a0b1b2cb61f1ddaad76edc768b4c383c42d830b124cc36b93d7bf993de'


def main():
    with tempfile.TemporaryDirectory(prefix='pcl-forge-build-') as directory:
        work = Path(directory)
        request = urllib.request.Request(INSTALLER_URL, headers={'User-Agent': 'PCL-fix-network-build'})
        with urllib.request.urlopen(request, timeout=120) as response:
            installer_bytes = response.read()
        if hashlib.sha256(installer_bytes).hexdigest() != INSTALLER_SHA256:
            raise RuntimeError('Forge installer checksum mismatch')
        installer = work / 'forge-installer.jar'
        installer.write_bytes(installer_bytes)
        classes = work / 'classes'
        classes.mkdir()
        source = ROOT / 'tools/forge-installer/src/com/bangbang93/ForgeInstaller.java'
        subprocess.run([
            'javac', '--release', '8', '-proc:none', '-encoding', 'UTF-8',
            '-classpath', str(installer), '-d', str(classes), str(source),
        ], check=True)
        destination = ROOT / 'Plain Craft Launcher 2/Resources/forge-installer.jar'
        with zipfile.ZipFile(destination, 'w', compression=zipfile.ZIP_DEFLATED) as archive:
            for file in sorted(classes.rglob('*.class')):
                entry = zipfile.ZipInfo(file.relative_to(classes).as_posix(), (1980, 1, 1, 0, 0, 0))
                entry.compress_type = zipfile.ZIP_DEFLATED
                archive.writestr(entry, file.read_bytes())
        print('Built Java 8 compatible Forge bootstrapper using official Maven downloads.')


if __name__ == '__main__':
    main()
