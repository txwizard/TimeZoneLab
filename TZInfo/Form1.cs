using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;

using System.Windows.Forms;

namespace TZInfo
{
	public partial class Form1 : Form
	{
		public Form1 ( )
		{
			InitializeComponent ( );

			//	----------------------------------------------------------------
			//	Apply our own labels, which incorporate mnemonic accelerators.
			//	----------------------------------------------------------------

			lblTimeZone.Text = Properties.Resources.LABEL_TEXT_LBLTIMEZONE;
			lblDisplayName.Text = Properties.Resources.LABEL_TEXT_LBLDISPLAYNAME;
			lblStandardName.Text = Properties.Resources.LABEL_TEXT_LBLSTANDARDNAME;
			lblDaylightName.Text = Properties.Resources.LABEL_TEXT_LBLDAYLIGHTNAME;
			lblBaseUTCoffset.Text = Properties.Resources.LABEL_TEXT_LBLBASEUTCOFFSET;
			lblSupportsDST.Text = Properties.Resources.LABEL_TEXT_LBLSUPPORTSDST;
			lblOutputFile.Text = Properties.Resources.LABEL_TEXT_LBLOUTPUTFILE;

			cmdBrowseForFile.Text = Properties.Resources.LABEL_TEXT_CMDBROWSEFORFILE;
			cmdGo.Text = Properties.Resources.LABEL_TEXT_CMDGO;

			//	----------------------------------------------------------------
			//	Populate the combo box from the time zones collection, and set
			//	its default value to the local time zone.
			//	----------------------------------------------------------------

			ReadOnlyCollection<TimeZoneInfo> tzCollection;
			tzCollection = TimeZoneInfo.GetSystemTimeZones ( );

			cboTimeZone.DataSource = tzCollection;
			cboTimeZone.DisplayMember = @"Id";
			cboTimeZone.ValueMember = @"Id";
			cboTimeZone.SelectedValue = TimeZoneInfo.Local.Id;
		}	// public Form1 constructor
	}	// public partial class Form1 : Form
}	// partial namespace TZInfo