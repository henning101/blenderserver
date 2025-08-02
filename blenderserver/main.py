import subprocess

from blenderserver import server
from blenderserver import argparser

args = argparser.args

if __name__ == '__main__':
    # Start the Flask server: 
    server.app.run(
        host=args.host,
        port=args.port
    )

