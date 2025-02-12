import logging
import os
import sys

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from versioning.git_utils import VersioningError, get_git_info
from versioning.version_info import create_version_info_class
from versioning.csproj_utils import update_csproj_versioning
from versioning.logging_config import setup_logging

"""
versioning.py: Main script to coordinate version retrieval, validation, 
project file updates, and VersionInfo generation.
"""

def main():
    """Orchestrates the versioning process, handles errors, and sets up logging."""

    solution_dir = os.path.abspath(sys.argv[1]) if len(sys.argv) > 1 else os.getcwd()
    project_name = os.path.basename(solution_dir)

    setup_logging(project_name)
    logging.info(f"Starting versioning script for project in {solution_dir}")

    try:
        version, formatted_commit_date, last_commit_sha, current_branch = get_git_info()
        create_version_info_class(version, formatted_commit_date, last_commit_sha, current_branch, solution_dir, project_name)
        update_csproj_versioning(version, solution_dir)
        logging.info("Versioning script completed successfully.")
        
    except VersioningError as e:
        logging.error("Versioning process failed.")
        sys.exit(1)

if __name__ == "__main__":
    main()
