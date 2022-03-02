﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
	public interface IDeclarationViewWindow
	{
		string Description {
			get;
			set;
		}
		void ShowDeclarationViewWindow();
		void CloseDeclarationViewWindow();
	}
	
	public class DeclarationViewWindow : Form, IDeclarationViewWindow
	{
		string description = String.Empty;
		bool fixedWidth;
		
		HighlightColor tipGradient;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				if (value == null && Visible) {
					Visible = false;
				} else if (value != null) {
					if (!Visible) ShowDeclarationViewWindow();
					Refresh();
				}
			}
		}
		
		public bool FixedWidth {
			get {
				return fixedWidth;
			}
			set {
				fixedWidth = value;
			}
		}
		
		public int GetRequiredLeftHandSideWidth(Point p) {
			if (description != null && description.Length > 0) {
				using (Graphics g = CreateGraphics()) {
					Size s = TipPainterTools.GetLeftHandSideDrawingSizeHelpTipFromCombinedDescription(this, g, Font, null, description, p);
					return s.Width;
				}
			}
			return 0;
		}
		
		public bool HideOnClick;
		
		public DeclarationViewWindow(Form parent, TextArea textArea)
		{
			SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			Owner           = parent;
			ShowInTaskbar   = false;
			Size            = new Size(0, 0);
			base.CreateHandle();

			tipGradient = textArea.Document.HighlightingStrategy.GetColorFor("TipsGradient");

			//Font = new Font(FontFamily.GenericSansSerif, 9.5f, FontStyle.Regular, GraphicsUnit.Point);
		}
		
		protected override CreateParams CreateParams {
			get {
				CreateParams p = base.CreateParams;
				ShadowWindow.AddShadowToWindow(p);
				return p;
			}
		}
		
		protected override bool ShowWithoutActivation {
			get {
				return true;
			}
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (HideOnClick) Hide();
		}
		
		public void ShowDeclarationViewWindow()
		{
			Show();
		}
		
		public void CloseDeclarationViewWindow()
		{
			Close();
			Dispose();
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			if (description != null && description.Length > 0) {
				if (fixedWidth) {
					TipPainterTools.DrawFixedWidthHelpTipFromCombinedDescription(this, pe.Graphics, Font, null, description);
				} else {
					TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics, Font, null, description);
				}
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			// Draw gradient background
			LinearGradientBrush gradient = new LinearGradientBrush(pe.ClipRectangle, 
																   tipGradient.Color,
																   tipGradient.BackgroundColor,
																   LinearGradientMode.Vertical);
			pe.Graphics.FillRectangle(gradient, pe.ClipRectangle);
		}
	}
}
