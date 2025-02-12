import os
import logging
from versioning.git_utils import VersioningError

"""
version_info.py: Handles the loading of the template and creation of the VersionInfo.cs file,
which contains versioning and Git-related information.
"""

def load_template(template_path):
    """Load the content of the template file from the given path."""
    
    try:
        logging.info(f"Loading template from {template_path}")
        with open(template_path, 'r') as file:
            return file.read()
    
    except FileNotFoundError as e:
        logging.error(f"Template file not found: {template_path}")
        logging.error(e)
        raise VersioningError(f"Template file not found: {template_path}") from e

def create_version_info_class(version, last_commit_date, last_commit_sha, current_branch, project_dir, project_name):
    """Create the VersionInfo.cs file in the specified project directory using the provided version information."""
    
    utilities_dir = os.path.join(project_dir, "Utilities")
    os.makedirs(utilities_dir, exist_ok=True)

    script_dir = os.path.dirname(os.path.realpath(__file__))
    template_path = os.path.join(script_dir, 'VersionInfoTemplate.cs')
    template = load_template(template_path)

    version_info = template.format(
        namespace=f"{project_name}.Utilities",
        version=version,
        commit_sha=last_commit_sha,
        build_date=last_commit_date,
        branch=current_branch
    )

    version_info_path = os.path.join(utilities_dir, "VersionInfo.cs")
    logging.info(f"Creating VersionInfo.cs at {version_info_path}")
    with open(version_info_path, "w") as file:
        file.write(version_info)
