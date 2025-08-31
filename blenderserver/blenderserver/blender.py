import subprocess

def run_blender():  
    result = subprocess.run([
        '/Applications/Blender.app/Contents/MacOS/Blender',
        '/Users/henning/blenderserver/blenderserver/example02.blend',
        '--background',
        '--python',
        '/Users/henning/blenderserver/blenderserver/blenderscript.py'
    ],
    capture_output=True,
    text=True)

    fobj = open('./blenderserver.obj')
    fmat = open('./blenderserver.mtl')
    return fobj.read() + '#####' + fmat.read()

    #print(result.stdout)
    #return result.stdout
