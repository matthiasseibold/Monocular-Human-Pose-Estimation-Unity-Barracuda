# Monocular-Human-Pose-Estimation-Unity-Barracuda
Monocular human pose estimation from a webcam texture in Unity using Vuforia and Barracuda with Pose-ResNet-50 pretrained on the MPII Dataset which regresses 16 human body points/joints.

![Steffi Graf](/img/steffigraf.jpg)   ![Steffi Graf](/img/steffigraf.png)
  
This repository is a Unity port from a blog post by GitHub user satyajitghana  
https://github.com/satyajitghana/TSAI-DeepVision-EVA4.0-Phase-2/blob/master/05-HumanPoseEstimation-ONNX/HumanPoseEstimation_ONNX_Quant.ipynb

# Dependencies
- Unity 2020.3.31f1  
- Barracuda 3.0.0  
- Vuforia 10.5.5  

# Install
To install Barracuda 3.0.0, in Unity, go to Window > Package Manager > + (in left top corner) > Add package from git URL > enter "com.unity.barracuda"
  
To install Vuforia 10.5.5, go to https://developer.vuforia.com/downloads/SDK and download the file "add-vuforia-package-10-5-5.unitypackage". Unfortunately, Unity 2020.3.31f1 is not able to import the package without the namespace error caused by the missing Vuforia namespace. As a workaround to correctly import this package, open the script "RGBHumanPoseDetector.cs" in Visual Studio, comment the whole script (with Edit > Advanced > Comment Selection), save the file and let Unity import Vuforia. After the import is finished, you can uncomment the script and save it.
  
Download the pretrained ONNX model from: https://drive.google.com/drive/folders/1IQKvE6dFe0DO0cHdagU_yAI1-cCVQ5JW?usp=sharing
and put it into ./Assets/Models/

As a final step, drag-and-drop the Model Asset "pose_resnet_50_256x256.onnx" to the GameObject "RGBHumanPose" under Model Asset.

# Parameters

![Inspector panel of the RGBHumanPose GameObject](/img/settings.png)

By default, the project is configured to track the joints of the upper body (joints 7 and 10-15), however all other joints can be configured in the Inspector by entering the following indices:

0 - r ankle  
1 - r knee   
2 - r hip    
3 - l hip    
4 - l knee   
5 - l ankle  
6 - pelvis   
7 - thorax   
8 - upper neck 
9 - head top 
10 - r wrist
11 - r elbow
12 - r shoulder
13- l shoulder 
14 - l elbow 
15 - l wrist      


