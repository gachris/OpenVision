global using OpenVision.Core.Configuration;
global using OpenVision.Core.DataTypes;
global using OpenVision.Core.Utils;
#if ANDROID
global using OpenCV.Core;
global using OpenCV.Calib3d;
global using OpenCV.Features2d;
global using OpenCV.ImgCodecs;
global using OpenCV.ImgProc;
global using Feature2D = OpenCV.Features2d.Feature2D;
global using MatOfKeyPoint = OpenCV.Core.MatOfKeyPoint;
global using KeyPoint = OpenCV.Core.KeyPoint;
#else
global using Emgu.CV;
global using Emgu.CV.CvEnum;
global using Emgu.CV.Features2D;
global using Emgu.CV.Structure;
global using Emgu.CV.Util;
global using Feature2D = Emgu.CV.Features2D.Feature2D;
global using MatOfKeyPoint = Emgu.CV.Util.VectorOfKeyPoint;
global using KeyPoint = Emgu.CV.Structure.MKeyPoint;
#endif