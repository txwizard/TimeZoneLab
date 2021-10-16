/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         FixedWidthReportItemsCollection

    File Name:          FixedWidthReportItemsCollection.cs

    Synopsis:           An instance of this class represents a collection of
                        FixedWidthReportItem objects from which to generate a
                        fixed width report.

    Remarks:            FixedWidthReportItem have a ColumnPosition field, which
                        is a standard array subscript. 

                        By implementing IComparable such that ColumnPosition is
                        the basis of comparisons, items may be added to this
                        collection in any order.

	License:            Copyright (C) 2014-2016, David A. Gray.
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    Created:            Sunday, 31 August 2014 and Monday, 01 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2014/09/01 1.0     DAG    Initial implementation.

	2016/06/15 2.0     DAG    1) Embed my three-clause BSD license, and improve
                                 the internal documentation.
    
							  2) Break dependence upon the deprecated SharedUtl2
                                 and ApplicationHelpers2 libraries.

							  3) Eliminate the UTIL class, since all it ever did
                                 was give local scope to constants defined in
                                 WizardWrx.DLLServices2.dll, WHICH IS ALREADY
                                 bound into the project for more substantial
								 reasons.

	2021/10/09 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using WizardWrx;


namespace TimeZoneLab
{
    /// <summary>
    /// Organize a collection of FixedWidthReportItem objects from which to 
    /// generate a fixed width report, either on the console or onto a specified
    /// file, which is created or overwritten.
    /// </summary>
    class FixedWidthReportItemsCollection: List<FixedWidthReportItem>
    {
        #region Public Enumerations and Constants
        /// <summary>
        /// The penmXfromRule argument of static utility method
        /// DeriveLabelFromPropertyName is specified in terms of this
        /// enumeration.
        /// </summary>
        public enum PropertyNameTransformation
        {
            /// <summary>
            /// Remove the text specified by pstrTeransformation from the
            /// beginning of the string.
            /// </summary>
            RemoveFixedTextFromBeginning ,

            /// <summary>
            /// Remove the text specified by pstrTeransformation from the end of
            /// the string.
            /// </summary>
            RemoveFixedTextFromEnd ,
        } ; // PropertyNameTransformation


        /// <summary>
        /// Unless overridden, this is the default transformation text.
        /// </summary>
        public const string DEFAULT_NAME_TRANSOFRM_TEXT = @"Display";


        /// <summary>
        /// Unless overridden, this is the default rule for applying the transformation text.
        /// </summary>
        public const PropertyNameTransformation DEFAULT_NAME_TRANSFORM_RULE = PropertyNameTransformation.RemoveFixedTextFromBeginning;


        /// <summary>
        /// This constant is exposed as a convenience. The field separator
        /// cannot be overridden.
        /// </summary>
		public const char FIELD_SEPARATOR = WizardWrx.SpecialCharacters.SPACE_CHAR;
        #endregion  // Public Enumerations and Constants


        #region Private Constants and Storage
        const string ERRMSG_FILE_IO = @"An {0} exception occurred while {1} on file {2}. See the Inner Exception for additional details.";

		const int FILE_BUFSIZE = WizardWrx.MagicNumbers.CAPACITY_08KB;
        const bool FILE_CREATE_OR_OVERWRITE = FileIOFlags.FILE_OUT_CREATE;
        enum ReportStage
        {
            UsingStdOut ,
            Opening ,
            WritingLabelRow ,
            WritingGuideRow ,
            WritingDetails ,
            Closing ,
            Aborting 
        }   // ReportStage

        ReportStage _enmReportStage = ReportStage.UsingStdOut;

        string _strOutputFileName = null;
        StreamWriter _swReport = null;
        #endregion  // Private Constants and Storage


        #region Public Constructors
        public FixedWidthReportItemsCollection ( )
        { }  // FixedWidthReportItemsCollection ( ) (1 of 4)


        public FixedWidthReportItemsCollection ( string pstrOutputFileName )
        {
            _strOutputFileName = pstrOutputFileName;
        }   // FixedWidthReportItemsCollection ( string pstrOutputFileName ) (2 of 4)


        public FixedWidthReportItemsCollection ( string [ ] pastrlOrderedLabels )
        {
            LoadLabelsFromArray ( pastrlOrderedLabels );
        }   // FixedWidthReportItemsCollection ( string [ ] pastrlOrderedLabels ) (3 of 4)


        public FixedWidthReportItemsCollection (
            string [ ] pastrlOrderedLabels , 
            string pstrOutputFileName )
        {
            _strOutputFileName = pstrOutputFileName;
            LoadLabelsFromArray ( pastrlOrderedLabels );
        }   // FixedWidthReportItemsCollection ( string [ ] pastrlOrderedLabels , string pstrOutputFileName ) (4 of 4)
        #endregion  // Public Constructors


        #region Public Instance Methods
        /// <summary>
        /// When all records have been processed, call this method once to
        /// cleanly close the output file and release its unmanaged resources.
        /// </summary>
        public void CloseReport ( )
        {
            if ( _swReport != null )
            {
                try
                {
                    _enmReportStage = ReportStage.Closing;

                    if ( _swReport.BaseStream.CanWrite )
                    {   // If the base stream is writable, it's still open.
                        _swReport.Close ( );
                    }   // if ( _swReport.BaseStream.CanWrite )

                    _swReport.Dispose ( );
                    _swReport = null;
                }
                catch ( Exception exAll )
                {
                    string strMsg = string.Format (
                        ERRMSG_FILE_IO ,                                        // Message template.
                        exAll.GetType ( ) ,                                     // Format Item 0
                        _enmReportStage ,                                       // Format Item 1
                        _strOutputFileName );                                   // Format Item 2
                    _enmReportStage = ReportStage.Aborting;                     // its value is already in the report.
                    throw new Exception ( strMsg , exAll );
                }
                finally
                {
                    if ( _enmReportStage == ReportStage.Aborting )
                    {
                        if ( _swReport != null )
                        {
                            _swReport.Close ( );
                            _swReport.Dispose ( );
                            _swReport = null;
                        }   // if ( _swReport != null )
                    }   // if ( _enmReportStage == ReportStage.Aborting )
                }   // Try/Catch/Finally block protecting file I/O operations.
            }
        }   // public void CloseReport


        /// <summary>
        /// Create the report headings. Please see the Remarks section for
        /// essential implementation requirements.
        /// </summary>
        /// <remarks>
        /// The class that owns the instance MUST call UpdateColumnWidths first.
        /// Since there is no way for this routine to know that it has been
        /// called for every object that will go into the report, you are on
        /// your own.
        /// 
        /// Perhaps a subsequent version will handle this, probably by means of
        /// a generic method that creates the whole report. If so, that method
        /// will need two arguments, both of which are related generics.
        /// </remarks>
        /// <see cref="UpdateColumnWidths"/>
        public void CreateReportHeading ( )
        {
            CreateReportHeading ( FixedWidthReportItem.LBL_GUIDE_USE_DEFAULT );
        }   // public void CreateReportHeading (1 of 2)


        /// <summary>
        /// Create the report headings. Please see the Remarks section for
        /// essential implementation requirements.
        /// </summary>
        /// <param name="pchrGuideCharacter">
        /// Specify the default guide character to use. Specify 
        /// FixedWidthReportItem.LBL_GUIDE_USE_DEFAULT to use the default guide
        /// character, defined as FixedWidthReportItem.LBL_GUIDE_DEFAULT.
        /// </param>
        /// <remarks>
        /// The class that owns the instance MUST call UpdateColumnWidths first.
        /// Since there is no way for this routine to know that it has been
        /// called for every object that will go into the report, you are on
        /// your own.
        /// 
        /// Perhaps a subsequent version will handle this, probably by means of
        /// a generic method that creates the whole report. If so, that method
        /// will need two arguments, both of which are related generics.
        /// </remarks>
        /// <see cref="UpdateColumnWidths"/>
        public void CreateReportHeading ( char pchrGuideCharacter )
        {
            StringBuilder sbLabelRow = new StringBuilder ( WizardWrx.MagicNumbers.CAPACITY_01KB );
            StringBuilder sbGuideRow = new StringBuilder ( WizardWrx.MagicNumbers.CAPACITY_01KB );

            int intNColumns = base.Count;

            for ( int intCurrCol = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                      intCurrCol < intNColumns ;
                      intCurrCol++ )
            {
                FixedWidthReportItem fwi = this [ intCurrCol ];
                sbLabelRow.Append ( fwi.CreateFixedWidthLabel ( ) );
                sbGuideRow.Append ( fwi.CreateLabelGuide ( pchrGuideCharacter ) );

                if ( Logic.MoreForIterationsToComeLE ( intCurrCol , intNColumns ) )
                {   // All but the last column is followed by a field separator.
                    sbLabelRow.Append ( FIELD_SEPARATOR );
                    sbGuideRow.Append ( FIELD_SEPARATOR );
                }	// if ( Logic.MoreForIterationsToComeLE ( intCurrCol , intNColumns ) )
            }   // for ( int intCurrCol = ArrayInfo.ARRAY_FIRST_ELEMENT ; intCurrCol < intNColumns ; intCurrCol++ )

            //  ----------------------------------------------------------------
            //  Write the heading and guide records. The degenerate case writes
            //  onto the console. Otherwise, a buffered TextStream is created,
            //  used, and left open for the next phase.
            //  ----------------------------------------------------------------

            if ( string.IsNullOrEmpty ( _strOutputFileName ) )
            {   // Since the instance was constructed without a file name, use standard output.
                Console.WriteLine ( sbLabelRow.ToString ( ) );
                Console.WriteLine ( sbGuideRow.ToString ( ) );
            }   // TRUE (degenerate case) block, if ( string.IsNullOrEmpty ( _strOutputFileName ) )
            else
            {   // A file was named. Use it.
                try
                {
                    _enmReportStage = ReportStage.Opening;
                    _swReport = new StreamWriter (
                        _strOutputFileName ,
                        FILE_CREATE_OR_OVERWRITE ,
                        Encoding.Unicode ,
                        FILE_BUFSIZE );

                    _enmReportStage = ReportStage.WritingLabelRow;
                    _swReport.WriteLine ( sbLabelRow.ToString ( ) );

                    _enmReportStage = ReportStage.WritingGuideRow;
                    _swReport.WriteLine ( sbGuideRow.ToString ( ) );

                    _enmReportStage = ReportStage.WritingDetails;
                }   // Finished with the heading.
                catch ( Exception exAll )
                {
                    string strMsg = string.Format (
                        ERRMSG_FILE_IO ,                                        // Message template.
                        exAll.GetType ( ) ,                                     // Format Item 0
                        _enmReportStage ,                                       // Format Item 1
                        _strOutputFileName );                                   // Format Item 2
                    _enmReportStage = ReportStage.Aborting;                     // its value is already in the report.
                    throw new Exception ( strMsg , exAll );
                }
                finally
                {
                    if ( _enmReportStage == ReportStage.Aborting )
                    {
                        if ( _swReport != null )
                        {
                            _swReport.Close ( );
                            _swReport.Dispose ( );
                            _swReport = null;
                        }   // if ( _swReport != null )
                    }   // if ( _enmReportStage == ReportStage.Aborting )
                }   // Try/Catch/Finally block protecting file I/O operations.
            }   // FALSE (standard case) block, if ( string.IsNullOrEmpty ( _strOutputFileName ) )
        }   // public void CreateReportHeading (2 of 2)


        /// <summary>
        /// Call this method once for each object in your collection to create a
        /// report record from it.
        /// 
        /// Please see the Remarks section for essential implementation details.
        /// </summary>
        /// <typeparam name="T">
        /// This method accepts ANY type of object, since it's up to the named
        /// field to be able to render a string representation of itself.
        /// </typeparam>
        /// <param name="pTColumSource">
        /// Specify the instance to evaluate against the current value of the
        /// Width property.
        /// </param>
        /// <remarks>
        /// You must call UpdateColumnWidths, followed by CreateReportHeading.
        /// In addition to creating the report heading text, CreateReportHeading
        /// opens the output file.
        /// </remarks>
        /// <see cref="CreateReportHeading"/>
        /// <see cref="UpdateColumnWidths"/>
        public void CreateReportRecord<T> ( T pTColumSource )
        {
            CreateReportRecord (
                pTColumSource ,
                FixedWidthReportItem.LBL_GUIDE_USE_DEFAULT );
        }   // public void CreateReportRecord<T> (1 of 2)


        /// <summary>
        /// Call this method once for each object in your collection to create a
        /// report record from it.
        /// 
        /// Please see the Remarks section for essential implementation details.
        /// </summary>
        /// <typeparam name="T">
        /// This method accepts ANY type of object, since it's up to the named
        /// field to be able to render a string representation of itself.
        /// </typeparam>
        /// <param name="pTColumSource">
        /// Specify the instance to evaluate against the current value of the
        /// Width property.
        /// </param>
        /// <param name="pchrGuideCharacter">
        /// Specify the default guide character to use. Specify 
        /// FixedWidthReportItem.LBL_GUIDE_USE_DEFAULT to use the default guide
        /// character, defined as FixedWidthReportItem.LBL_GUIDE_DEFAULT.
        /// </param>
        /// <remarks>
        /// You must call UpdateColumnWidths, followed by CreateReportHeading.
        /// In addition to creating the report heading text, CreateReportHeading
        /// opens the output file.
        /// </remarks>
        /// <see cref="CreateReportHeading"/>
        /// <see cref="UpdateColumnWidths"/>
        public void CreateReportRecord<T> (
            T pTColumSource ,
            char pchrGuideCharacter )
        {
			StringBuilder sbRecord = new StringBuilder ( MagicNumbers.CAPACITY_01KB );

            int intNColumns = base.Count;

            for ( int intCurrCol = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                      intCurrCol < intNColumns ;
                      intCurrCol++ )
            {
                FixedWidthReportItem fwi = this [ intCurrCol ];
                sbRecord.Append ( fwi.GetFieldValue ( pTColumSource ) );

                if ( Logic.MoreForIterationsToComeLE ( intCurrCol , intNColumns ) )
                {   // All but the last column is followed by a field separator.
                    sbRecord.Append ( FIELD_SEPARATOR );
				}   // if ( Logic.MoreForIterationsToComeLE ( intCurrCol , intNColumns ) )
			}   // for ( int intCurrCol = ArrayInfo.ARRAY_FIRST_ELEMENT ; intCurrCol < intNColumns ; intCurrCol++ )

            if ( _swReport == null )
            {
                Console.WriteLine ( sbRecord.ToString ( ) );
			}	// TRUE block, if ( _swReport == null )
            else
            {
                try
                {
                    _swReport.WriteLine ( sbRecord.ToString ( ) );
                }
                catch (Exception exAll)
                {
                    string strMsg = string.Format (
                        ERRMSG_FILE_IO ,                                        // Message template.
                        exAll.GetType ( ) ,                                     // Format Item 0
                        _enmReportStage ,                                       // Format Item 1
                        _strOutputFileName );                                   // Format Item 2
                    _enmReportStage = ReportStage.Aborting;                     // its value is already in the report.
                    throw new Exception (
                        strMsg ,
                        exAll );
                }
                finally
                {
                    if ( _enmReportStage == ReportStage.Aborting )
                    {
                        if ( _swReport != null )
                        {
                            _swReport.Close ( );
                            _swReport.Dispose ( );
                            _swReport = null;
                        }   // if ( _swReport != null )
                    }   // if ( _enmReportStage == ReportStage.Aborting )
                }   // Try/Catch/Finally block protecting file I/O operations.
			}	// FALSE block, if ( _swReport == null )
        }   // public void CreateReportRecord<T> (2 of 2)


        /// <summary>
        /// To update all field widths to accommodate all records, call this
        /// method once for each instance of pTColumSource to be included in 
        /// the report. If these objects are organized into a collection, such
        /// as a List, put a method into that list to do so.
        /// </summary>
        /// <typeparam name="T">
        /// This method accepts ANY type of object, since it's up to the named
        /// field to be able to render a string representation of itself.
        /// </typeparam>
        /// <param name="pTColumSource">
        /// Specify the instance to evaluate against the current value of the
        /// Width property.
        /// </param>
        /// <remarks>
        /// Once this method has processed every instance of pTColumSource that
        /// goes into the report, the next step is to call CreateReportHeading.
        /// </remarks>
        /// <see cref="CreateReportHeading"/>
        public void UpdateColumnWidths<T> ( T pTColumSource )
        {
            foreach ( FixedWidthReportItem fwri in this )
                fwri.UpdateWidth ( pTColumSource );
        }   // public void UpdateColumnWidths<T> ( T pTColumSource )
        #endregion  // Public Instance Methods


        #region Static Utility Methods
        /// <summary>
        /// Derive the label from the property name by removing text from the
        /// beginning or end of the property name.
        /// </summary>
        /// <param name="pstrPropertyName">
        /// Specify the property name to transform.
        /// </param>
        /// <param name="pstrTeransformation">
        /// Specify the string to remove, if present.
        /// </param>
        /// <param name="penmXfromRule">
        /// Specify where in the string to expect pstrTeransformation.
        /// </param>
        /// <returns>
        /// The return value is the property name, transformed as specified.
        /// </returns>
        public static string DeriveLabelFromPropertyName (
            string pstrPropertyName ,
            string pstrTeransformation ,
            PropertyNameTransformation penmXfromRule )
        {
            switch ( penmXfromRule )
            {
                case PropertyNameTransformation.RemoveFixedTextFromBeginning:
                    if ( pstrPropertyName.StartsWith ( pstrTeransformation ) )
                        return pstrPropertyName.Substring (
                            pstrTeransformation.Length );
                    else
                        return pstrPropertyName;
                case PropertyNameTransformation.RemoveFixedTextFromEnd:
                    if ( pstrPropertyName.EndsWith ( pstrTeransformation ) )
                        return pstrPropertyName.Substring (
                            ListInfo.SUBSTR_BEGINNING ,
                            pstrPropertyName.Length
                                - pstrTeransformation.Length );
                    else
                        return pstrPropertyName;
                default:
                    return pstrPropertyName;
            }   // switch ( penmXfromRule )
        }   // public static string DeriveLabelFromPropertyName
        #endregion  // Static Utility Methods


        #region Private Utility Methods
        private void LoadLabelsFromArray ( string [ ] pastrlOrderedLabels )
        {
            int intNFields = pastrlOrderedLabels.Length;

            if ( intNFields > ArrayInfo.ARRAY_IS_EMPTY )
            {
                for ( int intCurrFieldIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                          intCurrFieldIndex < intNFields ;
                          intCurrFieldIndex++ )
                {
                    FixedWidthReportItem fwi = new FixedWidthReportItem (
                        intCurrFieldIndex ,                                     // int pintColumnPosition
                        pastrlOrderedLabels [ intCurrFieldIndex ] ,             // string pstrPropertyName
                        DeriveLabelFromPropertyName (                           // string pstrLabel
                            pastrlOrderedLabels [ intCurrFieldIndex ] ,         //		string pstrPropertyName
                            DEFAULT_NAME_TRANSOFRM_TEXT ,                       //		string pstrTeransformation
                            DEFAULT_NAME_TRANSFORM_RULE ) );                    //		PropertyNameTransformation penmXfromRule
                    base.Add ( fwi );
				}   // for ( int intCurrFieldIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intCurrFieldIndex < intNFields ; intCurrFieldIndex++ )

                base.Sort ( );
			}   // if ( intNFields > ArrayInfo.ARRAY_IS_EMPTY )
        }   // private void LoadLabelsFromArray
        #endregion  // Private Utility Methods
    }   // class FixedWidthReportItemsCollection
}   // partial namespace TimeZoneLab