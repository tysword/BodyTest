using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class JointCaculater
    {
        public static double caculateHight(CameraSpacePoint pointA, CameraSpacePoint pointB)
        {
            return pointA.Y - pointB.Y;
        }

        public static CameraSpacePoint caculateCoordinateDifference(CameraSpacePoint pointA, CameraSpacePoint pointB)
        {
            CameraSpacePoint c = new CameraSpacePoint();
            c.X = pointA.X - pointB.X;
            c.Y = pointA.Y - pointB.Y;
            c.Z = pointA.Z - pointB.Z;

            return c;
        }

        public static double caculateDegreesDefference(CameraSpacePoint pointA, CameraSpacePoint pointB, CameraSpacePoint pointC, CameraSpacePoint pointD,
                                                       CameraSpacePoint pointA1, CameraSpacePoint pointB1, CameraSpacePoint pointC1, CameraSpacePoint pointD1)
        {
            double degrees1 = caculateDegrees(pointA, pointB, pointC, pointD);
            double degrees2 = caculateDegrees(pointA1, pointB1, pointC1, pointD1);

            return degrees1 - degrees2;
        }


        public static double caculateDegrees(CameraSpacePoint pointA, CameraSpacePoint pointB, CameraSpacePoint pointC, CameraSpacePoint pointD)
        {
            double AB_CD = Math.Abs((pointB.X - pointA.X)
                   * (pointD.X - pointC.X) + (pointB.Y - pointA.Y)
                   * (pointD.Y - pointC.Y) + (pointB.Z - pointA.Z)
                   * (pointD.Z - pointC.Z));
            double sprtAB = Math.Sqrt((pointB.X - pointA.X)
                    * (pointB.X - pointA.X) + (pointB.Y - pointA.Y)
                    * (pointB.Y - pointA.Y) + (pointB.Z - pointA.Z)
                    * (pointB.Z - pointA.Z));
            double sprtCD = Math.Sqrt((pointC.X - pointD.X)
                    * (pointC.X - pointD.X) + (pointC.Y - pointD.Y)
                    * (pointC.Y - pointD.Y) + (pointC.Z - pointD.Z)
                    * (pointC.Z - pointD.Z));
            double cos = AB_CD / (sprtCD * sprtAB);

            return (ToDegrees(Math.Acos(cos)));
        }

        private static double ToDegrees(double p)
        {
            return p * 180.0 / Math.PI;
        }



        internal static tab_analysis_result analysis(tab_analysis_type t, tab_exam_record rs)
        {
            switch (t.analysis_fun)
            {
                case 1:
                    return analysisDegrees(t,rs);
                case 2:
                    return analysisDegreesDefference(t, rs);
                case 3:
                    return analysisHight(t,rs);
                case 4:
                    return analysisX(t, rs);
                case 5:
                    return analysisCoordinate(t, rs);
            }

            return null;
        }

        private static tab_analysis_result analysisCoordinate(tab_analysis_type t, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;

            List<CameraSpacePoint> usePoints = getAnalysisJoints(t, record);
            if (usePoints.Count == 1)
            {
                r.x = usePoints[0].X;
                r.y = usePoints[0].Y;
                r.z = usePoints[0].Z;
                return r;
            }
            return null;
        }


        private static tab_analysis_result analysisCoordinateDefference(tab_analysis_type t, tab_exam_record baseRecord, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;

            List<CameraSpacePoint> comparePoints = getAnalysisJoints(t, record);
            List<CameraSpacePoint> basePoints = getAnalysisJoints(t, baseRecord);

            if (comparePoints.Count == 1 && basePoints.Count ==1)
            {
                r.x = comparePoints[0].X - basePoints[0].X;
                r.y = comparePoints[0].Y - basePoints[0].Y;
                r.z = comparePoints[0].Z - basePoints[0].Z;
                return r;
            }

            return null;
        }

        private static tab_analysis_result analysisX(tab_analysis_type t, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;

            List<CameraSpacePoint> usePoints = getAnalysisJoints(t, record);
            if (usePoints.Count == 2)
            {
                r.x = caculateX(usePoints[0], usePoints[1]);
                return r;
            }
            return null;
        }

        private static double? caculateX(CameraSpacePoint p1, CameraSpacePoint p2)
        {
            return p1.X - p2.X;
        }

        private static tab_analysis_result analysisDegreesDefference(tab_analysis_type t, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;

            List<CameraSpacePoint> usePoints = getAnalysisJoints(t, record);
            if (usePoints.Count == 8)
            {
                r.degrees = caculateDegreesDefference(usePoints[0], usePoints[1], usePoints[2], usePoints[3], usePoints[4], usePoints[5], usePoints[6], usePoints[7]);
                return r;
            }
            return null;
        }

        private static tab_analysis_result analysisHight(tab_analysis_type t, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;

            List<CameraSpacePoint> usePoints = getAnalysisJoints(t, record);
             if (usePoints.Count == 2)
            {
                r.y = caculateHight(usePoints[0], usePoints[1]);
                return r;
            }
            return null;
        }

        private static tab_analysis_result analysisDegrees(tab_analysis_type t,  tab_exam_record record)
        {
            tab_analysis_result r = createAnalysisResult(t, record);

            List<CameraSpacePoint> usePoints = getAnalysisJoints(t, record);
            if (usePoints.Count == 4)
            {
                r.degrees = caculateDegrees(usePoints[0], usePoints[1], usePoints[2], usePoints[3]);
                return r;
            }
            return null;
        }

        private static tab_analysis_result createAnalysisResult(tab_analysis_type t, tab_exam_record record)
        {
            tab_analysis_result r = new tab_analysis_result();

            r.exam_record = record.id;
            r.analysis_type = t.id;
            r.tab_analysis_type = t;
            r.tab_exam_record = record;
            return r;
        }

        private static List<CameraSpacePoint> getAnalysisJoints(tab_analysis_type t, tab_exam_record record)
        {
            Dictionary<JointType, CameraSpacePoint> points = record.convert2JointPoints();

            List<CameraSpacePoint> usePoints = new List<CameraSpacePoint>();
            String[] jointTypeValues = t.use_joints.Split(',');
            foreach (String s in jointTypeValues)
            {


                if (s.Equals("25"))
                {
                    CameraSpacePoint p = new CameraSpacePoint();
                    p.X = 0;
                    p.Y = 0;
                    p.Z = 0;
                    usePoints.Add(p);
                }
                else if (s.Equals("26"))
                {
                    CameraSpacePoint p = new CameraSpacePoint();
                    p.X = 0;
                    p.Y = 10;
                    p.Z = 0;
                    usePoints.Add(p);
                }
                else
                {
                    JointType j = (JointType)Enum.Parse(typeof(JointType), s);
                    if (points.ContainsKey(j)) { 
                        usePoints.Add(points[j]);
                    }
                    else
                    {
                        //throw new Exception();
                    }
                }
            }

            return usePoints;
        }

        public void analysis(tab_exam exam)
        {
            using (var ctx = new jointexamEntities())
            {
                Dictionary<tab_exam_type, Dictionary<String, tab_analysis_type>> analysisTypeOfExamType = new Dictionary<tab_exam_type, Dictionary<String, tab_analysis_type>>();
                StringBuilder b = new StringBuilder();
                b.Append("select * from tab_analysis_type t");
                DbSet<tab_analysis_type> pset = ctx.Set<tab_analysis_type>();
                List<tab_analysis_type> pl = pset.SqlQuery(b.ToString()).ToList<tab_analysis_type>();

                foreach (tab_analysis_type t in pl)
                {
                    if (!analysisTypeOfExamType.ContainsKey(t.tab_exam_type))
                        analysisTypeOfExamType.Add(t.tab_exam_type, new Dictionary<String, tab_analysis_type>());

                    analysisTypeOfExamType[t.tab_exam_type].Add(t.id.ToString(), t);
                }

                foreach (tab_exam_type et in analysisTypeOfExamType.Keys)
                {
                    if (et.style == 1)  //静态测试动作分析
                    {
                        foreach (tab_analysis_type t in analysisTypeOfExamType[et].Values)
                        {
                           
                            List<tab_exam_record> rs = getExamRecordOfType(t.exam_type, exam.id,ctx);

                            if (rs != null && rs.Count > 0)
                            {
                                tab_analysis_result analysisResult = JointCaculater.analysis(t, rs.First());

                                if (analysisResult != null)
                                {
                                    ctx.tab_analysis_result.Add(analysisResult);
                                    ctx.SaveChanges();
                                }
                            }
                        }
                    }
                    else   //动态测试动作分析
                    {
                        if (et.id == 8)   //右侧大字站立
                        {
                         /*   b = new StringBuilder();
                            b.Append("select * from tab_exam_record a where a.exam_type_id = '" + et.id + "' and a.exam_id = '" + exam.id + "'");
                            DbSet<tab_exam_record> records = ctx.Set<tab_exam_record>();
                            List<tab_exam_record> rs = records.SqlQuery(b.ToString()).ToList<tab_exam_record>();
                            */
                            List<tab_exam_record> rs = getExamRecordOfType(et.id, exam.id, ctx);
                            tab_exam_record ter = analysis44(analysisTypeOfExamType[et]["44"], analysisTypeOfExamType[et]["45"], rs);

                            if (ter != null)
                            {
                                tab_analysis_result analysisResult1 = JointCaculater.analysis(analysisTypeOfExamType[et]["44"], ter);
                                tab_analysis_result analysisResult2 = JointCaculater.analysis(analysisTypeOfExamType[et]["45"], ter);

                                ctx.tab_analysis_result.Add(analysisResult1);
                                ctx.tab_analysis_result.Add(analysisResult2);

                                ctx.SaveChanges();
                            }
                        }

                        if (et.id == 7)  //左侧大字站立
                        {
                          /*  b = new StringBuilder();
                            b.Append("select * from tab_exam_record a where a.exam_type_id = '" + et.id + "' and a.exam_id = '" + exam.id + "'");
                            DbSet<tab_exam_record> records = ctx.Set<tab_exam_record>();
                            List<tab_exam_record> rs = records.SqlQuery(b.ToString()).ToList<tab_exam_record>();*/
                            List<tab_exam_record> rs = getExamRecordOfType(et.id, exam.id, ctx);
                            tab_exam_record ter = analysis44(analysisTypeOfExamType[et]["46"], analysisTypeOfExamType[et]["47"], rs);
                            if (ter != null)
                            {
                                tab_analysis_result analysisResult1 = JointCaculater.analysis(analysisTypeOfExamType[et]["46"], ter);
                                tab_analysis_result analysisResult2 = JointCaculater.analysis(analysisTypeOfExamType[et]["47"], ter);

                                ctx.tab_analysis_result.Add(analysisResult1);
                                ctx.tab_analysis_result.Add(analysisResult2);

                                ctx.SaveChanges();
                            }
                        }

                        if (et.id == 6) //侧平举
                        {
                            List<tab_exam_record> rs = getExamRecordOfType(et.id, exam.id, ctx);
                            List<tab_exam_record> analysisRs = getExamRecordOfAnalysisType6(analysisTypeOfExamType[et], rs);

                            if (analysisRs.Count == 0)
                                continue;

                            tab_analysis_result analysisResult1 = JointCaculater.analysis(analysisTypeOfExamType[et]["48"], analysisRs[0]);
                            tab_analysis_result analysisResult2 = JointCaculater.analysis(analysisTypeOfExamType[et]["49"], analysisRs[analysisRs.Count-1]);

                            tab_analysis_result analysisResult3 = createAnalysisResult(analysisTypeOfExamType[et]["50"], analysisRs[analysisRs.Count - 1]);
                            analysisResult3.degrees = Math.Abs((double)(analysisResult1.degrees - analysisResult2.degrees)); 

                            tab_analysis_result analysisResult4 = JointCaculater.analysis(analysisTypeOfExamType[et]["51"], analysisRs[0]);
                            tab_analysis_result analysisResult5 = JointCaculater.analysis(analysisTypeOfExamType[et]["52"], analysisRs[analysisRs.Count-1]);

                            tab_analysis_result analysisResult6 = createAnalysisResult(analysisTypeOfExamType[et]["53"], analysisRs[analysisRs.Count - 1]);
                            analysisResult6.degrees = Math.Abs((double)(analysisResult4.degrees - analysisResult5.degrees));

                            ctx.tab_analysis_result.Add(analysisResult1);
                            ctx.tab_analysis_result.Add(analysisResult2);
                            ctx.tab_analysis_result.Add(analysisResult3);

                            ctx.tab_analysis_result.Add(analysisResult4);
                            ctx.tab_analysis_result.Add(analysisResult5);
                            ctx.tab_analysis_result.Add(analysisResult6);

                            int compareFrames = 0;

                            //左肘
                            tab_analysis_result ar6 = analysisCoordinate(analysisTypeOfExamType[et]["54"], analysisRs[0]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["55"].frames)));
                            tab_analysis_result ar8 = analysisCoordinateDefference(analysisTypeOfExamType[et]["55"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["56"].frames)));
                            tab_analysis_result ar9 = analysisCoordinateDefference(analysisTypeOfExamType[et]["56"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["57"].frames)));
                            tab_analysis_result ar10 = analysisCoordinateDefference(analysisTypeOfExamType[et]["57"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["58"].frames)));
                            tab_analysis_result ar11 = analysisCoordinateDefference(analysisTypeOfExamType[et]["58"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);
                            ctx.tab_analysis_result.Add(ar6);
                            ctx.tab_analysis_result.Add(ar8);
                            ctx.tab_analysis_result.Add(ar9);
                            ctx.tab_analysis_result.Add(ar10);
                            ctx.tab_analysis_result.Add(ar11);

                            //右肘         
                            tab_analysis_result ar16 = analysisCoordinate(analysisTypeOfExamType[et]["65"], analysisRs[0]);
                                   
                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["66"].frames)));
                            tab_analysis_result ar12 = analysisCoordinateDefference(analysisTypeOfExamType[et]["66"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["67"].frames)));
                            tab_analysis_result ar13 = analysisCoordinateDefference(analysisTypeOfExamType[et]["67"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["68"].frames)));
                            tab_analysis_result ar14 = analysisCoordinateDefference(analysisTypeOfExamType[et]["68"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            compareFrames = Convert.ToInt32(Math.Floor((double)((analysisRs.Count - 1) * analysisTypeOfExamType[et]["69"].frames)));
                            tab_analysis_result ar15 = analysisCoordinateDefference(analysisTypeOfExamType[et]["69"],
                                                                                    analysisRs[0],
                                                                                    analysisRs[compareFrames]);

                            ctx.tab_analysis_result.Add(ar16);
                            ctx.tab_analysis_result.Add(ar12);
                            ctx.tab_analysis_result.Add(ar13);
                            ctx.tab_analysis_result.Add(ar14);
                            ctx.tab_analysis_result.Add(ar15);

                            ctx.SaveChanges();
                        }



                        if (et.id == 5) //慢蹲慢起
                        {
                            List<tab_exam_record> rs = getExamRecordOfType(et.id, exam.id, ctx);
                            if (rs == null || rs.Count == 0)
                                continue;

                            tab_exam_record r = getExamRecordOfAnalysisType7(rs);

                            tab_analysis_result r1 = analysisDegrees(analysisTypeOfExamType[et]["70"], r);
                            tab_analysis_result r2 = analysisDegrees(analysisTypeOfExamType[et]["71"], r);
                            tab_analysis_result r3 = analysisDegrees(analysisTypeOfExamType[et]["72"], r);

                            if (r1 != null)
                                ctx.tab_analysis_result.Add(r1);
                            if (r2 != null)
                                ctx.tab_analysis_result.Add(r2);
                            if (r3 != null)
                                ctx.tab_analysis_result.Add(r3);

                            ctx.SaveChanges();
                        }
                    }
                }
            }
        }

        private tab_exam_record getExamRecordOfAnalysisType7(List<tab_exam_record> rs)
        {
            tab_exam_record temp = rs[0];
            foreach (tab_exam_record r in rs)
            {
                if (r.HipRightY < temp.HipRightY)
                {
                    temp = r;
                }
            }

            return temp;
        }

        private List<tab_exam_record> getExamRecordOfAnalysisType6(Dictionary<string, tab_analysis_type> dictionary, List<tab_exam_record> rs)
        {
            List<List<tab_exam_record>> ars = new List<List<tab_exam_record>>();

            ars.Add(new List<tab_exam_record>());
            foreach (tab_exam_record e in rs)
            {
                List<tab_exam_record> temp = ars[ars.Count - 1];
                if (temp.Count == 0 && e.ElbowLeftY != null && e.ElbowRightY != null)
                {
                    temp.Add(e);
                    continue;
                }

                if (e.ElbowLeftY != null 
                    && e.ElbowRightY!=null
                    && Math.Abs((double)(temp[temp.Count - 1].ElbowLeftY - e.ElbowLeftY)) < 0.1
                    && Math.Abs((double)(temp[temp.Count - 1].ElbowRightY - e.ElbowRightY)) < 0.1)
                {
                    temp.Add(e);
                }
                else
                {
                    ars.Add(new List<tab_exam_record>());
                }
            }

            List<tab_exam_record> t =  new List<tab_exam_record>();
            foreach( List<tab_exam_record> er in ars){
                if (er.Count > t.Count)
                    t = er;
            }

            return t;
        }

        private List<tab_exam_record> getExamRecordOfType(int? examtype, int examid,jointexamEntities ctx)
        {
            StringBuilder b = new StringBuilder();
            b.Append("select * from tab_exam_record a where a.exam_type_id = '" + examtype + "' and a.exam_id = '" + examid + "'");
            DbSet<tab_exam_record> records = ctx.Set<tab_exam_record>();
            return records.SqlQuery(b.ToString()).ToList<tab_exam_record>();
        }


        /**
         * t44为基本参照角，t45为主分析角
         */
        private tab_exam_record analysis44(tab_analysis_type t44, tab_analysis_type t45, List<tab_exam_record> rs)
        {
            double degrees_interval= 10;
            double degrees_spin_y = Double.MaxValue;
            int group_num = 14;

            //将动态测试数据分组，分组条件：与上一动作变化幅度不超过degrees_interval，每组数量达到group_num.
            List<List<tab_exam_record>> recordGroup = new List<List<tab_exam_record>>();
           
            //平均数计算，取消使用
            //List<double> degreesScore = new List<double>();
            //double degreesSum = 0;
            
            List<tab_exam_record> temp = new List<tab_exam_record>();
            foreach (tab_exam_record r in rs)
            {
                if (temp.Count == 0)
                {
                    temp.Add(r);
                }
                
                if(temp.Count > 0)
                {
                    tab_analysis_result c1 = analysisDegrees(t45, r);
                    tab_analysis_result c2 = analysisDegrees(t44, r);

                    if (c1 == null || c2 == null)
                    {
                        temp = new List<tab_exam_record>();
                        continue;
                    }

                    if (c1.degrees - analysisDegrees(t45, temp[temp.Count - 1]).degrees < degrees_interval && c2.degrees < degrees_spin_y)   // 连续帧腿角度小于degrees_leg_spin并且上半身倾斜角小于degrees_spin_y
                    {
                        temp.Add(r);
                        //degreesSum += (double)c1.degrees;
                        if (temp.Count == group_num)
                        {
                            recordGroup.Add(temp);
                            //degreesAverage.Add(degreesSum / 20);
                            temp = new List<tab_exam_record>();
                        }
                    }else
                    {
                        temp = new List<tab_exam_record>();
                    }
                }
            }

            //查询最大平均值的分组并使用分组内第11个记录作为结果返回
            /*double temp_average = 0;
            int index = -1;
            for (int i = 0; i < degreesAverage.Count; i++)
            {
                if (temp_average < degreesAverage[i])
                {
                    temp_average = degreesAverage[i];
                    index = i;
                }
            }
            */


            int index = -1;
            double degreesScore = Double.MinValue;
            for(int i=0;i<recordGroup.Count; i++ )
            {
                double score = caculate44WeightFactory(t44,t45,recordGroup[i][group_num / 2]);

                if (degreesScore < score)
                {
                    degreesScore = score;
                    index = i;
                }
            }

            if (index > -1)
                return recordGroup[index][group_num / 2];
            else
               return null;
        }

        private double caculate44WeightFactory(tab_analysis_type t44, tab_analysis_type t45,tab_exam_record r)
        {
            tab_analysis_result c1 = analysisDegrees(t45, r);
            tab_analysis_result c2 = analysisDegrees(t44, r);

            return (double)( c2.degrees - c1.degrees * c1.degrees / 3.3);
        }
    }
}
