"""CI guard against reintroducing mirror downloads or launcher update hooks."""

from pathlib import Path
import re
import struct
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[2]
LAUNCHER = ROOT / 'Plain Craft Launcher 2'
MIRROR_URL = re.compile(
    r'https?://[^\s"<>]*(?:bmclapi[^/]*|mcimirror\.[^/]*|gh-proxy\.[^/]*)',
    re.IGNORECASE,
)


def main():
    errors = []
    project = ET.parse(LAUNCHER / 'Plain Craft Launcher 2.vbproj')
    compiled_xaml = {
        LAUNCHER / item.attrib['Include'].replace('\\', '/')
        for item in project.iter()
        if item.tag.rsplit('}', 1)[-1] in {'Page', 'ApplicationDefinition'}
    }
    downloads = [
        *LAUNCHER.glob('Modules/Minecraft/*.vb'),
        *LAUNCHER.glob('Modules/Resource/*.vb'),
        LAUNCHER / 'Pages/PageDownload/ModDownloadLib.vb',
        *ROOT.glob('tools/forge-installer/**/*.java'),
    ]
    for path in downloads:
        if MIRROR_URL.search(path.read_text(encoding='utf-8-sig')):
            errors.append(f'Mirror download URL in {path.relative_to(ROOT)}')

    forbidden = re.compile(r'\b(?:RestrictMcDownload|UpdateReplace|UpdateRestart|IsUpdateWaitingRestart|SystemSystemUpdate)\b')
    for path in LAUNCHER.rglob('*'):
        if path.suffix not in {'.vb', '.xaml'}:
            continue
        source = path.read_text(encoding='utf-8-sig')
        if forbidden.search(source):
            errors.append(f'Restriction/updater hook in {path.relative_to(ROOT)}')
        if path in compiled_xaml:
            try:
                ET.fromstring(source)
            except ET.ParseError as error:
                errors.append(f'Invalid XAML in {path.relative_to(ROOT)}: {error}')

    with zipfile.ZipFile(LAUNCHER / 'Resources/forge-installer.jar') as archive:
        bootstrapper = archive.read('com/bangbang93/ForgeInstaller.class')
        if bootstrapper[:4] != b'\xca\xfe\xba\xbe' or struct.unpack('>H', bootstrapper[6:8])[0] != 52:
            errors.append('Forge bootstrapper must target Java 8')
        if b'https://maven.minecraftforge.net/' not in bootstrapper:
            errors.append('Forge bootstrapper must use official Maven')
        if any(host in bootstrapper.lower() for host in [b'bmclapi.', b'mcimirror.', b'gh-proxy.']):
            errors.append('Embedded Forge bootstrapper still contains a mirror URL')

    if errors:
        raise SystemExit('\n'.join(errors))
    print('Network policy checks passed: official download URLs, Java helper, XAML, and no update/restriction hooks.')


if __name__ == '__main__':
    main()
