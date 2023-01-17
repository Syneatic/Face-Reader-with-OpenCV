using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;

public class UIFaceDetector : MonoBehaviour
{
    #region Variables 
    public Texture2D sample;
	public TextAsset faces;
	public TextAsset eyes;
	public TextAsset mouth;

	private readonly Size requiredSize = new(128, 128);
	private readonly Size eyesSize = new Size(64, 64);
	private readonly Size mouthSize = new Size(128, 64);
	private readonly Size hairSize = new Size(64, 64);

	private CascadeClassifier cascadeFaces;
	private CascadeClassifier cascadeEyes;
	private CascadeClassifier cascadeMouth;

	public bool isRegionOn;
	// The bounding box around the face
	OpenCvSharp.Rect faceBoundingBox;
    #endregion
    void Start()
	{

        #region Cascade Classifier

        cascadeFaces = new CascadeClassifier(System.IO.Path.Combine(Application.dataPath, "haarcascade_frontalface_default.xml"));
		cascadeEyes = new CascadeClassifier(System.IO.Path.Combine(Application.dataPath, "haarcascade_eye.xml"));
		cascadeMouth = new CascadeClassifier(System.IO.Path.Combine(Application.dataPath, "haarcascade_mcs_mouth.xml"));
		// convert texture to cv image
		Mat image = OpenCvSharp.Unity.TextureToMat(this.sample);

        #endregion


		// Detect faces
		var gray = image.CvtColor(ColorConversionCodes.BGR2GRAY);
		Cv2.EqualizeHist(gray, gray);

		// detect matching regions (faces bounding)
		OpenCvSharp.Rect[] rawFaces = cascadeFaces.DetectMultiScale(gray, 1.1, 6, HaarDetectionType.ScaleImage);

        #region faceDetection

        // now per each detected face draw a marker and detect eyes inside the face rect
        foreach (var faceRect in rawFaces)
		{

			var grayFace = new Mat(gray, faceRect);
			if (requiredSize.Width > 0 && requiredSize.Height > 0)
			{
				grayFace = grayFace.Resize(requiredSize);
			}

			//Making a new mask for the cropped image
			OpenCvSharp.Rect roi = new OpenCvSharp.Rect(faceRect.TopLeft.X, faceRect.TopLeft.Y - (faceRect.TopLeft.Y / 4 * 3), (faceRect.TopRight.X - faceRect.TopLeft.X), (faceRect.BottomLeft.Y) );

			// Cropping the mask into a new mat
			Mat cropped_Image = image.SubMat(roi);
			var grayg = cropped_Image.CvtColor(ColorConversionCodes.BGR2GRAY);
			Cv2.EqualizeHist(grayg, grayg);

			OpenCvSharp.Rect[] rawEyes = cascadeEyes.DetectMultiScale(grayg, 2.5, 3);

			OpenCvSharp.Rect[] rawMouth = cascadeMouth.DetectMultiScale(grayg, 1.1, 3, HaarDetectionType.FindBiggestObject);

			OpenCvSharp.Rect[] rawHair = cascadeFaces.DetectMultiScale(grayg, 1.1, 6, HaarDetectionType.ScaleImage);

			Mat face_mask = Mat.Zeros(cropped_Image.Rows, cropped_Image.Cols, cropped_Image.Type());

			Cv2.Rectangle(face_mask, faceRect, Scalar.White, 1);

			Mat masked_image = new Mat();

			// Apply the mask to the image
			Cv2.BitwiseAnd(cropped_Image, face_mask, masked_image);

			int[] coords = new int[2] { 0, 0 };

			// Represent as the y-axis
			for (int i = 0; i < face_mask.Rows; i++)
			{
				// Represent as the x-axis
				for (int j = 0; j < face_mask.Cols; j++)
				{
					Vec3b pixel = face_mask.At<Vec3b>(i, j);
					// Check if the BGR values are within the skin tone range
					if (pixel[0] == 255)
					{
						if (coords[0] <= i)
                        {
							coords[0] = i;
                        }
						if (coords[1] <= j)
						{
							coords[1] = j;
						}

					}
				}
			}

			Debug.Log("The face is: " + cropped_Image.Width + ", " + cropped_Image.Height);
			Debug.Log("The pixel size for face is: (0, 0) , (" + coords[1] + " ," + coords[0] + ")\n _______________________________");

            #region eyesAndSkin

            Mat mask = Mat.Zeros(cropped_Image.Rows, cropped_Image.Cols, cropped_Image.Type());
			Mat eyeMask = Mat.Zeros(cropped_Image.Rows, cropped_Image.Cols, image.Type());
			foreach (var eyesRect in rawEyes)
			{
				var grayEyes = new Mat(grayg, eyesRect);
				if (eyesSize.Width > 0 && eyesSize.Height > 0)
				{
					grayEyes = grayEyes.Resize(eyesSize);
				}
				Cv2.Circle((InputOutputArray)mask, eyesRect.Center.X, (eyesRect.Center.Y + (eyesRect.Center.Y / 5)), 3, Scalar.White, -1);

				Cv2.Circle((InputOutputArray)eyeMask, eyesRect.Center.X, eyesRect.Center.Y, 1, Scalar.White, -1);

				//Show the Region Detected
				Cv2.Circle((InputOutputArray)cropped_Image, eyesRect.Center.X, (eyesRect.Center.Y + (eyesRect.Center.Y / 5)), 3, Scalar.Red, -1);
				Cv2.Circle((InputOutputArray)cropped_Image, eyesRect.Center.X, eyesRect.Center.Y, 1, Scalar.LightSteelBlue, -1);
			}

			Mat facemasked_image = new Mat();

			// Apply the mask to the image
			Cv2.BitwiseAnd(cropped_Image, mask, facemasked_image);

			int[] totalBGR = new int[3] { 0, 0, 0 };
			int skinPixelCount = 0;

			for (int i = 0; i < masked_image.Rows; i++)
			{
				for (int j = 0; j < masked_image.Cols; j++)
				{
					Vec3b pixel = mask.At<Vec3b>(i, j);
					Vec3b newpixel = facemasked_image.At<Vec3b>(i, j);
					// Check if the BGR values are within the skin tone range
					if (pixel[0] == 255)
					{
						// Add the BGR values to the total
						totalBGR[0] += newpixel[0];
						totalBGR[1] += newpixel[1];
						totalBGR[2] += newpixel[2];

						// Increment the skin pixel count
						skinPixelCount++;
					}
				}
			}

			// Calculate the average skin tone color
			int[] avgBGR = new int[3] { totalBGR[0] / skinPixelCount, totalBGR[1] / skinPixelCount, totalBGR[2] / skinPixelCount };

			// Print the result to the console
			Debug.Log("The average skin tone color is: (B: " + avgBGR[0] + ", G: " + avgBGR[1] + ", R: " + avgBGR[2] + ")\n _______________________________");

			Mat eyemasked_image = new Mat();

			// Apply the mask to the image
			Cv2.BitwiseAnd(cropped_Image, eyeMask, eyemasked_image);

			int[] eyecoords = new int[2] { 0, 0 };

			int[] totalBGR_eye = new int[3] { 0, 0, 0 };

			int eyePixelCount = 0;
			// Represent the Y-axis
			for (int i = 0; i < eyemasked_image.Rows; i++)
			{
				// Represents the X-axis
				for (int j = 0; j < eyemasked_image.Cols; j++)
				{
					Vec3b pixel = eyeMask.At<Vec3b>(i, j);
					Vec3b newpixel = eyemasked_image.At<Vec3b>(i, j);
					// Check if the BGR values are within the eye tone range
					if (pixel[0] == 255)
					{
						// Add the BGR values to the total
						totalBGR_eye[0] += (int)newpixel[0];
						totalBGR_eye[1] += (int)newpixel[1];
						totalBGR_eye[2] += (int)newpixel[2];

						// Increment the eye pixel count
						eyePixelCount++;
					}


				}
			}
			// Calculate the average eye color
			int[] avgBGR_eye = new int[3] { totalBGR_eye[0] / eyePixelCount, totalBGR_eye[1] / eyePixelCount, totalBGR_eye[2] / eyePixelCount };
			// Print the result to the console
			Debug.Log("The average for eye color is: (B: " + avgBGR_eye[0] + ", G: " + avgBGR_eye[1] + ", R: " + avgBGR_eye[2] + ")\n _______________________________");

            #endregion

            #region mouthRegion

            // Creating a mask speciacally for the mouth with the newlly cropped image
            Mat mouthMask = Mat.Zeros(cropped_Image.Rows, cropped_Image.Cols, cropped_Image.Type());

			foreach (var mouthRect in rawMouth)
			{
				var grayMouth = new Mat(grayg, mouthRect);
				if (mouthSize.Width > 0 && mouthSize.Height > 0)
				{
					grayMouth = grayMouth.Resize(mouthSize);
				}
				Cv2.Rectangle((InputOutputArray)mouthMask, new Point(mouthRect.Center.X, mouthRect.TopLeft.Y + (mouthRect.Height / 4)), new Point(mouthRect.Center.X, mouthRect.TopLeft.Y + 0.2), Scalar.White, -1);

				//Show mouth region detected
				Cv2.Rectangle((InputOutputArray)cropped_Image, new Point(mouthRect.Center.X, mouthRect.TopLeft.Y + (mouthRect.Height/ 4)), new Point(mouthRect.Center.X, mouthRect.Center.Y + 0.2), Scalar.White, -1);
				Debug.Log("The coordinate for the mouth is" + mouthRect.X + " , " + mouthRect.Y);
			}

			Mat mouthmasked_image = new Mat();

			// Apply the mask to the image
			Cv2.BitwiseAnd(cropped_Image, mouthMask, mouthmasked_image);

			int[] totalBGR_mouth = new int[3] { 0, 0, 0 };

			int mouthPixelCount = 0;
			//Represent the Y-axis
			for (int i = 0; i < mouthmasked_image.Rows; i++)
			{
				//Represents the X-axis
				for (int j = 0; j < mouthmasked_image.Cols; j++)
				{
					Vec3b pixel = mouthMask.At<Vec3b>(i, j);
					Vec3b newpixel = mouthmasked_image.At<Vec3b>(i, j);
					// Check if the BGR values are within the mouth tone range
					if (pixel[0] == 255)
					{
						if (newpixel[0] < 235)
                        {
							// Add the BGR values to the total
							totalBGR_mouth[0] += (int)newpixel[0];
							totalBGR_mouth[1] += (int)newpixel[1];
							totalBGR_mouth[2] += (int)newpixel[2];
							// Increment the mouth pixel count
							mouthPixelCount++;
						}
					}


				}
			}

            int[] avgBGR_mouth = new int[3] { totalBGR_mouth[0] / mouthPixelCount, totalBGR_mouth[1] / mouthPixelCount, totalBGR_mouth[2] / mouthPixelCount };
            // Print the result to the console
            Debug.Log("The average for mouth color is: (B: " + avgBGR_mouth[0] + ", G: " + avgBGR_mouth[1] + ", R: " + avgBGR_mouth[2] + ")\n _______________________________");

            #endregion

            #region hairMask
            //Show the hair
            Mat hairmask = Mat.Zeros(cropped_Image.Rows, cropped_Image.Cols, cropped_Image.Type());

			//Checking for every face and finding the closest forehead position
			foreach (var hairRect in rawHair)
			{
				var grayHair = new Mat(grayg, hairRect);
				if (hairSize.Width > 0 && hairSize.Height > 0)
				{
					grayHair = grayHair.Resize(hairSize);
				}
				Cv2.Rectangle((InputOutputArray)hairmask, new Point(hairmask.Width /5 * 2, hairmask.Height / 40), new Point(hairmask.Width / 5 * 3, hairmask.Height / 40 * 3), Scalar.White, -1);

				Cv2.Rectangle((InputOutputArray)cropped_Image, new Point(hairmask.Width / 5 * 2, hairmask.Height / 40), new Point(hairmask.Width / 5 * 3, hairmask.Height / 40 * 3), Scalar.Pink, -1);

				Mat hairmasked_image = new Mat();

				// Apply the mask to the image
				Cv2.BitwiseAnd(cropped_Image, hairmask, hairmasked_image);

				int[] totalBGR_hair = new int[3] { 0, 0, 0 };

				int hairPixelCount = 0;

				//Represent the Y-axis
				for (int i = 0; i < hairmasked_image.Rows; i++)
				{
					//Represents the X-axis
					for (int j = 0; j < hairmasked_image.Cols; j++)
					{
						Vec3b pixel = hairmask.At<Vec3b>(i, j);
						Vec3b newpixel = hairmasked_image.At<Vec3b>(i, j);
						// Check if the BGR values are within the hair tone range
						if (pixel[0] == 255)
						{
							// Add the BGR values to the total
							totalBGR_hair[0] += (int)newpixel[0];
							totalBGR_hair[1] += (int)newpixel[1];
							totalBGR_hair[2] += (int)newpixel[2];
							// Increment the hair pixel count
							hairPixelCount++;
						}


					}
				}

				// Calculate the BGR of the hairs average tone 
				int[] avgBGR_hair = new int[3] { totalBGR_hair[0] / hairPixelCount, totalBGR_hair[1] / hairPixelCount, totalBGR_hair[2] / hairPixelCount };
				
				// Comparing if the person in the image has hair
				if (avgBGR_hair[0] - avgBGR[0] > 20 || avgBGR_hair[0] - avgBGR[0] < -20)
                {
					Debug.Log("There's hair");
                }
                else
                {
					Debug.Log("Bald person");
                }

				Debug.Log("The average for hair color is: (R: " + avgBGR_hair[2] + ", G: " + avgBGR_hair[1] + ", B: " + avgBGR_hair[0] + ")\n _______________________________");
			}

            #endregion

            #region renderPicture

            // Render texture
			// This render is used for to check the different region being recorded in the correct position
			 if (isRegionOn == true)
            {
				var texture = OpenCvSharp.Unity.MatToTexture(cropped_Image);
				var rawImage = gameObject.GetComponent<RawImage>();
				rawImage.texture = texture;

				var transform = gameObject.GetComponent<RectTransform>();
				transform.sizeDelta = new Vector2(cropped_Image.Width, cropped_Image.Height);

			}

			#endregion
		}

        #endregion
    }

}
