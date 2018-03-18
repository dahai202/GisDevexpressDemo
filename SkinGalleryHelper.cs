using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;

namespace RibbonDemos {
	public class SkinGalleryHelper : IDisposable {
		#region static 
		static Bitmap GetSkinImage(SimpleButton button, int width, int height, int indent) {
			Bitmap image = new Bitmap(width, height);
			using(Graphics g = Graphics.FromImage(image)) {
				StyleObjectInfoArgs info = new StyleObjectInfoArgs(new GraphicsCache(g));
				info.Bounds = new Rectangle(0, 0, width, height);
				button.LookAndFeel.Painter.GroupPanel.DrawObject(info);
				button.LookAndFeel.Painter.Border.DrawObject(info);
				info.Bounds = new Rectangle(indent, indent, width - indent * 2, height - indent * 2);
				button.LookAndFeel.Painter.Button.DrawObject(info);
			}
			return image;
		}
		#endregion
		DevExpress.XtraBars.RibbonGalleryBarItem ribbonGallerySkins;
		public SkinGalleryHelper(DevExpress.XtraBars.RibbonGalleryBarItem ribbonGallerySkins) { 
			this.ribbonGallerySkins = ribbonGallerySkins;
			InitBarItem();
			FillImages();
		}
		public void Dispose() {
			if(ribbonGallerySkins != null) {
				ribbonGallerySkins.Gallery.ItemClick -= new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(ItemClick);
				ribbonGallerySkins.Gallery.InitDropDownGallery -= new DevExpress.XtraBars.Ribbon.InplaceGalleryEventHandler(InitDropDownGallery);
			}
		}
		void InitBarItem() {
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup4 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup5 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();
			DevExpress.XtraBars.Ribbon.GalleryItemGroup galleryItemGroup6 = new DevExpress.XtraBars.Ribbon.GalleryItemGroup();

			ribbonGallerySkins.Gallery.AllowHoverImages = true;
			ribbonGallerySkins.Gallery.Appearance.ItemCaption.Options.UseFont = true;
			ribbonGallerySkins.Gallery.Appearance.ItemCaption.Options.UseTextOptions = true;
			ribbonGallerySkins.Gallery.Appearance.ItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			ribbonGallerySkins.Gallery.ColumnCount = 4;
			ribbonGallerySkins.Gallery.FixedHoverImageSize = false;
			galleryItemGroup4.Caption = "Main Skins";
			galleryItemGroup5.Caption = "Office Skins";
			galleryItemGroup6.Caption = "Bonus Skins";
			ribbonGallerySkins.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
            galleryItemGroup4,
            galleryItemGroup5,
            galleryItemGroup6});
			ribbonGallerySkins.Gallery.ImageSize = new System.Drawing.Size(32, 17);
			ribbonGallerySkins.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Top;
			ribbonGallerySkins.Gallery.RowCount = 4;
			ribbonGallerySkins.Gallery.ItemClick += new DevExpress.XtraBars.Ribbon.GalleryItemClickEventHandler(ItemClick);
			ribbonGallerySkins.Gallery.InitDropDownGallery += new DevExpress.XtraBars.Ribbon.InplaceGalleryEventHandler(InitDropDownGallery);
		}
		void FillImages() {
			SimpleButton imageButton = new SimpleButton();
			foreach(SkinContainer cnt in SkinManager.Default.Skins) {
				imageButton.LookAndFeel.SetSkinStyle(cnt.SkinName);
				GalleryItem gItem = new GalleryItem();
				int groupIndex = 0;
				if(cnt.SkinName.IndexOf("Office") > -1)
					groupIndex = 1;
				else if(!cnt.IsEmbedded)
					groupIndex = 2;
				ribbonGallerySkins.Gallery.Groups[groupIndex].Items.Add(gItem);
				gItem.Caption = cnt.SkinName;

				gItem.Image = GetSkinImage(imageButton, 32, 17, 2);
				gItem.HoverImage = GetSkinImage(imageButton, 70, 36, 5);
				gItem.Caption = cnt.SkinName;
			}

		}
		void ItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e) {
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(e.Item.Caption);
		}
		void InitDropDownGallery(object sender, DevExpress.XtraBars.Ribbon.InplaceGalleryEventArgs e) {
			e.PopupGallery.CreateFrom(ribbonGallerySkins.Gallery);
			e.PopupGallery.AllowFilter = false;
			e.PopupGallery.ShowItemText = true;
			e.PopupGallery.ShowGroupCaption = true;
			e.PopupGallery.AllowHoverImages = false;
			foreach(GalleryItemGroup galleryGroup in e.PopupGallery.Groups)
				foreach(GalleryItem item in galleryGroup.Items)
					item.Image = item.HoverImage;
			e.PopupGallery.ColumnCount = 2;
			e.PopupGallery.ImageSize = new Size(70, 36);
		}
	}
}
