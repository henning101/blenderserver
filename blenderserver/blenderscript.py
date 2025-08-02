import os
import bpy

blend_file_path = bpy.data.filepath
directory = os.path.dirname(blend_file_path)
target_file = os.path.join(directory, 'blenderserver.obj')

bpy.ops.wm.obj_export(filepath=target_file)
