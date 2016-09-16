Martian Agora
========
A Mars colonization simulator that uses real colonization strategies and real Mars data. Built with Unity3D, Blender, Gimp, and Audacity.

#Code Quality
Since Martian Agora was only my second major Unity3D project, I've always felt the project quality could be greatly improved:

* There are nearly no code comments.
* Hardly any tests.
* I never used inheritance because I didn't properly know how in C#.
* Code architecture that in retrospect, could easily be simplified.

I waited a year thinking I'd make those improvements. Meanwhile I've been pestered by learners/students/YouTubers. So it's time to post it here!

#Features

###Drive
A drivable Mars rover. Press 'M' to change the camera view. You left click on the rover seat to hop in. WASD steers.

###Build
Structures like turbines, atmospheric water generators, solar panels, batteries, water tanks, etc. Press 'I' to access the inventory. From there, select a building. Then you need to equip the 'virtual reality visor'. By default, you do this by pressing '3', because it's the third item in your inventory slot. When a blue building blueprint pops up, left click to place it. Then fill its nodes with the required materials, and hold click on task nodes (cones) with the proper tools to construct the building.

###Resources
Press 'C' to view your resources and building statuses. Produce, store, and consume resources.

###Inventory
Inventory system with tools and materials for building. Press 'I' to access it. Click and drag to move them around. You can place tools in the bottom slots. Then press 0 through 9 on the keyboard to equip it.

###PNG Terrain Data
Another feature is the beginnings of using PNG files to construct terrain-like meshes. The main scene also uses real Mars terrain data, but I had to input it through Unity's wonky system and smooth it myself.

#Future
I foresee continuing this project in the future, especially when I get out of school in a year and start producing more of my Unity3D tutorial videos.