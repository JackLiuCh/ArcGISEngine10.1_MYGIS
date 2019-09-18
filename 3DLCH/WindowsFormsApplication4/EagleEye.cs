using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication4
{
    public partial class EagleEye : Form
    {
        private AxMapControl mMainMapControl;
        public EagleEye()
        {
            InitializeComponent();
        }
        public EagleEye(AxMapControl MainMapControl)
        {
            InitializeComponent();
            mMainMapControl = MainMapControl;
        }
        public AxMapControl MapControl
        {
            get
            {
                return axMapControl1;
            }

        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl1.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框 
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.mMainMapControl.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.mMainMapControl.Extent = pEnvelope;
                    this.mMainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框 
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl1.TrackRectangle();
                    this.mMainMapControl.Extent = pEnvelop;
                    this.mMainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 如果不是左键按下就直接返回 
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);

            this.mMainMapControl.CenterAt(pPoint);
            this.mMainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        public ILayer GetOverviewLayer(IMap map)
        {
            //获取主视图的第一个图层
            ILayer pLayer = map.get_Layer(0);
            //遍历其他图层，并比较视图范围的宽度，返回宽度最大的图层
            ILayer pTempLayer = null;
            for (int i = 1; i < map.LayerCount; i++)
            {
                pTempLayer = map.get_Layer(i);
                if (pLayer.AreaOfInterest.Width < pTempLayer.AreaOfInterest.Width)
                    pLayer = pTempLayer;
            }
            return pLayer;
        }
    }
}
