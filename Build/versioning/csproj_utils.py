import os
import logging
from xml.etree import ElementTree as ET
from versioning.git_utils import VersioningError

"""
csproj_utils.py: Handles the retrieval, comparison, and updating of version
information in the .csproj file.
"""

def get_current_versions(property_group):
    """Get the current versions from the .csproj file."""
    
    current_versions = {}
    for prop in ['AssemblyVersion', 'FileVersion', 'InformationalVersion', 'ProductVersion']:
        element = property_group.find(prop)
        current_versions[prop] = element.text if element is not None else None

    return current_versions

def versions_differ(current_versions, new_version):
    """Check if any of the current versions differ from the new version."""
    
    for prop, current_version in current_versions.items():
        if current_version != new_version:
            return True
        
    return False

def update_csproj_versioning(version, project_dir):
    """Update the version information in the .csproj file if it differs from the new version."""
    
    csproj_files = [f for f in os.listdir(project_dir) if f.endswith('.csproj')]
    
    if not csproj_files:
        logging.error("No .csproj file found in the project directory.")
        raise VersioningError("No .csproj file found in the project directory.")

    csproj_path = os.path.join(project_dir, csproj_files[0])
    logging.info(f"Checking .csproj file at {csproj_path} for version updates.")

    tree = ET.parse(csproj_path)
    root = tree.getroot()

    property_groups = root.findall('PropertyGroup')
    if not property_groups:
        logging.error("No <PropertyGroup> found in the .csproj file.")
        raise VersioningError("No <PropertyGroup> found in the .csproj file.")

    first_property_group = property_groups[0]

    current_versions = get_current_versions(first_property_group)

    if not versions_differ(current_versions, version):
        logging.info("Versions are up to date. No changes made to the .csproj file.")
        return

    for prop in ['AssemblyVersion', 'FileVersion', 'InformationalVersion', 'ProductVersion']:
        element = first_property_group.find(prop)
        if element is None:
            element = ET.SubElement(first_property_group, prop)
        element.text = version

    indent_xml(root)

    xml_str = ET.tostring(root, encoding='utf-8', method='xml')

    with open(csproj_path, "wb") as f:
        f.write(xml_str)

    logging.info(f"Updated versioning in {csproj_path} to {version}")

def indent_xml(elem, level=0):
    """Indent the XML elements to make the output more readable."""
    
    i = "\n" + level * "  "
    if len(elem):
        if not elem.text or not elem.text.strip():
            elem.text = i + "  "
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
        for elem in elem:
            indent_xml(elem, level + 1)
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
    else:
        if level and (not elem.tail or not elem.tail.strip()):
            elem.tail = i
