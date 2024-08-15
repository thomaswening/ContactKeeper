import subprocess
import logging
import re
from datetime import datetime

class VersioningError(Exception):
    """Custom exception for versioning errors."""
    pass

"""
git_utils.py: Handles Git interactions such as running Git commands,
retrieving version tags, and validating versioning rules.
"""

def run_git_command(args):
    """Run a Git command and return its output, raising an error if the command fails."""

    try:
        logging.info(f"Running git command: {' '.join(args)}")
        return subprocess.check_output(['git'] + args).decode('utf-8').strip()
    
    except subprocess.CalledProcessError as e:
        logging.error(f"Git command failed: {' '.join(args)}")
        raise VersioningError(f"Git command failed: {' '.join(args)}") from e

def parse_version(version_tag):
    """Parse a version tag like 'v1.2.3' into a tuple (1, 2, 3)."""

    major, minor, patch = map(int, version_tag[1:].split('.'))
    return major, minor, patch

def validate_versioning_rules(tags):
    """Ensure version tags adhere to versioning rules such as proper progression and valid bumps."""

    if len(tags) < 2:
        logging.info("Not enough version tags to validate version progression.")
        return

    for i in range(1, len(tags)):
        current_version = parse_version(tags[i - 1])
        previous_version = parse_version(tags[i])

        current_major, current_minor, current_patch = current_version
        previous_major, previous_minor, previous_patch = previous_version

        # Ensure version progression is valid
        if (current_major < previous_major 
            or (current_major == previous_major and current_minor < previous_minor) 
            or (current_major == previous_major and current_minor == previous_minor and current_patch <= previous_patch)):

            raise VersioningError(
                f"Version {tags[i]} is not higher than the previous version {tags[i - 1]}."
            )
        
        # Ensure major version bumps reset minor and patch versions
        if current_major != previous_major and not (current_minor == 0 and current_patch == 0):

            raise VersioningError(
                f"Invalid version bump from {tags[i - 1]} to {tags[i]}: "
                f"minor version and patch version must be 0 when major version is bumped."
            )
        
        # Ensure minor version bumps reset patch version
        if (current_major == previous_major and current_minor != previous_minor and current_patch != 0):

            raise VersioningError(
                f"Invalid version bump from {tags[i - 1]} to {tags[i]}: "
                f"patch version must be 0 when minor version is bumped."
            )

    logging.info("Versioning rules validation passed.")

def get_latest_version_tag():
    """Retrieve and validate the latest version tag from Git, defaulting to '0.0.0' if none are found."""

    try:
        logging.info("Retrieving the latest version tag...")
        tags = run_git_command(['tag', '--list', '--sort=-creatordate']).splitlines()
        valid_tags = [tag for tag in tags if re.match(r'v(\d+\.\d+\.\d+)', tag)]

        if not valid_tags:
            logging.warning("No version tags found, defaulting to 'v0.0.0'.")
            return '0.0.0'

        validate_versioning_rules(valid_tags)
        latest_tag = valid_tags[0]
        logging.info(f"Found latest version tag: {latest_tag}")
        return latest_tag[1:]
    
    except VersioningError as e:
        logging.error(f"Versioning rules validation failed: {e}")
        raise

    except Exception as e:
        logging.error("Failed to retrieve tags.")
        raise VersioningError("Failed to retrieve version tags.") from e

def get_git_info():
    """Collect and format Git-related information including the version, commit date, SHA, and branch name."""

    try:
        version = get_latest_version_tag()
        last_commit_date = run_git_command(['log', '-1', '--format=%ci'])
        last_commit_sha = run_git_command(['rev-parse', 'HEAD'])
        current_branch = run_git_command(['rev-parse', '--abbrev-ref', 'HEAD'])

        commit_datetime = datetime.strptime(last_commit_date, "%Y-%m-%d %H:%M:%S %z")
        formatted_commit_date = commit_datetime.strftime("%d.%m.%Y-%H:%M:%S%z")

        logging.info(f"Version: {version}, Last Commit Date: {formatted_commit_date}, "
                     f"Commit SHA: {last_commit_sha}, Branch: {current_branch}")

        return version, formatted_commit_date, last_commit_sha, current_branch
    
    except VersioningError as e:
        logging.error("Failed to retrieve git information.")
        raise
