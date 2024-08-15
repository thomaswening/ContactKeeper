import subprocess
import logging
import re
import os
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

        # Ensure we have all tags (important for CI environments with shallow clones)
        run_git_command(['fetch', '--tags', '--force'])

        # Try to describe the latest tag
        try:
            latest_tag = run_git_command(['describe', '--tags', '--abbrev=0'])
        except VersioningError:
            logging.warning("No valid tags found or no tags exist. Defaulting to '0.0.0'.")
            return '0.0.0'

        # Validate the retrieved tag
        if not re.match(r'v(\d+\.\d+\.\d+)', latest_tag):
            logging.warning(f"Latest tag '{latest_tag}' does not match version pattern, defaulting to '0.0.0'.")
            return '0.0.0'

        validate_versioning_rules([latest_tag])
        logging.info(f"Found latest version tag: {latest_tag}")
        return latest_tag[1:]  # Strip the 'v' from 'vX.Y.Z'
    
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

        # Use GitHub Actions environment variables if available
        current_branch = os.getenv('GITHUB_HEAD_REF') or os.getenv('GITHUB_REF_NAME')
        commit_sha = os.getenv('GITHUB_SHA')
        
        if not current_branch:
            # Fallback to using git command if not in a CI environment
            current_branch = run_git_command(['rev-parse', '--abbrev-ref', 'HEAD'])

        if not commit_sha:
            # As a fallback, try to get the SHA using git commands
            commit_sha = run_git_command(['rev-parse', 'HEAD'])

        last_commit_date = run_git_command(['log', '-1', '--format=%ci'])

        commit_datetime = datetime.strptime(last_commit_date, "%Y-%m-%d %H:%M:%S %z")
        formatted_commit_date = commit_datetime.strftime("%d.%m.%Y-%H:%M:%S%z")

        logging.info(f"Version: {version}, Last Commit Date: {formatted_commit_date}, "
                     f"Commit SHA: {commit_sha}, Branch: {current_branch}")

        return version, formatted_commit_date, commit_sha, current_branch
    
    except VersioningError as e:
        logging.error(f"Failed to retrieve git information. {e}")
        raise
