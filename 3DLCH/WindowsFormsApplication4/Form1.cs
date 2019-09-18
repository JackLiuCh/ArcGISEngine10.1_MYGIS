using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using _3DToolSet;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Display;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        private ResultForm mResultForm = new ResultForm();
        private IHit3DSet mHit3DSet;
        private Boolean mIsSceneChanging = false;


        //标记查询类型，1—点查询，2—线查询，3—矩形查询，4—圆查询
        private int mMouseFlag = 0;

        private Edit mEdit;

        private Boolean mNetworkAnalysisOn = false;
        //网络分析成员变量
        //几何网络
        private IGeometricNetwork mGeometricNetwork;
        //给定点的集合
        private IPointCollection mPointCollection;
        //获取给定点最近的Network元素的工具
        private IPointToEID mPointToEID;
        //返回结果变量
        private IEnumNetEID mEnumNetEID_Junctions;
        private IEnumNetEID mEnumNetEID_Edges;
        private double mdblPathCost;

        private EagleEye mEagleEye;


        public Form1()
        {
            InitializeComponent();
            axTOCControl1.SetBuddyControl(axSceneControl1);
            axTOCControl2.SetBuddyControl(axMapControl1);


            //编辑方式combox初始化
            cboTasks.Items.Add("新建");
            cboTasks.Items.Add("移动");
            cboTasks.SelectedIndex = 0;

            //开始编辑之前，将编辑按钮设为不可用
            this.cboTasks.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnStopEditing.Enabled = false;

            
        }

        private void btn_ToolZoomIn_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsSceneZoomInTool();
            pCommand.OnCreate(axSceneControl1.Object);
            axSceneControl1.CurrentTool = pCommand as ITool;
            ICommand pCommand2 = new ControlsMapZoomInToolClass();
            pCommand2.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand2 as ITool;
        }

        private void btn_ToolZoomOut_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsSceneZoomOutTool();
            pCommand.OnCreate(axSceneControl1.Object);
            axSceneControl1.CurrentTool = pCommand as ITool;
            ICommand pCommand2 = new ControlsMapZoomOutToolClass();
            pCommand2.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand2 as ITool;
        }

        private void axSceneControl1_OnMouseDown(object sender, ISceneControlEvents_OnMouseDownEvent e)
        {
            if (mPointSearch.Checked)
            {
                //查询
                axSceneControl1.SceneGraph.LocateMultiple(axSceneControl1.SceneGraph.ActiveViewer, e.x, e.y, esriScenePickMode.esriScenePickAll, false,out mHit3DSet);
                if (mHit3DSet == null) //未选中对象
                {
                    MessageBox.Show("没有选中对象");

                }
                else
                {
                    //显示在ResultForm控件中mHit3DSet为查询结果集合
                    mResultForm.ShowDialog();
                    mResultForm.refreshView(mHit3DSet);
           
                }
            }
        }
        /// <summary>
        /// 往map控件中加入图层
        /// </summary>
        /// <param name="pLayer"></param>
        private void addLayerToMap(ILayer pLayer)
        {
            axMapControl1.AddLayer(pLayer);
        }

        private void btn_RefreshLayer_Click(object sender, EventArgs e)
        {
            mLayerCombox.Items.Clear();
            //得到当前场景中的所有图层
            int nCount = axSceneControl1.Scene.LayerCount;
            if(nCount <= 0){//没有图层
                MessageBox.Show("场景中没有图层，请加入图层");
                return;
            }
            int i;
            ILayer pLayer = null;
            //将所有的图层的名称显示到复选框中
            for (i = 0; i < nCount; i++)
            {
                pLayer = axSceneControl1.Scene.get_Layer(i);
                mLayerCombox.Items.Add(pLayer.Name);
            }
            //将复选框设置为选中第一项
            mLayerCombox.SelectedIndex = 0;
            //addField
            addFieldNameToCombox(mLayerCombox.Text);
            mTINTypeCombox.SelectedIndex = 0;

            #region 将scene中的图层同步到Map控件中

            List<ILayer> pPointLayers = new List<ILayer>(); 
            List<ILayer> pLineLayers = new List<ILayer>(); 
            List<ILayer> pPolygonLayers = new List<ILayer>(); 
            List<ILayer> pRasterLayers = new List<ILayer>();
            IEnumLayer pEnumLayer = axSceneControl1.Scene.Layers;
            pEnumLayer.Reset();
            for(i = 0; i < axSceneControl1.Scene.LayerCount; i++)
            {
                ILayer pTempLayer = pEnumLayer.Next();
                if (pTempLayer != null)
                {
                    if(pTempLayer is IFeatureLayer)
                    {
                        if((pTempLayer as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                        {
                            pPointLayers.Add(pTempLayer);
                        }
                        else if((pTempLayer as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                        {
                            pLineLayers.Add(pTempLayer);
                        }
                        else if ((pTempLayer as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                        {
                            pPolygonLayers.Add(pTempLayer);
                        }
                    }
                    else
                    {
                        pRasterLayers.Add(pTempLayer);
                    }
                }
            }
            axMapControl1.ClearLayers();
            pRasterLayers.ForEach(new Action<ILayer>(addLayerToMap));
            pPolygonLayers.ForEach(new Action<ILayer>(addLayerToMap));
            pLineLayers.ForEach(new Action<ILayer>(addLayerToMap));
            pPointLayers.ForEach(new Action<ILayer>(addLayerToMap));
            #endregion
        }

        //将图层对应的字段加入到Fieldcombox中
        private void addFieldNameToCombox(string layerName)
        {
            mFieldCombox.Items.Clear();
            int i;
            IFeatureLayer pFeatureLayer = null;
            IFields pFields;
            int nCount = axSceneControl1.Scene.LayerCount;
            ILayer pLayer;
            //寻找名称为layerName的FeatureLayer
            for(i =0;i < nCount;i++)
            {
                pLayer = axSceneControl1.Scene.get_Layer(i) as IFeatureLayer;
                if (pLayer != null && pLayer.Name == layerName)//找到了layerName对应的FeatureLayer
                {
                    pFeatureLayer = pLayer as FeatureLayer;
                    break;
                }
            }
            if (pFeatureLayer != null)//判断是否找到
            {
                pFields = pFeatureLayer.FeatureClass.Fields;
                nCount = pFields.FieldCount;
                //将该图层中所用的字段写入到mFieldCombox中
                for (i = 0; i < nCount; i++)
                {
                    mFieldCombox.Items.Add(pFields.Field[i].Name);
                }
            }
            //mFieldCombox.SelectedIndex = 0;
        }

        private void mLayerCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addFieldNameToCombox(mLayerCombox.Text);
        }

        private void btn_ConstructTin_Click(object sender, EventArgs e)
        {
            if (mLayerCombox.Text == "" || mFieldCombox.Text == "")//判断输入合法性
            {
                MessageBox.Show("没有相应的图层");
                return;
            }
            ITinEdit pTin = new TinClass();
            //寻找FeatureLayer
            IFeatureLayer pFeatureLayer = axSceneControl1.Scene.get_Layer(mLayerCombox.SelectedIndex) as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                IEnvelope pEnvelope = new Envelope() as IEnvelope;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IQueryFilter pQueryFilter = new QueryFilter();
                IField pField = null;
                //找字段
                pField = pFeatureClass.Fields.get_Field(pFeatureClass.Fields.FindField(mFieldCombox.Text));
                if (pField.Type == esriFieldType.esriFieldTypeInteger || pField.Type == esriFieldType.esriFieldTypeDouble || pField.Type == esriFieldType.esriFieldTypeSingle)//判断类型
                {
                    IGeoDataset pGeoDataset = pFeatureLayer as IGeoDataset;
                    pEnvelope = pGeoDataset.Extent;
                    //设置空间参考系
                    ISpatialReference pSpatialReference;
                    pSpatialReference = pGeoDataset.SpatialReference;
                    //选择生成TIN的输入类型
                    esriTinSurfaceType pSurfaceType = esriTinSurfaceType.esriTinMassPoint;
                    switch (mTINTypeCombox.Text)
                    {
                        case "点":
                            pSurfaceType = esriTinSurfaceType.esriTinMassPoint;
                            break;
                        case "直线":
                            pSurfaceType = esriTinSurfaceType.esriTinSoftLine;
                            break;
                        case "光滑线":
                            pSurfaceType = esriTinSurfaceType.esriTinHardLine;
                            break;
                    }
                    //创建TIN
                    pTin.InitNew(pEnvelope);
                    object missing = Type.Missing;
                    //生成TIN
                    pTin.AddFromFeatureClass(pFeatureClass, pQueryFilter, pField, pField, pSurfaceType, ref missing);
                    pTin.SetSpatialReference(pSpatialReference);
                    //创建Tin图层并将Tin图层加入到场景中去
                    ITinLayer pTinLayer = new TinLayer();
                    pTinLayer.Dataset = pTin as ITin;
                    axSceneControl1.Scene.AddLayer(pTinLayer, true);
                }
                else 
                {
                    MessageBox.Show("该字段的类型不符合构建TIN的条件");
                }
            }

        }
        

        private void btn_CreateContour_Click(object sender, EventArgs e)
        {
            ITinLayer pTinLayer = null;
            IEnumLayer pEnumLayers = axSceneControl1.Scene.Layers;
            pEnumLayers.Reset();
            for(int i = 0; i < axSceneControl1.Scene.LayerCount; i++)
            {
                ILayer pLayer = pEnumLayers.Next();
                if (pLayer is ITinLayer) pTinLayer = pLayer as ITinLayer;
            }
            if (pTinLayer != null)
            {
                //IFeatureLayer pLayer = axSceneControl1.Scene.get_Layer(0) as IFeatureLayer;
                //IFeatureClass pFeatureClass = pLayer.FeatureClass as IFeatureClass;
                //IFeatureDataset pFeatureDataset = pFeatureClass.FeatureDataset;
                //IWorkspace pWorkspace = pFeatureDataset.Workspace;
                //IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;

                IField pField1 = new Field();
                IFieldEdit pField1Edit = pField1 as IFieldEdit;
                pField1Edit.Name_2 = "OID";
                pField1Edit.Type_2 = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeOID;
                pField1Edit.IsNullable_2 = false;
   

                IField pField2 = new Field();
                IFieldEdit pField2Edit = pField2 as IFieldEdit;
                pField2Edit.Name_2 = "geo";
                pField2Edit.Type_2 = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry;
                IGeometryDef pGeoDef = new GeometryDef();
                IGeometryDefEdit pGeoDefEdit = pGeoDef as IGeometryDefEdit;
                pGeoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                pGeoDefEdit.SpatialReference_2 = axSceneControl1.Scene.SpatialReference;
                pField2Edit.GeometryDef_2 = pGeoDef;

                IFields pFields = new Fields();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                pFieldsEdit.AddField(pField1);
                pFieldsEdit.AddField(pField2);



                IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
                IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile("E://slope", 0);
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;

                IFeatureClass pNewFeatureClass = pFeatureWorkspace.CreateFeatureClass("contour_"+ new Random().GetHashCode(), pFields, null, null, esriFeatureType.esriFTSimple, "geo", "");


                ISurface pSurface = pTinLayer.Dataset as ISurface;

                pSurface.Contour(100, 10, pNewFeatureClass, "hgt", 3);

                IFeatureLayer pFeatureLayer = new FeatureLayer();
                pFeatureLayer.FeatureClass = pNewFeatureClass;
                axSceneControl1.Scene.AddLayer(pFeatureLayer);
            }
            else
            {
                MessageBox.Show("当前并没有可用的TINLayer用于生成等高线");
            }
        }
      
        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            #region //二三维互动
            if (mIsSceneChanging == false)
            {
                IActiveView pActiveView1 = this.axMapControl1.Map as IActiveView;   //获取当前二维活动区域               
                IEnvelope enve = pActiveView1.Extent as IEnvelope;      //将此二维区域的Extent 保存在Envelope中

                IPoint point = new PointClass();        //将此区域的中心点保存起来
                point.X = (enve.XMax + enve.XMin) / 2;  //取得视角中心点X坐标
                point.Y = (enve.YMax + enve.YMin) / 2;  //取得视角中心点Y坐标

                IPoint ptTaget = new PointClass();      //创建一个目标点
                ptTaget = point;        //视觉区域中心点作为目标点
                ptTaget.Z = 0;         //设置目标点高度，这里设为 0米

                IPoint ptObserver = new PointClass();   //创建观察点 的X，Y，Z
                ptObserver.X = point.X;     //设置观察点坐标的X坐标
                ptObserver.Y = point.Y + 90;     //设置观察点坐标的Y坐标（这里加90米，是在南北方向上加了90米，当然这个数字可以自己定，意思就是将观察点和目标点有一定的偏差，从南向北观察
                double height = (enve.Width < enve.Height) ? enve.Width : enve.Height;      //计算观察点合适的高度，这里用三目运算符实现的，效果稍微好一些，当然可以自己拟定
                ptObserver.Z = height;              //设置观察点坐标的Y坐标

                ICamera pCamera = this.axSceneControl1.Camera;      //取得三维活动区域的Camara      ，就像你照相一样的视角，它有Taget（目标点）和Observer（观察点）两个属性需要设置    
                pCamera.Target = ptTaget;       //赋予目标点
                pCamera.Observer = ptObserver;      //将上面设置的观察点赋予camera的观察点
                pCamera.Inclination = 30;       //设置三维场景视角，也就是高度角，视线与地面所成的角度
                pCamera.Azimuth = 180;          //设置三维场景方位角，视线与向北的方向所成的角度
                axSceneControl1.SceneGraph.RefreshViewers();        //刷新地图
            }
            #endregion

            #region 部分鹰眼功能实现
            if (mEagleEye != null) {
                //创建鹰眼中线框
                IEnvelope pEnv = (IEnvelope)e.newEnvelope;
                IRectangleElement pRectangleEle = new RectangleElementClass();
                IElement pEle = pRectangleEle as IElement;
                pEle.Geometry = pEnv;

                //设置线框的边线对象，包括颜色和线宽
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 255;
                // 产生一个线符号对象 
                ILineSymbol pOutline = new SimpleLineSymbolClass();
                pOutline.Width = 2;
                pOutline.Color = pColor;

                // 设置颜色属性 
                pColor.Red = 255;
                pColor.Green = 0;
                pColor.Blue = 0;
                pColor.Transparency = 0;

                // 设置线框填充符号的属性 
                IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                pFillSymbol.Color = pColor;
                pFillSymbol.Outline = pOutline;
                IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
                pFillShapeEle.Symbol = pFillSymbol;

                // 得到鹰眼视图中的图形元素容器
                IGraphicsContainer pGra = mEagleEye.MapControl.Map as IGraphicsContainer;
                IActiveView pAv = pGra as IActiveView;
                // 在绘制前，清除 axMapControl2 中的任何图形元素 
                pGra.DeleteAllElements();
                // 鹰眼视图中添加线框
                pGra.AddElement((IElement)pFillShapeEle, 0);
                // 刷新鹰眼
                pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            #endregion
        }

        private void axSceneControl1_OnMouseUp(object sender, ISceneControlEvents_OnMouseUpEvent e)
        {
            //三二维联动
            if (axSceneControl1.Scene.LayerCount > 0)
            {
                mIsSceneChanging = true;
                LinkageCommand commd = new LinkageCommand();
                commd.OnCreate(axSceneControl1.Object);
                commd.OnCreate(axMapControl1.Object);
                commd.OnClick();
                mIsSceneChanging = false;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            axSceneControl1.CurrentTool = null;
            axMapControl1.CurrentTool = null;
            mMouseFlag = 0;
            axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            axSceneControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            mNetworkAnalysisOn = false;
        }

        private void btn_SlopeAnalysis_Click(object sender, EventArgs e)
        {
            SlopeAnalysis pSA = new SlopeAnalysis();
            pSA.OnCreate(axSceneControl1.Object);
            pSA.OnClick(comBox_ChooseRaster.Text);
        }

        private void btn_AddRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog pFdlg = new OpenFileDialog();
            pFdlg.Title = "选择栅格路径";
            pFdlg.Filter = "栅格数据文件(*.tif)|*.tif|栅格数据文件(*.img)|*.img";
            if (pFdlg.ShowDialog() == DialogResult.OK)
            {
                string pPathName = pFdlg.FileName;
                RasterWorkspaceFactory pRasterworkspaceFactory = new RasterWorkspaceFactoryClass();
                IWorkspace pWorkspace = pRasterworkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(pPathName), 0);

                //IEnumDataset pEnumDataset = pWorkspace.Datasets[esriDatasetType.esriDTRasterBand];
                //pEnumDataset.Reset();  
                //IRasterDataset pRasterDataset = pEnumDataset.Next() as IRasterDataset;

                IRasterDataset pRasterDataset = (pWorkspace as IRasterWorkspace).OpenRasterDataset(System.IO.Path.GetFileName(pPathName));
                RasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(pRasterDataset);
                axSceneControl1.Scene.AddLayer(pRasterLayer);
                //axMapControl1.Extent = pRasterLayer.VisibleExtent;
                //axMapControl1.Refresh();

                //更新选择栅格图层的Combox
                comBox_ChooseRaster.Items.Add(System.IO.Path.GetFileName(pPathName));
            }

        }

        private void btn_removeAllLayers_Click(object sender, EventArgs e)
        {
            axSceneControl1.Scene.ClearLayers();
            axMapControl1.ClearLayers();
            axSceneControl1.Scene.SpatialReference = null;
            axMapControl1.SpatialReference = null;
        }

        private void btn_AspectAnalysis_Click(object sender, EventArgs e)
        {
            AspectAnalysis pAA = new AspectAnalysis();
            pAA.OnCreate(axSceneControl1.Object);
            pAA.OnClick(comBox_ChooseRaster.Text);
        }

        private void btn_VisibilityAnalysis_Click(object sender, EventArgs e)
        {
            //有问题仍不能正确执行
            ViewAnalyst pViewAnalysis = new ViewAnalyst();
            pViewAnalysis.OnCreate(axSceneControl1.Object);
            pViewAnalysis.OnClick(comBox_ChooseRaster.Text);
        }

        private void btn_LoadTin_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pFBdlg = new FolderBrowserDialog();
            if(pFBdlg.ShowDialog() == DialogResult.OK)
            {
                string pPath = pFBdlg.SelectedPath;
                IWorkspaceFactory pWorkspaceFactory = new TinWorkspaceFactoryClass();
                Boolean isTin = pWorkspaceFactory.IsWorkspace(System.IO.Path.GetDirectoryName(pPath));

                if(isTin == true)
                {
                    IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(pPath), 0);
                    ITinWorkspace pTinWorkspace = pWorkspace as ITinWorkspace;
                    ITinLayer pTinLayer = new TinLayerClass();
                    ITin pTin = pTinWorkspace.OpenTin(System.IO.Path.GetFileName(pPath));
                    pTinLayer.Dataset = pTin;
                    pTinLayer.Name = System.IO.Path.GetFileName(pPath);
                    axSceneControl1.Scene.AddLayer(pTinLayer as ILayer);
                }
                else
                {
                    MessageBox.Show("该路径并非TIN！");
                }

            }
        }

        private void btn_SaveTin_Click(object sender, EventArgs e)
        {
            IEnumLayer pEnumLayers = axSceneControl1.Scene.Layers;
            pEnumLayers.Reset();
            ILayer pLayer = pEnumLayers.Next();
            Boolean hasTin = false;
            while(pLayer != null)
            {
                if(pLayer is ITinLayer)
                {
                    SaveFileDialog pSaveTinDlg = new SaveFileDialog();
                    pSaveTinDlg.DefaultExt = "tin";
                    if(pSaveTinDlg.ShowDialog() == DialogResult.OK)
                    {
                        string pPath = pSaveTinDlg.FileName;
                        ITinEdit pTinEdit = (pLayer as ITinLayer).Dataset as ITinEdit;
                        object overwrite = true;
                        pTinEdit.SaveAs(pPath, ref overwrite);
                        hasTin = true;
                    }
                    else
                    {
                        return;
                    }
                }
                pLayer = pEnumLayers.Next();
            }
            if(hasTin == false)
            {
                MessageBox.Show("当前没有可供保存的TIN");
            }
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (checkBox_TinSearch.Checked)
            {
                IEnumLayer pEnumLayers = axSceneControl1.Scene.Layers;
                pEnumLayers.Reset();
                ILayer pLayer = pEnumLayers.Next();
                Boolean hasTin = false;
                while (pLayer != null)
                {
                    if (pLayer is ITinLayer)
                    {
                        hasTin = true;
                        ITin pTin = (pLayer as ITinLayer).Dataset;
                        ISurface pSurface = pTin as ISurface;
                        IPoint pPoint = new PointClass();
                        pPoint.X = e.mapX;
                        pPoint.Y = e.mapY;
                        double pElevation = pSurface.GetElevation(pPoint);
                        double pSlopeDegrees = pSurface.GetSlopeDegrees(pPoint);
                        double pAspect = pSurface.GetAspectDegrees(pPoint);
                        TINInfoForm pTinInfoform = new TINInfoForm();
                        pTinInfoform.TextBox_X = pPoint.X.ToString();
                        pTinInfoform.TextBox_Y = pPoint.Y.ToString();
                        pTinInfoform.TextBox_Elevation = pElevation.ToString();
                        pTinInfoform.TextBox_Slope = pSlopeDegrees.ToString();
                        pTinInfoform.TextBox_Aspect = pAspect.ToString();
                        pTinInfoform.Text = pLayer.Name;
                        pTinInfoform.Show();
                    }
                    pLayer = pEnumLayers.Next();
                }
                if (hasTin == false)
                {
                    MessageBox.Show("当前没有可供查询的TIN");
                }
            }

            if(mMouseFlag != 0)
            {
                //点查询
                if (mMouseFlag == 1)
                {
                    IActiveView pActiveView;
                    IPoint pPoint;
                    double length;
                    //获取视图范围
                    pActiveView = this.axMapControl1.ActiveView;
                    //获取鼠标点击屏幕坐标
                    pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                    //屏幕距离转换为地图距离
                    length = ConvertPixelToMapUnits(pActiveView, 2);

                    ITopologicalOperator pTopoOperator;
                    IGeometry pGeoBuffer;
                    //根据缓冲半径生成空间过滤器
                    pTopoOperator = pPoint as ITopologicalOperator;
                    pGeoBuffer = pTopoOperator.Buffer(length);
                    QuerySpatial(this.axMapControl1, pGeoBuffer);
                }
                else if (mMouseFlag == 2)//线查询
                {
                    QuerySpatial(this.axMapControl1, this.axMapControl1.TrackLine());
                }
                else if (mMouseFlag == 3)//矩形查询
                {
                    QuerySpatial(this.axMapControl1, this.axMapControl1.TrackRectangle());
                }
                else if (mMouseFlag == 4)//圆查询
                {
                    QuerySpatial(this.axMapControl1, this.axMapControl1.TrackCircle());
                }
            }

            if (mNetworkAnalysisOn)
            {
                //记录鼠标点击的点
                IPoint pNewPoint = new PointClass();
                pNewPoint.PutCoords(e.mapX, e.mapY);

                if (mPointCollection == null)
                    mPointCollection = new MultipointClass();
                //添加点，before和after标记添加点的索引，这里不定义
                object before = Type.Missing;
                object after = Type.Missing;
                mPointCollection.AddPoint(pNewPoint, ref before, ref after);
            }

            //判断是否处于编辑状态
            if (mEdit != null && mEdit.IsEditing())
            {
                switch (cboTasks.SelectedIndex)
                {
                    case 0:
                        mEdit.CreateMouseDown(e.mapX, e.mapY);
                        break;
                    case 1:
                        mEdit.PanMouseDown(e.mapX, e.mapY);
                        break;
                }
            }
        }

        private void checkBox_TinSearch_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox_TinSearch.Checked)
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerIdentify;
            }
            else
            {
                axMapControl1.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开MXD";
            openFileDialog.Filter = "MXD文件(*.mxd) | *.mxd";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                axMapControl1.LoadMxFile(path);
            }
        }

        private void addLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开lyr";
            openFileDialog.Filter = "ESRI lyr文件(*.lyr) | *.lyr";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                axMapControl1.AddLayerFromFile(path);
            }
        }

        private void addShapefileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开shapefile";
            openFileDialog.Filter = "shapefile文件(*.shp) | *.shp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                axMapControl1.AddShapeFile(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path));
            }
        }

        private void addPersonalGeodatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog pgdbdlg = new OpenFileDialog();
            pgdbdlg.Title = "Load Data from Personal GDB";
            pgdbdlg.Filter = "ESRI Personal GDB(*.mdb)| *.mdb";
            if (pgdbdlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = pgdbdlg.FileName;
            //个人数据库是微软的Access
            IWorkspaceFactory pPGDBwkf = new AccessWorkspaceFactory();
            IWorkspace pwksp = pPGDBwkf.OpenFromFile(fileName, 0);
            if (pwksp == null)
            {
                return;
            }
            IEnumDataset pEnumDataset = pwksp.get_Datasets(esriDatasetType.esriDTAny);
            pEnumDataset.Reset();
            //get the first data
            IDataset pDataset = pEnumDataset.Next();
            while (pDataset != null)
            {
                if (pDataset is IFeatureDataset)
                {
                    //FeatureClass
                    IFeatureDataset pFdt = (IFeatureDataset)pDataset;
                    IEnumDataset pFEnumDt = pFdt.Subsets;
                    pFEnumDt.Reset();
                    IDataset psubFDt = pFEnumDt.Next();
                    while (psubFDt != null)
                    {
                        //is or not featureclass
                        if (psubFDt is IFeatureClass)
                        {
                            IFeatureClass pFC = (IFeatureClass)psubFDt;
                            IFeatureLayer pFLyer = new FeatureLayer();
                            pFLyer.FeatureClass = pFC;
                            pFLyer.Name = pFC.AliasName;
                            //add this fc to map
                            axMapControl1.AddLayer(pFLyer);
                        }
                        psubFDt = pFEnumDt.Next();
                    }
                }
                else if (pDataset is IFeatureClass)
                {
                    IFeatureClass pFC = (IFeatureClass)pDataset;
                    IFeatureLayer pFLyer = new FeatureLayer();
                    pFLyer.FeatureClass = pFC;
                    pFLyer.Name = pFC.AliasName;
                    //add this fc to map
                    axMapControl1.AddLayer(pFLyer);
                }
                pDataset = pEnumDataset.Next();
            }
        }

        private void addFileGeodatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdlg = new FolderBrowserDialog();
            if (fbdlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = fbdlg.SelectedPath;
            IWorkspaceFactory pGDBwkf = new FileGDBWorkspaceFactory();
            try
            {
                IWorkspace pwksp = pGDBwkf.OpenFromFile(fileName, 0);

                if (pwksp == null)
                {
                    return;
                }
                IEnumDataset pEnumDataset = pwksp.get_Datasets(esriDatasetType.esriDTAny);
                pEnumDataset.Reset();
                //get the first data
                IDataset pDataset = pEnumDataset.Next();
                while (pDataset != null)
                {
                    if (pDataset is IFeatureDataset)
                    {
                        //FeatureClass
                        IFeatureDataset pFdt = (IFeatureDataset)pDataset;
                        IEnumDataset pFEnumDt = pFdt.Subsets;
                        pFEnumDt.Reset();
                        IDataset psubFDt = pFEnumDt.Next();
                        while (psubFDt != null)
                        {
                            //is or not featureclass
                            if (psubFDt is IFeatureClass)
                            {
                                IFeatureClass pFC = (IFeatureClass)psubFDt;
                                IFeatureLayer pFLyer = new FeatureLayer();
                                pFLyer.FeatureClass = pFC;
                                pFLyer.Name = pFC.AliasName;
                                //add this fc to map
                                axMapControl1.AddLayer(pFLyer);
                            }
                            psubFDt = pFEnumDt.Next();
                        }
                    }
                    else if (pDataset is IFeatureClass)
                    {
                        IFeatureClass pFC = (IFeatureClass)pDataset;
                        IFeatureLayer pFLyer = new FeatureLayer();
                        pFLyer.FeatureClass = pFC;
                        pFLyer.Name = pFC.AliasName;
                        //add this fc to map
                        axMapControl1.AddLayer(pFLyer);
                    }
                    pDataset = pEnumDataset.Next();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsSceneZoomInTool();
            pCommand.OnCreate(axSceneControl1.Object);
            axSceneControl1.CurrentTool = pCommand as ITool;
            ICommand pCommand2 = new ControlsMapZoomInToolClass();
            pCommand2.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand2 as ITool;
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsSceneZoomOutTool();
            pCommand.OnCreate(axSceneControl1.Object);
            axSceneControl1.CurrentTool = pCommand as ITool;
            ICommand pCommand2 = new ControlsMapZoomOutToolClass();
            pCommand2.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand2 as ITool;
        }

        private void panToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsScenePanTool();
            pCommand.OnCreate(axSceneControl1.Object);
            axSceneControl1.CurrentTool = pCommand as ITool;
            ICommand pCommand2 = new ControlsMapPanToolClass();
            pCommand2.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pCommand2 as ITool;
        }

        private void fullExtentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ICommand pCommand = new ControlsSceneFullExtentCommand();
            pCommand.OnCreate(axSceneControl1.Object);
            pCommand.OnClick();
            ICommand pCommand2 = new ControlsMapFullExtentCommandClass();
            pCommand2.OnCreate(axMapControl1.Object);
            pCommand2.OnClick();
        }

        private void attributeSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttributeQueryForm pAQForm = new AttributeQueryForm(axMapControl1);
            pAQForm.Show();
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMouseFlag = 1;
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
        }

        private void lineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mMouseFlag = 2;
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMouseFlag = 4;
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
        }

        private void polygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMouseFlag = 3;
            this.axMapControl1.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
        }

        /// <summary>
        /// 空间查询
        /// </summary>
        /// <param name="mapControl">MapControl</param>
        /// <param name="geometry">空间查询方式</param>
        /// <returns>void</returns>
        private void QuerySpatial(AxMapControl mapControl, IGeometry geometry)
        {
            //本例添加一个图层进行查询，多个图层时返回
            if (mapControl.LayerCount < 1)
                return ;

            //清除已有选择
            mapControl.Map.ClearSelection();
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass;
            //获取图层和要素类，为空时返回
            pFeatureLayer = mapControl.Map.get_Layer(0) as IFeatureLayer;
            pFeatureClass = pFeatureLayer.FeatureClass;
            if (pFeatureClass == null)
                return ;

            //初始化空间过滤器
            ISpatialFilter pSpatialFilter;
            pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = geometry;
            //根据图层类型选择缓冲方式
            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }
            //定义空间过滤器的空间字段
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;

            IQueryFilter pQueryFilter;
            IFeatureCursor pFeatureCursor;
            IFeature pFeature;
            //利用要素过滤器查询要素
            pQueryFilter = pSpatialFilter as IQueryFilter;
            pFeatureCursor = pFeatureLayer.Search(pQueryFilter, false);
            pFeature = pFeatureCursor.NextFeature();

            List<IFeature> pFeatureList = new List<IFeature>();

            int featureNum = 0;
            while (pFeature != null)
            {
                pFeatureList.Add(pFeature);
                featureNum++;
                //高亮选中要素
                mapControl.Map.SelectFeature((ILayer)pFeatureLayer, pFeature);
                pFeature = pFeatureCursor.NextFeature();
            }
            mapControl.Refresh();
            if (featureNum > 0)
            {
                ResultForm pResultForm = new ResultForm();
                pResultForm.refreshFeatureListView(pFeatureList);
                pResultForm.Show();
            }
            return;
        }

        /// <summary>
        /// 根据屏幕像素计算实际的地理距离
        /// </summary>
        /// <param name="activeView">屏幕视图</param>
        /// <param name="pixelUnits">像素个数</param>
        /// <returns></returns>
        private double ConvertPixelToMapUnits(IActiveView activeView, double pixelUnits)
        {
            double realWorldDiaplayExtent;
            int pixelExtent;
            double sizeOfOnePixel;
            double mapUnits;

            //获取设备中视图显示宽度，即像素个数
            pixelExtent = activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right -
                activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;
            //获取地图坐标系中地图显示范围
            realWorldDiaplayExtent = activeView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            //每个像素大小代表的实际距离
            sizeOfOnePixel = realWorldDiaplayExtent / pixelExtent;
            //地理距离
            mapUnits = pixelUnits * sizeOfOnePixel;

            return mapUnits;
        }

        private void bufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BufferForm bufferForm = new BufferForm(this.axMapControl1.Object);
            if (bufferForm.ShowDialog() == DialogResult.OK)
            {
                //获取输出文件路径
                string strBufferPath = bufferForm.strOutputPath;
                //缓冲区图层载入到MapControl
                int index = strBufferPath.LastIndexOf("\\");
                this.axMapControl1.AddShapeFile(strBufferPath.Substring(0, index), strBufferPath.Substring(index));
            }
        }

        private void overLayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OverlayForm overlayForm = new OverlayForm();
            if (overlayForm.ShowDialog() == DialogResult.OK)
            {
                string strOverlayPath = overlayForm.strOutputPath;
                int index = strOverlayPath.LastIndexOf("\\");
                this.axMapControl1.AddShapeFile(strOverlayPath.Substring(0, index), strOverlayPath.Substring(index));
            }
        }

        private void networkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //将网络分析状态设置为true
            mNetworkAnalysisOn = true;
            //获取几何网络文件路径
            //注意修改此路径为当前存储路径
            string strPath = @"C:\Users\11\Desktop\地理信息工程综合实习\2019地理信息工程综合实习文档\例子数据\Network\USA_Highway_Network_GDB.mdb";
            //打开工作空间
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactory();
            IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(strPath, 0) as IFeatureWorkspace;
            //获取要素数据集
            //注意名称的设置要与上面创建保持一致
            IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset("high");

            //获取network集合
            INetworkCollection pNetWorkCollection = pFeatureDataset as INetworkCollection;
            //获取network的数量,为零时返回
            int intNetworkCount = pNetWorkCollection.GeometricNetworkCount;
            if (intNetworkCount < 1)
                return;
            //FeatureDataset可能包含多个network，我们获取指定的network
            //注意network的名称的设置要与上面创建保持一致
            mGeometricNetwork = pNetWorkCollection.get_GeometricNetworkByName("high_net");

            //将Network中的每个要素类作为一个图层加入地图控件
            IFeatureClassContainer pFeatClsContainer = mGeometricNetwork as IFeatureClassContainer;
            //获取要素类数量，为零时返回
            int intFeatClsCount = pFeatClsContainer.ClassCount;
            if (intFeatClsCount < 1)
                return;
            IFeatureClass pFeatureClass;
            IFeatureLayer pFeatureLayer;
            for (int i = 0; i < intFeatClsCount; i++)
            {
                //获取要素类
                pFeatureClass = pFeatClsContainer.get_Class(i);
                pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pFeatureClass.AliasName;
                //加入地图控件
                this.axMapControl1.AddLayer((ILayer)pFeatureLayer, 0);
            }

            //计算snap tolerance为图层最大宽度的1/100
            //获取图层数量
            int intLayerCount = this.axMapControl1.LayerCount;
            IGeoDataset pGeoDataset;
            IEnvelope pMaxEnvelope = new EnvelopeClass();
            for (int i = 0; i < intLayerCount; i++)
            {
                //获取图层
                pFeatureLayer = this.axMapControl1.get_Layer(i) as IFeatureLayer;
                pGeoDataset = pFeatureLayer as IGeoDataset;
                //通过Union获得较大图层范围
                pMaxEnvelope.Union(pGeoDataset.Extent);
            }
            double dblWidth = pMaxEnvelope.Width;
            double dblHeight = pMaxEnvelope.Height;
            double dblSnapTol;
            if (dblHeight < dblWidth)
                dblSnapTol = dblWidth * 0.01;
            else
                dblSnapTol = dblHeight * 0.01;

            //设置源地图，几何网络以及捕捉容差
            mPointToEID = new PointToEIDClass();
            mPointToEID.SourceMap = this.axMapControl1.Map;
            mPointToEID.GeometricNetwork = mGeometricNetwork;
            mPointToEID.SnapTolerance = dblSnapTol;
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            if (mNetworkAnalysisOn)
            {
                try
                {
                    //路径计算
                    //注意权重名称与设置保持一致
                    SolvePath("LENGTH");
                    //路径转换为几何要素
                    IPolyline pPolyLineResult = PathToPolyLine();
                    //获取屏幕显示
                    IActiveView pActiveView = this.axMapControl1.ActiveView;
                    IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
                    //设置显示符号
                    ILineSymbol pLineSymbol = new CartographicLineSymbolClass();
                    IRgbColor pColor = new RgbColorClass();
                    pColor.Red = 255;
                    pColor.Green = 0;
                    pColor.Blue = 0;
                    //设置线宽
                    pLineSymbol.Width = 4;
                    //设置颜色
                    pLineSymbol.Color = pColor as IColor;
                    //绘制线型符号
                    pScreenDisplay.StartDrawing(0, 0);
                    pScreenDisplay.SetSymbol((ISymbol)pLineSymbol);
                    pScreenDisplay.DrawPolyline(pPolyLineResult);
                    pScreenDisplay.FinishDrawing();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("路径分析出现错误:" + "\r\n" + ex.Message);
                }
                //点集设为空
                mPointCollection = null;
                mEnumNetEID_Edges = null;
            }
            //判断是否处于编辑状态
            if (mEdit != null && mEdit.IsEditing())
            {
                switch (cboTasks.SelectedIndex)
                {
                    case 0:
                        mEdit.CreateDoubleClick(e.mapX, e.mapY);
                        break;
                    case 1:
                        break;
                }
            }
        }
        /// <summary>
        /// 路径计算
        /// </summary>
        /// <param name="weightName">权重名称</param>
        [HandleProcessCorruptedStateExceptions]
        private void SolvePath(string weightName)
        {
            //创建ITraceFlowSolverGEN
            ITraceFlowSolverGEN pTraceFlowSolverGEN = new TraceFlowSolverClass();
            INetSolver pNetSolver = pTraceFlowSolverGEN as INetSolver;
            //初始化用于路径计算的Network
            INetwork pNetWork = mGeometricNetwork.Network;
            pNetSolver.SourceNetwork = pNetWork;

            //获取分析经过的点的个数
            int intCount = mPointCollection.PointCount;
            if (intCount <= 1)
                return;


            INetFlag pNetFlag;
            //用于存储路径计算得到的边
            IEdgeFlag[] pEdgeFlags = new IEdgeFlag[intCount];


            IPoint pEdgePoint = new PointClass();
            int intEdgeEID;
            IPoint pFoundEdgePoint;
            double dblEdgePercent;

            //用于获取几何网络元素的UserID, UserClassID,UserSubID
            INetElements pNetElements = pNetWork as INetElements;
            int intEdgeUserClassID;
            int intEdgeUserID;
            int intEdgeUserSubID;
            for (int i = 0; i < intCount; i++)
            {
                pNetFlag = new EdgeFlagClass();
                //获取用户点击点
                pEdgePoint = mPointCollection.get_Point(i);
                //获取距离用户点击点最近的边
                mPointToEID.GetNearestEdge(pEdgePoint, out intEdgeEID, out pFoundEdgePoint, out dblEdgePercent);
                if (intEdgeEID <= 0)
                    continue;
                //根据得到的边查询对应的几何网络中的元素UserID, UserClassID,UserSubID
                pNetElements.QueryIDs(intEdgeEID, esriElementType.esriETEdge,
                    out intEdgeUserClassID, out intEdgeUserID, out intEdgeUserSubID);
                if (intEdgeUserClassID <= 0 || intEdgeUserID <= 0)
                    continue;

                pNetFlag.UserClassID = intEdgeUserClassID;
                pNetFlag.UserID = intEdgeUserID;
                pNetFlag.UserSubID = intEdgeUserSubID;
                pEdgeFlags[i] = pNetFlag as IEdgeFlag;
            }
            //设置路径求解的边
            try
            {
                pTraceFlowSolverGEN.PutEdgeOrigins(ref pEdgeFlags);//路径计算权重
                INetSchema pNetSchema = pNetWork as INetSchema;
                INetWeight pNetWeight = pNetSchema.get_WeightByName(weightName);
                if (pNetWeight == null)
                    return;

                //设置权重，这里双向的权重设为一致
                INetSolverWeights pNetSolverWeights = pTraceFlowSolverGEN as INetSolverWeights;
                pNetSolverWeights.ToFromEdgeWeight = pNetWeight;
                pNetSolverWeights.FromToEdgeWeight = pNetWeight;

                object[] arrResults = new object[intCount - 1];
                //执行路径计算
                pTraceFlowSolverGEN.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum,
                    out mEnumNetEID_Junctions, out mEnumNetEID_Edges, intCount - 1, ref arrResults);

                //获取路径计算总代价（cost）
                mdblPathCost = 0;
                for (int i = 0; i < intCount - 1; i++)
                    mdblPathCost += (double)arrResults[i];
            }
            catch (AccessViolationException ex)
            {
                throw new Exception("点击的点太远，找不到离它最近的Edge！");
            }
        }

        /// <summary>
        /// 路径转换为几何要素
        /// </summary>
        /// <returns></returns>
        private IPolyline PathToPolyLine()
        {
            IPolyline pPolyLine = new PolylineClass();
            IGeometryCollection pNewGeometryCollection = pPolyLine as IGeometryCollection;
            if (mEnumNetEID_Edges == null)
                return null;

            IEIDHelper pEIDHelper = new EIDHelperClass();
            //获取几何网络
            pEIDHelper.GeometricNetwork = mGeometricNetwork;
            //获取地图空间参考
            ISpatialReference pSpatialReference = this.axMapControl1.Map.SpatialReference;
            pEIDHelper.OutputSpatialReference = pSpatialReference;
            pEIDHelper.ReturnGeometries = true;
            //根据边的ID获取边的信息
            IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(mEnumNetEID_Edges);
            int intCount = pEnumEIDInfo.Count;
            pEnumEIDInfo.Reset();

            IEIDInfo pEIDInfo;
            IGeometry pGeometry;
            for (int i = 0; i < intCount; i++)
            {
                pEIDInfo = pEnumEIDInfo.Next();
                //获取边的几何要素
                pGeometry = pEIDInfo.Geometry;
                pNewGeometryCollection.AddGeometryCollection((IGeometryCollection)pGeometry);
            }
            return pPolyLine;
        }

        private void btnRefreshLayers_Click(object sender, EventArgs e)
        {
            //清空原有选项
            cboLayers.Items.Clear();
            //没有添加图层时返回
            if (this.axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("MapControl中未添加图层！", "提示");
                return;
            }
            //加载图层
            for (int i = 0; i < this.axMapControl1.Map.LayerCount; i++)
            {
                ILayer pLayer = this.axMapControl1.get_Layer(i);
                cboLayers.Items.Add(pLayer.Name);
            }
            this.axMapControl1.Refresh();
            cboLayers.SelectedIndex = 0;
        }

        private void btnStartEditing_Click(object sender, EventArgs e)
        {
            //判断是否存在可编辑图层
            if (this.axMapControl1.Map.LayerCount == 0)
                return;
            if (this.cboLayers.Items.Count == 0)
            {
                MessageBox.Show("请加载要编辑的图层并点击刷新按钮", "提示");
                return;
            }

            //获取编辑图层
            IMap pMap = this.axMapControl1.Map;
            IFeatureLayer pFeatureLayer = this.axMapControl1.get_Layer(cboLayers.SelectedIndex) as IFeatureLayer;
            //初始化编辑
            if (mEdit == null)
            {
                mEdit = new Edit(pFeatureLayer, pMap);
            }
            //开始编辑
            mEdit.StartEditing();
            //开始编辑设为不可用，将其他编辑按钮设为可用
            this.btnStartEditing.Enabled = false;
            this.cboTasks.Enabled = true;
            this.btnStopEditing.Enabled = true;
            this.btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //判断编辑是否初始化
            if (mEdit == null)
                return;
            //处于编辑状态且已编辑则保存
            if (mEdit.IsEditing() && mEdit.HasEdited())
            {
                mEdit.SaveEditing(true);
            }
        }

        private void btnStopEditing_Click(object sender, EventArgs e)
        {
            if (mEdit == null)
                return;
            if (mEdit.HasEdited())
            {
                DialogResult dr = MessageBox.Show("图层已编辑，是否保存？", "提示", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                    mEdit.SaveEditing(true);
                else
                    mEdit.SaveEditing(false);
            }
            btnStartEditing.Enabled = true;
            this.cboTasks.Enabled = false;
            this.btnStopEditing.Enabled = false;
            this.btnSave.Enabled = false;
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //判断是否处于编辑状态
            if (mEdit != null && mEdit.IsEditing())
            {
                switch (cboTasks.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        mEdit.MouseMove(e.mapX, e.mapY);
                        break;
                }
            }
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //判断是否处于编辑状态
            if (mEdit != null && mEdit.IsEditing())
            {
                switch (cboTasks.SelectedIndex)
                {
                    case 0:
                        break;
                    case 1:
                        mEdit.PanMouseUp(e.mapX, e.mapY);
                        break;
                }
            }
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            if (this.mEagleEye != null && this.axMapControl1.LayerCount > 0)
            {
                //获取鹰眼图层
                this.mEagleEye.MapControl.AddLayer(this.mEagleEye.GetOverviewLayer(this.axMapControl1.Map));
                // 设置 MapControl 显示范围至数据的全局范围
                this.mEagleEye.MapControl.Extent = this.axMapControl1.FullExtent;
                // 刷新鹰眼控件地图
                this.mEagleEye.MapControl.Refresh();
            }
        }

        private void axMapControl1_OnFullExtentUpdated(object sender, IMapControlEvents2_OnFullExtentUpdatedEvent e)
        {
            if (this.mEagleEye != null && this.axMapControl1.LayerCount>0)
            {
                //获取鹰眼图层
                this.mEagleEye.MapControl.AddLayer(this.mEagleEye.GetOverviewLayer(this.axMapControl1.Map));
                // 设置 MapControl 显示范围至数据的全局范围
                this.mEagleEye.MapControl.Extent = this.axMapControl1.FullExtent;
                // 刷新鹰眼控件地图
                this.mEagleEye.MapControl.Refresh();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //初始化鹰眼
            mEagleEye = new EagleEye(axMapControl1);
            mEagleEye.Show();
        }
    }
}