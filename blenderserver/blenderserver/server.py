from flask import Flask 

from blenderserver import blender

app = Flask(__name__)

@app.route('/')
def hello_world():
    return blender.run_blender()
