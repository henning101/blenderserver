import os
import argparse

args = None
argparser = argparse.ArgumentParser(description='Blender Server')

def parse_args():
    """ Parses user arguments.
    """
    global args
    argparser = argparse.ArgumentParser()
    argparser.add_argument(
        '--host',
        action='store',
        dest='host',
        help='Server host',
        default='0.0.0.0'
    )
    argparser.add_argument(
        '--port',
        action='store',
        dest='port',
        help='Server port',
        type=int,
        default=8000
    )
    args = argparser.parse_args()

parse_args()
