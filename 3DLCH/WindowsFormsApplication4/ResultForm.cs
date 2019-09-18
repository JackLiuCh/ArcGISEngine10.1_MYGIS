using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace WindowsFormsApplication4
{
    public partial class ResultForm : Form
    {
        public ResultForm()
        {
            InitializeComponent();
        }
        //显示查询结果
        public void refreshView(IHit3DSet pHit3DSet)
        {
            //用tree控件显示查询结果
            treeView1.BeginUpdate();
            //清空tree控件的内容
            treeView1.Nodes.Clear();
            IHit3D pHit3D;
            int i;
            //遍历结果集
            for (i = 0; i < pHit3DSet.Hits.Count; i++)
            {
                pHit3D = pHit3DSet.Hits.get_Element(i) as IHit3D;
                if (pHit3D.Owner is ILayer)
                {
                    ILayer pLayer = pHit3D.Owner as ILayer;
                    //将图层的名称和坐标显示在树节点中
                    TreeNode node = treeView1.Nodes.Add(pLayer.Name);
                    node.Nodes.Add("X=" + pHit3D.Point.X);
                    node.Nodes.Add("Y=" + pHit3D.Point.Y);
                    node.Nodes.Add("Z=" + pHit3D.Point.Z);
                    //将该图层中的所有元素显示在该树节点的子节点
                    if (pHit3D.Object != null)
                    {
                        if (pHit3D.Object is IFeature)
                        {
                            IFeature pFeature = pHit3D.Object as IFeature;
                            int j;
                            //显示Feature中的内容
                            for (j = 0; j < pFeature.Fields.FieldCount; j++)
                            {
                                node.Nodes.Add(pFeature.Fields.get_Field(j).Name + ":" + pFeature.get_Value(j).ToString());
                            }
                        }
                    }

                }
            }
            treeView1.EndUpdate();
        }

        public void refreshFeatureListView(List<IFeature> pFeatureList)
        {
            //用tree控件显示查询结果
            treeView1.BeginUpdate();
            //清空tree控件的内容
            treeView1.Nodes.Clear();

            for(int i = 0; i < pFeatureList.Count; i++)
            {
                TreeNode node = treeView1.Nodes.Add("Feature"+(i+1)+":");
                for (int j = 0; j < pFeatureList[i].Fields.FieldCount; j++)
                {
                    string name = pFeatureList[i].Fields.get_Field(j).Name;
                    string value = pFeatureList[i].get_Value(j) == null ? "NULL" : pFeatureList[i].get_Value(j).ToString();
                    node.Nodes.Add(name + ":" + value);
                }
            }
            treeView1.EndUpdate();
        }
    }
}
