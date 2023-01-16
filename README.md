# Face-Reader-with-OpenCV

This script is to detect the face from a picture and find the different landmarks of the face while determining the colour of the different landmarks.

Table of contents:

Disclaimer
General Info
Screenshots
Setup
Features
FAQ

Disclaimer
As it is using the OpenCV plus Unity (free version) in the Unity Asset Store, it does not use the latest version of OpenCV codes(currently Version 1.7.1 - January 24, 2019).

Since it is converted from Python to C#. there are little to none tutorials or help for OpenCV for C#.

Additionally, some Haarcascade are not in the Unity Package so it needs to be added in manually.
Link to the GitHub to download the Haarcascades: https://github.com/opencv/opencv/tree/master/data/haarcascades

General Information

It uses Haarcascade which is a Text Asset that automatically detects parts of an image based on its scale in image and location.

In this research it help finds the colour and position of the skin tone, eyes, lips and hair.

It checks if the person in the photo has any hair on the top of his/ her head.

Screenshots

Examples of Photos that can work using OpenCV:
Colour Representation: 
Red = Skin Colour
White = Mouth Detection
Pink = Hair Detection
Bright Blue = Eye colour

Best way to show face for most accurate scan:
Face is straight
Smile (Recommend: Do not show teeth)
Look at the camera
Is not too far nor too near to the camera, just right the face is covering the entire screen







Examples that cannot use to detect the faces:
Worst way to detect face:
Face is sideways (Not vertically straight/ slightly turn)
Neither eyes are open
The person is too far away or too close to the camera
The person is smiling (sometimes it works and doesn’t)

Setting up:

Script (UI Face Detection)
Photo of person
Eye Haarcascade (haarcascade_eye.xml)
Mouth Haarcascade (haarcascade_mcs_mouth.xml)
Face Haarcascade (haarcascade_frontalface_default.xml)

Installation:

Download the haarcascade pack
Download the Asset Package 
Enable the photo to read/ write in unity inspector
Add in the according Haarcascade into the unity asset (At the moment place it outside, not in any folder)
Drag the script into a RawImage UI in a canvas
Drag the according haarcascade into the designated labels in the script
Drag the photo into the Texture2D in the script



