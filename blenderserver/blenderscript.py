import os
import bpy
import json

blend_file_path = bpy.data.filepath
directory = os.path.dirname(blend_file_path)
target_file = os.path.join(directory, 'blenderserver.obj')

# Apply config.json to Geometry Node:
with open(os.path.join(directory, 'config.json')) as text:
    data = json.load(text)

modifier = bpy.data.objects['TargetGeometry'].modifiers['GeometryNodes']

for item in modifier.node_group.interface.items_tree:
    if item.name in data.keys():
        modifier[item.identifier] = data[item.name]

modifier.node_group.interface_update(bpy.context)

bpy.ops.wm.obj_export(filepath=target_file)
