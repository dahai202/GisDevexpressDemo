using System;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Helpers;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using LinGIS;
using RibbonReportDesigner.SplashScreen;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;

namespace RibbonReportDesigner
{
    public partial class MainForm : RibbonForm
    {
        public MainForm(bool showAppearancePage)
        {
            Splasher.Status = "正在展示相关的内容";
            System.Threading.Thread.Sleep(100);
            InitializeComponent();

            Splasher.Status = "初始化完毕";
            System.Threading.Thread.Sleep(50);
            Splasher.Close();

            if (showAppearancePage)
            {
                //SkinHelper.InitSkinGallery(ribbonGallerySkins, true);
            }
            else
                this.ribbonControl1.Pages.Remove(appearanceRibbonPage);
        }
        public MainForm()
            : this(true)
        {

            this.treeView1.ItemHeight = 20;
            this.treeView1.ExpandAll();

           // this.treeList1.OptionsView.ShowFocusedFrame = false;
            this.treeList1.OptionsView.AutoWidth = true;
            //this.treeList1.OptionsSelection.EnableAppearanceFocusedCell = false;
            //this.treeList1.OptionsSelection.EnableAppearanceFocusedRow = false;

            this.treeList1.ExpandAll();
            //this.treeList1.RowHeight = 10;

            this.treeList1.OptionsBehavior.Editable = false;

            //this.treeList1.OptionsBehavior.MoveOnEdit = false;
            this.treeList1.OptionsBehavior.ShowEditorOnMouseUp = false;
            this.treeList1.OptionsBehavior.ShowToolTips = false;
            
            this.treeList1.OptionsView.ShowColumns = false;
            this.treeList1.OptionsView.ShowHorzLines = false;
            this.treeList1.OptionsView.ShowVertLines = false;
            this.treeList1.OptionsView.ShowIndicator = false;

            
            /// 去掉选中节点时的虚框
            this.treeList1.OptionsView.ShowFocusedFrame =false;
            /// 
            /// 设置选中时节点的背景色
            this.treeList1.Appearance.FocusedCell.BackColor = System.Drawing.Color.LightSteelBlue;
            this.treeList1.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.SteelBlue;
            this.treeList1.Appearance.FocusedCell.Options.UseBackColor = true;
            /// 
            /// 选中时会把节点中没显示完全的信息全部显示
            this.treeList1.Appearance.FocusedCell.Options.UseTextOptions = true;
            this.treeList1.Appearance.FocusedCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

        }

        
        /// <summary>
        /// Handles the GetSelectImage event of the treelArea control.
        /// 设置SelectImage的状态，如果是选中的则换成另选中时应该显示的图标
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraTreeList.GetSelectImageEventArgs"/> instance containing the event data.</param>
        private void treelArea_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            e.NodeImageIndex = e.Node.Expanded ? 0 : e.Node.Nodes.Count > 0 ? 1 : 2;

            if (e.FocusedNode)
            {
                e.NodeImageIndex += 3;
            }
        }

        void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
        public void OpenReport(DevExpress.XtraReports.UI.XtraReport newReport)
        {
            xrDesignMdiController1.OpenReport(newReport);
        }
        public void CreateNewReport()
        {
            xrDesignMdiController1.CreateNewReport();
        }
        public XRDesignPanel ActiveXRDesignPanel
        {
            get { return xrDesignMdiController1.ActiveDesignPanel; }
        }

        private ESRI.ArcGIS.Controls.IMapControl3 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;
        private ControlsSynchronizer m_controlsSynchronizer = null;

        private IMapDocument pMapDocument;

        private void MainFrm_Load(object sender, EventArgs e)
        {
            //get a reference to the MapControl and the PageLayoutControl
            //取得MapControl和PageLayoutControl的引用
            m_mapControl = (IMapControl3)this.mainMapControl.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl.Object;

            //initialize the controls synchronization calss
            //初始化controls synchronization calss
            m_controlsSynchronizer = new ControlsSynchronizer(m_mapControl, m_pageLayoutControl);

            //bind the controls together (both point at the same map) and set the MapControl as the active control
            //把MapControl和PageLayoutControl帮顶起来(两个都指向同一个Map),然后设置MapControl为活动的Control
            m_controlsSynchronizer.BindControls(true);

            //add the framework controls (TOC and Toolbars) in order to synchronize then when the
            //active control changes (call SetBuddyControl)
            //m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            //m_controlsSynchronizer.AddFrameworkControl(axToolbarControl2.Object);
            m_controlsSynchronizer.AddFrameworkControl(this.axTOCControl.Object);

            //add the Open Map Document command onto the toolbar
            //OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            //axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            GalleryDropDown skins = new GalleryDropDown();
            skins.Ribbon = ribbonControl1;
            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGalleryDropDown(skins);
            iPaintStyle.DropDownControl = skins;

        }
        private void commandBarItem35_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.mainMapControl.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前地图？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                //if (result == DialogResult.Yes) this.btnSaveDoc_Click(null, null);
            }
            this.openFileDialog.Title = "请选择地图文件";
            this.openFileDialog.Filter = "MXD地图文件|*.mxd";
            this.openFileDialog.Multiselect = false;
            this.openFileDialog.RestoreDirectory = true;
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Application.DoEvents();
                string docName = this.openFileDialog.FileName;
                try
                {
                    this.pMapDocument = new MapDocumentClass();
                    if (pMapDocument.get_IsPresent(docName) && !pMapDocument.get_IsPasswordProtected(docName))
                    {
                        pMapDocument.Open(docName, null);
                        IMap pMap = pMapDocument.get_Map(0);
                        m_controlsSynchronizer.ReplaceMap(pMap);
                        this.mainMapControl.DocumentFilename = docName;
                        this.Text = System.IO.Path.GetFileName(this.openFileDialog.FileName) + " - " + "LinGIS - LinInfo";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.pMapDocument);
                    Application.DoEvents();
                    this.pMapDocument = null;
                }
            }
        }

        private void dockPanel1_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem101_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.mainMapControl.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前地图？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                //if (result == DialogResult.Yes) this.btnSaveDoc_Click(null, null);
            }
            this.openFileDialog.Title = "请选择地图文件";
            this.openFileDialog.Filter = "MXD地图文件|*.mxd";
            this.openFileDialog.Multiselect = false;
            this.openFileDialog.RestoreDirectory = true;
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Application.DoEvents();
                string docName = this.openFileDialog.FileName;
                try
                {
                    this.pMapDocument = new MapDocumentClass();
                    if (pMapDocument.get_IsPresent(docName) && !pMapDocument.get_IsPasswordProtected(docName))
                    {
                        pMapDocument.Open(docName, null);
                        IMap pMap = pMapDocument.get_Map(0);
                        m_controlsSynchronizer.ReplaceMap(pMap);
                        this.mainMapControl.DocumentFilename = docName;
                        this.Text = System.IO.Path.GetFileName(this.openFileDialog.FileName) + " - " + "LinGIS - LinInfo";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.pMapDocument);
                    Application.DoEvents();
                    this.pMapDocument = null;
                }
            }
        }

        #region 桌面事件

        private void mainMapControl_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            IEnvelope pEnvelope = e.newEnvelope as IEnvelope;

            ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDash;
            pSimpleLineSymbol.Width = 2;
            pSimpleLineSymbol.Color = this.getRGBColor(0, 0, 0);

            ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
            pSimpleFillSymbol.Color = this.getRGBColor(255, 0, 0);
            pSimpleFillSymbol.Outline = pSimpleLineSymbol as ILineSymbol;
            pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;


            IRectangleElement pRectangleElement = new RectangleElementClass();
            IElement pElement = pRectangleElement as IElement;
            pElement.Geometry = pEnvelope as IGeometry;
            IFillShapeElement pFillShapeElement = pRectangleElement as IFillShapeElement;
            pFillShapeElement.Symbol = pSimpleFillSymbol as IFillSymbol;

            IGraphicsContainer pGraphicsContainer = this.EagleaxMapControl.Map as IGraphicsContainer;
            pGraphicsContainer.DeleteAllElements();
            pGraphicsContainer.AddElement(pElement, 0);

            this.EagleaxMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void EagleaxMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.EagleaxMapControl.Map.LayerCount != 0)
            {

                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);

                    IEnvelope pEnvelope = this.mainMapControl.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.mainMapControl.Extent = pEnvelope;
                    this.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.EagleaxMapControl.TrackRectangle();
                    this.mainMapControl.Extent = pEnvelop;
                    this.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }

            }
        }

        private void EagleaxMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);

            this.mainMapControl.CenterAt(pPoint);
            this.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        private IColor getRGBColor(int r, int g, int b)
        {
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = r;
            pRgbColor.Green = g;
            pRgbColor.Blue = b;
            IColor pColor = pRgbColor as IColor;
            return pColor;
        }
        

        private void mainMapControl_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            this.EagleaxMapControl.Map = new MapClass();
            for (int i = 1; i <= this.mainMapControl.LayerCount; i++)
            {
                this.EagleaxMapControl.AddLayer(this.mainMapControl.get_Layer(this.mainMapControl.LayerCount - i));
            }
            this.EagleaxMapControl.Extent = this.mainMapControl.FullExtent;
            this.EagleaxMapControl.Refresh();
        }

        private IPoint mapRightClickPoint;
        private IPoint mapTextPoint;
        private void mainMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                if (this.mapRightClickPoint == null)
                {
                    this.mapRightClickPoint = new PointClass();
                }
                this.mapRightClickPoint.PutCoords(e.mapX, e.mapY);
                IGraphicsContainer pGraphicContainer = this.mainMapControl.Map as IGraphicsContainer;
                IEnumElement pEnumElement = pGraphicContainer.LocateElements(this.mapRightClickPoint, this.mainMapControl.ActiveView.Extent.Width / 500);
                if (pEnumElement != null)
                {
                    return;
                }
                else
                {
                   // this.contextMenuMapNormal.Show(this.mainMapControl as Control, new System.Drawing.Point(e.x, e.y));
                }
            }
            else if (e.button == 1)
            {
                //if (this.btnDrawNewText.Checked == false) return;

                if (this.mapTextPoint == null)
                {
                    this.mapTextPoint = new PointClass();
                }
                this.mapTextPoint.PutCoords(e.mapX, e.mapY);
                //this.txtNewText.Location = new System.Drawing.Point(e.x, e.y);
                //this.txtNewText.Text = "文本";
                //this.txtNewText.Visible = true;
                //this.txtNewText.Focus();
                //this.txtNewText.SelectAll();
                //this.btnDrawNewText.Checked = false;
            }
        }

        private void mainMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //this.lblCurrentLocation.Text = string.Format("{0},{1},{2}", e.mapX.ToString("#######.###"), e.mapY.ToString("#######.###"), this.mainMapControl.MapUnits.ToString().Substring(4));
            this.printPreviewStaticItem1.Caption = string.Format("{0}  {1}  {2}", e.mapX.ToString("#######.###"), e.mapY.ToString("#######.###"), this.mainMapControl.MapUnits.ToString().Substring(4));
        }

        #endregion

        
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(this.mainMapControl.Object);
            pCommand.OnClick();
        }

        private void commandBarItem41_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void commandBarItem42_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapMeasureToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapIdentifyToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsSelectToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }


        private void commandBarItem38_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
        }

        private void commandBarItem39_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapZoomInToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void commandBarItem40_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void barButtonItem102_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapPanToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void barButtonItem103_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapZoomInToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void barButtonItem104_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapZoomOutToolClass();
            ITool pTool = pCommand as ITool;
            switch (this.xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    pCommand.OnCreate(this.mainMapControl.Object);
                    this.mainMapControl.CurrentTool = pTool;
                    break;
                case 1:
                    pCommand.OnCreate(this.axPageLayoutControl.Object);
                    this.axPageLayoutControl.CurrentTool = pTool;
                    break;
            }
        }

        private void barButtonItem105_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapZoomToLastExtentBackCommandClass();
            pCommand.OnCreate(this.mainMapControl.Object);
            pCommand.OnClick();
        }

        private void barButtonItem106_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pCommand = new ControlsMapZoomToLastExtentForwardCommandClass();
            pCommand.OnCreate(this.mainMapControl.Object);
            pCommand.OnClick();
        }

        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.Utils.About.frmAbout dlg = new DevExpress.Utils.About.frmAbout("YH测试bywjh设计");
            dlg.ShowDialog();
        }
    }
}
