//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using Microsoft.Kinect;
    using System;
    using System.Collections.Generic;
    
    public partial class tab_exam_record
    {
        public tab_exam_record()
        {
            this.tab_analysis_result = new HashSet<tab_analysis_result>();
        }
    
        public int id { get; set; }
        public int exam_type_id { get; set; }
        public int exam_id { get; set; }
        public Nullable<double> SpineBaseX { get; set; }
        public Nullable<double> SpineBaseY { get; set; }
        public Nullable<double> SpineBaseZ { get; set; }
        public Nullable<double> SpineMidX { get; set; }
        public Nullable<double> SpineMidY { get; set; }
        public Nullable<double> SpineMidZ { get; set; }
        public Nullable<double> NeckX { get; set; }
        public Nullable<double> NeckY { get; set; }
        public Nullable<double> NeckZ { get; set; }
        public Nullable<double> HeadX { get; set; }
        public Nullable<double> HeadY { get; set; }
        public Nullable<double> HeadZ { get; set; }
        public Nullable<double> SpineShoulderX { get; set; }
        public Nullable<double> SpineShoulderY { get; set; }
        public Nullable<double> SpineShoulderZ { get; set; }
        public Nullable<double> ShoulderLeftX { get; set; }
        public Nullable<double> ShoulderLeftY { get; set; }
        public Nullable<double> ShoulderLeftZ { get; set; }
        public Nullable<double> ElbowLeftX { get; set; }
        public Nullable<double> ElbowLeftY { get; set; }
        public Nullable<double> ElbowLeftZ { get; set; }
        public Nullable<double> WristLeftX { get; set; }
        public Nullable<double> WristLeftY { get; set; }
        public Nullable<double> WristLeftZ { get; set; }
        public Nullable<double> HandLeftX { get; set; }
        public Nullable<double> HandLeftY { get; set; }
        public Nullable<double> HandLeftZ { get; set; }
        public Nullable<double> HipLeftX { get; set; }
        public Nullable<double> HipLeftY { get; set; }
        public Nullable<double> HipLeftZ { get; set; }
        public Nullable<double> KneeLeftX { get; set; }
        public Nullable<double> KneeLeftY { get; set; }
        public Nullable<double> KneeLeftZ { get; set; }
        public Nullable<double> AnkleLeftX { get; set; }
        public Nullable<double> AnkleLeftY { get; set; }
        public Nullable<double> AnkleLeftZ { get; set; }
        public Nullable<double> FootLeftX { get; set; }
        public Nullable<double> FootLeftY { get; set; }
        public Nullable<double> FootLeftZ { get; set; }
        public Nullable<double> HandTipLeftX { get; set; }
        public Nullable<double> HandTipLeftY { get; set; }
        public Nullable<double> HandTipLeftZ { get; set; }
        public Nullable<double> ThumbLeftX { get; set; }
        public Nullable<double> ThumbLeftY { get; set; }
        public Nullable<double> ThumbLeftZ { get; set; }
        public Nullable<double> ShoulderRightX { get; set; }
        public Nullable<double> ShoulderRightY { get; set; }
        public Nullable<double> ShoulderRightZ { get; set; }
        public Nullable<double> ElbowRightX { get; set; }
        public Nullable<double> ElbowRightY { get; set; }
        public Nullable<double> ElbowRightZ { get; set; }
        public Nullable<double> WristRightX { get; set; }
        public Nullable<double> WristRightY { get; set; }
        public Nullable<double> WristRightZ { get; set; }
        public Nullable<double> HandRightX { get; set; }
        public Nullable<double> HandRightY { get; set; }
        public Nullable<double> HandRightZ { get; set; }
        public Nullable<double> HipRightX { get; set; }
        public Nullable<double> HipRightY { get; set; }
        public Nullable<double> HipRightZ { get; set; }
        public Nullable<double> KneeRightX { get; set; }
        public Nullable<double> KneeRightY { get; set; }
        public Nullable<double> KneeRightZ { get; set; }
        public Nullable<double> AnkleRightX { get; set; }
        public Nullable<double> AnkleRightY { get; set; }
        public Nullable<double> AnkleRightZ { get; set; }
        public Nullable<double> FootRightX { get; set; }
        public Nullable<double> FootRightY { get; set; }
        public Nullable<double> FootRightZ { get; set; }
        public Nullable<double> HandTipRightX { get; set; }
        public Nullable<double> HandTipRightY { get; set; }
        public Nullable<double> HandTipRightZ { get; set; }
        public Nullable<double> ThumbRightX { get; set; }
        public Nullable<double> ThumbRightY { get; set; }
        public Nullable<double> ThumbRightZ { get; set; }
        public Nullable<System.DateTime> exam_time { get; set; }
        public string exam_snapshot_path { get; set; }
    
        public virtual tab_exam tab_exam { get; set; }
        public virtual tab_exam_type tab_exam_type { get; set; }
        public virtual ICollection<tab_analysis_result> tab_analysis_result { get; set; }

        public Dictionary<JointType, CameraSpacePoint> convert2JointPoints()
        {
            Dictionary<JointType, CameraSpacePoint> jointPoints = new Dictionary<JointType, CameraSpacePoint>();

            #region body
            CameraSpacePoint p = new CameraSpacePoint();

            if (HeadX != null)
            {
                p.X = (float)this.HeadX; p.Y = (float)this.HeadY; p.Z = (float)this.HeadZ; jointPoints.Add(JointType.Head, p);
            }

            if (NeckX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.NeckX; p.Y = (float)this.NeckY; p.Z = (float)this.NeckZ; jointPoints.Add(JointType.Neck, p);
            }

            if (SpineShoulderX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.SpineShoulderX; p.Y = (float)this.SpineShoulderY; p.Z = (float)this.SpineShoulderZ; jointPoints.Add(JointType.SpineShoulder, p);
            }

            if (SpineBaseX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.SpineBaseX; p.Y = (float)this.SpineBaseY; p.Z = (float)this.SpineBaseZ; jointPoints.Add(JointType.SpineBase, p);
            }

            if (SpineMidX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.SpineMidX; p.Y = (float)this.SpineMidY; p.Z = (float)this.SpineMidZ; jointPoints.Add(JointType.SpineMid, p);
            }
            #endregion body

            #region body left
            if (ShoulderLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ShoulderLeftX; p.Y = (float)this.ShoulderLeftY; p.Z = (float)this.ShoulderLeftZ; jointPoints.Add(JointType.ShoulderLeft, p);
            }

            if (ElbowLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ElbowLeftX; p.Y = (float)this.ElbowLeftY; p.Z = (float)this.ElbowLeftZ; jointPoints.Add(JointType.ElbowLeft, p);
            }

            if (WristLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.WristLeftX; p.Y = (float)this.WristLeftY; p.Z = (float)this.WristLeftZ; jointPoints.Add(JointType.WristLeft, p);
            }

            if (HandLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HandLeftX; p.Y = (float)this.HandLeftY; p.Z = (float)this.HandLeftZ; jointPoints.Add(JointType.HandLeft, p);
            }

            if (HandTipLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HandTipLeftX; p.Y = (float)this.HandTipLeftY; p.Z = (float)this.HandTipLeftZ; jointPoints.Add(JointType.HandTipLeft, p);
            }

            if (ThumbLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ThumbLeftX; p.Y = (float)this.ThumbLeftY; p.Z = (float)this.ThumbLeftZ; jointPoints.Add(JointType.ThumbLeft, p);
            }

            if (HipLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HipLeftX; p.Y = (float)this.HipLeftY; p.Z = (float)this.HipLeftZ; jointPoints.Add(JointType.HipLeft, p);
            }

            if (KneeLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.KneeLeftX; p.Y = (float)this.KneeLeftY; p.Z = (float)this.KneeLeftZ; jointPoints.Add(JointType.KneeLeft, p);
            }

            if (AnkleLeftX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.AnkleLeftX; p.Y = (float)this.AnkleLeftY; p.Z = (float)this.AnkleLeftZ; jointPoints.Add(JointType.AnkleLeft, p);
            }
            #endregion body left

            #region body right
            if (ShoulderRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ShoulderRightX; p.Y = (float)this.ShoulderRightY; p.Z = (float)this.ShoulderRightZ; jointPoints.Add(JointType.ShoulderRight, p);
            }

            if (ElbowRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ElbowRightX; p.Y = (float)this.ElbowRightY; p.Z = (float)this.ElbowRightZ; jointPoints.Add(JointType.ElbowRight, p);
            }

            if (WristRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.WristRightX; p.Y = (float)this.WristRightY; p.Z = (float)this.WristRightZ; jointPoints.Add(JointType.WristRight, p);
            }
            if (HandRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HandRightX; p.Y = (float)this.HandRightY; p.Z = (float)this.HandRightZ; jointPoints.Add(JointType.HandRight, p);
            }


            if (HandTipRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HandTipRightX; p.Y = (float)this.HandTipRightY; p.Z = (float)this.HandTipRightZ; jointPoints.Add(JointType.HandTipRight, p);
            }

            if (ThumbRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.ThumbRightX; p.Y = (float)this.ThumbRightY; p.Z = (float)this.ThumbRightZ; jointPoints.Add(JointType.ThumbRight, p);
            }

            if (HipRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.HipRightX; p.Y = (float)this.HipRightY; p.Z = (float)this.HipRightZ; jointPoints.Add(JointType.HipRight, p);
            }

            if (KneeRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.KneeRightX; p.Y = (float)this.KneeRightY; p.Z = (float)this.KneeRightZ; jointPoints.Add(JointType.KneeRight, p);
            }

            if (AnkleRightX != null)
            {
                p = new CameraSpacePoint(); p.X = (float)this.AnkleRightX; p.Y = (float)this.AnkleRightY; p.Z = (float)this.AnkleRightZ; jointPoints.Add(JointType.AnkleRight, p);
            }
            #endregion body right


            return jointPoints;
        }

        public void setPoints(Dictionary<JointType, CameraSpacePoint> jointPoints)
        {
            #region body
            if (jointPoints.ContainsKey(JointType.Head))
            {
                this.HeadX = jointPoints[JointType.Head].X;
                this.HeadY = jointPoints[JointType.Head].Y;
                this.HeadZ = jointPoints[JointType.Head].Z;
            }

            if (jointPoints.ContainsKey(JointType.Neck))
            {
                this.NeckX = jointPoints[JointType.Neck].X;
                this.NeckY = jointPoints[JointType.Neck].Y;
                this.NeckZ = jointPoints[JointType.Neck].Z;
            }

            if (jointPoints.ContainsKey(JointType.SpineShoulder))
            {
                this.SpineShoulderX = jointPoints[JointType.SpineShoulder].X;
                this.SpineShoulderY = jointPoints[JointType.SpineShoulder].Y;
                this.SpineShoulderZ = jointPoints[JointType.SpineShoulder].Z;
            }

            if (jointPoints.ContainsKey(JointType.SpineBase))
            {
                this.SpineBaseX = jointPoints[JointType.SpineBase].X;
                this.SpineBaseY = jointPoints[JointType.SpineBase].Y;
                this.SpineBaseZ = jointPoints[JointType.SpineBase].Z;
            }

            if (jointPoints.ContainsKey(JointType.SpineMid))
            {
                this.SpineMidX = jointPoints[JointType.SpineMid].X;
                this.SpineMidY = jointPoints[JointType.SpineMid].Y;
                this.SpineMidZ = jointPoints[JointType.SpineMid].Z;
            }
            #endregion body

            #region body left
            if (jointPoints.ContainsKey(JointType.ShoulderLeft))
            {
                this.ShoulderLeftX = jointPoints[JointType.ShoulderLeft].X;
                this.ShoulderLeftY = jointPoints[JointType.ShoulderLeft].Y;
                this.ShoulderLeftZ = jointPoints[JointType.ShoulderLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.ElbowLeft))
            {
                this.ElbowLeftX = jointPoints[JointType.ElbowLeft].X;
                this.ElbowLeftY = jointPoints[JointType.ElbowLeft].Y;
                this.ElbowLeftZ = jointPoints[JointType.ElbowLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.WristLeft))
            {
                this.WristLeftX = jointPoints[JointType.WristLeft].X;
                this.WristLeftY = jointPoints[JointType.WristLeft].Y;
                this.WristLeftZ = jointPoints[JointType.WristLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.HandLeft))
            {
                this.HandLeftX = jointPoints[JointType.HandLeft].X;
                this.HandLeftY = jointPoints[JointType.HandLeft].Y;
                this.HandLeftZ = jointPoints[JointType.HandLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.HandTipLeft))
            {
                this.HandTipLeftX = jointPoints[JointType.HandTipLeft].X;
                this.HandTipLeftY = jointPoints[JointType.HandTipLeft].Y;
                this.HandTipLeftZ = jointPoints[JointType.HandTipLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.ThumbLeft))
            {
                this.ThumbLeftX = jointPoints[JointType.ThumbLeft].X;
                this.ThumbLeftY = jointPoints[JointType.ThumbLeft].Y;
                this.ThumbLeftZ = jointPoints[JointType.ThumbLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.HipLeft))
            {
                this.HipLeftX = jointPoints[JointType.HipLeft].X;
                this.HipLeftY = jointPoints[JointType.HipLeft].Y;
                this.HipLeftZ = jointPoints[JointType.HipLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.KneeLeft))
            {
                this.KneeLeftX = jointPoints[JointType.KneeLeft].X;
                this.KneeLeftY = jointPoints[JointType.KneeLeft].Y;
                this.KneeLeftZ = jointPoints[JointType.KneeLeft].Z;
            }

            if (jointPoints.ContainsKey(JointType.AnkleLeft))
            {
                this.AnkleLeftX = jointPoints[JointType.AnkleLeft].X;
                this.AnkleLeftY = jointPoints[JointType.AnkleLeft].Y;
                this.AnkleLeftZ = jointPoints[JointType.AnkleLeft].Z;
            }
            #endregion body left

            #region body right
            if (jointPoints.ContainsKey(JointType.ShoulderRight))
            {
                this.ShoulderRightX = jointPoints[JointType.ShoulderRight].X;
                this.ShoulderRightY = jointPoints[JointType.ShoulderRight].Y;
                this.ShoulderRightZ = jointPoints[JointType.ShoulderRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.ElbowRight))
            {
                this.ElbowRightX = jointPoints[JointType.ElbowRight].X;
                this.ElbowRightY = jointPoints[JointType.ElbowRight].Y;
                this.ElbowRightZ = jointPoints[JointType.ElbowRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.WristRight))
            {
                this.WristRightX = jointPoints[JointType.WristRight].X;
                this.WristRightY = jointPoints[JointType.WristRight].Y;
                this.WristRightZ = jointPoints[JointType.WristRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.HandRight))
            {
                this.HandRightX = jointPoints[JointType.HandRight].X;
                this.HandRightY = jointPoints[JointType.HandRight].Y;
                this.HandRightZ = jointPoints[JointType.HandRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.HandTipRight))
            {
                this.HandTipRightX = jointPoints[JointType.HandTipRight].X;
                this.HandTipRightY = jointPoints[JointType.HandTipRight].Y;
                this.HandTipRightZ = jointPoints[JointType.HandTipRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.ThumbRight))
            {
                this.ThumbRightX = jointPoints[JointType.ThumbRight].X;
                this.ThumbRightY = jointPoints[JointType.ThumbRight].Y;
                this.ThumbRightZ = jointPoints[JointType.ThumbRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.HipRight))
            {
                this.HipRightX = jointPoints[JointType.HipRight].X;
                this.HipRightY = jointPoints[JointType.HipRight].Y;
                this.HipRightZ = jointPoints[JointType.HipRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.KneeRight))
            {
                this.KneeRightX = jointPoints[JointType.KneeRight].X;
                this.KneeRightY = jointPoints[JointType.KneeRight].Y;
                this.KneeRightZ = jointPoints[JointType.KneeRight].Z;
            }

            if (jointPoints.ContainsKey(JointType.AnkleRight))
            {
                this.AnkleRightX = jointPoints[JointType.AnkleRight].X;
                this.AnkleRightY = jointPoints[JointType.AnkleRight].Y;
                this.AnkleRightZ = jointPoints[JointType.AnkleRight].Z;
            }
            #endregion body right

        }
    }
}
