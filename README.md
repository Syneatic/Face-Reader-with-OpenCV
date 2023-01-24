# Face-Reader-with-OpenCV

This script is to detect the face from a picture and find the different landmarks of the face while determining the colour of the different landmarks.

## Disclaimer
As it is using the OpenCV plus Unity (free version) in the Unity Asset Store, it does not use the latest version of OpenCV codes(currently Version 1.7.1 - January 24, 2019).

Since it is converted from Python to C#. there are little to none tutorials or help for OpenCV for C#.

Additionally, some Haarcascade are not in the Unity Package so it needs to be added in manually.
Link to the GitHub to download the Haarcascades: 

[GithubOpenCV](https://github.com/opencv/opencv/tree/master/data/haarcascades)

It for some reason recommend to use BGR (Blue, Green, Red) instead of the normal RGB value.

## General Information

It uses Haarcascade which is a Text Asset that automatically detects parts of an image based on its scale in image and location.

In this research it help finds the colour and position of the skin tone, eyes, lips and hair.

It checks if the person in the photo has any hair on the top of his/ her head.

## Screenshots

##### Examples of Photos that can work using OpenCV:
- Colour Representation: 
- Red = Skin Colour
- White = Mouth Detection
- Pink = Hair Detection
- Bright Blue = Eye colour

## Best way to show face for most accurate scan:

- Face is straight
- Smile (Recommend: Do not show teeth)
- Look at the camera
- Is not too far nor too near to the camera, just right the face is covering the entire screen
 ### Examples:
![Screenshot 2023-01-13 140350](https://user-images.githubusercontent.com/94235882/212593937-7e9d99f7-4f21-4120-89dc-2529764c6623.png)

![Screenshot 2023-01-13 141838](https://user-images.githubusercontent.com/94235882/212593942-ccce04bf-3970-4856-af41-42cf68f49018.png)

Examples that cannot use to detect the faces:

## Inaccurate Face Detection:

- Face is sideways (Not vertically straight/ slightly turn)
- Neither eyes are open
- The person is too far away or too close to the camera
- The person is smiling (sometimes it works and doesn’t)
 ### Examples:
![New Project](https://user-images.githubusercontent.com/94235882/212596085-b231072c-cdf0-4fc4-8cd5-d26862077ecc.png)

## Installation:

Unity Asset Package: [OpenCV plus Unity](https://assetstore.unity.com/packages/tools/integration/opencv-plus-unity-85928) 

Script (UI Face Detection)

Photo of a person

Eye Haarcascade (haarcascade_eye.xml)

Mouth Haarcascade (haarcascade_mcs_mouth.xml)

Face Haarcascade (haarcascade_frontalface_default.xml)

## Setting up the game"

1. Download the haarcascade pack
2. Download the Asset Package 
3. Enable the photo to read/ write in unity inspector
4. Add in the according Haarcascade into the unity asset (At the moment place it outside, not in any folder)
![Asset Folder](https://user-images.githubusercontent.com/94235882/212603193-831815e8-cb59-4304-bef5-66601b61e6c3.png)

6. Drag the script into a RawImage UI in a canvas
7. Drag the according haarcascade into the designated labels in the script
8. Drag the photo into the Texture2D in the script




