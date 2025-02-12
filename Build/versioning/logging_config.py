import logging
import os

"""
logging_config.py: Configures logging to output to both a log file in the build directory
and the console, ensuring that all important messages are captured during the versioning process.
"""

def setup_logging(project_name):
    """Set up logging to output to both a log file and the console."""
    
    script_dir = os.path.dirname(os.path.realpath(__file__))
    build_dir = os.path.abspath(os.path.join(script_dir, '..'))

    log_filename = f'versioning_{project_name}.log'
    log_path = os.path.join(build_dir, log_filename)

    logging.basicConfig(level=logging.INFO,  # Set level to INFO to capture all INFO messages
                        format='%(asctime)s %(levelname)s: %(message)s',
                        handlers=[
                            logging.FileHandler(log_path, mode='w'),
                            logging.StreamHandler()  # Stream to console
                        ])
