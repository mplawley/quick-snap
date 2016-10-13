# quick-snap

This is a variation of the first-person-shooter genre (FPS). In it, there is no violence; instead, you take "shots" with a camera. Your mission is to find all the interesting objects presented in the upper-left-hand corner and take their matching pictures in the scene. In other words, search around the scene and try to match your pictures with the ones presented in your upper-left-hand corner. As you attempt to do so, the game will show you a percentage that represents how accurately your shot matches the target shot of the interesting object. You might have to jump with spacebar to get a matching shot!

Additional information about position vector matching and RayCast hit matching is also presented, both in the console and visually as wireframe spheres if you have Gizmos turned on (green for position, red for Raycast hits). 

Example image of gameplay:

![alt tag](https://github.com/mplawley/quick-snap/blob/master/Example.jpg)

# installation and play

Contact me for a 200MB GitIgnore folder. This prototype has a lot of pretty assets that have to be placed just so. Downloading the asset package from online sources (e.g. Unity, Prototools, etc.) won't do it because I made modifications. 

Once you have the GitIgnore folder, open Unity and start Scene 0.

# motivation

I am going through Jeremy Gibson's <i>Game Design, Prototyping, and Development</i> textbook and building the prototypes he gives the reader to do. This is the seventh and penultimate prototype.

#directions

###Edit mode
Turn on edit mode (located on the Target Camera script on the Target Camera) to enter level-design mode. While in edit mode...

- Set a target shot with a left click of the mouse button. These target shots are the new shots the player must find
- Right click to replace the current target shot
- Q and E to cycle through picture shots.
- Hold right click to instant-compare current shot with potential replacement shot, which occurs when you let right-click go! This is a live comparison of the current target-setting shot and the target shot on display in the upper-left-hand corner. Let go of right-click to actually set the new target shot.
- Left Shit + S to save all of your target shots!
- Click "Check to Delete Player Shots" in the "Danger Zone" of the Inspector to get rid of all your picture shots and start over with target shot setting.

###Play mode
- Be sure to turn off edit mode in order to play as a player.
- Mouse to look
- WASD to walk around.
- Hold left-shift while walking in order to run.
- Q and E to cycle through your picture shots. These are your target shots! Find them in the scene and click left-click to take a picture to match your target shots.
- Hold tab to view your most recent picture shot. If you turn on Unity's Gizmos, you'll see RayCast hit (red wire-frame spheres) and position vector information (in green wire-frame spheres) about the shot.
- Shift+S to save your picture shots.
