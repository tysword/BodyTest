//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        private const int Time_span =1000/30;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        KinectSensor _sensor;

        MultiSourceFrameReader _reader;

        CameraMode _mode = CameraMode.Color;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        public tab_exam exam = null;

        private Dictionary<String, tab_exam_type> typeList;

        private tab_exam_type currrentType;

        private Dictionary<JointType, CameraSpacePoint> jointPoints = new Dictionary<JointType,CameraSpacePoint>();

        private bool showHands = false;

        private IReadOnlyCollection<JointType> handsJoints = new List<JointType>(){
            JointType.ThumbLeft,JointType.HandLeft,JointType.HandTipLeft,JointType.HandTipRight,JointType.HandRight,JointType.ThumbRight
        };

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap colorBitmap = null;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow(tab_exam exam)
        {
            this.exam = exam;
            
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);

                // create the colorFrameDescription from the ColorFrameSource using Bgra format
                FrameDescription colorFrameDescription = this._sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

                // create the bitmap to display
                this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
               
            }

            #region bind bones
            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));
            
            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            // populate body colors, one for each BodyIndex
            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            #endregion

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            initExam(); 
        }


        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }
   

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
        }
       
        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
          
            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {

                    FrameDescription colorFrameDescription = frame.FrameDescription;

                    using (KinectBuffer colorBuffer = frame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                           frame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                          
                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                            
                        }

                        this.colorBitmap.Unlock();
                    }
                }
            }
            // Depth
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Depth)
                    {
                        //camera.Source = frame.ToBitmap();
                    }
                }
            }
            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Infrared)
                    {
                       // camera.Source = frame.LockImageBuffer
                    }
                }
            }
            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();
                    bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

                   
                    foreach (var body in bodies)
                    {
                        if (body.IsTracked)
                        {
                            this.jointPoints = new Dictionary<JointType, CameraSpacePoint>();
                            // COORDINATE MAPPING
                            foreach (JointType joint in body.Joints.Keys)
                            {

                                //show joints of hands
                                if (!showHands)
                                {
                                    if (handsJoints.Contains(joint))
                                        continue;
                                }

                                if ( body.Joints[joint].TrackingState == TrackingState.Tracked)
                                {
                                   
                                    // 3D space point
                                    CameraSpacePoint jointPosition =  body.Joints[joint].Position;

                                    // the joints for save 
                                    this.jointPoints.Add(joint, jointPosition);


                                    // 2D space point
                                    Point point = new Point();
                                    if (_mode == CameraMode.Color)
                                    {
                                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
                                        point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                                        point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                                    }
                                    else if (_mode == CameraMode.Depth || _mode == CameraMode.Infrared) // Change the Image and Canvas dimensions to 512x424
                                    {
                                        DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(jointPosition);
                                        point.X = float.IsInfinity(depthPoint.X) ? 0 : depthPoint.X;
                                        point.Y = float.IsInfinity(depthPoint.Y) ? 0 : depthPoint.Y;
                                    }
                                    // Draw


                                    Ellipse ellipse = new Ellipse
                                    {
                                        Fill = Brushes.Red,
                                        Width =10,
                                        Height = 10
                                    };
                                    Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                    Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
                                    canvas.Children.Add(ellipse);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (exam.finish_flag != tab_exam.ExamFinishFlagEnd)
            {
                MessageBoxResult result = MessageBox.Show("当前测试未完成，是否确定退出?", "", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (_reader != null)
            {
                _reader.Dispose();
            }
            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.colorBitmap;
            }
        }


        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

     

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            //this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
            //                                                : Properties.Resources.SensorNotAvailableStatusText;
        }


        private void initExam()
        {
            using (var ctx = new jointexamEntities())
            {
                #region 设置person信息
                tab_person p = ctx.tab_person.Find(exam.person_id);

                this.txtName.Text = p.name;
                this.txtID.Text = p.id.ToString();
                this.txtSex.Text = p.sex;
                this.txtRace.Text = p.race;
                this.txtWeight.Text = p.weight.ToString();
                this.txtHeight.Text = p.height.ToString();
                this.txtBirthday.Text = ((DateTime)p.birthday).ToLongDateString();
                this.txtAge.Text = exam.exam_age.ToString();
                #endregion

                #region 设置检验类型
                var ts = from c in ctx.tab_exam_type select c;
                typeList = new Dictionary<String, tab_exam_type>();

                List<tab_exam_type> types = ts.ToList<tab_exam_type>();
                foreach (var t in types)
                {
                    Button btn = new Button { Content = t.name, Name = t.name };
                    btn.Click += btn_Click;
                    btn.Margin = new Thickness(5);

                    wpExamType.Children.Add(btn);
                    typeList.Add(t.name, t);
                }
            }
                #endregion
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (currrentType != null)
            {
                MessageBox.Show("当前测试未完成，请完成后在试！");
                return;
            }

            Button b = (Button)sender;
            b.Background = new SolidColorBrush(Colors.Red);

            currrentType = typeList[b.Name];

            switch (currrentType.style)
            {
                case 1:
                    this.btnEndExam.Visibility = Visibility.Visible;
                    break;
                case 2:
                    this.btnStartExam.Visibility = Visibility.Visible;
                    this.btnEndExam.Visibility = Visibility.Visible;
                    break;
            }
        }

        private DispatcherTimer dispatcherTimer = null;
        private int dynamic_exam_times =0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
         

            if (b.Name == "btnStartExam")
            {
                dynamic_exam_times = 0;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Time_span);


                dispatcherTimer.Tick+=dispatcherTimer_Tick;
                dispatcherTimer.Start();

                this.btnStartExam.Visibility = Visibility.Hidden;
            }

            if (b.Name == "btnEndExam")
            {
                switch (currrentType.style)
                {
                    case 1:
                        Dictionary<JointType, CameraSpacePoint> jointPoints = this.jointPoints;

                        if (saveJoints(currrentType, jointPoints))
                        {
                            MessageBox.Show(currrentType.name + "测试完成！");
                            this.btnEndExam.Visibility = Visibility.Hidden;
                            currrentType = null;
                        }
                        else
                        {
                            MessageBox.Show("无法获得检查数据，请调整姿势重新测量！");
                        }
                        break;
                    case 2:
                        if (dynamic_exam_times > 0) { 
                            dispatcherTimer.Stop();
                            MessageBox.Show(currrentType.name + "测试完成！");
                            this.btnEndExam.Visibility = Visibility.Hidden;
                            currrentType = null;
                        }
                        else
                        {
                            MessageBox.Show("未获得检查数据，请调整姿势重新测量！");
                        }
                        break;
                }


            }

            #region save to excel
            /*
            this.kinectSensor.Close();
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）  
            Microsoft.Office.Interop.Excel.Worksheet excelWS = (Microsoft.Office.Interop.Excel.Worksheet)excelWB.Worksheets[1];

            excelWS.get_Range("A1", System.Type.Missing).Value = "头";
            excelWS.get_Range("B1", System.Type.Missing).Value = this.HeadX.Content.ToString();
            excelWS.get_Range("C1", System.Type.Missing).Value = this.HeadY.Content.ToString();
            excelWS.get_Range("D1", System.Type.Missing).Value = this.HeadZ.Content.ToString();

            excelWS.get_Range("A2", System.Type.Missing).Value = "颈";
            excelWS.get_Range("B2", System.Type.Missing).Value = this.NeckX.Content.ToString();
            excelWS.get_Range("C2", System.Type.Missing).Value = this.NeckY.Content.ToString();
            excelWS.get_Range("D2", System.Type.Missing).Value = this.NeckZ.Content.ToString();

            excelWS.get_Range("A3", System.Type.Missing).Value = "肩脊";
            excelWS.get_Range("B3", System.Type.Missing).Value = this.SpineShoulderX.Content.ToString();
            excelWS.get_Range("C3", System.Type.Missing).Value = this.SpineShoulderY.Content.ToString();
            excelWS.get_Range("D3", System.Type.Missing).Value = this.SpineShoulderZ.Content.ToString();

            excelWS.get_Range("A4", System.Type.Missing).Value = "脊中";
            excelWS.get_Range("B4", System.Type.Missing).Value = this.SpineMidX.Content.ToString();
            excelWS.get_Range("C4", System.Type.Missing).Value = this.SpineMidY.Content.ToString();
            excelWS.get_Range("D4", System.Type.Missing).Value = this.SpineMidZ.Content.ToString();

            excelWS.get_Range("A5", System.Type.Missing).Value = "脊尾";
            excelWS.get_Range("B5", System.Type.Missing).Value = this.SpineBaseX.Content.ToString();
            excelWS.get_Range("C5", System.Type.Missing).Value = this.SpineBaseY.Content.ToString();
            excelWS.get_Range("D5", System.Type.Missing).Value = this.SpineBaseZ.Content.ToString();

            excelWS.get_Range("A6", System.Type.Missing).Value = "左肩";
            excelWS.get_Range("B6", System.Type.Missing).Value = this.ShoulderLeftX.Content.ToString();
            excelWS.get_Range("C6", System.Type.Missing).Value = this.ShoulderLeftY.Content.ToString();
            excelWS.get_Range("D6", System.Type.Missing).Value = this.ShoulderLeftZ.Content.ToString();

            excelWS.get_Range("A7", System.Type.Missing).Value = "左肘";
            excelWS.get_Range("B7", System.Type.Missing).Value = this.ElbowLeftX.Content.ToString();
            excelWS.get_Range("C7", System.Type.Missing).Value = this.ElbowLeftY.Content.ToString();
            excelWS.get_Range("D7", System.Type.Missing).Value = this.ElbowLeftZ.Content.ToString();

            excelWS.get_Range("A8", System.Type.Missing).Value = "左腕";
            excelWS.get_Range("B8", System.Type.Missing).Value = this.WristLeftX.Content.ToString();
            excelWS.get_Range("C8", System.Type.Missing).Value = this.WristLeftY.Content.ToString();
            excelWS.get_Range("D8", System.Type.Missing).Value = this.WristLeftZ.Content.ToString();

            excelWS.get_Range("A9", System.Type.Missing).Value = "左手";
            excelWS.get_Range("B9", System.Type.Missing).Value = this.HandLeftX.Content.ToString();
            excelWS.get_Range("C9", System.Type.Missing).Value = this.HandLeftY.Content.ToString();
            excelWS.get_Range("D9", System.Type.Missing).Value = this.HandLeftZ.Content.ToString();

            excelWS.get_Range("A10", System.Type.Missing).Value = "左手尖";
            excelWS.get_Range("B10", System.Type.Missing).Value = this.HandTipLeftX.Content.ToString();
            excelWS.get_Range("C10", System.Type.Missing).Value = this.HandTipLeftY.Content.ToString();
            excelWS.get_Range("D10", System.Type.Missing).Value = this.HandTipLeftZ.Content.ToString();

            excelWS.get_Range("A11", System.Type.Missing).Value = "左拇指";
            excelWS.get_Range("B11", System.Type.Missing).Value = this.ThumbLeftX.Content.ToString();
            excelWS.get_Range("C11", System.Type.Missing).Value = this.ThumbLeftY.Content.ToString();
            excelWS.get_Range("D11", System.Type.Missing).Value = this.ThumbLeftZ.Content.ToString();

            excelWS.get_Range("A12", System.Type.Missing).Value = "左臀";
            excelWS.get_Range("B12", System.Type.Missing).Value = this.HipLeftX.Content.ToString();
            excelWS.get_Range("C12", System.Type.Missing).Value = this.HipLeftY.Content.ToString();
            excelWS.get_Range("D12", System.Type.Missing).Value = this.HipLeftZ.Content.ToString();

            excelWS.get_Range("A13", System.Type.Missing).Value = "左膝";
            excelWS.get_Range("B13", System.Type.Missing).Value = this.KneeLeftX.Content.ToString();
            excelWS.get_Range("C13", System.Type.Missing).Value = this.KneeLeftY.Content.ToString();
            excelWS.get_Range("D13", System.Type.Missing).Value = this.KneeLeftZ.Content.ToString();

            excelWS.get_Range("A14", System.Type.Missing).Value = "左踝";
            excelWS.get_Range("B14", System.Type.Missing).Value = this.AnkleLeftX.Content.ToString();
            excelWS.get_Range("C14", System.Type.Missing).Value = this.AnkleLeftY.Content.ToString();
            excelWS.get_Range("D14", System.Type.Missing).Value = this.AnkleLeftZ.Content.ToString();

            excelWS.get_Range("A15", System.Type.Missing).Value = "左脚";
            excelWS.get_Range("B15", System.Type.Missing).Value = this.FootLeftX.Content.ToString();
            excelWS.get_Range("C15", System.Type.Missing).Value = this.FootLeftY.Content.ToString();
            excelWS.get_Range("D15", System.Type.Missing).Value = this.FootLeftZ.Content.ToString();

            excelWS.get_Range("A16", System.Type.Missing).Value = "右肩";
            excelWS.get_Range("B16", System.Type.Missing).Value = this.ShoulderRightX.Content.ToString();
            excelWS.get_Range("C16", System.Type.Missing).Value = this.ShoulderRightY.Content.ToString();
            excelWS.get_Range("D16", System.Type.Missing).Value = this.ShoulderRightZ.Content.ToString();

            excelWS.get_Range("A17", System.Type.Missing).Value = "右肘";
            excelWS.get_Range("B17", System.Type.Missing).Value = this.ElbowRightX.Content.ToString();
            excelWS.get_Range("C17", System.Type.Missing).Value = this.ElbowRightY.Content.ToString();
            excelWS.get_Range("D17", System.Type.Missing).Value = this.ElbowRightZ.Content.ToString();

            excelWS.get_Range("A18", System.Type.Missing).Value = "右腕";
            excelWS.get_Range("B18", System.Type.Missing).Value = this.WristRightX.Content.ToString();
            excelWS.get_Range("C18", System.Type.Missing).Value = this.WristRightY.Content.ToString();
            excelWS.get_Range("D18", System.Type.Missing).Value = this.WristRightZ.Content.ToString();

            excelWS.get_Range("A19", System.Type.Missing).Value = "右手";
            excelWS.get_Range("B19", System.Type.Missing).Value = this.HandRightX.Content.ToString();
            excelWS.get_Range("C19", System.Type.Missing).Value = this.HandRightY.Content.ToString();
            excelWS.get_Range("D19", System.Type.Missing).Value = this.HandRightZ.Content.ToString();

            excelWS.get_Range("A20", System.Type.Missing).Value = "右手尖";
            excelWS.get_Range("B20", System.Type.Missing).Value = this.HandTipRightX.Content.ToString();
            excelWS.get_Range("C20", System.Type.Missing).Value = this.HandTipRightY.Content.ToString();
            excelWS.get_Range("D20", System.Type.Missing).Value = this.HandTipRightZ.Content.ToString();

            excelWS.get_Range("A21", System.Type.Missing).Value = "右拇指";
            excelWS.get_Range("B21", System.Type.Missing).Value = this.ThumbRightX.Content.ToString();
ect\A                    excelWS.getssing).V_Range("B22", Systemy.page.Mise = this.VHipRig.)htX.Content.ToString();
    excelWS.get_Range("C21", System.Type.Missing).Value = ThmbRiughtis.Yth.ContTent.ToString();
            excelWS.get_Range("D21", System.Type.Missing).Value = this.ThumbRightZ.Content.ToString();
sinlu右ksp
            excelWS.get_Range("A22", System.Type.MialBue oay\asics-WPFp\\dakiBnp.config"; 
            excelWS.get_Range("C22", System.Type.Missing).Value = this.HipRightY.Content.ToString();
            excelWS.get_Range("D22", System.Type.Missing).Value = this.HipRightZ.Content.ToString();

            excelWS.get_Range("A23", System.Type.Missing).Value = "右膝";
            excelWS.get_Range("B23", System.Type.Missing).Value = this.KneeRightX.Content.ToString();
            excelWS.get_Range("C23", System.Type.Missing).Value = this.KneeRightY.Content.ToString();
            excelWS.get_Range("D23", System.Type.Missing).Value = this.KneeRightZ.Content.ToString();

            excelWS.get_Range("A24", System.Type.Missing).Value = "右踝";
            excelWS.get_Range("B24", System.Type.Missing).Value = this.AnkleRightX.Content.ToString();
            excelWS.get_Range("C24", System.Type.Missing).Value = this.AnkleRightY.Content.ToString();
            excelWS.get_Range("D24", System.Type.Missing).Value = this.AnkleRightZ.Content.ToString();

            excelWS.get_Range("A25", System.Type.Missing).Value = "右脚";
            excelWS.get_Range("B25", System.Type.Missing).Value = this.FootRightX.Content.ToString();
            excelWS.get_Range("C25", System.Type.Missing).Value = this.FootRightY.Content.ToString();
            excelWS.get_Range("D25", System.Type.Missing).Value = this.FootRightZ.Content.ToString();

            DateTime d = System.DateTime.Now;
            String fileName = d.Month + "_" + d.Day + "_" + d.Hour + "_" + d.Minute;
            excelWB.SaveAs("D:\\分析\\" + fileName + ".xlsx");//将其进行保存到指定的路径  
            excelWB.Close();
            excelApp.Quit();
            this.kinectSensor.Open();
            MessageBox.Show("文件导出成功！");
             */

            #endregion
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Dictionary<JointType, CameraSpacePoint> jointPoints = this.jointPoints;

            if (saveJoints(currrentType,jointPoints))
            {
                dynamic_exam_times++;
            }
        }

        private bool saveJoints(tab_exam_type type, Dictionary<JointType, CameraSpacePoint> jointPoints)
        {
           

            if (!validatedJointPoints(type,jointPoints))
            {
                return false;
            }

            String path = saveImage(jointPoints);

            using (var ctx = new jointexamEntities())
            {
                tab_exam_record t = new tab_exam_record()
                {
                    exam_id = exam.id,
                    exam_type_id = type.id,
                    exam_time = DateTime.Now,
                    exam_snapshot_path = path
                };

                t.setPoints(jointPoints);
               
                ctx.tab_exam_record.Add(t);
                ctx.SaveChanges();

                this.jointPoints = new Dictionary<JointType, CameraSpacePoint>();
            }
            return true;
        }

        private bool validatedJointPoints(tab_exam_type type, Dictionary<JointType, CameraSpacePoint> joints)
        {
            if (joints != null && joints.ContainsKey(JointType.SpineMid))
            {
                return true;
            }
            else
                return false;
        }

        private String saveImage(Dictionary<JointType, CameraSpacePoint> jointPoints)
        {
            if (this.colorBitmap != null)
            {
                WriteableBitmap b = this.colorBitmap.Clone();

                foreach (JointType joint in jointPoints.Keys)
                {
                    ColorSpacePoint point = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPoints[joint]);
                    point.X = float.IsInfinity(point.X) ? 0 : point.X;
                    point.Y = float.IsInfinity(point.Y) ? 0 : point.Y;
                    b.FillEllipseCentered((int)point.X,(int) point.Y, 5, 5, Color.FromRgb(255, 0, 0));
                }

                // create a png bitmap encoder which knows how to save a .png file
                BitmapEncoder encoder = new JpegBitmapEncoder();

                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(b));

                string time = System.DateTime.Now.ToString("yyyyMMdd'-'mm'-'ss'-'fff", CultureInfo.CurrentUICulture.DateTimeFormat);

                string myPhotos = "d:\\myPhotos\\";

                string path = System.IO.Path.Combine(myPhotos, "KinectScreenshot-Color-" + time + ".jpg");

                // write the new file to disk
                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                    return path;
                }
                catch (IOException e)
                {
                  Console.WriteLine(e);
                  
                }
            }

            return null;
        }

        private void Button_Finish_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new jointexamEntities())
            {
                var ex =ctx.tab_exam.SingleOrDefault(a => a.id == exam.id);
                ex.finish_flag = tab_exam.ExamFinishFlagEnd;
                ctx.SaveChanges();
            }

            this.Close();
        }
}

    enum CameraMode
    {
        Color,
        Depth,
        Infrared
    }
}
