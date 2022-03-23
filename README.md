# Monocular-Human-Pose-Estimation-Unity-Barracuda
Monocular human pose estimation from a webcam texture in Unity using Vuforia and Barracuda with Pose-ResNet-50  
  
This repository is a Unity port from a blog post by GitHub user satyajitghana  
https://github.com/satyajitghana/TSAI-DeepVision-EVA4.0-Phase-2/blob/master/05-HumanPoseEstimation-ONNX/HumanPoseEstimation_ONNX_Quant.ipynb

# Dependencies
- Unity 2020.3.31f1  
- Barracuda 3.0.0  
- Vuforia 10.5.5  

# Install
To install Barracuda 3.0.0, in Unity, go to Window > Package Manager > + (in left top corner) > Add package from git URL > enter "com.unity.barracuda"
  
To install Vuforia 10.5.5, go to https://developer.vuforia.com/downloads/SDK
  
Download the pretrained ONNX model from: https://drive.google.com/drive/folders/1IQKvE6dFe0DO0cHdagU_yAI1-cCVQ5JW?usp=sharing
and put it into ./Assets/Models/
