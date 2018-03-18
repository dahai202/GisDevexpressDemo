using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using LinGIS;

namespace RibbonReportDesigner {
	static class Program {

        private static LicenseInitializer m_AOLicenseInitializer = new LinGIS.LicenseInitializer();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {

            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine },
            new esriLicenseExtensionCode[] { });
       
            
			DevExpress.UserSkins.OfficeSkins.Register();
			DevExpress.UserSkins.BonusSkins.Register();
			DevExpress.Skins.SkinManager.EnableFormSkins();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.LookAndFeel.UserLookAndFeel.Default.UseWindowsXPTheme = false;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "Black";

            RibbonReportDesigner.SplashScreen.Splasher.Show(typeof(WHC.OrderWater.ServerSide.SplashScreen.frmSplash));

            MainForm mainForm = new MainForm();
            mainForm.CreateNewReport();
			Application.Run(mainForm);

            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
		}
	}
}
