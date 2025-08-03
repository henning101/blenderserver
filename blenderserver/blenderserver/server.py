import json
from flask import Flask, request

from blenderserver import blender

app = Flask(__name__)

@app.route('/', methods=['GET', 'POST'])
def index():
    data = request.json
    with open('./config.json', 'w') as f:
        f.write(json.dumps(data, indent=2))

    return blender.run_blender()
